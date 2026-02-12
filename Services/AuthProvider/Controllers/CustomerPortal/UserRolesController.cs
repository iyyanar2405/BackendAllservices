using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthProvider.Context;
using AuthProvider.Models.CustomerPortal;
using AuthProvider.DTOs.CustomerPortal;
using AuthProvider.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthProvider.Controllers.CustomerPortal
{
    /// <summary>
    /// User Roles management operations for user role assignments
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - User Roles")]
    public class UserRolesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        public UserRolesController(CustomerPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserRoleResponseDto>>), 200)]
        public async Task<IActionResult> GetUserRoles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? userId = null,
            [FromQuery] int? roleId = null,
            [FromQuery] bool? includeExpired = false)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.UserRoles
                .Include(u => u.User)
                .Include(u => u.Role)
                .AsQueryable();

            if (userId.HasValue) query = query.Where(u => u.UserId == userId.Value);
            if (roleId.HasValue) query = query.Where(u => u.RoleId == roleId.Value);
            
            if (!includeExpired.GetValueOrDefault())
                query = query.Where(u => u.ExpiresAt == null || u.ExpiresAt > DateTime.UtcNow);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.User.LastName)
                .ThenBy(u => u.Role.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserRoleResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    AssignedAt = u.AssignedAt,
                    ExpiresAt = u.ExpiresAt,
                    IsActive = u.User.IsActive && u.Role.IsActive && (u.ExpiresAt == null || u.ExpiresAt > DateTime.UtcNow)
                })
                .ToListAsync();

            var response = new PagedResponseDto<UserRoleResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<UserRoleResponseDto>>.CreateSuccess(response, "User roles retrieved successfully"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserRoleResponseDto>), 200)]
        public async Task<IActionResult> GetUserRole([Required] int id)
        {
            var userRole = await _context.UserRoles
                .Include(u => u.User)
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .Select(u => new UserRoleResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    AssignedAt = u.AssignedAt,
                    ExpiresAt = u.ExpiresAt,
                    IsActive = u.User.IsActive && u.Role.IsActive && (u.ExpiresAt == null || u.ExpiresAt > DateTime.UtcNow)
                })
                .FirstOrDefaultAsync();

            if (userRole == null)
                return NotFound(ApiResponse.CreateError("User role not found", 404));

            return Ok(ApiResponse<UserRoleResponseDto>.CreateSuccess(userRole, "User role retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserRoleResponseDto>), 201)]
        public async Task<IActionResult> CreateUserRole([FromBody] CreateUserRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId && u.IsActive);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == model.RoleId && r.IsActive);
            if (!roleExists) return NotFound(ApiResponse.CreateError("Role not found or inactive", 404));

            var existing = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == model.UserId && u.RoleId == model.RoleId && (u.ExpiresAt == null || u.ExpiresAt > DateTime.UtcNow));
            if (existing != null) return Conflict(ApiResponse.CreateError("User already has this active role", 409));

            var userRole = new UserRole
            {
                UserId = model.UserId,
                RoleId = model.RoleId,
                AssignedAt = DateTime.UtcNow,
                ExpiresAt = model.ExpiresAt
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            await _context.Entry(userRole).Reference(u => u.User).LoadAsync();
            await _context.Entry(userRole).Reference(u => u.Role).LoadAsync();

            var dto = new UserRoleResponseDto
            {
                Id = userRole.Id,
                UserId = userRole.UserId,
                UserName = $"{userRole.User.FirstName} {userRole.User.LastName}",
                RoleId = userRole.RoleId,
                RoleName = userRole.Role.Name,
                AssignedAt = userRole.AssignedAt,
                ExpiresAt = userRole.ExpiresAt,
                IsActive = userRole.User.IsActive && userRole.Role.IsActive && (userRole.ExpiresAt == null || userRole.ExpiresAt > DateTime.UtcNow)
            };

            return CreatedAtAction(nameof(GetUserRole), new { id = userRole.Id },
                ApiResponse<UserRoleResponseDto>.CreateSuccess(dto, "User role created successfully", 201));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserRoleResponseDto>), 200)]
        public async Task<IActionResult> UpdateUserRole([Required] int id, [FromBody] UpdateUserRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userRole = await _context.UserRoles
                .Include(u => u.User)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userRole == null)
                return NotFound(ApiResponse.CreateError("User role not found", 404));

            userRole.ExpiresAt = model.ExpiresAt;

            await _context.SaveChangesAsync();

            var dto = new UserRoleResponseDto
            {
                Id = userRole.Id,
                UserId = userRole.UserId,
                UserName = $"{userRole.User.FirstName} {userRole.User.LastName}",
                RoleId = userRole.RoleId,
                RoleName = userRole.Role.Name,
                AssignedAt = userRole.AssignedAt,
                ExpiresAt = userRole.ExpiresAt,
                IsActive = userRole.User.IsActive && userRole.Role.IsActive && (userRole.ExpiresAt == null || userRole.ExpiresAt > DateTime.UtcNow)
            };

            return Ok(ApiResponse<UserRoleResponseDto>.CreateSuccess(dto, "User role updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> DeleteUserRole([Required] int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null) return NotFound(ApiResponse.CreateError("User role not found", 404));

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("User role deleted successfully"));
        }

        [HttpGet("user/{userId}/roles")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        public async Task<IActionResult> GetUserRolesByUser([Required] int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found", 404));

            var roles = await _context.UserRoles
                .Include(u => u.Role)
                .Where(u => u.UserId == userId && u.Role.IsActive && (u.ExpiresAt == null || u.ExpiresAt > DateTime.UtcNow))
                .Select(u => new { u.Id, u.RoleId, u.Role.Name, u.AssignedAt, u.ExpiresAt })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(roles, "User roles retrieved successfully"));
        }

        [HttpGet("role/{roleId}/users")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        public async Task<IActionResult> GetRoleUsers([Required] int roleId)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
            if (!roleExists) return NotFound(ApiResponse.CreateError("Role not found", 404));

            var users = await _context.UserRoles
                .Include(u => u.User)
                .Where(u => u.RoleId == roleId && u.User.IsActive && (u.ExpiresAt == null || u.ExpiresAt > DateTime.UtcNow))
                .Select(u => new { u.Id, u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", u.User.Email, u.AssignedAt, u.ExpiresAt })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(users, "Role users retrieved successfully"));
        }
    }
}