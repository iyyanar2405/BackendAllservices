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
    /// Audit Types management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Audit Types")]
    public class AuditTypesController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        public AuditTypesController(CustomerPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<AuditTypeResponseDto>>), 200)]
        public async Task<IActionResult> GetAuditTypes(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.AuditTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(at => at.Name.Contains(searchTerm) || (at.Description != null && at.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(at => at.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(at => at.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(at => new AuditTypeResponseDto
                {
                    Id = at.Id,
                    Name = at.Name,
                    Description = at.Description,
                    IsActive = at.IsActive,
                    CreatedAt = at.CreatedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<AuditTypeResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<AuditTypeResponseDto>>.CreateSuccess(response, "Audit types retrieved successfully"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditTypeResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAuditType([Required] int id)
        {
            var auditType = await _context.AuditTypes
                .Where(at => at.Id == id)
                .Select(at => new AuditTypeResponseDto
                {
                    Id = at.Id,
                    Name = at.Name,
                    Description = at.Description,
                    IsActive = at.IsActive,
                    CreatedAt = at.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (auditType == null)
                return NotFound(ApiResponse.CreateError("Audit type not found", 404));

            return Ok(ApiResponse<AuditTypeResponseDto>.CreateSuccess(auditType, "Audit type retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuditTypeResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateAuditType([FromBody] CreateAuditTypeDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.AuditTypes.FirstOrDefaultAsync(at => at.Name == model.Name);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Audit type with this name already exists", 409));

            var auditType = new AuditType
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditTypes.Add(auditType);
            await _context.SaveChangesAsync();

            var dto = new AuditTypeResponseDto
            {
                Id = auditType.Id,
                Name = auditType.Name,
                Description = auditType.Description,
                IsActive = auditType.IsActive,
                CreatedAt = auditType.CreatedAt
            };

            return CreatedAtAction(nameof(GetAuditType), new { id = auditType.Id },
                ApiResponse<AuditTypeResponseDto>.CreateSuccess(dto, "Audit type created successfully", 201));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditTypeResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateAuditType([Required] int id, [FromBody] UpdateAuditTypeDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var auditType = await _context.AuditTypes.FindAsync(id);
            if (auditType == null)
                return NotFound(ApiResponse.CreateError("Audit type not found", 404));

            if (auditType.Name != model.Name)
            {
                var existing = await _context.AuditTypes.FirstOrDefaultAsync(at => at.Name == model.Name && at.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Audit type name is already in use", 409));
            }

            auditType.Name = model.Name;
            auditType.Description = model.Description;
            auditType.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            var dto = new AuditTypeResponseDto
            {
                Id = auditType.Id,
                Name = auditType.Name,
                Description = auditType.Description,
                IsActive = auditType.IsActive,
                CreatedAt = auditType.CreatedAt
            };

            return Ok(ApiResponse<AuditTypeResponseDto>.CreateSuccess(dto, "Audit type updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteAuditType([Required] int id)
        {
            var auditType = await _context.AuditTypes.Include(at => at.Audits).FirstOrDefaultAsync(at => at.Id == id);
            if (auditType == null)
                return NotFound(ApiResponse.CreateError("Audit type not found", 404));

            if (auditType.Audits.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete audit type with existing audits"));

            _context.AuditTypes.Remove(auditType);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Audit type deleted successfully"));
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateAuditTypeStatus([Required] int id, [FromQuery] bool isActive)
        {
            var auditType = await _context.AuditTypes.FindAsync(id);
            if (auditType == null)
                return NotFound(ApiResponse.CreateError("Audit type not found", 404));

            auditType.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Audit type {(isActive ? "activated" : "deactivated")} successfully"));
        }
    }
}