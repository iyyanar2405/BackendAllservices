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
    /// Services management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Services")]
    public class ServicesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the ServicesController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public ServicesController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all services with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering services by name, code, or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of services</returns>
        /// <response code="200">Returns paginated list of services</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<ServiceResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetServices(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Services
                .Include(s => s.AuditServices)
                .Include(s => s.UserServiceAccesses)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(s => s.Name.Contains(searchTerm) || 
                                       (s.Code != null && s.Code.Contains(searchTerm)) ||
                                       (s.Description != null && s.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(s => s.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ServiceResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    AuditsCount = s.AuditServices.Count(),
                    UsersCount = s.UserServiceAccesses.Count()
                })
                .ToListAsync();

            var response = new PagedResponseDto<ServiceResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<ServiceResponseDto>>.CreateSuccess(response, "Services retrieved successfully"));
        }

        /// <summary>
        /// Get a specific service by ID
        /// </summary>
        /// <param name="id">Service unique identifier</param>
        /// <returns>Service details</returns>
        /// <response code="200">Returns service details</response>
        /// <response code="404">Service not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ServiceResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetService([Required] int id)
        {
            var service = await _context.Services
                .Include(s => s.AuditServices)
                .Include(s => s.UserServiceAccesses)
                .Where(s => s.Id == id)
                .Select(s => new ServiceResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    AuditsCount = s.AuditServices.Count(),
                    UsersCount = s.UserServiceAccesses.Count()
                })
                .FirstOrDefaultAsync();

            if (service == null)
                return NotFound(ApiResponse.CreateError("Service not found", 404));

            return Ok(ApiResponse<ServiceResponseDto>.CreateSuccess(service, "Service retrieved successfully"));
        }

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="model">Service creation data</param>
        /// <returns>Created service details</returns>
        /// <response code="201">Service created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Service with name or code already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ServiceResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.Services.FirstOrDefaultAsync(s => s.Name == model.Name);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Service with this name already exists", 409));

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                var existingByCode = await _context.Services.FirstOrDefaultAsync(s => s.Code == model.Code);
                if (existingByCode != null)
                    return Conflict(ApiResponse.CreateError("Service with this code already exists", 409));
            }

            var service = new CustomerPortalService
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            var dto = new ServiceResponseDto
            {
                Id = service.Id,
                Name = service.Name,
                Code = service.Code,
                Description = service.Description,
                IsActive = service.IsActive,
                CreatedAt = service.CreatedAt,
                AuditsCount = 0,
                UsersCount = 0
            };

            return CreatedAtAction(nameof(GetService), new { id = service.Id },
                ApiResponse<ServiceResponseDto>.CreateSuccess(dto, "Service created successfully", 201));
        }

        /// <summary>
        /// Update an existing service
        /// </summary>
        /// <param name="id">Service unique identifier</param>
        /// <param name="model">Service update data</param>
        /// <returns>Updated service details</returns>
        /// <response code="200">Service updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Service not found</response>
        /// <response code="409">Service name or code is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ServiceResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateService([Required] int id, [FromBody] UpdateServiceDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var service = await _context.Services
                .Include(s => s.AuditServices)
                .Include(s => s.UserServiceAccesses)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
                return NotFound(ApiResponse.CreateError("Service not found", 404));

            if (service.Name != model.Name)
            {
                var existing = await _context.Services.FirstOrDefaultAsync(s => s.Name == model.Name && s.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Service name is already in use", 409));
            }

            if (service.Code != model.Code && !string.IsNullOrWhiteSpace(model.Code))
            {
                var existingByCode = await _context.Services.FirstOrDefaultAsync(s => s.Code == model.Code && s.Id != id);
                if (existingByCode != null)
                    return Conflict(ApiResponse.CreateError("Service code is already in use", 409));
            }

            service.Name = model.Name;
            service.Code = model.Code;
            service.Description = model.Description;
            service.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new ServiceResponseDto
            {
                Id = service.Id,
                Name = service.Name,
                Code = service.Code,
                Description = service.Description,
                IsActive = service.IsActive,
                CreatedAt = service.CreatedAt,
                AuditsCount = service.AuditServices.Count(),
                UsersCount = service.UserServiceAccesses.Count()
            };

            return Ok(ApiResponse<ServiceResponseDto>.CreateSuccess(dto, "Service updated successfully"));
        }

        /// <summary>
        /// Delete a service
        /// </summary>
        /// <param name="id">Service unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Service deleted successfully</response>
        /// <response code="400">Cannot delete service with existing relationships</response>
        /// <response code="404">Service not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteService([Required] int id)
        {
            var service = await _context.Services
                .Include(s => s.AuditServices)
                .Include(s => s.UserServiceAccesses)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound(ApiResponse.CreateError("Service not found", 404));

            if (service.AuditServices.Any() || service.UserServiceAccesses.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete service with existing audit or user relationships"));

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Service deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a service
        /// </summary>
        /// <param name="id">Service unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Service status updated successfully</response>
        /// <response code="404">Service not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateServiceStatus([Required] int id, [FromQuery] bool isActive)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound(ApiResponse.CreateError("Service not found", 404));

            service.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Service {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Search services by name, code, or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching services</returns>
        /// <response code="200">Returns matching services</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServiceResponseDto>>), 200)]
        public async Task<IActionResult> SearchServices([FromQuery][Required] string searchTerm)
        {
            var services = await _context.Services
                .Where(s => s.IsActive && (s.Name.Contains(searchTerm) ||
                           (s.Code != null && s.Code.Contains(searchTerm)) ||
                           (s.Description != null && s.Description.Contains(searchTerm))))
                .OrderBy(s => s.Name)
                .Select(s => new ServiceResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    AuditsCount = s.AuditServices.Count(),
                    UsersCount = s.UserServiceAccesses.Count()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<ServiceResponseDto>>.CreateSuccess(services, "Services search completed successfully"));
        }
    }
}