using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthProvider.Context;
using AuthProvider.Models.CustomerPortal;
using AuthProvider.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RoleResponseDto = AuthProvider.DTOs.CustomerPortal.RoleResponseDto;
using CreateRoleDto = AuthProvider.DTOs.CustomerPortal.CreateRoleDto;
using UpdateRoleDto = AuthProvider.DTOs.CustomerPortal.UpdateRoleDto;

namespace AuthProvider.Controllers.CustomerPortal
{
    /// <summary>
    /// Roles management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Roles")]
    public class RolesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the RolesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public RolesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all roles with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering roles by name or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of roles</returns>
        /// <response code="200">Returns paginated list of roles</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<RoleResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetRoles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Roles
                .Include(r => r.UserRoles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(r => r.Name.Contains(searchTerm) || (r.Description != null && r.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(r => r.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(r => r.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UsersCount = r.UserRoles.Count()
                })
                .ToListAsync();

            var response = new PagedResponseDto<RoleResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<RoleResponseDto>>.CreateSuccess(response, "Roles retrieved successfully"));
        }

        /// <summary>
        /// Get a specific role by ID
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <returns>Role details</returns>
        /// <response code="200">Returns role details</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetRole([Required] int id)
        {
            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .Where(r => r.Id == id)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UsersCount = r.UserRoles.Count()
                })
                .FirstOrDefaultAsync();

            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            return Ok(ApiResponse<RoleResponseDto>.CreateSuccess(role, "Role retrieved successfully"));
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="model">Role creation data</param>
        /// <returns>Created role details</returns>
        /// <response code="201">Role created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Role with name already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.Roles.FirstOrDefaultAsync(r => r.Name == model.Name);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Role with this name already exists", 409));

            var role = new CustomerPortalRole
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var dto = new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                UsersCount = 0
            };

            return CreatedAtAction(nameof(GetRole), new { id = role.Id },
                ApiResponse<RoleResponseDto>.CreateSuccess(dto, "Role created successfully", 201));
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <param name="model">Role update data</param>
        /// <returns>Updated role details</returns>
        /// <response code="200">Role updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Role not found</response>
        /// <response code="409">Role name is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateRole([Required] int id, [FromBody] UpdateRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            if (role.Name != model.Name)
            {
                var existing = await _context.Roles.FirstOrDefaultAsync(r => r.Name == model.Name && r.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Role name is already in use", 409));
            }

            role.Name = model.Name;
            role.Description = model.Description;
            role.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                UsersCount = role.UserRoles.Count()
            };

            return Ok(ApiResponse<RoleResponseDto>.CreateSuccess(dto, "Role updated successfully"));
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Role deleted successfully</response>
        /// <response code="400">Cannot delete role with assigned users</response>
        /// <response code="404">Role not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteRole([Required] int id)
        {
            var role = await _context.Roles.Include(r => r.UserRoles).FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            if (role.UserRoles.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete role with assigned users"));

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Role deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a role
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Role status updated successfully</response>
        /// <response code="404">Role not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateRoleStatus([Required] int id, [FromQuery] bool isActive)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            role.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Role {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Get users assigned to a role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of users with this role</returns>
        /// <response code="200">Returns users with this role</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{id}/users")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetRoleUsers([Required] int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == id);
            if (!roleExists)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            var users = await _context.UserRoles
                .Include(ur => ur.User)
                .Where(ur => ur.RoleId == id)
                .Select(ur => new
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    UserName = $"{ur.User.FirstName} {ur.User.LastName}",
                    UserEmail = ur.User.Email,
                    AssignedAt = ur.AssignedAt,
                    ExpiresAt = ur.ExpiresAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(users, "Role users retrieved successfully"));
        }

        /// <summary>
        /// Search roles by name or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching roles</returns>
        /// <response code="200">Returns matching roles</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleResponseDto>>), 200)]
        public async Task<IActionResult> SearchRoles([FromQuery][Required] string searchTerm)
        {
            var roles = await _context.Roles
                .Where(r => r.IsActive && (r.Name.Contains(searchTerm) || (r.Description != null && r.Description.Contains(searchTerm))))
                .OrderBy(r => r.Name)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UsersCount = r.UserRoles.Count()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<RoleResponseDto>>.CreateSuccess(roles, "Roles search completed successfully"));
        }
    }
}