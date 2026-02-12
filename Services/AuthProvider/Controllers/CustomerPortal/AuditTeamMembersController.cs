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
    /// Audit Team Members management operations for team members assigned to audits
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Audit Team Members")]
    public class AuditTeamMembersController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the AuditTeamMembersController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public AuditTeamMembersController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all audit team members with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="auditId">Filter by audit</param>
        /// <param name="userId">Filter by user</param>
        /// <param name="role">Filter by role</param>
        /// <returns>Paginated list of audit team members</returns>
        /// <response code="200">Returns paginated list of audit team members</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<AuditTeamMemberResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetAuditTeamMembers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? auditId = null,
            [FromQuery] int? userId = null,
            [FromQuery] string? role = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.AuditTeamMembers
                .Include(a => a.Audit)
                .Include(a => a.User)
                .AsQueryable();

            if (auditId.HasValue)
                query = query.Where(a => a.AuditId == auditId.Value);

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(a => a.Role.Contains(role));

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.Audit.AuditNumber)
                .ThenBy(a => a.Role)
                .ThenBy(a => a.User.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditTeamMemberResponseDto
                {
                    Id = a.Id,
                    AuditId = a.AuditId,
                    AuditNumber = a.Audit.AuditNumber,
                    AuditTitle = a.Audit.Title,
                    UserId = a.UserId,
                    UserName = $"{a.User.FirstName} {a.User.LastName}",
                    UserEmail = a.User.Email,
                    Role = a.Role,
                    AssignedAt = a.AssignedAt
                })
                .ToListAsync();

            var response = new PagedResponseDto<AuditTeamMemberResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<AuditTeamMemberResponseDto>>.CreateSuccess(response, "Audit team members retrieved successfully"));
        }

        /// <summary>
        /// Get a specific audit team member by ID
        /// </summary>
        /// <param name="id">Audit team member unique identifier</param>
        /// <returns>Audit team member details</returns>
        /// <response code="200">Returns audit team member details</response>
        /// <response code="404">Audit team member not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditTeamMemberResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAuditTeamMember([Required] int id)
        {
            var teamMember = await _context.AuditTeamMembers
                .Include(a => a.Audit)
                .Include(a => a.User)
                .Where(a => a.Id == id)
                .Select(a => new AuditTeamMemberResponseDto
                {
                    Id = a.Id,
                    AuditId = a.AuditId,
                    AuditNumber = a.Audit.AuditNumber,
                    AuditTitle = a.Audit.Title,
                    UserId = a.UserId,
                    UserName = $"{a.User.FirstName} {a.User.LastName}",
                    UserEmail = a.User.Email,
                    Role = a.Role,
                    AssignedAt = a.AssignedAt
                })
                .FirstOrDefaultAsync();

            if (teamMember == null)
                return NotFound(ApiResponse.CreateError("Audit team member not found", 404));

            return Ok(ApiResponse<AuditTeamMemberResponseDto>.CreateSuccess(teamMember, "Audit team member retrieved successfully"));
        }

        /// <summary>
        /// Create a new audit team member assignment
        /// </summary>
        /// <param name="model">Audit team member creation data</param>
        /// <returns>Created audit team member details</returns>
        /// <response code="201">Audit team member created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Audit or user not found</response>
        /// <response code="409">User already assigned to this audit</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuditTeamMemberResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateAuditTeamMember([FromBody] CreateAuditTeamMemberDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var auditExists = await _context.Audits.AnyAsync(a => a.Id == model.AuditId);
            if (!auditExists)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId && u.IsActive);
            if (!userExists)
                return NotFound(ApiResponse.CreateError("User not found or inactive", 404));

            var existing = await _context.AuditTeamMembers.FirstOrDefaultAsync(a => a.AuditId == model.AuditId && a.UserId == model.UserId);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("User already assigned to this audit", 409));

            var teamMember = new AuditTeamMember
            {
                AuditId = model.AuditId,
                UserId = model.UserId,
                Role = model.Role,
                AssignedAt = DateTime.UtcNow
            };

            _context.AuditTeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();

            await _context.Entry(teamMember).Reference(a => a.Audit).LoadAsync();
            await _context.Entry(teamMember).Reference(a => a.User).LoadAsync();

            var dto = new AuditTeamMemberResponseDto
            {
                Id = teamMember.Id,
                AuditId = teamMember.AuditId,
                AuditNumber = teamMember.Audit.AuditNumber,
                AuditTitle = teamMember.Audit.Title,
                UserId = teamMember.UserId,
                UserName = $"{teamMember.User.FirstName} {teamMember.User.LastName}",
                UserEmail = teamMember.User.Email,
                Role = teamMember.Role,
                AssignedAt = teamMember.AssignedAt
            };

            return CreatedAtAction(nameof(GetAuditTeamMember), new { id = teamMember.Id },
                ApiResponse<AuditTeamMemberResponseDto>.CreateSuccess(dto, "Audit team member created successfully", 201));
        }

        /// <summary>
        /// Update an existing audit team member
        /// </summary>
        /// <param name="id">Audit team member unique identifier</param>
        /// <param name="model">Audit team member update data</param>
        /// <returns>Updated audit team member details</returns>
        /// <response code="200">Audit team member updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Audit team member not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditTeamMemberResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateAuditTeamMember([Required] int id, [FromBody] UpdateAuditTeamMemberDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var teamMember = await _context.AuditTeamMembers
                .Include(a => a.Audit)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (teamMember == null)
                return NotFound(ApiResponse.CreateError("Audit team member not found", 404));

            teamMember.Role = model.Role;

            await _context.SaveChangesAsync();

            var dto = new AuditTeamMemberResponseDto
            {
                Id = teamMember.Id,
                AuditId = teamMember.AuditId,
                AuditNumber = teamMember.Audit.AuditNumber,
                AuditTitle = teamMember.Audit.Title,
                UserId = teamMember.UserId,
                UserName = $"{teamMember.User.FirstName} {teamMember.User.LastName}",
                UserEmail = teamMember.User.Email,
                Role = teamMember.Role,
                AssignedAt = teamMember.AssignedAt
            };

            return Ok(ApiResponse<AuditTeamMemberResponseDto>.CreateSuccess(dto, "Audit team member updated successfully"));
        }

        /// <summary>
        /// Delete an audit team member
        /// </summary>
        /// <param name="id">Audit team member unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Audit team member deleted successfully</response>
        /// <response code="404">Audit team member not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteAuditTeamMember([Required] int id)
        {
            var teamMember = await _context.AuditTeamMembers.FindAsync(id);
            if (teamMember == null)
                return NotFound(ApiResponse.CreateError("Audit team member not found", 404));

            _context.AuditTeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Audit team member deleted successfully"));
        }

        /// <summary>
        /// Get team members by audit
        /// </summary>
        /// <param name="auditId">Audit ID</param>
        /// <returns>List of team members for the audit</returns>
        /// <response code="200">Returns team members for the audit</response>
        /// <response code="404">Audit not found</response>
        [HttpGet("audit/{auditId}/members")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AuditTeamMemberResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetTeamMembersByAudit([Required] int auditId)
        {
            var auditExists = await _context.Audits.AnyAsync(a => a.Id == auditId);
            if (!auditExists)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            var teamMembers = await _context.AuditTeamMembers
                .Include(a => a.Audit)
                .Include(a => a.User)
                .Where(a => a.AuditId == auditId)
                .OrderBy(a => a.Role)
                .ThenBy(a => a.User.LastName)
                .Select(a => new AuditTeamMemberResponseDto
                {
                    Id = a.Id,
                    AuditId = a.AuditId,
                    AuditNumber = a.Audit.AuditNumber,
                    AuditTitle = a.Audit.Title,
                    UserId = a.UserId,
                    UserName = $"{a.User.FirstName} {a.User.LastName}",
                    UserEmail = a.User.Email,
                    Role = a.Role,
                    AssignedAt = a.AssignedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<AuditTeamMemberResponseDto>>.CreateSuccess(teamMembers, "Audit team members retrieved successfully"));
        }

        /// <summary>
        /// Get audits by user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of audits the user is assigned to</returns>
        /// <response code="200">Returns audits for the user</response>
        /// <response code="404">User not found</response>
        [HttpGet("user/{userId}/audits")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAuditsByUser([Required] int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound(ApiResponse.CreateError("User not found", 404));

            var audits = await _context.AuditTeamMembers
                .Include(a => a.Audit)
                .ThenInclude(au => au.Company)
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    Id = a.Id,
                    AuditId = a.AuditId,
                    AuditNumber = a.Audit.AuditNumber,
                    AuditTitle = a.Audit.Title,
                    CompanyName = a.Audit.Company.Name,
                    Role = a.Role,
                    Status = a.Audit.Status,
                    PlannedStartDate = a.Audit.PlannedStartDate,
                    PlannedEndDate = a.Audit.PlannedEndDate,
                    AssignedAt = a.AssignedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(audits, "User audits retrieved successfully"));
        }

        /// <summary>
        /// Bulk assign team members to an audit
        /// </summary>
        /// <param name="auditId">Audit ID</param>
        /// <param name="members">List of team member assignments</param>
        /// <returns>Bulk assignment confirmation</returns>
        /// <response code="200">Team members assigned successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Audit not found or some users not found</response>
        [HttpPost("audit/{auditId}/members/bulk")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> BulkAssignTeamMembers([Required] int auditId, [FromBody] List<CreateAuditTeamMemberDto> members)
        {
            if (members == null || !members.Any())
                return BadRequest(ApiResponse.CreateError("Team members list cannot be empty"));

            var auditExists = await _context.Audits.AnyAsync(a => a.Id == auditId);
            if (!auditExists)
                return NotFound(ApiResponse.CreateError("Audit not found", 404));

            var userIds = members.Select(m => m.UserId).Distinct().ToList();
            var existingUsers = await _context.Users
                .Where(u => userIds.Contains(u.Id) && u.IsActive)
                .Select(u => u.Id)
                .ToListAsync();

            if (existingUsers.Count != userIds.Count)
                return NotFound(ApiResponse.CreateError("Some users not found or inactive", 404));

            // Get existing assignments
            var existingAssignments = await _context.AuditTeamMembers
                .Where(a => a.AuditId == auditId && userIds.Contains(a.UserId))
                .Select(a => a.UserId)
                .ToListAsync();

            // Only add new assignments
            var newMembers = members.Where(m => !existingAssignments.Contains(m.UserId)).ToList();

            if (newMembers.Any())
            {
                var newAssignments = newMembers.Select(m => new AuditTeamMember
                {
                    AuditId = auditId,
                    UserId = m.UserId,
                    Role = m.Role,
                    AssignedAt = DateTime.UtcNow
                });

                _context.AuditTeamMembers.AddRange(newAssignments);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse.CreateSuccess($"Successfully assigned {newMembers.Count} new team members to audit. {existingAssignments.Count} users were already assigned."));
        }
    }
}