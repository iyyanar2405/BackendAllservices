using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthProvider.Context;
using AuthProvider.Models.CustomerPortal;
using AuthProvider.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CustomerPortalUserResponseDto = AuthProvider.DTOs.CustomerPortal.UserResponseDto;
using CustomerPortalCreateUserDto = AuthProvider.DTOs.CustomerPortal.CreateUserDto;
using CustomerPortalUpdateUserDto = AuthProvider.DTOs.CustomerPortal.UpdateUserDto;
using ActionResponseDto = AuthProvider.DTOs.CustomerPortal.ActionResponseDto;

namespace AuthProvider.Controllers.CustomerPortal
{
    /// <summary>
    /// Users management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Users")]
    public class UsersController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the UsersController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public UsersController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering users by name or email</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="department">Filter by department</param>
        /// <returns>Paginated list of users</returns>
        /// <response code="200">Returns paginated list of users</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<CustomerPortalUserResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? department = null)
        {
            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserCityAccesses).ThenInclude(uca => uca.City)
                .Include(u => u.UserCountryAccesses).ThenInclude(uca => uca.Country)
                .Include(u => u.UserServiceAccesses).ThenInclude(usa => usa.Service)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.FirstName.Contains(searchTerm) ||
                                       u.LastName.Contains(searchTerm) ||
                                       u.Email.Contains(searchTerm) ||
                                       (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(u => u.Department != null && u.Department.Contains(department));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new CustomerPortalUserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Department = u.Department,
                    JobTitle = u.JobTitle,
                    DateOfBirth = u.DateOfBirth,
                    HireDate = u.HireDate,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name),
                    Cities = u.UserCityAccesses.Select(uca => uca.City.Name),
                    Countries = u.UserCountryAccesses.Select(uca => uca.Country.Name),
                    Services = u.UserServiceAccesses.Select(usa => usa.Service.Name)
                })
                .ToListAsync();

            var pagedResponse = new PagedResponseDto<CustomerPortalUserResponseDto>
            {
                Items = users,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<CustomerPortalUserResponseDto>>.CreateSuccess(pagedResponse, "Users retrieved successfully"));
        }

        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <returns>User details</returns>
        /// <response code="200">Returns user details</response>
        /// <response code="404">User not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerPortalUserResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUser([Required] int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserCityAccesses).ThenInclude(uca => uca.City)
                .Include(u => u.UserCountryAccesses).ThenInclude(uca => uca.Country)
                .Include(u => u.UserServiceAccesses).ThenInclude(usa => usa.Service)
                .Where(u => u.Id == id)
                .Select(u => new CustomerPortalUserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Department = u.Department,
                    JobTitle = u.JobTitle,
                    DateOfBirth = u.DateOfBirth,
                    HireDate = u.HireDate,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name),
                    Cities = u.UserCityAccesses.Select(uca => uca.City.Name),
                    Countries = u.UserCountryAccesses.Select(uca => uca.Country.Name),
                    Services = u.UserServiceAccesses.Select(usa => usa.Service.Name)
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            return Ok(ApiResponse<CustomerPortalUserResponseDto>.CreateSuccess(user, "User retrieved successfully"));
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="model">User creation data</param>
        /// <returns>Created user details</returns>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">User with email already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CustomerPortalUserResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateUser([FromBody] CustomerPortalCreateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            // Check if user with email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return Conflict(ApiResponse.CreateError("User with this email already exists", 409));

            var user = new CustomerPortalUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Department = model.Department,
                JobTitle = model.JobTitle,
                DateOfBirth = model.DateOfBirth,
                HireDate = model.HireDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new CustomerPortalUserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Department = user.Department,
                JobTitle = user.JobTitle,
                DateOfBirth = user.DateOfBirth,
                HireDate = user.HireDate,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = new List<string>(),
                Cities = new List<string>(),
                Countries = new List<string>(),
                Services = new List<string>()
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id },
                ApiResponse<CustomerPortalUserResponseDto>.CreateSuccess(userDto, "User created successfully", 201));
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
        /// <response code="409">Email is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerPortalUserResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateUser([Required] int id, [FromBody] CustomerPortalUpdateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            // Check if email is being changed and if new email already exists
            if (user.Email != model.Email)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Id != id);
                if (existingUser != null)
                    return Conflict(ApiResponse.CreateError("Email is already in use", 409));
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Department = model.Department;
            user.JobTitle = model.JobTitle;
            user.DateOfBirth = model.DateOfBirth;
            user.HireDate = model.HireDate;
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Load related data for response
            await _context.Entry(user)
                .Collection(u => u.UserRoles)
                .Query()
                .Include(ur => ur.Role)
                .LoadAsync();

            var userDto = new CustomerPortalUserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Department = user.Department,
                JobTitle = user.JobTitle,
                DateOfBirth = user.DateOfBirth,
                HireDate = user.HireDate,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = user.UserRoles.Select(ur => ur.Role.Name),
                Cities = new List<string>(),
                Countries = new List<string>(),
                Services = new List<string>()
            };

            return Ok(ApiResponse<CustomerPortalUserResponseDto>.CreateSuccess(userDto, "User updated successfully"));
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">User deleted successfully</response>
        /// <response code="400">Cannot delete user with existing relationships</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteUser([Required] int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .Include(u => u.AuditTeamMembers)
                .Include(u => u.AssignedActions)
                .Include(u => u.CreatedActions)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            // Check if user has any relationships that prevent deletion
            if (user.AuditTeamMembers.Any() || user.AssignedActions.Any() || user.CreatedActions.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete user with existing audit assignments or actions. Please reassign or remove them first."));

            // Remove all user access relationships
            _context.UserCityAccess.RemoveRange(user.UserCityAccesses);
            _context.UserCountryAccess.RemoveRange(user.UserCountryAccesses);
            _context.UserServiceAccess.RemoveRange(user.UserServiceAccesses);
            _context.UserNotificationAccess.RemoveRange(user.UserNotificationAccesses);
            _context.UserPreferences.RemoveRange(user.UserPreferences);
            _context.UserRoles.RemoveRange(user.UserRoles);
            _context.UserTrainings.RemoveRange(user.UserTrainings);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("User deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a user
        /// </summary>
        /// <param name="id">User unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">User status updated successfully</response>
        /// <response code="404">User not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateUserStatus([Required] int id, [FromQuery] bool isActive)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var status = isActive ? "activated" : "deactivated";
            return Ok(ApiResponse.CreateSuccess($"User {status} successfully"));
        }

        /// <summary>
        /// Search users by name or email
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching users</returns>
        /// <response code="200">Returns matching users</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerPortalUserResponseDto>>), 200)]
        public async Task<IActionResult> SearchUsers([FromQuery][Required] string searchTerm)
        {
            var users = await _context.Users
                .Where(u => u.IsActive && (u.FirstName.Contains(searchTerm) ||
                           u.LastName.Contains(searchTerm) ||
                           u.Email.Contains(searchTerm)))
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new CustomerPortalUserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Department = u.Department,
                    JobTitle = u.JobTitle,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<CustomerPortalUserResponseDto>>.CreateSuccess(users, "Users search completed successfully"));
        }

        /// <summary>
        /// Get user's actions
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of user's actions</returns>
        /// <response code="200">Returns user's actions</response>
        /// <response code="404">User not found</response>
        [HttpGet("{id}/actions")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ActionResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUserActions([Required] int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == id);
            if (!userExists)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Actions
                .Include(a => a.AssignedToUser)
                .Include(a => a.CreatedByUser)
                .Where(a => a.AssignedToUserId == id || a.CreatedByUserId == id);

            var totalCount = await query.CountAsync();
            var actions = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ActionResponseDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    AssignedToUserId = a.AssignedToUserId,
                    AssignedToUserName = a.AssignedToUser != null ? $"{a.AssignedToUser.FirstName} {a.AssignedToUser.LastName}" : null,
                    CreatedByUserId = a.CreatedByUserId,
                    CreatedByUserName = a.CreatedByUser != null ? $"{a.CreatedByUser.FirstName} {a.CreatedByUser.LastName}" : null,
                    DueDate = a.DueDate,
                    Status = a.Status,
                    Priority = a.Priority,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

            var pagedResponse = new PagedResponseDto<ActionResponseDto>
            {
                Items = actions,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<ActionResponseDto>>.CreateSuccess(pagedResponse, "User actions retrieved successfully"));
        }
    }
}