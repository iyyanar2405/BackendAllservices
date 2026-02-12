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
    /// Audits management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Audits")]
    public class AuditsController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the AuditsController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public AuditsController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all audits with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering audits by title or audit number</param>
        /// <param name="status">Filter by audit status</param>
        /// <param name="auditTypeId">Filter by audit type</param>
        /// <param name="companyId">Filter by company</param>
        /// <param name="leadAuditorId">Filter by lead auditor</param>
        /// <returns>Paginated list of audits</returns>
        /// <response code="200">Returns paginated list of audits</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<AuditResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetAudits(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] AuditStatus? status = null,
            [FromQuery] int? auditTypeId = null,
            [FromQuery] int? companyId = null,
            [FromQuery] int? leadAuditorId = null)
        {
            if (pageNumber < 1)
                return BadRequest(ApiResponse.CreateError("Page number must be greater than 0"));

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Page size must be between 1 and 100"));

            var query = _context.Audits
                .Include(a => a.AuditType)
                .Include(a => a.Company)
                .Include(a => a.Site)
                .Include(a => a.LeadAuditor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm) ||
                                       a.AuditNumber.Contains(searchTerm) ||
                                       (a.Description != null && a.Description.Contains(searchTerm)));
            }

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            if (auditTypeId.HasValue)
            {
                query = query.Where(a => a.AuditTypeId == auditTypeId.Value);
            }

            if (companyId.HasValue)
            {
                query = query.Where(a => a.CompanyId == companyId.Value);
            }

            if (leadAuditorId.HasValue)
            {
                query = query.Where(a => a.LeadAuditorId == leadAuditorId.Value);
            }

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

            return Ok(ApiResponse<PagedResponseDto<AuditResponseDto>>.CreateSuccess(pagedResponse, "Audits retrieved successfully"));
        }

        /// <summary>
        /// Get a specific audit by ID
        /// </summary>
        /// <param name="id">Audit unique identifier</param>
        /// <returns>Audit details</returns>
        /// <response code="200">Returns audit details</response>
        /// <response code="404">Audit not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAudit([Required] int id)
        {
            var audit = await _context.Audits
                .Include(a => a.AuditType)
                .Include(a => a.Company)
                .Include(a => a.Site)
                .Include(a => a.LeadAuditor)
                .Where(a => a.Id == id)
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
                .FirstOrDefaultAsync();

            if (audit == null)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            return Ok(ApiResponse<AuditResponseDto>.CreateSuccess(audit, "Audit retrieved successfully"));
        }

        /// <summary>
        /// Create a new audit
        /// </summary>
        /// <param name="model">Audit creation data</param>
        /// <returns>Created audit details</returns>
        /// <response code="201">Audit created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Related entity not found</response>
        /// <response code="409">Audit number already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuditResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateAudit([FromBody] CreateAuditDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            // Validate planned dates
            if (model.PlannedEndDate <= model.PlannedStartDate)
                return BadRequest(ApiResponse.CreateError("Planned end date must be after planned start date"));

            // Check if audit number already exists
            var existingAudit = await _context.Audits
                .FirstOrDefaultAsync(a => a.AuditNumber == model.AuditNumber);
            if (existingAudit != null)
                return Conflict(ApiResponse.CreateError("Audit number already exists", 409));

            // Validate related entities exist
            var auditTypeExists = await _context.AuditTypes.AnyAsync(at => at.Id == model.AuditTypeId && at.IsActive);
            if (!auditTypeExists)
                return NotFound(ApiResponse.CreateError("Audit type not found or inactive", 404));

            var companyExists = await _context.Companies.AnyAsync(c => c.Id == model.CompanyId && c.IsActive);
            if (!companyExists)
                return NotFound(ApiResponse.CreateError("Company not found or inactive", 404));

            var siteExists = await _context.Sites.AnyAsync(s => s.Id == model.SiteId && s.IsActive);
            if (!siteExists)
                return NotFound(ApiResponse.CreateError("Site not found or inactive", 404));

            if (model.LeadAuditorId.HasValue)
            {
                var auditorExists = await _context.Users.AnyAsync(u => u.Id == model.LeadAuditorId.Value && u.IsActive);
                if (!auditorExists)
                    return NotFound(ApiResponse.CreateError("Lead auditor not found or inactive", 404));
            }

            var audit = new CustomerPortalAudit
            {
                AuditNumber = model.AuditNumber,
                Title = model.Title,
                Description = model.Description,
                AuditTypeId = model.AuditTypeId,
                CompanyId = model.CompanyId,
                SiteId = model.SiteId,
                LeadAuditorId = model.LeadAuditorId,
                PlannedStartDate = model.PlannedStartDate,
                PlannedEndDate = model.PlannedEndDate,
                Status = AuditStatus.Planned,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Audits.Add(audit);
            await _context.SaveChangesAsync();

            // Load related data for response
            await _context.Entry(audit).Reference(a => a.AuditType).LoadAsync();
            await _context.Entry(audit).Reference(a => a.Company).LoadAsync();
            await _context.Entry(audit).Reference(a => a.Site).LoadAsync();
            if (audit.LeadAuditorId.HasValue)
                await _context.Entry(audit).Reference(a => a.LeadAuditor).LoadAsync();

            var auditDto = new AuditResponseDto
            {
                Id = audit.Id,
                AuditNumber = audit.AuditNumber,
                Title = audit.Title,
                Description = audit.Description,
                AuditTypeId = audit.AuditTypeId,
                AuditTypeName = audit.AuditType.Name,
                CompanyId = audit.CompanyId,
                CompanyName = audit.Company.Name,
                SiteId = audit.SiteId,
                SiteName = audit.Site.Name,
                LeadAuditorId = audit.LeadAuditorId,
                LeadAuditorName = audit.LeadAuditor != null ? $"{audit.LeadAuditor.FirstName} {audit.LeadAuditor.LastName}" : null,
                PlannedStartDate = audit.PlannedStartDate,
                PlannedEndDate = audit.PlannedEndDate,
                ActualStartDate = audit.ActualStartDate,
                ActualEndDate = audit.ActualEndDate,
                Status = audit.Status,
                CreatedAt = audit.CreatedAt,
                UpdatedAt = audit.UpdatedAt
            };

            return CreatedAtAction(nameof(GetAudit), new { id = audit.Id },
                ApiResponse<AuditResponseDto>.CreateSuccess(auditDto, "Audit created successfully", 201));
        }

        /// <summary>
        /// Update an existing audit
        /// </summary>
        /// <param name="id">Audit unique identifier</param>
        /// <param name="model">Audit update data</param>
        /// <returns>Updated audit details</returns>
        /// <response code="200">Audit updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Audit or related entity not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateAudit([Required] int id, [FromBody] UpdateAuditDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.CreateValidationError(errors));
            }

            var audit = await _context.Audits
                .Include(a => a.AuditType)
                .Include(a => a.Company)
                .Include(a => a.Site)
                .Include(a => a.LeadAuditor)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (audit == null)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            // Validate dates
            if (model.PlannedEndDate <= model.PlannedStartDate)
                return BadRequest(ApiResponse.CreateError("Planned end date must be after planned start date"));

            if (model.ActualStartDate.HasValue && model.ActualEndDate.HasValue &&
                model.ActualEndDate.Value <= model.ActualStartDate.Value)
                return BadRequest(ApiResponse.CreateError("Actual end date must be after actual start date"));

            // Validate related entities exist
            var auditTypeExists = await _context.AuditTypes.AnyAsync(at => at.Id == model.AuditTypeId && at.IsActive);
            if (!auditTypeExists)
                return NotFound(ApiResponse.CreateError("Audit type not found or inactive", 404));

            var companyExists = await _context.Companies.AnyAsync(c => c.Id == model.CompanyId && c.IsActive);
            if (!companyExists)
                return NotFound(ApiResponse.CreateError("Company not found or inactive", 404));

            var siteExists = await _context.Sites.AnyAsync(s => s.Id == model.SiteId && s.IsActive);
            if (!siteExists)
                return NotFound(ApiResponse.CreateError("Site not found or inactive", 404));

            if (model.LeadAuditorId.HasValue)
            {
                var auditorExists = await _context.Users.AnyAsync(u => u.Id == model.LeadAuditorId.Value && u.IsActive);
                if (!auditorExists)
                    return NotFound(ApiResponse.CreateError("Lead auditor not found or inactive", 404));
            }

            audit.Title = model.Title;
            audit.Description = model.Description;
            audit.AuditTypeId = model.AuditTypeId;
            audit.CompanyId = model.CompanyId;
            audit.SiteId = model.SiteId;
            audit.LeadAuditorId = model.LeadAuditorId;
            audit.PlannedStartDate = model.PlannedStartDate;
            audit.PlannedEndDate = model.PlannedEndDate;
            audit.ActualStartDate = model.ActualStartDate;
            audit.ActualEndDate = model.ActualEndDate;
            audit.Status = model.Status;
            audit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var auditDto = new AuditResponseDto
            {
                Id = audit.Id,
                AuditNumber = audit.AuditNumber,
                Title = audit.Title,
                Description = audit.Description,
                AuditTypeId = audit.AuditTypeId,
                AuditTypeName = audit.AuditType.Name,
                CompanyId = audit.CompanyId,
                CompanyName = audit.Company.Name,
                SiteId = audit.SiteId,
                SiteName = audit.Site.Name,
                LeadAuditorId = audit.LeadAuditorId,
                LeadAuditorName = audit.LeadAuditor != null ? $"{audit.LeadAuditor.FirstName} {audit.LeadAuditor.LastName}" : null,
                PlannedStartDate = audit.PlannedStartDate,
                PlannedEndDate = audit.PlannedEndDate,
                ActualStartDate = audit.ActualStartDate,
                ActualEndDate = audit.ActualEndDate,
                Status = audit.Status,
                CreatedAt = audit.CreatedAt,
                UpdatedAt = audit.UpdatedAt
            };

            return Ok(ApiResponse<AuditResponseDto>.CreateSuccess(auditDto, "Audit updated successfully"));
        }

        /// <summary>
        /// Delete an audit
        /// </summary>
        /// <param name="id">Audit unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Audit deleted successfully</response>
        /// <response code="400">Cannot delete audit with existing relationships</response>
        /// <response code="404">Audit not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteAudit([Required] int id)
        {
            var audit = await _context.Audits
                .Include(a => a.AuditTeamMembers)
                .Include(a => a.AuditServices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (audit == null)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            // Check if audit has relationships that prevent deletion
            if (audit.AuditTeamMembers.Any() || audit.AuditServices.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete audit with existing team members or services. Please remove them first."));

            _context.Audits.Remove(audit);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Audit deleted successfully"));
        }

        /// <summary>
        /// Update audit status
        /// </summary>
        /// <param name="id">Audit unique identifier</param>
        /// <param name="status">New status</param>
        /// <returns>Status update confirmation</returns>
        /// <response code="200">Audit status updated successfully</response>
        /// <response code="404">Audit not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateAuditStatus([Required] int id, [FromQuery][Required] AuditStatus status)
        {
            var audit = await _context.Audits.FindAsync(id);
            if (audit == null)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            audit.Status = status;
            audit.UpdatedAt = DateTime.UtcNow;

            // Auto-set actual dates based on status
            if (status == AuditStatus.InProgress && audit.ActualStartDate == null)
                audit.ActualStartDate = DateTime.UtcNow;
            else if (status == AuditStatus.Completed && audit.ActualEndDate == null)
                audit.ActualEndDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Audit status updated to {status} successfully"));
        }

        /// <summary>
        /// Get audit team members
        /// </summary>
        /// <param name="id">Audit ID</param>
        /// <returns>List of audit team members</returns>
        /// <response code="200">Returns audit team members</response>
        /// <response code="404">Audit not found</response>
        [HttpGet("{id}/team-members")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAuditTeamMembers([Required] int id)
        {
            var auditExists = await _context.Audits.AnyAsync(a => a.Id == id);
            if (!auditExists)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            var teamMembers = await _context.AuditTeamMembers
                .Include(atm => atm.User)
                .Where(atm => atm.AuditId == id)
                .Select(atm => new
                {
                    Id = atm.Id,
                    UserId = atm.UserId,
                    UserName = $"{atm.User.FirstName} {atm.User.LastName}",
                    Role = atm.Role,
                    AssignedAt = atm.AssignedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(teamMembers, "Audit team members retrieved successfully"));
        }

        /// <summary>
        /// Get audit services
        /// </summary>
        /// <param name="id">Audit ID</param>
        /// <returns>List of audit services</returns>
        /// <response code="200">Returns audit services</response>
        /// <response code="404">Audit not found</response>
        [HttpGet("{id}/services")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAuditServices([Required] int id)
        {
            var auditExists = await _context.Audits.AnyAsync(a => a.Id == id);
            if (!auditExists)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            var services = await _context.AuditServices
                .Include(aus => aus.Service)
                .Where(aus => aus.AuditId == id)
                .Select(aus => new
                {
                    Id = aus.Id,
                    ServiceId = aus.ServiceId,
                    ServiceName = aus.Service.Name,
                    ServiceCode = aus.Service.Code,
                    ServiceDescription = aus.Service.Description
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(services, "Audit services retrieved successfully"));
        }
    }
}