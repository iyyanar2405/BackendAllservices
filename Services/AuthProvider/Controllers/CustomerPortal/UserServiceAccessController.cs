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
    /// User Service Access management operations for service-level user permissions
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - User Service Access")]
    public class UserServiceAccessController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        public UserServiceAccessController(CustomerPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<UserAccessResponseDto>>), 200)]
        public async Task<IActionResult> GetUserServiceAccess(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? userId = null,
            [FromQuery] int? serviceId = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.UserServiceAccess
                .Include(u => u.User)
                .Include(u => u.Service)
                .AsQueryable();

            if (userId.HasValue) query = query.Where(u => u.UserId == userId.Value);
            if (serviceId.HasValue) query = query.Where(u => u.ServiceId == serviceId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.User.LastName)
                .ThenBy(u => u.Service.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserAccessResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    EntityId = u.ServiceId,
                    EntityName = u.Service.Name,
                    GrantedAt = u.GrantedAt,
                    ExpiresAt = null,
                    IsActive = u.User.IsActive && u.Service.IsActive
                })
                .ToListAsync();

            var response = new PagedResponseDto<UserAccessResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<UserAccessResponseDto>>.CreateSuccess(response, "User service access records retrieved successfully"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserAccessResponseDto>), 200)]
        public async Task<IActionResult> GetUserServiceAccess([Required] int id)
        {
            var userAccess = await _context.UserServiceAccess
                .Include(u => u.User)
                .Include(u => u.Service)
                .Where(u => u.Id == id)
                .Select(u => new UserAccessResponseDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    EntityId = u.ServiceId,
                    EntityName = u.Service.Name,
                    GrantedAt = u.GrantedAt,
                    ExpiresAt = null,
                    IsActive = u.User.IsActive && u.Service.IsActive
                })
                .FirstOrDefaultAsync();

            if (userAccess == null)
                return NotFound(ApiResponse.CreateError("User service access not found", 404));

            return Ok(ApiResponse<UserAccessResponseDto>.CreateSuccess(userAccess, "User service access retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserAccessResponseDto>), 201)]
        public async Task<IActionResult> CreateUserServiceAccess([FromBody] CreateUserAccessDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId && u.IsActive);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var serviceExists = await _context.Services.AnyAsync(s => s.Id == model.EntityId && s.IsActive);
            if (!serviceExists) return NotFound(ApiResponse.CreateError("Service not found or inactive", 404));

            var existing = await _context.UserServiceAccess.FirstOrDefaultAsync(u => u.UserId == model.UserId && u.ServiceId == model.EntityId);
            if (existing != null) return Conflict(ApiResponse.CreateError("User service access already exists", 409));

            var userServiceAccess = new UserServiceAccess
            {
                UserId = model.UserId,
                ServiceId = model.EntityId,
                GrantedAt = DateTime.UtcNow
            };

            _context.UserServiceAccess.Add(userServiceAccess);
            await _context.SaveChangesAsync();

            await _context.Entry(userServiceAccess).Reference(u => u.User).LoadAsync();
            await _context.Entry(userServiceAccess).Reference(u => u.Service).LoadAsync();

            var dto = new UserAccessResponseDto
            {
                Id = userServiceAccess.Id,
                UserId = userServiceAccess.UserId,
                UserName = $"{userServiceAccess.User.FirstName} {userServiceAccess.User.LastName}",
                EntityId = userServiceAccess.ServiceId,
                EntityName = userServiceAccess.Service.Name,
                GrantedAt = userServiceAccess.GrantedAt,
                ExpiresAt = null,
                IsActive = userServiceAccess.User.IsActive && userServiceAccess.Service.IsActive
            };

            return CreatedAtAction(nameof(GetUserServiceAccess), new { id = userServiceAccess.Id },
                ApiResponse<UserAccessResponseDto>.CreateSuccess(dto, "User service access created successfully", 201));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> DeleteUserServiceAccess([Required] int id)
        {
            var userServiceAccess = await _context.UserServiceAccess.FindAsync(id);
            if (userServiceAccess == null) return NotFound(ApiResponse.CreateError("User service access not found", 404));

            _context.UserServiceAccess.Remove(userServiceAccess);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("User service access deleted successfully"));
        }

        [HttpGet("user/{userId}/services")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        public async Task<IActionResult> GetUserServices([Required] int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found", 404));

            var services = await _context.UserServiceAccess
                .Include(u => u.Service)
                .Where(u => u.UserId == userId && u.Service.IsActive)
                .Select(u => new { u.Id, u.ServiceId, u.Service.Name, u.Service.Code, u.Service.Description, u.GrantedAt })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(services, "User services retrieved successfully"));
        }

        [HttpGet("service/{serviceId}/users")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        public async Task<IActionResult> GetServiceUsers([Required] int serviceId)
        {
            var serviceExists = await _context.Services.AnyAsync(s => s.Id == serviceId);
            if (!serviceExists) return NotFound(ApiResponse.CreateError("Service not found", 404));

            var users = await _context.UserServiceAccess
                .Include(u => u.User)
                .Where(u => u.ServiceId == serviceId && u.User.IsActive)
                .Select(u => new { u.Id, u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", u.User.Email, u.User.Department, u.GrantedAt })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(users, "Service users retrieved successfully"));
        }

        [HttpPost("user/{userId}/services/bulk")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> BulkAssignServicesToUser([Required] int userId, [FromBody] List<int> serviceIds)
        {
            if (serviceIds == null || !serviceIds.Any())
                return BadRequest(ApiResponse.CreateError("Service IDs list cannot be empty"));

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId && u.IsActive);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var existingServices = await _context.Services
                .Where(s => serviceIds.Contains(s.Id) && s.IsActive)
                .Select(s => s.Id)
                .ToListAsync();

            if (existingServices.Count != serviceIds.Count)
                return NotFound(ApiResponse.CreateError("Some services not found or inactive", 404));

            var existingAccess = await _context.UserServiceAccess
                .Where(u => u.UserId == userId && serviceIds.Contains(u.ServiceId))
                .Select(u => u.ServiceId)
                .ToListAsync();

            var newServiceIds = serviceIds.Except(existingAccess).ToList();

            if (newServiceIds.Any())
            {
                var newAccess = newServiceIds.Select(serviceId => new UserServiceAccess
                {
                    UserId = userId,
                    ServiceId = serviceId,
                    GrantedAt = DateTime.UtcNow
                });

                _context.UserServiceAccess.AddRange(newAccess);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse.CreateSuccess($"Successfully assigned {newServiceIds.Count} new services to user. {existingAccess.Count} services were already assigned."));
        }
    }
}