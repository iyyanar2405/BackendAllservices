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
    /// Chapters management operations for document organization
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Chapters")]
    public class ChaptersController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the ChaptersController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public ChaptersController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all chapters with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering chapters by number, title, or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of chapters</returns>
        /// <response code="200">Returns paginated list of chapters</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ChapterResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetChapters(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Chapters
                .Include(c => c.Clauses)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.ChapterNumber.Contains(searchTerm) || 
                                       c.Title.Contains(searchTerm) ||
                                       (c.Description != null && c.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.ChapterNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ChapterResponseDto
                {
                    Id = c.Id,
                    ChapterNumber = c.ChapterNumber,
                    Title = c.Title,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    Clauses = c.Clauses.Where(cl => cl.IsActive).Select(cl => new ClauseResponseDto
                    {
                        Id = cl.Id,
                        ClauseNumber = cl.ClauseNumber,
                        Title = cl.Title,
                        Description = cl.Description,
                        ChapterId = cl.ChapterId,
                        ChapterTitle = c.Title,
                        IsActive = cl.IsActive,
                        CreatedAt = cl.CreatedAt
                    })
                })
                .ToListAsync();

            var response = new PagedResponseDto<ChapterResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<ChapterResponseDto>>.CreateSuccess(response, "Chapters retrieved successfully"));
        }

        /// <summary>
        /// Get a specific chapter by ID with all its clauses
        /// </summary>
        /// <param name="id">Chapter unique identifier</param>
        /// <returns>Chapter details with clauses</returns>
        /// <response code="200">Returns chapter details</response>
        /// <response code="404">Chapter not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ChapterResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetChapter([Required] int id)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Clauses)
                .Where(c => c.Id == id)
                .Select(c => new ChapterResponseDto
                {
                    Id = c.Id,
                    ChapterNumber = c.ChapterNumber,
                    Title = c.Title,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    Clauses = c.Clauses.Select(cl => new ClauseResponseDto
                    {
                        Id = cl.Id,
                        ClauseNumber = cl.ClauseNumber,
                        Title = cl.Title,
                        Description = cl.Description,
                        ChapterId = cl.ChapterId,
                        ChapterTitle = c.Title,
                        IsActive = cl.IsActive,
                        CreatedAt = cl.CreatedAt
                    })
                })
                .FirstOrDefaultAsync();

            if (chapter == null)
                return NotFound(ApiResponse.CreateError("Chapter not found", 404));

            return Ok(ApiResponse<ChapterResponseDto>.CreateSuccess(chapter, "Chapter retrieved successfully"));
        }

        /// <summary>
        /// Create a new chapter
        /// </summary>
        /// <param name="model">Chapter creation data</param>
        /// <returns>Created chapter details</returns>
        /// <response code="201">Chapter created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Chapter number already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ChapterResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateChapter([FromBody] CreateChapterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.Chapters.FirstOrDefaultAsync(c => c.ChapterNumber == model.ChapterNumber);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Chapter with this number already exists", 409));

            var chapter = new Chapter
            {
                ChapterNumber = model.ChapterNumber,
                Title = model.Title,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();

            var dto = new ChapterResponseDto
            {
                Id = chapter.Id,
                ChapterNumber = chapter.ChapterNumber,
                Title = chapter.Title,
                Description = chapter.Description,
                IsActive = chapter.IsActive,
                CreatedAt = chapter.CreatedAt,
                Clauses = new List<ClauseResponseDto>()
            };

            return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id },
                ApiResponse<ChapterResponseDto>.CreateSuccess(dto, "Chapter created successfully", 201));
        }

        /// <summary>
        /// Update an existing chapter
        /// </summary>
        /// <param name="id">Chapter unique identifier</param>
        /// <param name="model">Chapter update data</param>
        /// <returns>Updated chapter details</returns>
        /// <response code="200">Chapter updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Chapter not found</response>
        /// <response code="409">Chapter number is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ChapterResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateChapter([Required] int id, [FromBody] UpdateChapterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var chapter = await _context.Chapters
                .Include(c => c.Clauses)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (chapter == null)
                return NotFound(ApiResponse.CreateError("Chapter not found", 404));

            if (chapter.ChapterNumber != model.ChapterNumber)
            {
                var existing = await _context.Chapters.FirstOrDefaultAsync(c => c.ChapterNumber == model.ChapterNumber && c.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Chapter number is already in use", 409));
            }

            chapter.ChapterNumber = model.ChapterNumber;
            chapter.Title = model.Title;
            chapter.Description = model.Description;
            chapter.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new ChapterResponseDto
            {
                Id = chapter.Id,
                ChapterNumber = chapter.ChapterNumber,
                Title = chapter.Title,
                Description = chapter.Description,
                IsActive = chapter.IsActive,
                CreatedAt = chapter.CreatedAt,
                Clauses = chapter.Clauses.Select(cl => new ClauseResponseDto
                {
                    Id = cl.Id,
                    ClauseNumber = cl.ClauseNumber,
                    Title = cl.Title,
                    Description = cl.Description,
                    ChapterId = cl.ChapterId,
                    ChapterTitle = chapter.Title,
                    IsActive = cl.IsActive,
                    CreatedAt = cl.CreatedAt
                })
            };

            return Ok(ApiResponse<ChapterResponseDto>.CreateSuccess(dto, "Chapter updated successfully"));
        }

        /// <summary>
        /// Delete a chapter
        /// </summary>
        /// <param name="id">Chapter unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Chapter deleted successfully</response>
        /// <response code="400">Cannot delete chapter with existing clauses</response>
        /// <response code="404">Chapter not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteChapter([Required] int id)
        {
            var chapter = await _context.Chapters.Include(c => c.Clauses).FirstOrDefaultAsync(c => c.Id == id);
            if (chapter == null)
                return NotFound(ApiResponse.CreateError("Chapter not found", 404));

            if (chapter.Clauses.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete chapter with existing clauses"));

            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Chapter deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a chapter
        /// </summary>
        /// <param name="id">Chapter unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Chapter status updated successfully</response>
        /// <response code="404">Chapter not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateChapterStatus([Required] int id, [FromQuery] bool isActive)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
                return NotFound(ApiResponse.CreateError("Chapter not found", 404));

            chapter.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Chapter {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Get clauses by chapter
        /// </summary>
        /// <param name="id">Chapter ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of clauses in the chapter</returns>
        /// <response code="200">Returns clauses in the chapter</response>
        /// <response code="404">Chapter not found</response>
        [HttpGet("{id}/clauses")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ClauseResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetChapterClauses([Required] int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
                return NotFound(ApiResponse.CreateError("Chapter not found", 404));

            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Clauses
                .Include(c => c.Chapter)
                .Where(c => c.ChapterId == id);

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
        /// Search chapters by number, title, or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching chapters</returns>
        /// <response code="200">Returns matching chapters</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChapterResponseDto>>), 200)]
        public async Task<IActionResult> SearchChapters([FromQuery][Required] string searchTerm)
        {
            var chapters = await _context.Chapters
                .Where(c => c.IsActive && (c.ChapterNumber.Contains(searchTerm) ||
                           c.Title.Contains(searchTerm) ||
                           (c.Description != null && c.Description.Contains(searchTerm))))
                .OrderBy(c => c.ChapterNumber)
                .Select(c => new ChapterResponseDto
                {
                    Id = c.Id,
                    ChapterNumber = c.ChapterNumber,
                    Title = c.Title,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    Clauses = null // Don't load clauses for search results
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<ChapterResponseDto>>.CreateSuccess(chapters, "Chapters search completed successfully"));
        }
    }
}