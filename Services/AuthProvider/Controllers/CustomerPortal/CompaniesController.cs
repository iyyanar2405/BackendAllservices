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
    /// Companies management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the CompaniesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public CompaniesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all companies with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering companies by name, code, or email</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of companies</returns>
        /// <response code="200">Returns paginated list of companies</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<CompanyResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetCompanies(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Companies
                .Include(c => c.Sites)
                .Include(c => c.Audits)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) ||
                                       (c.Code != null && c.Code.Contains(searchTerm)) ||
                                       (c.Email != null && c.Email.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var companies = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CompanyResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Address = c.Address,
                    Phone = c.Phone,
                    Email = c.Email,
                    Website = c.Website,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    SitesCount = c.Sites.Count(s => s.IsActive),
                    AuditsCount = c.Audits.Count()
                })
                .ToListAsync();

            var pagedResponse = new PagedResponseDto<CompanyResponseDto>
            {
                Items = companies,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<CompanyResponseDto>>.CreateSuccess(pagedResponse, "Companies retrieved successfully"));
        }

        /// <summary>
        /// Get a specific company by ID
        /// </summary>
        /// <param name="id">Company unique identifier</param>
        /// <returns>Company details</returns>
        /// <response code="200">Returns company details</response>
        /// <response code="404">Company not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CompanyResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetCompany([Required] int id)
        {
            var company = await _context.Companies
                .Include(c => c.Sites)
                .Include(c => c.Audits)
                .Where(c => c.Id == id)
                .Select(c => new CompanyResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Address = c.Address,
                    Phone = c.Phone,
                    Email = c.Email,
                    Website = c.Website,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    SitesCount = c.Sites.Count(s => s.IsActive),
                    AuditsCount = c.Audits.Count()
                })
                .FirstOrDefaultAsync();

            if (company == null)
                return NotFound(ApiResponse.CreateError("Company not found", 404));

            return Ok(ApiResponse<CompanyResponseDto>.CreateSuccess(company, "Company retrieved successfully"));
        }

        /// <summary>
        /// Create a new company
        /// </summary>
        /// <param name="model">Company creation data</param>
        /// <returns>Created company details</returns>
        /// <response code="201">Company created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Company with name or code already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CompanyResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            // Check if company with name already exists
            var existingCompany = await _context.Companies
                .FirstOrDefaultAsync(c => c.Name == model.Name);
            if (existingCompany != null)
                return Conflict(ApiResponse.CreateError("Company with this name already exists", 409));

            // Check if company with code already exists (if code is provided)
            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                var existingByCode = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Code == model.Code);
                if (existingByCode != null)
                    return Conflict(ApiResponse.CreateError("Company with this code already exists", 409));
            }

            // Check if company with email already exists (if email is provided)
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var existingByEmail = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Email == model.Email);
                if (existingByEmail != null)
                    return Conflict(ApiResponse.CreateError("Company with this email already exists", 409));
            }

            var company = new Company
            {
                Name = model.Name,
                Code = model.Code,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                Website = model.Website,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            var companyDto = new CompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                Code = company.Code,
                Address = company.Address,
                Phone = company.Phone,
                Email = company.Email,
                Website = company.Website,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt,
                SitesCount = 0,
                AuditsCount = 0
            };

            return CreatedAtAction(nameof(GetCompany), new { id = company.Id },
                ApiResponse<CompanyResponseDto>.CreateSuccess(companyDto, "Company created successfully", 201));
        }

        /// <summary>
        /// Update an existing company
        /// </summary>
        /// <param name="id">Company unique identifier</param>
        /// <param name="model">Company update data</param>
        /// <returns>Updated company details</returns>
        /// <response code="200">Company updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Company not found</response>
        /// <response code="409">Company name, code, or email is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CompanyResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateCompany([Required] int id, [FromBody] UpdateCompanyDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var company = await _context.Companies
                .Include(c => c.Sites)
                .Include(c => c.Audits)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
                return NotFound(ApiResponse.CreateError("Company not found", 404));

            // Check if name is being changed and if new name already exists
            if (company.Name != model.Name)
            {
                var existingCompany = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Name == model.Name && c.Id != id);
                if (existingCompany != null)
                    return Conflict(ApiResponse.CreateError("Company name is already in use", 409));
            }

            // Check if code is being changed and if new code already exists
            if (company.Code != model.Code && !string.IsNullOrWhiteSpace(model.Code))
            {
                var existingByCode = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Code == model.Code && c.Id != id);
                if (existingByCode != null)
                    return Conflict(ApiResponse.CreateError("Company code is already in use", 409));
            }

            // Check if email is being changed and if new email already exists
            if (company.Email != model.Email && !string.IsNullOrWhiteSpace(model.Email))
            {
                var existingByEmail = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Email == model.Email && c.Id != id);
                if (existingByEmail != null)
                    return Conflict(ApiResponse.CreateError("Company email is already in use", 409));
            }

            company.Name = model.Name;
            company.Code = model.Code;
            company.Address = model.Address;
            company.Phone = model.Phone;
            company.Email = model.Email;
            company.Website = model.Website;
            company.IsActive = model.IsActive;
            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var companyDto = new CompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                Code = company.Code,
                Address = company.Address,
                Phone = company.Phone,
                Email = company.Email,
                Website = company.Website,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt,
                SitesCount = company.Sites.Count(s => s.IsActive),
                AuditsCount = company.Audits.Count()
            };

            return Ok(ApiResponse<CompanyResponseDto>.CreateSuccess(companyDto, "Company updated successfully"));
        }

        /// <summary>
        /// Delete a company
        /// </summary>
        /// <param name="id">Company unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Company deleted successfully</response>
        /// <response code="400">Cannot delete company with existing sites or audits</response>
        /// <response code="404">Company not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteCompany([Required] int id)
        {
            var company = await _context.Companies
                .Include(c => c.Sites)
                .Include(c => c.Audits)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return NotFound(ApiResponse.CreateError("Company not found", 404));

            // Check if company has sites or audits
            if (company.Sites.Any() || company.Audits.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete company with existing sites or audits. Please move or delete them first."));

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Company deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a company
        /// </summary>
        /// <param name="id">Company unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Company status updated successfully</response>
        /// <response code="404">Company not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateCompanyStatus([Required] int id, [FromQuery] bool isActive)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return NotFound(ApiResponse.CreateError("Company not found", 404));

            company.IsActive = isActive;
            company.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var status = isActive ? "activated" : "deactivated";
            return Ok(ApiResponse.CreateSuccess($"Company {status} successfully"));
        }

        /// <summary>
        /// Get sites by company
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of sites in the company</returns>
        /// <response code="200">Returns sites in the company</response>
        /// <response code="404">Company not found</response>
        [HttpGet("{id}/sites")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<SiteResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetSitesByCompany([Required] int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var companyExists = await _context.Companies.AnyAsync(c => c.Id == id);
            if (!companyExists)
                return NotFound(ApiResponse.CreateError("Company not found", 404));

            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Sites
                .Include(s => s.Company)
                .Include(s => s.City)
                .Include(s => s.Audits)
                .Where(s => s.CompanyId == id);

            var totalCount = await query.CountAsync();
            var sites = await query
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

            var pagedResponse = new PagedResponseDto<SiteResponseDto>
            {
                Items = sites,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<SiteResponseDto>>.CreateSuccess(pagedResponse, "Company sites retrieved successfully"));
        }

        /// <summary>
        /// Get audits by company
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of audits for the company</returns>
        /// <response code="200">Returns audits for the company</response>
        /// <response code="404">Company not found</response>
        [HttpGet("{id}/audits")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<AuditResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAuditsByCompany([Required] int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var companyExists = await _context.Companies.AnyAsync(c => c.Id == id);
            if (!companyExists)
                return NotFound(ApiResponse.CreateError("Company not found", 404));

            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Audits
                .Include(a => a.AuditType)
                .Include(a => a.Company)
                .Include(a => a.Site)
                .Include(a => a.LeadAuditor)
                .Where(a => a.CompanyId == id);

            var totalCount = await query.CountAsync();
            var audits = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditResponseDto
                {
                    Id = a.Id,
                    AuditNumber = a.AuditNumber,
                    Title = a.Title,
                    Description = a.Description,
                    AuditTypeId = a.AuditTypeId,
                    AuditTypeName = a.AuditType.Name,
                    CompanyId = a.CompanyId,
                    CompanyName = a.Company.Name,
                    SiteId = a.SiteId,
                    SiteName = a.Site.Name,
                    LeadAuditorId = a.LeadAuditorId,
                    LeadAuditorName = a.LeadAuditor != null ? $"{a.LeadAuditor.FirstName} {a.LeadAuditor.LastName}" : null,
                    PlannedStartDate = a.PlannedStartDate,
                    PlannedEndDate = a.PlannedEndDate,
                    ActualStartDate = a.ActualStartDate,
                    ActualEndDate = a.ActualEndDate,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

            var pagedResponse = new PagedResponseDto<AuditResponseDto>
            {
                Items = audits,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResponseDto<AuditResponseDto>>.CreateSuccess(pagedResponse, "Company audits retrieved successfully"));
        }

        /// <summary>
        /// Search companies by name, code, or email
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching companies</returns>
        /// <response code="200">Returns matching companies</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyResponseDto>>), 200)]
        public async Task<IActionResult> SearchCompanies([FromQuery][Required] string searchTerm)
        {
            var companies = await _context.Companies
                .Where(c => c.IsActive && (c.Name.Contains(searchTerm) ||
                           (c.Code != null && c.Code.Contains(searchTerm)) ||
                           (c.Email != null && c.Email.Contains(searchTerm))))
                .OrderBy(c => c.Name)
                .Select(c => new CompanyResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Address = c.Address,
                    Phone = c.Phone,
                    Email = c.Email,
                    Website = c.Website,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    SitesCount = c.Sites.Count(s => s.IsActive),
                    AuditsCount = c.Audits.Count()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<CompanyResponseDto>>.CreateSuccess(companies, "Companies search completed successfully"));
        }
    }
}