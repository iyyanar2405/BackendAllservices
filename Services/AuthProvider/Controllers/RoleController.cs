using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AuthProvider.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthProvider.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthProvider.Controllers
{
    /// <summary>
    /// Role management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Role Management")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the RoleController
        /// </summary>
        /// <param name="roleManager">Role manager service</param>
        /// <param name="userManager">User manager service for role counting</param>
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        /// <summary>
        /// Get all roles with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering roles by name</param>
        /// <returns>Paginated list of roles</returns>
        /// <response code="200">Returns paginated list of roles</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<RoleResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetRoles(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));
            
            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _roleManager.Roles.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r => r.Name.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var roles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(role => new RoleResponseDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    UserCount = 0 // Will be populated below
                })
                .ToListAsync();

            // Get user count for each role
            foreach (var roleDto in roles)
            {
                var users = await _userManager.GetUsersInRoleAsync(roleDto.Name);
                roleDto.UserCount = users.Count;
            }

            var pagedResponse = new PagedResponseDto<RoleResponseDto>
            {
                Items = roles,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<RoleResponseDto>>.CreateSuccess(pagedResponse, "Roles retrieved successfully"));
        }

        /// <summary>
        /// Get a specific role by ID
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <returns>Role details</returns>
        /// <response code="200">Returns role details</response>
        /// <response code="400">Invalid role ID</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetRole([Required] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ApiResponse.CreateError("Role ID is required"));

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            var roleDto = new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                UserCount = users.Count
            };

            return Ok(ApiResponse<RoleResponseDto>.CreateSuccess(roleDto, "Role retrieved successfully"));
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="model">Role creation data</param>
        /// <returns>Created role details</returns>
        /// <response code="201">Role created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Role already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            // Check if role already exists
            var existingRole = await _roleManager.FindByNameAsync(model.Name);
            if (existingRole != null)
                return Conflict(ApiResponse.CreateError("Role with this name already exists", 409));

            var identityRole = new IdentityRole { Name = model.Name };
            var result = await _roleManager.CreateAsync(identityRole);
            
            if (result.Succeeded)
            {
                var roleDto = new RoleResponseDto
                {
                    Id = identityRole.Id,
                    Name = identityRole.Name,
                    Description = model.Description,
                    UserCount = 0
                };

                return CreatedAtAction(nameof(GetRole), new { id = identityRole.Id }, 
                    ApiResponse<RoleResponseDto>.CreateSuccess(roleDto, "Role created successfully", 201));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Role creation failed"));
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
        /// <response code="409">Role name already exists</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateRole([Required] string id, [FromBody] CreateRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            // Check if new name already exists (excluding current role)
            if (role.Name != model.Name)
            {
                var existingRole = await _roleManager.FindByNameAsync(model.Name);
                if (existingRole != null && existingRole.Id != role.Id)
                    return Conflict(ApiResponse.CreateError("Role name is already in use", 409));
            }

            role.Name = model.Name;
            var result = await _roleManager.UpdateAsync(role);
            
            if (result.Succeeded)
            {
                var users = await _userManager.GetUsersInRoleAsync(role.Name);
                var roleDto = new RoleResponseDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = model.Description,
                    UserCount = users.Count
                };

                return Ok(ApiResponse<RoleResponseDto>.CreateSuccess(roleDto, "Role updated successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Role update failed"));
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Role deleted successfully</response>
        /// <response code="400">Invalid role ID or role has users assigned</response>
        /// <response code="404">Role not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteRole([Required] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ApiResponse.CreateError("Role ID is required"));

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(ApiResponse.CreateError("Role not found", 404));

            // Check if role has users assigned
            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            if (users.Any())
                return BadRequest(ApiResponse.CreateError($"Cannot delete role '{role.Name}' because it has {users.Count} user(s) assigned"));

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok(ApiResponse.CreateSuccess("Role deleted successfully"));
            }

            var identityErrors = result.Errors.Select(x => x.Description);
            return BadRequest(ApiResponse.CreateValidationError(identityErrors, "Role deletion failed"));
        }

        /// <summary>
        /// Get all users assigned to a specific role
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <returns>Paginated list of users in the role</returns>
        /// <response code="200">Returns paginated list of users in the role</response>
        /// <response code="400">Invalid parameters</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{id}/users")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUsersInRole(
            [Required] string id, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ApiResponse.CreateError("Role ID is required"));

            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));
            
            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var role = await _roleManager.FindByIdAsync(id);
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
                    TwoFactorEnabled = user.TwoFactorEnabled
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
    }
}