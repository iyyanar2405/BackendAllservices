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
    /// Notification Categories management operations for categorizing notifications
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Notification Categories")]
    public class NotificationCategoriesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the NotificationCategoriesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public NotificationCategoriesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all notification categories with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering categories by name or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of notification categories</returns>
        /// <response code="200">Returns paginated list of notification categories</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<NotificationCategoryResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetNotificationCategories(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.NotificationCategories
                .Include(nc => nc.UserNotificationAccesses)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(nc => nc.Name.Contains(searchTerm) || (nc.Description != null && nc.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(nc => nc.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(nc => nc.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(nc => new NotificationCategoryResponseDto
                {
                    Id = nc.Id,
                    Name = nc.Name,
                    Description = nc.Description,
                    IsActive = nc.IsActive,
                    CreatedAt = nc.CreatedAt,
                    UsersCount = nc.UserNotificationAccesses.Count()
                })
                .ToListAsync();

            var response = new PagedResponseDto<NotificationCategoryResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<NotificationCategoryResponseDto>>.CreateSuccess(response, "Notification categories retrieved successfully"));
        }

        /// <summary>
        /// Get a specific notification category by ID
        /// </summary>
        /// <param name="id">Notification category unique identifier</param>
        /// <returns>Notification category details</returns>
        /// <response code="200">Returns notification category details</response>
        /// <response code="404">Notification category not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<NotificationCategoryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetNotificationCategory([Required] int id)
        {
            var category = await _context.NotificationCategories
                .Include(nc => nc.UserNotificationAccesses)
                .Where(nc => nc.Id == id)
                .Select(nc => new NotificationCategoryResponseDto
                {
                    Id = nc.Id,
                    Name = nc.Name,
                    Description = nc.Description,
                    IsActive = nc.IsActive,
                    CreatedAt = nc.CreatedAt,
                    UsersCount = nc.UserNotificationAccesses.Count()
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound(ApiResponse.CreateError("Notification category not found", 404));

            return Ok(ApiResponse<NotificationCategoryResponseDto>.CreateSuccess(category, "Notification category retrieved successfully"));
        }

        /// <summary>
        /// Create a new notification category
        /// </summary>
        /// <param name="model">Notification category creation data</param>
        /// <returns>Created notification category details</returns>
        /// <response code="201">Notification category created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Notification category with name already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<NotificationCategoryResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateNotificationCategory([FromBody] CreateSimpleCategoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.NotificationCategories.FirstOrDefaultAsync(nc => nc.Name == model.Name);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Notification category with this name already exists", 409));

            var category = new NotificationCategory
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.NotificationCategories.Add(category);
            await _context.SaveChangesAsync();

            var dto = new NotificationCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UsersCount = 0
            };

            return CreatedAtAction(nameof(GetNotificationCategory), new { id = category.Id },
                ApiResponse<NotificationCategoryResponseDto>.CreateSuccess(dto, "Notification category created successfully", 201));
        }

        /// <summary>
        /// Update an existing notification category
        /// </summary>
        /// <param name="id">Notification category unique identifier</param>
        /// <param name="model">Notification category update data</param>
        /// <returns>Updated notification category details</returns>
        /// <response code="200">Notification category updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Notification category not found</response>
        /// <response code="409">Notification category name is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<NotificationCategoryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateNotificationCategory([Required] int id, [FromBody] UpdateSimpleCategoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var category = await _context.NotificationCategories
                .Include(nc => nc.UserNotificationAccesses)
                .FirstOrDefaultAsync(nc => nc.Id == id);
            if (category == null)
                return NotFound(ApiResponse.CreateError("Notification category not found", 404));

            if (category.Name != model.Name)
            {
                var existing = await _context.NotificationCategories.FirstOrDefaultAsync(nc => nc.Name == model.Name && nc.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Notification category name is already in use", 409));
            }

            category.Name = model.Name;
            category.Description = model.Description;
            category.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new NotificationCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UsersCount = category.UserNotificationAccesses.Count()
            };

            return Ok(ApiResponse<NotificationCategoryResponseDto>.CreateSuccess(dto, "Notification category updated successfully"));
        }

        /// <summary>
        /// Delete a notification category
        /// </summary>
        /// <param name="id">Notification category unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Notification category deleted successfully</response>
        /// <response code="400">Cannot delete category with existing user relationships</response>
        /// <response code="404">Notification category not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteNotificationCategory([Required] int id)
        {
            var category = await _context.NotificationCategories
                .Include(nc => nc.UserNotificationAccesses)
                .FirstOrDefaultAsync(nc => nc.Id == id);

            if (category == null)
                return NotFound(ApiResponse.CreateError("Notification category not found", 404));

            if (category.UserNotificationAccesses.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete notification category with existing user access relationships"));

            _context.NotificationCategories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Notification category deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a notification category
        /// </summary>
        /// <param name="id">Notification category unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Notification category status updated successfully</response>
        /// <response code="404">Notification category not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateNotificationCategoryStatus([Required] int id, [FromQuery] bool isActive)
        {
            var category = await _context.NotificationCategories.FindAsync(id);
            if (category == null)
                return NotFound(ApiResponse.CreateError("Notification category not found", 404));

            category.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Notification category {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Get users with access to a notification category
        /// </summary>
        /// <param name="id">Notification category ID</param>
        /// <returns>List of users with access to this notification category</returns>
        /// <response code="200">Returns users with access to this notification category</response>
        /// <response code="404">Notification category not found</response>
        [HttpGet("{id}/users")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetNotificationCategoryUsers([Required] int id)
        {
            var categoryExists = await _context.NotificationCategories.AnyAsync(nc => nc.Id == id);
            if (!categoryExists)
                return NotFound(ApiResponse.CreateError("Notification category not found", 404));

            var users = await _context.UserNotificationAccess
                .Include(una => una.User)
                .Where(una => una.NotificationCategoryId == id)
                .Select(una => new
                {
                    Id = una.Id,
                    UserId = una.UserId,
                    UserName = $"{una.User.FirstName} {una.User.LastName}",
                    UserEmail = una.User.Email,
                    IsEnabled = una.IsEnabled,
                    CreatedAt = una.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(users, "Notification category users retrieved successfully"));
        }

        /// <summary>
        /// Search notification categories by name or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching notification categories</returns>
        /// <response code="200">Returns matching notification categories</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationCategoryResponseDto>>), 200)]
        public async Task<IActionResult> SearchNotificationCategories([FromQuery][Required] string searchTerm)
        {
            var categories = await _context.NotificationCategories
                .Where(nc => nc.IsActive && (nc.Name.Contains(searchTerm) || (nc.Description != null && nc.Description.Contains(searchTerm))))
                .OrderBy(nc => nc.Name)
                .Select(nc => new NotificationCategoryResponseDto
                {
                    Id = nc.Id,
                    Name = nc.Name,
                    Description = nc.Description,
                    IsActive = nc.IsActive,
                    CreatedAt = nc.CreatedAt,
                    UsersCount = nc.UserNotificationAccesses.Count()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<NotificationCategoryResponseDto>>.CreateSuccess(categories, "Notification categories search completed successfully"));
        }
    }
}