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
    /// Finding Categories management operations for audit findings categorization
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Finding Categories")]
    public class FindingCategoriesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the FindingCategoriesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public FindingCategoriesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all finding categories with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering categories by name or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of finding categories</returns>
        /// <response code="200">Returns paginated list of finding categories</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<SimpleCategoryResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetFindingCategories(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.FindingCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(fc => fc.Name.Contains(searchTerm) || (fc.Description != null && fc.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(fc => fc.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(fc => fc.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(fc => new SimpleCategoryResponseDto
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    Description = fc.Description,
                    IsActive = fc.IsActive,
                    CreatedAt = fc.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<SimpleCategoryResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<SimpleCategoryResponseDto>>.CreateSuccess(response, "Finding categories retrieved successfully"));
        }

        /// <summary>
        /// Get a specific finding category by ID
        /// </summary>
        /// <param name="id">Finding category unique identifier</param>
        /// <returns>Finding category details</returns>
        /// <response code="200">Returns finding category details</response>
        /// <response code="404">Finding category not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetFindingCategory([Required] int id)
        {
            var category = await _context.FindingCategories
                .Where(fc => fc.Id == id)
                .Select(fc => new SimpleCategoryResponseDto
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    Description = fc.Description,
                    IsActive = fc.IsActive,
                    CreatedAt = fc.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound(ApiResponse.CreateError("Finding category not found", 404));

            return Ok(ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(category, "Finding category retrieved successfully"));
        }

        /// <summary>
        /// Create a new finding category
        /// </summary>
        /// <param name="model">Finding category creation data</param>
        /// <returns>Created finding category details</returns>
        /// <response code="201">Finding category created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Finding category with name already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateFindingCategory([FromBody] CreateSimpleCategoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.FindingCategories.FirstOrDefaultAsync(fc => fc.Name == model.Name);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Finding category with this name already exists", 409));

            var category = new FindingCategory
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.FindingCategories.Add(category);
            await _context.SaveChangesAsync();

            var dto = new SimpleCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };

            return CreatedAtAction(nameof(GetFindingCategory), new { id = category.Id },
                ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(dto, "Finding category created successfully", 201));
        }

        /// <summary>
        /// Update an existing finding category
        /// </summary>
        /// <param name="id">Finding category unique identifier</param>
        /// <param name="model">Finding category update data</param>
        /// <returns>Updated finding category details</returns>
        /// <response code="200">Finding category updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Finding category not found</response>
        /// <response code="409">Finding category name is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SimpleCategoryResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateFindingCategory([Required] int id, [FromBody] UpdateSimpleCategoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var category = await _context.FindingCategories.FindAsync(id);
            if (category == null)
                return NotFound(ApiResponse.CreateError("Finding category not found", 404));

            if (category.Name != model.Name)
            {
                var existing = await _context.FindingCategories.FirstOrDefaultAsync(fc => fc.Name == model.Name && fc.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Finding category name is already in use", 409));
            }

            category.Name = model.Name;
            category.Description = model.Description;
            category.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new SimpleCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };

            return Ok(ApiResponse<SimpleCategoryResponseDto>.CreateSuccess(dto, "Finding category updated successfully"));
        }

        /// <summary>
        /// Delete a finding category
        /// </summary>
        /// <param name="id">Finding category unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Finding category deleted successfully</response>
        /// <response code="404">Finding category not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteFindingCategory([Required] int id)
        {
            var category = await _context.FindingCategories.FindAsync(id);
            if (category == null)
                return NotFound(ApiResponse.CreateError("Finding category not found", 404));

            _context.FindingCategories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Finding category deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a finding category
        /// </summary>
        /// <param name="id">Finding category unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Finding category status updated successfully</response>
        /// <response code="404">Finding category not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateFindingCategoryStatus([Required] int id, [FromQuery] bool isActive)
        {
            var category = await _context.FindingCategories.FindAsync(id);
            if (category == null)
                return NotFound(ApiResponse.CreateError("Finding category not found", 404));

            category.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Finding category {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Search finding categories by name or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching finding categories</returns>
        /// <response code="200">Returns matching finding categories</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SimpleCategoryResponseDto>>), 200)]
        public async Task<IActionResult> SearchFindingCategories([FromQuery][Required] string searchTerm)
        {
            var categories = await _context.FindingCategories
                .Where(fc => fc.IsActive && (fc.Name.Contains(searchTerm) || (fc.Description != null && fc.Description.Contains(searchTerm))))
                .OrderBy(fc => fc.Name)
                .Select(fc => new SimpleCategoryResponseDto
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    Description = fc.Description,
                    IsActive = fc.IsActive,
                    CreatedAt = fc.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<SimpleCategoryResponseDto>>.CreateSuccess(categories, "Finding categories search completed successfully"));
        }
    }
}