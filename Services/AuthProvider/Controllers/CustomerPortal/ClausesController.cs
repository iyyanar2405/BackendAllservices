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
    /// Clauses management operations for document clause organization within chapters
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Clauses")]
    public class ClausesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the ClausesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public ClausesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all clauses with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering clauses by number, title, or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="chapterId">Filter by chapter</param>
        /// <returns>Paginated list of clauses</returns>
        /// <response code="200">Returns paginated list of clauses</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ClauseResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetClauses(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? chapterId = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Clauses
                .Include(c => c.Chapter)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.ClauseNumber.Contains(searchTerm) || 
                                       c.Title.Contains(searchTerm) ||
                                       (c.Description != null && c.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (chapterId.HasValue)
                query = query.Where(c => c.ChapterId == chapterId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Chapter.ChapterNumber)
                .ThenBy(c => c.ClauseNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClauseResponseDto
                {
                    Id = c.Id,
                    ClauseNumber = c.ClauseNumber,
                    Title = c.Title,
                    Description = c.Description,
                    ChapterId = c.ChapterId,
                    ChapterTitle = c.Chapter.Title,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<ClauseResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<ClauseResponseDto>>.CreateSuccess(response, "Clauses retrieved successfully"));
        }

        /// <summary>
        /// Get a specific clause by ID
        /// </summary>
        /// <param name="id">Clause unique identifier</param>
        /// <returns>Clause details</returns>
        /// <response code="200">Returns clause details</response>
        /// <response code="404">Clause not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ClauseResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetClause([Required] int id)
        {
            var clause = await _context.Clauses
                .Include(c => c.Chapter)
                .Where(c => c.Id == id)
                .Select(c => new ClauseResponseDto
                {
                    Id = c.Id,
                    ClauseNumber = c.ClauseNumber,
                    Title = c.Title,
                    Description = c.Description,
                    ChapterId = c.ChapterId,
                    ChapterTitle = c.Chapter.Title,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (clause == null)
                return NotFound(ApiResponse.CreateError("Clause not found", 404));

            return Ok(ApiResponse<ClauseResponseDto>.CreateSuccess(clause, "Clause retrieved successfully"));
        }

        /// <summary>
        /// Create a new clause
        /// </summary>
        /// <param name="model">Clause creation data</param>
        /// <returns>Created clause details</returns>
        /// <response code="201">Clause created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Chapter not found</response>
        /// <response code="409">Clause number already exists in the chapter</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ClauseResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateClause([FromBody] CreateClauseDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var chapterExists = await _context.Chapters.AnyAsync(ch => ch.Id == model.ChapterId && ch.IsActive);
            if (!chapterExists)
                return NotFound(ApiResponse.CreateError("Chapter not found or inactive", 404));

            var existing = await _context.Clauses.FirstOrDefaultAsync(c => c.ClauseNumber == model.ClauseNumber && c.ChapterId == model.ChapterId);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Clause with this number already exists in the chapter", 409));

            var clause = new Clause
            {
                ClauseNumber = model.ClauseNumber,
                Title = model.Title,
                Description = model.Description,
                ChapterId = model.ChapterId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Clauses.Add(clause);
            await _context.SaveChangesAsync();

            await _context.Entry(clause).Reference(c => c.Chapter).LoadAsync();

            var dto = new ClauseResponseDto
            {
                Id = clause.Id,
                ClauseNumber = clause.ClauseNumber,
                Title = clause.Title,
                Description = clause.Description,
                ChapterId = clause.ChapterId,
                ChapterTitle = clause.Chapter.Title,
                IsActive = clause.IsActive,
                CreatedAt = clause.CreatedAt
            };

            return CreatedAtAction(nameof(GetClause), new { id = clause.Id },
                ApiResponse<ClauseResponseDto>.CreateSuccess(dto, "Clause created successfully", 201));
        }

        /// <summary>
        /// Update an existing clause
        /// </summary>
        /// <param name="id">Clause unique identifier</param>
        /// <param name="model">Clause update data</param>
        /// <returns>Updated clause details</returns>
        /// <response code="200">Clause updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Clause or chapter not found</response>
        /// <response code="409">Clause number is already in use in this chapter</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ClauseResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateClause([Required] int id, [FromBody] UpdateClauseDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var clause = await _context.Clauses
                .Include(c => c.Chapter)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (clause == null)
                return NotFound(ApiResponse.CreateError("Clause not found", 404));

            var chapterExists = await _context.Chapters.AnyAsync(ch => ch.Id == model.ChapterId && ch.IsActive);
            if (!chapterExists)
                return NotFound(ApiResponse.CreateError("Chapter not found or inactive", 404));

            if (clause.ClauseNumber != model.ClauseNumber || clause.ChapterId != model.ChapterId)
            {
                var existing = await _context.Clauses.FirstOrDefaultAsync(c => c.ClauseNumber == model.ClauseNumber && c.ChapterId == model.ChapterId && c.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Clause number is already in use in this chapter", 409));
            }

            clause.ClauseNumber = model.ClauseNumber;
            clause.Title = model.Title;
            clause.Description = model.Description;
            clause.ChapterId = model.ChapterId;
            clause.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new ClauseResponseDto
            {
                Id = clause.Id,
                ClauseNumber = clause.ClauseNumber,
                Title = clause.Title,
                Description = clause.Description,
                ChapterId = clause.ChapterId,
                ChapterTitle = clause.Chapter.Title,
                IsActive = clause.IsActive,
                CreatedAt = clause.CreatedAt
            };

            return Ok(ApiResponse<ClauseResponseDto>.CreateSuccess(dto, "Clause updated successfully"));
        }

        /// <summary>
        /// Delete a clause
        /// </summary>
        /// <param name="id">Clause unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Clause deleted successfully</response>
        /// <response code="404">Clause not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteClause([Required] int id)
        {
            var clause = await _context.Clauses.FindAsync(id);
            if (clause == null)
                return NotFound(ApiResponse.CreateError("Clause not found", 404));

            _context.Clauses.Remove(clause);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Clause deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a clause
        /// </summary>
        /// <param name="id">Clause unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Clause status updated successfully</response>
        /// <response code="404">Clause not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateClauseStatus([Required] int id, [FromQuery] bool isActive)
        {
            var clause = await _context.Clauses.FindAsync(id);
            if (clause == null)
                return NotFound(ApiResponse.CreateError("Clause not found", 404));

            clause.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Clause {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Get clauses by chapter
        /// </summary>
        /// <param name="chapterId">Chapter ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of clauses in the chapter</returns>
        /// <response code="200">Returns clauses in the chapter</response>
        /// <response code="404">Chapter not found</response>
        [HttpGet("chapter/{chapterId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ClauseResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetClausesByChapter([Required] int chapterId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var chapter = await _context.Chapters.FindAsync(chapterId);
            if (chapter == null)
                return NotFound(ApiResponse.CreateError("Chapter not found", 404));

            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Clauses
                .Include(c => c.Chapter)
                .Where(c => c.ChapterId == chapterId);

            var totalCount = await query.CountAsync();
            var clauses = await query
                .OrderBy(c => c.ClauseNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClauseResponseDto
                {
                    Id = c.Id,
                    ClauseNumber = c.ClauseNumber,
                    Title = c.Title,
                    Description = c.Description,
                    ChapterId = c.ChapterId,
                    ChapterTitle = c.Chapter.Title,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<ClauseResponseDto>
            {
                Items = clauses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<ClauseResponseDto>>.CreateSuccess(response, "Chapter clauses retrieved successfully"));
        }

        /// <summary>
        /// Search clauses by number, title, or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching clauses</returns>
        /// <response code="200">Returns matching clauses</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClauseResponseDto>>), 200)]
        public async Task<IActionResult> SearchClauses([FromQuery][Required] string searchTerm)
        {
            var clauses = await _context.Clauses
                .Include(c => c.Chapter)
                .Where(c => c.IsActive && (c.ClauseNumber.Contains(searchTerm) ||
                           c.Title.Contains(searchTerm) ||
                           (c.Description != null && c.Description.Contains(searchTerm))))
                .OrderBy(c => c.Chapter.ChapterNumber)
                .ThenBy(c => c.ClauseNumber)
                .Select(c => new ClauseResponseDto
                {
                    Id = c.Id,
                    ClauseNumber = c.ClauseNumber,
                    Title = c.Title,
                    Description = c.Description,
                    ChapterId = c.ChapterId,
                    ChapterTitle = c.Chapter.Title,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<ClauseResponseDto>>.CreateSuccess(clauses, "Clauses search completed successfully"));
        }
    }
}