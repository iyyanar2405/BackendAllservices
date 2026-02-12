using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthProvider.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthProvider.Models;
using CoreIdentity.API.Identity.Models;
using AuthProvider.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthProvider.Controllers
{
    /// <summary>
    /// User management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("User Management")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the UserController
        /// </summary>
        /// <param name="userManager">User manager service</param>
        /// <param name="roleManager">Role manager service</param>
        /// <param name="signInManager">Sign in manager service</param>
        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
        }

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        /// <param name="model">Login credentials to verify</param>
        /// <returns>Authentication status</returns>
        /// <response code="200">Returns authentication status</response>
        /// <response code="400">Invalid credentials provided</response>
        [HttpPost("is-authenticated")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> IsAuthenticated([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var user = await _userManager.FindByEmailAsync(model.UserName).ConfigureAwait(false);
            if (user == null)
                return BadRequest(ApiResponse.CreateError("Invalid credentials."));

            var claimUser = await _signInManager.CreateUserPrincipalAsync(user).ConfigureAwait(false);
            var isAuthenticated = _signInManager.IsSignedIn(claimUser);
            
            return Ok(ApiResponse<bool>.CreateSuccess(isAuthenticated, "Authentication status retrieved successfully"));
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering users by email</param>
        /// <returns>Paginated list of users</returns>
        /// <response code="200">Returns paginated list of users</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));
            
            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _userManager.Users.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.Email.Contains(searchTerm) || u.UserName.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LastPasswordChanged = user.LastPasswordChanged
                })
                .ToListAsync();

            // Get roles for each user
            foreach (var userDto in users)
            {
                var user = await _userManager.FindByIdAsync(userDto.Id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDto.Roles = roles;
                }
            }

            var pagedResponse = new PagedResponseDto<UserResponseDto>
            {
                Items = users,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<UserResponseDto>>.CreateSuccess(pagedResponse, "Users retrieved successfully"));
        }

        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <returns>User details</returns>
        /// <response code="200">Returns user details</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="404">User not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUser([Required] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ApiResponse.CreateError("User ID is required"));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LastPasswordChanged = user.LastPasswordChanged,
                Roles = roles
            };

            return Ok(ApiResponse<UserResponseDto>.CreateSuccess(userDto, "User retrieved successfully"));
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="model">User creation data</param>
        /// <returns>Created user details</returns>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">User already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return Conflict(ApiResponse.CreateError("User with this email already exists", 409));

            var user = new ApplicationUser(_userManager)
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = model.EmailConfirmed,
                PhoneNumber = model.PhoneNumber,
                LastPasswordChanged = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var userDto = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LastPasswordChanged = user.LastPasswordChanged,
                    Roles = new List<string>()
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, 
                    ApiResponse<UserResponseDto>.CreateSuccess(userDto, "User created successfully", 201));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "User creation failed"));
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <param name="model">User update data</param>
        /// <returns>Updated user details</returns>
        /// <response code="200">User updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">User not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateUser([Required] string id, [FromBody] UpdateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            // Check if email is being changed and if new email already exists
            if (user.Email != model.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                    return BadRequest(ApiResponse.CreateError("Email is already in use by another user"));
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumber = model.PhoneNumber;
            user.LockoutEnabled = model.LockoutEnabled;
            user.TwoFactorEnabled = model.TwoFactorEnabled;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LastPasswordChanged = user.LastPasswordChanged,
                    Roles = roles
                };

                return Ok(ApiResponse<UserResponseDto>.CreateSuccess(userDto, "User updated successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "User update failed"));
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">User deleted successfully</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteUser([Required] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ApiResponse.CreateError("User ID is required"));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(ApiResponse.CreateSuccess("User deleted successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "User deletion failed"));
        }

        /// <summary>
        /// Lock out a user account
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <param name="lockoutEnd">When the lockout should end (optional, defaults to indefinite)</param>
        /// <returns>Lockout confirmation</returns>
        /// <response code="200">User locked out successfully</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="404">User not found</response>
        [HttpPost("{id}/lockout")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> LockoutUser([Required] string id, [FromQuery] DateTime? lockoutEnd = null)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ApiResponse.CreateError("User ID is required"));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var lockoutEndDate = lockoutEnd ?? DateTimeOffset.MaxValue;
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);
            
            if (result.Succeeded)
            {
                return Ok(ApiResponse.CreateSuccess("User locked out successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Lockout failed"));
        }

        /// <summary>
        /// Unlock a user account
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <returns>Unlock confirmation</returns>
        /// <response code="200">User unlocked successfully</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="404">User not found</response>
        [HttpPost("{id}/unlock")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UnlockUser([Required] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ApiResponse.CreateError("User ID is required"));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            
            if (result.Succeeded)
            {
                // Reset access failed count
                await _userManager.ResetAccessFailedCountAsync(user);
                return Ok(ApiResponse.CreateSuccess("User unlocked successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Unlock failed"));
        }

        /// <summary>
        /// Reset user password (Admin only)
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <param name="newPassword">New password for the user</param>
        /// <returns>Password reset confirmation</returns>
        /// <response code="200">Password reset successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">User not found</response>
        [HttpPost("{id}/reset-password")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> ResetUserPassword([Required] string id, [FromBody][Required] string newPassword)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ApiResponse.CreateError("User ID is required"));

            if (string.IsNullOrEmpty(newPassword))
                return BadRequest(ApiResponse.CreateError("New password is required"));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // Reset password using the token
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            
            if (result.Succeeded)
            {
                user.LastPasswordChanged = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                return Ok(ApiResponse.CreateSuccess("Password reset successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Password reset failed"));
        }
    }
}
