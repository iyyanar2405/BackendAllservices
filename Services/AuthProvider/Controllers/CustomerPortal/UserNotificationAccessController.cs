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
    /// User Notification Access management operations for notification preferences
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - User Notification Access")]
    public class UserNotificationAccessController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        public UserNotificationAccessController(CustomerPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<object>>), 200)]
        public async Task<IActionResult> GetUserNotificationAccess(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? userId = null,
            [FromQuery] int? categoryId = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.UserNotificationAccess
                .Include(u => u.User)
                .Include(u => u.NotificationCategory)
                .AsQueryable();

            if (userId.HasValue) query = query.Where(u => u.UserId == userId.Value);
            if (categoryId.HasValue) query = query.Where(u => u.NotificationCategoryId == categoryId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.User.LastName)
                .ThenBy(u => u.NotificationCategory.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    CategoryId = u.NotificationCategoryId,
                    CategoryName = u.NotificationCategory.Name,
                    u.IsEnabled,
                    u.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<object> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<object>>.CreateSuccess(response, "User notification access records retrieved successfully"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetUserNotificationAccess([Required] int id)
        {
            var userAccess = await _context.UserNotificationAccess
                .Include(u => u.User)
                .Include(u => u.NotificationCategory)
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    CategoryId = u.NotificationCategoryId,
                    CategoryName = u.NotificationCategory.Name,
                    u.IsEnabled,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (userAccess == null)
                return NotFound(ApiResponse.CreateError("User notification access not found", 404));

            return Ok(ApiResponse<object>.CreateSuccess(userAccess, "User notification access retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), 201)]
        public async Task<IActionResult> CreateUserNotificationAccess([FromBody] CreateUserAccessDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId && u.IsActive);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var categoryExists = await _context.NotificationCategories.AnyAsync(c => c.Id == model.EntityId && c.IsActive);
            if (!categoryExists) return NotFound(ApiResponse.CreateError("Notification category not found or inactive", 404));

            var existing = await _context.UserNotificationAccess.FirstOrDefaultAsync(u => u.UserId == model.UserId && u.NotificationCategoryId == model.EntityId);
            if (existing != null) return Conflict(ApiResponse.CreateError("User notification access already exists", 409));

            var userNotificationAccess = new UserNotificationAccess
            {
                UserId = model.UserId,
                NotificationCategoryId = model.EntityId,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserNotificationAccess.Add(userNotificationAccess);
            await _context.SaveChangesAsync();

            await _context.Entry(userNotificationAccess).Reference(u => u.User).LoadAsync();
            await _context.Entry(userNotificationAccess).Reference(u => u.NotificationCategory).LoadAsync();

            var dto = new
            {
                userNotificationAccess.Id,
                userNotificationAccess.UserId,
                UserName = $"{userNotificationAccess.User.FirstName} {userNotificationAccess.User.LastName}",
                CategoryId = userNotificationAccess.NotificationCategoryId,
                CategoryName = userNotificationAccess.NotificationCategory.Name,
                userNotificationAccess.IsEnabled,
                userNotificationAccess.CreatedAt
            };

            return CreatedAtAction(nameof(GetUserNotificationAccess), new { id = userNotificationAccess.Id },
                ApiResponse<object>.CreateSuccess(dto, "User notification access created successfully", 201));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdateUserNotificationAccess([Required] int id, [FromBody] UpdateUserAccessDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var userNotificationAccess = await _context.UserNotificationAccess
                .Include(u => u.User)
                .Include(u => u.NotificationCategory)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userNotificationAccess == null)
                return NotFound(ApiResponse.CreateError("User notification access not found", 404));

            userNotificationAccess.IsEnabled = model.IsActive;
            await _context.SaveChangesAsync();

            var dto = new
            {
                userNotificationAccess.Id,
                userNotificationAccess.UserId,
                UserName = $"{userNotificationAccess.User.FirstName} {userNotificationAccess.User.LastName}",
                CategoryId = userNotificationAccess.NotificationCategoryId,
                CategoryName = userNotificationAccess.NotificationCategory.Name,
                userNotificationAccess.IsEnabled,
                userNotificationAccess.CreatedAt
            };

            return Ok(ApiResponse<object>.CreateSuccess(dto, "User notification access updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> DeleteUserNotificationAccess([Required] int id)
        {
            var userNotificationAccess = await _context.UserNotificationAccess.FindAsync(id);
            if (userNotificationAccess == null) return NotFound(ApiResponse.CreateError("User notification access not found", 404));

            _context.UserNotificationAccess.Remove(userNotificationAccess);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("User notification access deleted successfully"));
        }

        [HttpGet("user/{userId}/categories")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        public async Task<IActionResult> GetUserNotificationCategories([Required] int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists) return NotFound(ApiResponse.CreateError("User not found", 404));

            var categories = await _context.UserNotificationAccess
                .Include(u => u.NotificationCategory)
                .Where(u => u.UserId == userId && u.NotificationCategory.IsActive)
                .Select(u => new { u.Id, CategoryId = u.NotificationCategoryId, u.NotificationCategory.Name, u.IsEnabled, u.CreatedAt })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(categories, "User notification categories retrieved successfully"));
        }
    }
}