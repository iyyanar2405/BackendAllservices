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
    /// Actions management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Actions")]
    public class ActionsController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the ActionsController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public ActionsController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all actions with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering actions by title</param>
        /// <param name="status">Filter by action status</param>
        /// <param name="priority">Filter by action priority</param>
        /// <param name="assignedToUserId">Filter by assigned user</param>
        /// <returns>Paginated list of actions</returns>
        /// <response code="200">Returns paginated list of actions</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ActionResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetActions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] ActionStatus? status = null,
            [FromQuery] ActionPriority? priority = null,
            [FromQuery] int? assignedToUserId = null)
        {
            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Actions
                .Include(a => a.AssignedToUser)
                .Include(a => a.CreatedByUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm) ||
                                       (a.Description != null && a.Description.Contains(searchTerm)));
            }

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(a => a.Priority == priority.Value);
            }

            if (assignedToUserId.HasValue)
            {
                query = query.Where(a => a.AssignedToUserId == assignedToUserId.Value);
            }

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

            return Ok(ApiResponse<PagedResponseDto<ActionResponseDto>>.CreateSuccess(pagedResponse, "Actions retrieved successfully"));
        }

        /// <summary>
        /// Get a specific action by ID
        /// </summary>
        /// <param name="id">Action unique identifier</param>
        /// <returns>Action details</returns>
        /// <response code="200">Returns action details</response>
        /// <response code="404">Action not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ActionResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAction([Required] int id)
        {
            var action = await _context.Actions
                .Include(a => a.AssignedToUser)
                .Include(a => a.CreatedByUser)
                .Where(a => a.Id == id)
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
                .FirstOrDefaultAsync();

            if (action == null)
                return NotFound(ApiResponse.CreateError("Action not found", 404));

            return Ok(ApiResponse<ActionResponseDto>.CreateSuccess(action, "Action retrieved successfully"));
        }

        /// <summary>
        /// Create a new action
        /// </summary>
        /// <param name="model">Action creation data</param>
        /// <returns>Created action details</returns>
        /// <response code="201">Action created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Assigned user not found</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ActionResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> CreateAction([FromBody] CreateActionDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            // Validate due date is in the future
            if (model.DueDate <= DateTime.UtcNow)
                return BadRequest(ApiResponse.CreateError("Due date must be in the future"));

            // Check if assigned user exists
            if (model.AssignedToUserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == model.AssignedToUserId.Value && u.IsActive);
                if (!userExists)
                    return NotFound(ApiResponse.CreateError("Assigned user not found or inactive", 404));
            }

            var action = new CustomerPortalAction
            {
                Title = model.Title,
                Description = model.Description,
                AssignedToUserId = model.AssignedToUserId,
                CreatedByUserId = GetCurrentUserId(), // You'll need to implement this method
                DueDate = model.DueDate,
                Status = ActionStatus.Open,
                Priority = model.Priority,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Actions.Add(action);
            await _context.SaveChangesAsync();

            // Load related data for response
            await _context.Entry(action)
                .Reference(a => a.AssignedToUser)
                .LoadAsync();
            await _context.Entry(action)
                .Reference(a => a.CreatedByUser)
                .LoadAsync();

            var actionDto = new ActionResponseDto
            {
                Id = action.Id,
                Title = action.Title,
                Description = action.Description,
                AssignedToUserId = action.AssignedToUserId,
                AssignedToUserName = action.AssignedToUser != null ? $"{action.AssignedToUser.FirstName} {action.AssignedToUser.LastName}" : null,
                CreatedByUserId = action.CreatedByUserId,
                CreatedByUserName = action.CreatedByUser != null ? $"{action.CreatedByUser.FirstName} {action.CreatedByUser.LastName}" : null,
                DueDate = action.DueDate,
                Status = action.Status,
                Priority = action.Priority,
                CreatedAt = action.CreatedAt,
                UpdatedAt = action.UpdatedAt
            };

            return CreatedAtAction(nameof(GetAction), new { id = action.Id },
                ApiResponse<ActionResponseDto>.CreateSuccess(actionDto, "Action created successfully", 201));
        }

        /// <summary>
        /// Update an existing action
        /// </summary>
        /// <param name="id">Action unique identifier</param>
        /// <param name="model">Action update data</param>
        /// <returns>Updated action details</returns>
        /// <response code="200">Action updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Action or assigned user not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ActionResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateAction([Required] int id, [FromBody] UpdateActionDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var action = await _context.Actions
                .Include(a => a.AssignedToUser)
                .Include(a => a.CreatedByUser)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (action == null)
                return NotFound(ApiResponse.CreateError("Action not found", 404));

            // Check if assigned user exists
            if (model.AssignedToUserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == model.AssignedToUserId.Value && u.IsActive);
                if (!userExists)
                    return NotFound(ApiResponse.CreateError("Assigned user not found or inactive", 404));
            }

            action.Title = model.Title;
            action.Description = model.Description;
            action.AssignedToUserId = model.AssignedToUserId;
            action.DueDate = model.DueDate;
            action.Status = model.Status;
            action.Priority = model.Priority;
            action.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var actionDto = new ActionResponseDto
            {
                Id = action.Id,
                Title = action.Title,
                Description = action.Description,
                AssignedToUserId = action.AssignedToUserId,
                AssignedToUserName = action.AssignedToUser != null ? $"{action.AssignedToUser.FirstName} {action.AssignedToUser.LastName}" : null,
                CreatedByUserId = action.CreatedByUserId,
                CreatedByUserName = action.CreatedByUser != null ? $"{action.CreatedByUser.FirstName} {action.CreatedByUser.LastName}" : null,
                DueDate = action.DueDate,
                Status = action.Status,
                Priority = action.Priority,
                CreatedAt = action.CreatedAt,
                UpdatedAt = action.UpdatedAt
            };

            return Ok(ApiResponse<ActionResponseDto>.CreateSuccess(actionDto, "Action updated successfully"));
        }

        /// <summary>
        /// Delete an action
        /// </summary>
        /// <param name="id">Action unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Action deleted successfully</response>
        /// <response code="404">Action not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteAction([Required] int id)
        {
            var action = await _context.Actions.FindAsync(id);
            if (action == null)
                return NotFound(ApiResponse.CreateError("Action not found", 404));

            _context.Actions.Remove(action);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Action deleted successfully"));
        }

        /// <summary>
        /// Update action status
        /// </summary>
        /// <param name="id">Action unique identifier</param>
        /// <param name="status">New status</param>
        /// <returns>Status update confirmation</returns>
        /// <response code="200">Action status updated successfully</response>
        /// <response code="404">Action not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateActionStatus([Required] int id, [FromQuery][Required] ActionStatus status)
        {
            var action = await _context.Actions.FindAsync(id);
            if (action == null)
                return NotFound(ApiResponse.CreateError("Action not found", 404));

            action.Status = status;
            action.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Action status updated to {status} successfully"));
        }

        /// <summary>
        /// Get actions by assigned user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of user's actions</returns>
        /// <response code="200">Returns user's actions</response>
        /// <response code="404">User not found</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ActionResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetActionsByUser([Required] int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            return await GetActions(pageNumber, pageSize, null, null, null, userId);
        }

        /// <summary>
        /// Get overdue actions
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of overdue actions</returns>
        /// <response code="200">Returns overdue actions</response>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ActionResponseDto>>), 200)]
        public async Task<IActionResult> GetOverdueActions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Actions
                .Include(a => a.AssignedToUser)
                .Include(a => a.CreatedByUser)
                .Where(a => a.DueDate < DateTime.UtcNow && a.Status != ActionStatus.Completed && a.Status != ActionStatus.Cancelled);

            var totalCount = await query.CountAsync();
            var actions = await query
                .OrderBy(a => a.DueDate)
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

            return Ok(ApiResponse<PagedResponseDto<ActionResponseDto>>.CreateSuccess(pagedResponse, "Overdue actions retrieved successfully"));
        }

        /// <summary>
        /// Helper method to get current user ID - implement based on your authentication system
        /// </summary>
        private int? GetCurrentUserId()
        {
            // TODO: Implement based on your authentication system
            // This is a placeholder - you would typically get this from JWT claims or session
            return 1; // Replace with actual implementation
        }
    }
}