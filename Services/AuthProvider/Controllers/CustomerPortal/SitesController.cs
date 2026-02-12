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
    /// Sites management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Sites")]
    public class SitesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the SitesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public SitesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all sites with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering sites by name or code</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="companyId">Filter by company</param>
        /// <param name="cityId">Filter by city</param>
        /// <returns>Paginated list of sites</returns>
        /// <response code="200">Returns paginated list of sites</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<SiteResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetSites(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? companyId = null,
            [FromQuery] int? cityId = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Sites
                .Include(s => s.Company)
                .Include(s => s.City)
                .Include(s => s.Audits)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(s => s.Name.Contains(searchTerm) || (s.Code != null && s.Code.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(s => s.IsActive == isActive.Value);

            if (companyId.HasValue)
                query = query.Where(s => s.CompanyId == companyId.Value);

            if (cityId.HasValue)
                query = query.Where(s => s.CityId == cityId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SiteResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    Address = s.Address,
                    CompanyId = s.CompanyId,
                    CompanyName = s.Company.Name,
                    CityId = s.CityId,
                    CityName = s.City.Name,
                    Phone = s.Phone,
                    Email = s.Email,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    AuditsCount = s.Audits.Count()
                })
                .ToListAsync();

            var response = new PagedResponseDto<SiteResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<SiteResponseDto>>.CreateSuccess(response, "Sites retrieved successfully"));
        }

        /// <summary>
        /// Get a specific site by ID
        /// </summary>
        /// <param name="id">Site unique identifier</param>
        /// <returns>Site details</returns>
        /// <response code="200">Returns site details</response>
        /// <response code="404">Site not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SiteResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetSite([Required] int id)
        {
            var site = await _context.Sites
                .Include(s => s.Company)
                .Include(s => s.City)
                .Include(s => s.Audits)
                .Where(s => s.Id == id)
                .Select(s => new SiteResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    Address = s.Address,
                    CompanyId = s.CompanyId,
                    CompanyName = s.Company.Name,
                    CityId = s.CityId,
                    CityName = s.City.Name,
                    Phone = s.Phone,
                    Email = s.Email,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    AuditsCount = s.Audits.Count()
                })
                .FirstOrDefaultAsync();

            if (site == null)
                return NotFound(ApiResponse.CreateError("Site not found", 404));

            return Ok(ApiResponse<SiteResponseDto>.CreateSuccess(site, "Site retrieved successfully"));
        }

        /// <summary>
        /// Create a new site
        /// </summary>
        /// <param name="model">Site creation data</param>
        /// <returns>Created site details</returns>
        /// <response code="201">Site created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Company or city not found</response>
        /// <response code="409">Site with name already exists in the company</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SiteResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateSite([FromBody] CreateSiteDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var companyExists = await _context.Companies.AnyAsync(c => c.Id == model.CompanyId && c.IsActive);
            if (!companyExists)
                return NotFound(ApiResponse.CreateError("Company not found or inactive", 404));

            var cityExists = await _context.Cities.AnyAsync(c => c.Id == model.CityId && c.IsActive);
            if (!cityExists)
                return NotFound(ApiResponse.CreateError("City not found or inactive", 404));

            var existing = await _context.Sites.FirstOrDefaultAsync(s => s.Name == model.Name && s.CompanyId == model.CompanyId);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Site with this name already exists in the company", 409));

            var site = new Site
            {
                Name = model.Name,
                Code = model.Code,
                Address = model.Address,
                CompanyId = model.CompanyId,
                CityId = model.CityId,
                Phone = model.Phone,
                Email = model.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Sites.Add(site);
            await _context.SaveChangesAsync();

            await _context.Entry(site).Reference(s => s.Company).LoadAsync();
            await _context.Entry(site).Reference(s => s.City).LoadAsync();

            var dto = new SiteResponseDto
            {
                Id = site.Id,
                Name = site.Name,
                Code = site.Code,
                Address = site.Address,
                CompanyId = site.CompanyId,
                CompanyName = site.Company.Name,
                CityId = site.CityId,
                CityName = site.City.Name,
                Phone = site.Phone,
                Email = site.Email,
                IsActive = site.IsActive,
                CreatedAt = site.CreatedAt,
                UpdatedAt = site.UpdatedAt,
                AuditsCount = 0
            };

            return CreatedAtAction(nameof(GetSite), new { id = site.Id },
                ApiResponse<SiteResponseDto>.CreateSuccess(dto, "Site created successfully", 201));
        }

        /// <summary>
        /// Update an existing site
        /// </summary>
        /// <param name="id">Site unique identifier</param>
        /// <param name="model">Site update data</param>
        /// <returns>Updated site details</returns>
        /// <response code="200">Site updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Site, company, or city not found</response>
        /// <response code="409">Site name is already in use in this company</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SiteResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateSite([Required] int id, [FromBody] UpdateSiteDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var site = await _context.Sites
                .Include(s => s.Company)
                .Include(s => s.City)
                .Include(s => s.Audits)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (site == null)
                return NotFound(ApiResponse.CreateError("Site not found", 404));

            var companyExists = await _context.Companies.AnyAsync(c => c.Id == model.CompanyId && c.IsActive);
            if (!companyExists)
                return NotFound(ApiResponse.CreateError("Company not found or inactive", 404));

            var cityExists = await _context.Cities.AnyAsync(c => c.Id == model.CityId && c.IsActive);
            if (!cityExists)
                return NotFound(ApiResponse.CreateError("City not found or inactive", 404));

            if (site.Name != model.Name || site.CompanyId != model.CompanyId)
            {
                var existing = await _context.Sites.FirstOrDefaultAsync(s => s.Name == model.Name && s.CompanyId == model.CompanyId && s.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Site name is already in use in this company", 409));
            }

            site.Name = model.Name;
            site.Code = model.Code;
            site.Address = model.Address;
            site.CompanyId = model.CompanyId;
            site.CityId = model.CityId;
            site.Phone = model.Phone;
            site.Email = model.Email;
            site.IsActive = model.IsActive;
            site.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var dto = new SiteResponseDto
            {
                Id = site.Id,
                Name = site.Name,
                Code = site.Code,
                Address = site.Address,
                CompanyId = site.CompanyId,
                CompanyName = site.Company.Name,
                CityId = site.CityId,
                CityName = site.City.Name,
                Phone = site.Phone,
                Email = site.Email,
                IsActive = site.IsActive,
                CreatedAt = site.CreatedAt,
                UpdatedAt = site.UpdatedAt,
                AuditsCount = site.Audits.Count()
            };

            return Ok(ApiResponse<SiteResponseDto>.CreateSuccess(dto, "Site updated successfully"));
        }

        /// <summary>
        /// Delete a site
        /// </summary>
        /// <param name="id">Site unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Site deleted successfully</response>
        /// <response code="400">Cannot delete site with existing audits</response>
        /// <response code="404">Site not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteSite([Required] int id)
        {
            var site = await _context.Sites.Include(s => s.Audits).FirstOrDefaultAsync(s => s.Id == id);
            if (site == null)
                return NotFound(ApiResponse.CreateError("Site not found", 404));

            if (site.Audits.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete site with existing audits"));

            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Site deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a site
        /// </summary>
        /// <param name="id">Site unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Site status updated successfully</response>
        /// <response code="404">Site not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateSiteStatus([Required] int id, [FromQuery] bool isActive)
        {
            var site = await _context.Sites.FindAsync(id);
            if (site == null)
                return NotFound(ApiResponse.CreateError("Site not found", 404));

            site.IsActive = isActive;
            site.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Site {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Search sites by name or code
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching sites</returns>
        /// <response code="200">Returns matching sites</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SiteResponseDto>>), 200)]
        public async Task<IActionResult> SearchSites([FromQuery][Required] string searchTerm)
        {
            var sites = await _context.Sites
                .Include(s => s.Company)
                .Include(s => s.City)
                .Where(s => s.IsActive && (s.Name.Contains(searchTerm) || (s.Code != null && s.Code.Contains(searchTerm))))
                .OrderBy(s => s.Name)
                .Select(s => new SiteResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    Address = s.Address,
                    CompanyId = s.CompanyId,
                    CompanyName = s.Company.Name,
                    CityId = s.CityId,
                    CityName = s.City.Name,
                    Phone = s.Phone,
                    Email = s.Email,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    AuditsCount = s.Audits.Count()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<SiteResponseDto>>.CreateSuccess(sites, "Sites search completed successfully"));
        }
    }
}