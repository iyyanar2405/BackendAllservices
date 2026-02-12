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
    /// Error Logs management operations for system error tracking and logging
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Error Logs")]
    public class ErrorLogsController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the ErrorLogsController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public ErrorLogsController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all error logs with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering logs by message or source</param>
        /// <param name="source">Filter by error source</param>
        /// <param name="userId">Filter by user</param>
        /// <param name="dateFrom">Filter by date from (inclusive)</param>
        /// <param name="dateTo">Filter by date to (inclusive)</param>
        /// <returns>Paginated list of error logs</returns>
        /// <response code="200">Returns paginated list of error logs</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ErrorLogResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetErrorLogs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? source = null,
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.ErrorLogs
                .Include(e => e.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(e => e.Message.Contains(searchTerm) || (e.Source != null && e.Source.Contains(searchTerm)));

            if (!string.IsNullOrWhiteSpace(source))
                query = query.Where(e => e.Source != null && e.Source.Contains(source));

            if (userId.HasValue)
                query = query.Where(e => e.UserId == userId.Value);

            if (dateFrom.HasValue)
                query = query.Where(e => e.CreatedAt >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                query = query.Where(e => e.CreatedAt < dateTo.Value.Date.AddDays(1));

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new ErrorLogResponseDto
                {
                    Id = e.Id,
                    Message = e.Message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    UserId = e.UserId,
                    UserName = e.User != null ? $"{e.User.FirstName} {e.User.LastName}" : null,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<ErrorLogResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<ErrorLogResponseDto>>.CreateSuccess(response, "Error logs retrieved successfully"));
        }

        /// <summary>
        /// Get a specific error log by ID
        /// </summary>
        /// <param name="id">Error log unique identifier</param>
        /// <returns>Error log details</returns>
        /// <response code="200">Returns error log details</response>
        /// <response code="404">Error log not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ErrorLogResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetErrorLog([Required] int id)
        {
            var errorLog = await _context.ErrorLogs
                .Include(e => e.User)
                .Where(e => e.Id == id)
                .Select(e => new ErrorLogResponseDto
                {
                    Id = e.Id,
                    Message = e.Message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    UserId = e.UserId,
                    UserName = e.User != null ? $"{e.User.FirstName} {e.User.LastName}" : null,
                    CreatedAt = e.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (errorLog == null)
                return NotFound(ApiResponse.CreateError("Error log not found", 404));

            return Ok(ApiResponse<ErrorLogResponseDto>.CreateSuccess(errorLog, "Error log retrieved successfully"));
        }

        /// <summary>
        /// Create a new error log entry
        /// </summary>
        /// <param name="model">Error log creation data</param>
        /// <returns>Created error log details</returns>
        /// <response code="201">Error log created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">User not found (if userId provided)</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ErrorLogResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> CreateErrorLog([FromBody] CreateErrorLogDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            if (model.UserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId.Value);
                if (!userExists)
                    return NotFound(ApiResponse.CreateError("User not found", 404));
            }

            var errorLog = new ErrorLog
            {
                Message = model.Message,
                StackTrace = model.StackTrace,
                Source = model.Source,
                UserId = model.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ErrorLogs.Add(errorLog);
            await _context.SaveChangesAsync();

            if (errorLog.UserId.HasValue)
                await _context.Entry(errorLog).Reference(e => e.User).LoadAsync();

            var dto = new ErrorLogResponseDto
            {
                Id = errorLog.Id,
                Message = errorLog.Message,
                StackTrace = errorLog.StackTrace,
                Source = errorLog.Source,
                UserId = errorLog.UserId,
                UserName = errorLog.User != null ? $"{errorLog.User.FirstName} {errorLog.User.LastName}" : null,
                CreatedAt = errorLog.CreatedAt
            };

            return CreatedAtAction(nameof(GetErrorLog), new { id = errorLog.Id },
                ApiResponse<ErrorLogResponseDto>.CreateSuccess(dto, "Error log created successfully", 201));
        }

        /// <summary>
        /// Delete an error log entry
        /// </summary>
        /// <param name="id">Error log unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Error log deleted successfully</response>
        /// <response code="404">Error log not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteErrorLog([Required] int id)
        {
            var errorLog = await _context.ErrorLogs.FindAsync(id);
            if (errorLog == null)
                return NotFound(ApiResponse.CreateError("Error log not found", 404));

            _context.ErrorLogs.Remove(errorLog);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Error log deleted successfully"));
        }

        /// <summary>
        /// Get error logs by source
        /// </summary>
        /// <param name="source">Error source</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of error logs from the specified source</returns>
        /// <response code="200">Returns error logs from the source</response>
        [HttpGet("source/{source}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ErrorLogResponseDto>>), 200)]
        public async Task<IActionResult> GetErrorLogsBySource([Required] string source, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.ErrorLogs
                .Include(e => e.User)
                .Where(e => e.Source != null && e.Source.Contains(source));

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new ErrorLogResponseDto
                {
                    Id = e.Id,
                    Message = e.Message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    UserId = e.UserId,
                    UserName = e.User != null ? $"{e.User.FirstName} {e.User.LastName}" : null,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<ErrorLogResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<ErrorLogResponseDto>>.CreateSuccess(response, "Error logs by source retrieved successfully"));
        }

        /// <summary>
        /// Get error logs by user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of error logs for the specified user</returns>
        /// <response code="200">Returns error logs for the user</response>
        /// <response code="404">User not found</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ErrorLogResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetErrorLogsByUser([Required] int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.ErrorLogs
                .Include(e => e.User)
                .Where(e => e.UserId == userId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new ErrorLogResponseDto
                {
                    Id = e.Id,
                    Message = e.Message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    UserId = e.UserId,
                    UserName = e.User != null ? $"{e.User.FirstName} {e.User.LastName}" : null,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<ErrorLogResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<ErrorLogResponseDto>>.CreateSuccess(response, "User error logs retrieved successfully"));
        }

        /// <summary>
        /// Get error statistics summary
        /// </summary>
        /// <param name="dateFrom">Date from (default: 30 days ago)</param>
        /// <param name="dateTo">Date to (default: today)</param>
        /// <returns>Error statistics summary</returns>
        /// <response code="200">Returns error statistics</response>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetErrorStatistics([FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null)
        {
            dateFrom ??= DateTime.UtcNow.AddDays(-30).Date;
            dateTo ??= DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);

            var query = _context.ErrorLogs.Where(e => e.CreatedAt >= dateFrom && e.CreatedAt <= dateTo);

            var statistics = new
            {
                TotalErrors = await query.CountAsync(),
                ErrorsBySource = await query
                    .GroupBy(e => e.Source ?? "Unknown")
                    .Select(g => new { Source = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync(),
                ErrorsByDay = await query
                    .GroupBy(e => e.CreatedAt.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToListAsync(),
                TopUsers = await query
                    .Where(e => e.UserId != null)
                    .Include(e => e.User)
                    .GroupBy(e => new { e.UserId, e.User.FirstName, e.User.LastName })
                    .Select(g => new { 
                        UserId = g.Key.UserId, 
                        UserName = $"{g.Key.FirstName} {g.Key.LastName}", 
                        Count = g.Count() 
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync()
            };

            return Ok(ApiResponse<object>.CreateSuccess(statistics, "Error statistics retrieved successfully"));
        }

        /// <summary>
        /// Bulk delete error logs by criteria
        /// </summary>
        /// <param name="dateFrom">Delete logs from this date</param>
        /// <param name="dateTo">Delete logs to this date</param>
        /// <param name="source">Delete logs from specific source (optional)</param>
        /// <returns>Deletion confirmation with count</returns>
        /// <response code="200">Error logs deleted successfully</response>
        /// <response code="400">Invalid date parameters</response>
        [HttpDelete("bulk")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> BulkDeleteErrorLogs([FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, [FromQuery] string? source = null)
        {
            if (!dateFrom.HasValue && !dateTo.HasValue && string.IsNullOrWhiteSpace(source))
                return BadRequest(ApiResponse.CreateError("At least one filter criteria must be provided"));

            var query = _context.ErrorLogs.AsQueryable();

            if (dateFrom.HasValue)
                query = query.Where(e => e.CreatedAt >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                query = query.Where(e => e.CreatedAt < dateTo.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(source))
                query = query.Where(e => e.Source != null && e.Source.Contains(source));

            var logsToDelete = await query.ToListAsync();
            var count = logsToDelete.Count;

            _context.ErrorLogs.RemoveRange(logsToDelete);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Successfully deleted {count} error logs"));
        }
    }
}