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
    /// Audit Services management operations for services associated with audits
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Audit Services")]
    public class AuditServicesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the AuditServicesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public AuditServicesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all audit services with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="auditId">Filter by audit</param>
        /// <param name="serviceId">Filter by service</param>
        /// <returns>Paginated list of audit services</returns>
        /// <response code="200">Returns paginated list of audit services</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<AuditServiceResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetAuditServices(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? auditId = null,
            [FromQuery] int? serviceId = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.AuditServices
                .Include(a => a.Audit)
                .Include(a => a.Service)
                .AsQueryable();

            if (auditId.HasValue)
                query = query.Where(a => a.AuditId == auditId.Value);

            if (serviceId.HasValue)
                query = query.Where(a => a.ServiceId == serviceId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.Audit.AuditNumber)
                .ThenBy(a => a.Service.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditServiceResponseDto
                {
                    Id = a.Id,
                    AuditId = a.AuditId,
                    AuditNumber = a.Audit.AuditNumber,
                    AuditTitle = a.Audit.Title,
                    ServiceId = a.ServiceId,
                    ServiceName = a.Service.Name,
                    ServiceCode = a.Service.Code
                })
                .ToListAsync();

            var response = new PagedResponseDto<AuditServiceResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<AuditServiceResponseDto>>.CreateSuccess(response, "Audit services retrieved successfully"));
        }

        /// <summary>
        /// Get a specific audit service by ID
        /// </summary>
        /// <param name="id">Audit service unique identifier</param>
        /// <returns>Audit service details</returns>
        /// <response code="200">Returns audit service details</response>
        /// <response code="404">Audit service not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditServiceResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAuditService([Required] int id)
        {
            var auditService = await _context.AuditServices
                .Include(a => a.Audit)
                .Include(a => a.Service)
                .Where(a => a.Id == id)
                .Select(a => new AuditServiceResponseDto
                {
                    Id = a.Id,
                    AuditId = a.AuditId,
                    AuditNumber = a.Audit.AuditNumber,
                    AuditTitle = a.Audit.Title,
                    ServiceId = a.ServiceId,
                    ServiceName = a.Service.Name,
                    ServiceCode = a.Service.Code
                })
                .FirstOrDefaultAsync();

            if (auditService == null)
                return NotFound(ApiResponse.CreateError("Audit service not found", 404));

            return Ok(ApiResponse<AuditServiceResponseDto>.CreateSuccess(auditService, "Audit service retrieved successfully"));
        }

        /// <summary>
        /// Create a new audit service association
        /// </summary>
        /// <param name="model">Audit service creation data</param>
        /// <returns>Created audit service details</returns>
        /// <response code="201">Audit service created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Audit or service not found</response>
        /// <response code="409">Audit service association already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuditServiceResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateAuditService([FromBody] CreateAuditServiceDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var auditExists = await _context.Audits.AnyAsync(a => a.Id == model.AuditId);
            if (!auditExists)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            var serviceExists = await _context.Services.AnyAsync(s => s.Id == model.ServiceId && s.IsActive);
            if (!serviceExists)
                return NotFound(ApiResponse.CreateError("Service not found or inactive", 404));

            var existing = await _context.AuditServices.FirstOrDefaultAsync(a => a.AuditId == model.AuditId && a.ServiceId == model.ServiceId);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Audit service association already exists", 409));

            var auditService = new AuditService
            {
                AuditId = model.AuditId,
                ServiceId = model.ServiceId
            };

            _context.AuditServices.Add(auditService);
            await _context.SaveChangesAsync();

            await _context.Entry(auditService).Reference(a => a.Audit).LoadAsync();
            await _context.Entry(auditService).Reference(a => a.Service).LoadAsync();

            var dto = new AuditServiceResponseDto
            {
                Id = auditService.Id,
                AuditId = auditService.AuditId,
                AuditNumber = auditService.Audit.AuditNumber,
                AuditTitle = auditService.Audit.Title,
                ServiceId = auditService.ServiceId,
                ServiceName = auditService.Service.Name,
                ServiceCode = auditService.Service.Code
            };

            return CreatedAtAction(nameof(GetAuditService), new { id = auditService.Id },
                ApiResponse<AuditServiceResponseDto>.CreateSuccess(dto, "Audit service created successfully", 201));
        }

        /// <summary>
        /// Delete an audit service association
        /// </summary>
        /// <param name="id">Audit service unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Audit service deleted successfully</response>
        /// <response code="404">Audit service not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteAuditService([Required] int id)
        {
            var auditService = await _context.AuditServices.FindAsync(id);
            if (auditService == null)
                return NotFound(ApiResponse.CreateError("Audit service not found", 404));

            _context.AuditServices.Remove(auditService);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Audit service deleted successfully"));
        }

        /// <summary>
        /// Get services by audit
        /// </summary>
        /// <param name="auditId">Audit ID</param>
        /// <returns>List of services associated with the audit</returns>
        /// <response code="200">Returns services for the audit</response>
        /// <response code="404">Audit not found</response>
        [HttpGet("audit/{auditId}/services")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetServicesByAudit([Required] int auditId)
        {
            var auditExists = await _context.Audits.AnyAsync(a => a.Id == auditId);
            if (!auditExists)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            var services = await _context.AuditServices
                .Include(a => a.Service)
                .Where(a => a.AuditId == auditId)
                .Select(a => new
                {
                    Id = a.Id,
                    ServiceId = a.ServiceId,
                    ServiceName = a.Service.Name,
                    ServiceCode = a.Service.Code,
                    ServiceDescription = a.Service.Description
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(services, "Audit services retrieved successfully"));
        }

        /// <summary>
        /// Get audits by service
        /// </summary>
        /// <param name="serviceId">Service ID</param>
        /// <returns>List of audits associated with the service</returns>
        /// <response code="200">Returns audits for the service</response>
        /// <response code="404">Service not found</response>
        [HttpGet("service/{serviceId}/audits")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAuditsByService([Required] int serviceId)
        {
            var serviceExists = await _context.Services.AnyAsync(s => s.Id == serviceId);
            if (!serviceExists)
                return NotFound(ApiResponse.CreateError("Service not found", 404));

            var audits = await _context.AuditServices
                .Include(a => a.Audit)
                .ThenInclude(au => au.Company)
                .Where(a => a.ServiceId == serviceId)
                .Select(a => new
                {
                    Id = a.Id,
                    AuditId = a.AuditId,
                    AuditNumber = a.Audit.AuditNumber,
                    AuditTitle = a.Audit.Title,
                    CompanyName = a.Audit.Company.Name,
                    Status = a.Audit.Status,
                    PlannedStartDate = a.Audit.PlannedStartDate
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(audits, "Service audits retrieved successfully"));
        }

        /// <summary>
        /// Bulk assign services to an audit
        /// </summary>
        /// <param name="auditId">Audit ID</param>
        /// <param name="serviceIds">List of service IDs to assign</param>
        /// <returns>Bulk assignment confirmation</returns>
        /// <response code="200">Services assigned successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Audit not found or some services not found</response>
        [HttpPost("audit/{auditId}/services/bulk")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> BulkAssignServicesToAudit([Required] int auditId, [FromBody] List<int> serviceIds)
        {
            if (serviceIds == null || !serviceIds.Any())
                return BadRequest(ApiResponse.CreateError("Service IDs list cannot be empty"));

            var auditExists = await _context.Audits.AnyAsync(a => a.Id == auditId);
            if (!auditExists)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            var existingServices = await _context.Services
                .Where(s => serviceIds.Contains(s.Id) && s.IsActive)
                .Select(s => s.Id)
                .ToListAsync();

            if (existingServices.Count != serviceIds.Count)
                return NotFound(ApiResponse.CreateError("Some services not found or inactive", 404));

            // Get existing associations
            var existingAssociations = await _context.AuditServices
                .Where(a => a.AuditId == auditId && serviceIds.Contains(a.ServiceId))
                .Select(a => a.ServiceId)
                .ToListAsync();

            // Only add new associations
            var newServiceIds = serviceIds.Except(existingAssociations).ToList();

            if (newServiceIds.Any())
            {
                var newAssociations = newServiceIds.Select(serviceId => new AuditService
                {
                    AuditId = auditId,
                    ServiceId = serviceId
                });

                _context.AuditServices.AddRange(newAssociations);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse.CreateSuccess($"Successfully assigned {newServiceIds.Count} new services to audit. {existingAssociations.Count} services were already assigned."));
        }
    }
}