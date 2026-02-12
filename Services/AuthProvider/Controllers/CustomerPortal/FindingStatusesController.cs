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
    /// Finding Statuses management operations for audit finding status tracking
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Finding Statuses")]
    public class FindingStatusesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the FindingStatusesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public FindingStatusesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all finding statuses with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering statuses by name or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of finding statuses</returns>
        /// <response code="200">Returns paginated list of finding statuses</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<SimpleCategoryResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetFindingStatuses(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.FindingStatuses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(fs => fs.Name.Contains(searchTerm) || (fs.Description != null && fs.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(fs => fs.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(fs => fs.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(fs => new SimpleCategoryResponseDto
                {
                    Id = fs.Id,
                    Name = fs.Name,
                    Description = fs.Description,
                    IsActive = fs.IsActive,
                    CreatedAt = fs.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<SimpleCategoryResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<SimpleCategoryResponseDto>>.CreateSuccess(response, "Finding statuses retrieved successfully"));
        }

        /// <summary>
        /// Get a specific finding status by ID
        /// </summary>
        /// <param name="id">Finding status unique identifier</param>
        /// <returns>Finding status details</returns>
        /// <response code="200">Returns finding status details</response>
        /// <response code="404">Finding status not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetFindingStatus([Required] int id)
        {
            var status = await _context.FindingStatuses
                .Where(fs => fs.Id == id)
                .Select(fs => new SimpleCategoryResponseDto
                {
                    Id = fs.Id,
                    Name = fs.Name,
                    Description = fs.Description,
                    IsActive = fs.IsActive,
                    CreatedAt = fs.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound(ApiResponse.CreateError("Finding status not found", 404));

            return Ok(ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(status, "Finding status retrieved successfully"));
        }

        /// <summary>
        /// Create a new finding status
        /// </summary>
        /// <param name="model">Finding status creation data</param>
        /// <returns>Created finding status details</returns>
        /// <response code="201">Finding status created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Finding status with name already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateFindingStatus([FromBody] CreateSimpleCategoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.FindingStatuses.FirstOrDefaultAsync(fs => fs.Name == model.Name);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Finding status with this name already exists", 409));

            var status = new FindingStatus
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.FindingStatuses.Add(status);
            await _context.SaveChangesAsync();

            var dto = new SimpleCategoryResponseDto
            {
                Id = status.Id,
                Name = status.Name,
                Description = status.Description,
                IsActive = status.IsActive,
                CreatedAt = status.CreatedAt
            };

            return CreatedAtAction(nameof(GetFindingStatus), new { id = status.Id },
                ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(dto, "Finding status created successfully", 201));
        }

        /// <summary>
        /// Update an existing finding status
        /// </summary>
        /// <param name="id">Finding status unique identifier</param>
        /// <param name="model">Finding status update data</param>
        /// <returns>Updated finding status details</returns>
        /// <response code="200">Finding status updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Finding status not found</response>
        /// <response code="409">Finding status name is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateFindingStatus([Required] int id, [FromBody] UpdateSimpleCategoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var status = await _context.FindingStatuses.FindAsync(id);
            if (status == null)
                return NotFound(ApiResponse.CreateError("Finding status not found", 404));

            if (status.Name != model.Name)
            {
                var existing = await _context.FindingStatuses.FirstOrDefaultAsync(fs => fs.Name == model.Name && fs.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Finding status name is already in use", 409));
            }

            status.Name = model.Name;
            status.Description = model.Description;
            status.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new SimpleCategoryResponseDto
            {
                Id = status.Id,
                Name = status.Name,
                Description = status.Description,
                IsActive = status.IsActive,
                CreatedAt = status.CreatedAt
            };

            return Ok(ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(dto, "Finding status updated successfully"));
        }

        /// <summary>
        /// Delete a finding status
        /// </summary>
        /// <param name="id">Finding status unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Finding status deleted successfully</response>
        /// <response code="404">Finding status not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteFindingStatus([Required] int id)
        {
            var status = await _context.FindingStatuses.FindAsync(id);
            if (status == null)
                return NotFound(ApiResponse.CreateError("Finding status not found", 404));

            _context.FindingStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Finding status deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a finding status
        /// </summary>
        /// <param name="id">Finding status unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Finding status updated successfully</response>
        /// <response code="404">Finding status not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateFindingStatusActive([Required] int id, [FromQuery] bool isActive)
        {
            var status = await _context.FindingStatuses.FindAsync(id);
            if (status == null)
                return NotFound(ApiResponse.CreateError("Finding status not found", 404));

            status.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Finding status {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Search finding statuses by name or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching finding statuses</returns>
        /// <response code="200">Returns matching finding statuses</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SimpleCategoryResponseDto>>), 200)]
        public async Task<IActionResult> SearchFindingStatuses([FromQuery][Required] string searchTerm)
        {
            var statuses = await _context.FindingStatuses
                .Where(fs => fs.IsActive && (fs.Name.Contains(searchTerm) || (fs.Description != null && fs.Description.Contains(searchTerm))))
                .OrderBy(fs => fs.Name)
                .Select(fs => new SimpleCategoryResponseDto
                {
                    Id = fs.Id,
                    Name = fs.Name,
                    Description = fs.Description,
                    IsActive = fs.IsActive,
                    CreatedAt = fs.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<SimpleCategoryResponseDto>>.CreateSuccess(statuses, "Finding statuses search completed successfully"));
        }
    }
}