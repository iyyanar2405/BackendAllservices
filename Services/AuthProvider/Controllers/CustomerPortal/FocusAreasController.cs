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
    /// Focus Areas management operations for audit focus area definitions
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Focus Areas")]
    public class FocusAreasController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the FocusAreasController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public FocusAreasController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all focus areas with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering focus areas by name or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of focus areas</returns>
        /// <response code="200">Returns paginated list of focus areas</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<SimpleCategoryResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetFocusAreas(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.FocusAreas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(fa => fa.Name.Contains(searchTerm) || (fa.Description != null && fa.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(fa => fa.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(fa => fa.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(fa => new SimpleCategoryResponseDto
                {
                    Id = fa.Id,
                    Name = fa.Name,
                    Description = fa.Description,
                    IsActive = fa.IsActive,
                    CreatedAt = fa.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<SimpleCategoryResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<SimpleCategoryResponseDto>>.CreateSuccess(response, "Focus areas retrieved successfully"));
        }

        /// <summary>
        /// Get a specific focus area by ID
        /// </summary>
        /// <param name="id">Focus area unique identifier</param>
        /// <returns>Focus area details</returns>
        /// <response code="200">Returns focus area details</response>
        /// <response code="404">Focus area not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetFocusArea([Required] int id)
        {
            var focusArea = await _context.FocusAreas
                .Where(fa => fa.Id == id)
                .Select(fa => new SimpleCategoryResponseDto
                {
                    Id = fa.Id,
                    Name = fa.Name,
                    Description = fa.Description,
                    IsActive = fa.IsActive,
                    CreatedAt = fa.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (focusArea == null)
                return NotFound(ApiResponse.CreateError("Focus area not found", 404));

            return Ok(ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(focusArea, "Focus area retrieved successfully"));
        }

        /// <summary>
        /// Create a new focus area
        /// </summary>
        /// <param name="model">Focus area creation data</param>
        /// <returns>Created focus area details</returns>
        /// <response code="201">Focus area created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Focus area with name already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateFocusArea([FromBody] CreateSimpleCategoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.FocusAreas.FirstOrDefaultAsync(fa => fa.Name == model.Name);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Focus area with this name already exists", 409));

            var focusArea = new FocusArea
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.FocusAreas.Add(focusArea);
            await _context.SaveChangesAsync();

            var dto = new SimpleCategoryResponseDto
            {
                Id = focusArea.Id,
                Name = focusArea.Name,
                Description = focusArea.Description,
                IsActive = focusArea.IsActive,
                CreatedAt = focusArea.CreatedAt
            };

            return CreatedAtAction(nameof(GetFocusArea), new { id = focusArea.Id },
                ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(dto, "Focus area created successfully", 201));
        }

        /// <summary>
        /// Update an existing focus area
        /// </summary>
        /// <param name="id">Focus area unique identifier</param>
        /// <param name="model">Focus area update data</param>
        /// <returns>Updated focus area details</returns>
        /// <response code="200">Focus area updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Focus area not found</response>
        /// <response code="409">Focus area name is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateFocusArea([Required] int id, [FromBody] UpdateSimpleCategoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var focusArea = await _context.FocusAreas.FindAsync(id);
            if (focusArea == null)
                return NotFound(ApiResponse.CreateError("Focus area not found", 404));

            if (focusArea.Name != model.Name)
            {
                var existing = await _context.FocusAreas.FirstOrDefaultAsync(fa => fa.Name == model.Name && fa.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Focus area name is already in use", 409));
            }

            focusArea.Name = model.Name;
            focusArea.Description = model.Description;
            focusArea.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new SimpleCategoryResponseDto
            {
                Id = focusArea.Id,
                Name = focusArea.Name,
                Description = focusArea.Description,
                IsActive = focusArea.IsActive,
                CreatedAt = focusArea.CreatedAt
            };

            return Ok(ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(dto, "Focus area updated successfully"));
        }

        /// <summary>
        /// Delete a focus area
        /// </summary>
        /// <param name="id">Focus area unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Focus area deleted successfully</response>
        /// <response code="404">Focus area not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteFocusArea([Required] int id)
        {
            var focusArea = await _context.FocusAreas.FindAsync(id);
            if (focusArea == null)
                return NotFound(ApiResponse.CreateError("Focus area not found", 404));

            _context.FocusAreas.Remove(focusArea);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Focus area deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a focus area
        /// </summary>
        /// <param name="id">Focus area unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Focus area status updated successfully</response>
        /// <response code="404">Focus area not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateFocusAreaStatus([Required] int id, [FromQuery] bool isActive)
        {
            var focusArea = await _context.FocusAreas.FindAsync(id);
            if (focusArea == null)
                return NotFound(ApiResponse.CreateError("Focus area not found", 404));

            focusArea.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Focus area {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Search focus areas by name or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching focus areas</returns>
        /// <response code="200">Returns matching focus areas</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SimpleCategoryResponseDto>>), 200)]
        public async Task<IActionResult> SearchFocusAreas([FromQuery][Required] string searchTerm)
        {
            var focusAreas = await _context.FocusAreas
                .Where(fa => fa.IsActive && (fa.Name.Contains(searchTerm) || (fa.Description != null && fa.Description.Contains(searchTerm))))
                .OrderBy(fa => fa.Name)
                .Select(fa => new SimpleCategoryResponseDto
                {
                    Id = fa.Id,
                    Name = fa.Name,
                    Description = fa.Description,
                    IsActive = fa.IsActive,
                    CreatedAt = fa.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<SimpleCategoryResponseDto>>.CreateSuccess(focusAreas, "Focus areas search completed successfully"));
        }
    }
}