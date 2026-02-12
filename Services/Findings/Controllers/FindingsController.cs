using FindingsAPI.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindingsAPI.Gateway.Controllers
{
    [ApiController]
    [Route("api/findings")]
    public class FindingsController : ControllerBase
    {
        private readonly IFindingService _findingService;

        public FindingsController(IFindingService findingService)
        {
            _findingService = findingService;
        }

        [HttpGet]
        [Authorize("CanViewFindings")]
        public async Task<ActionResult<IEnumerable<Finding>>> GetFindings(
            [FromQuery] int? companyId,
            [FromQuery] string? status,
            [FromQuery] string? category,
            [FromQuery] bool includeCompany = false,
            [FromQuery] bool includeSite = false)
        {
            var query = new GetFindingsQuery
            {
                CompanyId = companyId,
                Status = status,
                Category = category,
                IncludeCompany = includeCompany,
                IncludeSite = includeSite
            };

            var findings = await _findingService.GetFindingsAsync(query);
            return Ok(findings);
        }

        [HttpGet("{id:int}")]
        [Authorize("CanViewFindings")]
        public async Task<ActionResult<Finding>> GetFinding(int id, [FromQuery] bool includeCompany = false)
        {
            var finding = await _findingService.GetFindingByIdAsync(id, includeCompany);
            if (finding == null)
            {
                return NotFound();
            }

            return Ok(finding);
        }

        [HttpGet("search")]
        [Authorize("CanViewFindings")]
        public async Task<ActionResult<IEnumerable<Finding>>> SearchFindings(
            [FromQuery] string term,
            [FromQuery] SearchField searchIn = SearchField.All)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Ok(Array.Empty<Finding>());
            }

            var query = new SearchFindingsQuery
            {
                SearchTerm = term,
                SearchIn = searchIn,
                IncludeCompany = true
            };

            var findings = await _findingService.SearchFindingsAsync(query);
            return Ok(findings);
        }

        [HttpPost]
        [Authorize(Policy = "Auditor")]
        public async Task<ActionResult<Finding>> CreateFinding([FromBody] CreateFindingCommand command)
        {
            var finding = await _findingService.CreateFindingAsync(command);
            return CreatedAtAction(nameof(GetFinding), new { id = finding.FindingsId }, finding);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Auditor")]
        public async Task<ActionResult<Finding>> UpdateFinding(int id, [FromBody] UpdateFindingCommand command)
        {
            if (id != command.FindingId)
            {
                return BadRequest("Finding ID mismatch.");
            }

            var finding = await _findingService.UpdateFindingAsync(command);
            return Ok(finding);
        }

        [HttpPost("{id:int}/close")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<Finding>> CloseFinding(int id, [FromBody] CloseFindingCommand command)
        {
            if (id != command.FindingId)
            {
                return BadRequest("Finding ID mismatch.");
            }

            var finding = await _findingService.CloseFindingAsync(command);
            return Ok(finding);
        }

        [HttpPost("bulk-status")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<BulkUpdateResult>> BulkUpdateStatus([FromBody] BulkUpdateStatusCommand command)
        {
            var result = await _findingService.BulkUpdateStatusAsync(command);
            return Ok(result);
        }
    }
}
