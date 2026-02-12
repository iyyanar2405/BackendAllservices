using ContractService.Models;
using ContractService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IContractService _service;

        public UsersController(IContractService service)
        {
            _service = service;
        }

        [HttpGet("validate")]
        public async Task<ActionResult<ApiResponse<UserValidationResponse>>> GetUserValidation(
            [FromQuery] string? userId,
            [FromQuery] string? veracityId,
            [FromHeader(Name = "X-Veracity-ID")] string? veracityIdHeader,
            [FromHeader(Name = "Authorization")] string? authorization)
        {
            var resolvedVeracityId = veracityId ?? veracityIdHeader;
            var response = await _service.GetUserValidationAsync(userId, resolvedVeracityId);
            return Ok(response);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserProfileDetailsResponse>>> GetUserProfile(
            [FromQuery] string? userId,
            [FromQuery] string? veracityId,
            [FromHeader(Name = "X-Veracity-ID")] string? veracityIdHeader,
            [FromQuery] bool? includeAccessLevels = null,
            [FromQuery] bool? includePreferences = null,
            [FromQuery] bool? includeCompanyDetails = null)
        {
            var resolvedVeracityId = veracityId ?? veracityIdHeader;
            var response = await _service.GetUserProfileAsync(userId, resolvedVeracityId);
            return Ok(response);
        }
    }
}
