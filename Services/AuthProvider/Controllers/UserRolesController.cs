using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthProvider.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using AuthProvider.Models;

namespace AuthProvider.Controllers
{
    /// <summary>
    /// User-Role assignment operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("User Role Management")]
    public class UserRolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the UserRolesController
        /// </summary>
        /// <param name="userManager">User manager service</param>
        /// <param name="roleManager">Role manager service</param>
        public UserRolesController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Get all roles assigned to a user
        /// </summary>
        /// <param name="userId">User unique identifier</param>
        /// <returns>List of roles assigned to the user</returns>
        /// <response code="200">Returns list of user roles</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="404">User not found</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUserRoles([Required] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(ApiResponse.CreateError("User ID is required"));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = new List<RoleResponseDto>();

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
                    roles.Add(new RoleResponseDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                        UserCount = usersInRole.Count
                    });
                }
            }

            return Ok(ApiResponse<IEnumerable<RoleResponseDto>>.CreateSuccess(roles, "User roles retrieved successfully"));
        }

        /// <summary>
        /// Get current user's roles
        /// </summary>
        /// <returns>List of roles assigned to the current user</returns>
        /// <response code="200">Returns list of current user roles</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">User not found</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetCurrentUserRoles()
        {
            var userClaims = User.Identities;
            if (!userClaims.Any())
                return Unauthorized(ApiResponse.CreateError("User not authenticated", 401));

            var userName = userClaims.First().Claims.First().Value;
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(ApiResponse<IEnumerable<string>>.CreateSuccess(roles, "Current user roles retrieved successfully"));
        }

        /// <summary>
        /// Get all users assigned to a role
        /// </summary>
        /// <param name="roleId">Role unique identifier</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <returns>Paginated list of users in the role</returns>
        /// <response code="200">Returns paginated list of users in the role</response>
        /// <response code="400">Invalid parameters</response>
        /// <response code="404">Role not found</response>
        [HttpGet("role/{roleId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetRoleUsers(
            [Required] string roleId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(roleId))
                return BadRequest(ApiResponse.CreateError("Role ID is required"));

            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));
            
            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            var allUsersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            var totalCount = allUsersInRole.Count;
            
            var pagedUsers = allUsersInRole
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
                .ToList();

            var pagedResponse = new PagedResponseDto<UserResponseDto>
            {
                Items = pagedUsers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<UserResponseDto>>.CreateSuccess(pagedResponse, $"Users in role '{role.Name}' retrieved successfully"));
        }

        /// <summary>
        /// Assign a role to a user
        /// </summary>
        /// <param name="model">User-role assignment data</param>
        /// <returns>Assignment confirmation</returns>
        /// <response code="200">Role assigned successfully</response>
        /// <response code="400">Invalid user ID or role ID</response>
        /// <response code="404">User or role not found</response>
        /// <response code="409">User already has this role</response>
        [HttpPost("assign")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            // Check if user already has this role
            if (await _userManager.IsInRoleAsync(user, role.Name))
                return Conflict(ApiResponse.CreateError("User already has this role", 409));

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                return Ok(ApiResponse.CreateSuccess($"Role '{role.Name}' assigned to user '{user.Email}' successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Role assignment failed"));
        }

        /// <summary>
        /// Remove a role from a user
        /// </summary>
        /// <param name="model">User-role removal data</param>
        /// <returns>Removal confirmation</returns>
        /// <response code="200">Role removed successfully</response>
        /// <response code="400">Invalid user ID or role ID</response>
        /// <response code="404">User, role not found, or user doesn't have this role</response>
        [HttpDelete("remove")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            // Check if user has this role
            if (!await _userManager.IsInRoleAsync(user, role.Name))
                return NotFound(ApiResponse.CreateError("User does not have this role", 404));

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                return Ok(ApiResponse.CreateSuccess($"Role '{role.Name}' removed from user '{user.Email}' successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Role removal failed"));
        }

        /// <summary>
        /// Check if a user has a specific role
        /// </summary>
        /// <param name="userId">User unique identifier</param>
        /// <param name="roleId">Role unique identifier</param>
        /// <returns>Role assignment status</returns>
        /// <response code="200">Returns role assignment status</response>
        /// <response code="400">Invalid user ID or role ID</response>
        /// <response code="404">User or role not found</response>
        [HttpGet("check/{userId}/{roleId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> CheckUserHasRole([Required] string userId, [Required] string roleId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(ApiResponse.CreateError("User ID is required"));

            if (string.IsNullOrEmpty(roleId))
                return BadRequest(ApiResponse.CreateError("Role ID is required"));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            var hasRole = await _userManager.IsInRoleAsync(user, role.Name);
            return Ok(ApiResponse<bool>.CreateSuccess(hasRole, $"User {(hasRole ? "has" : "does not have")} role '{role.Name}'"));
        }
    }
}
