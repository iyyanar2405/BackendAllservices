using Microsoft.AspNetCore.Mvc;
using SettingsService.Models;
using SettingsService.Services;

namespace SettingsService.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _service;

        public SettingsController(ISettingsService service)
        {
            _service = service;
        }

        [HttpGet("company-details")]
        public async Task<ActionResult<ApiResponse<SettingsCompanyDetailsResponse>>> GetCompanyDetails([FromQuery] int? userId)
        {
            var response = await _service.GetCompanyDetailsAsync(userId);
            return Ok(response);
        }

        [HttpGet("admins")]
        public async Task<ActionResult<ApiResponse<List<AdminUserResponse>>>> GetAdminList(
            [FromQuery] int? userId,
            [FromQuery] string? accountDNVId)
        {
            var response = await _service.GetAdminListAsync(userId, accountDNVId);
            return Ok(response);
        }

        [HttpGet("members")]
        public async Task<ActionResult<ApiResponse<List<MemberUserResponse>>>> GetMemberList(
            [FromQuery] int? userId,
            [FromQuery] string? accountDNVId)
        {
            var response = await _service.GetMemberListAsync(userId, accountDNVId);
            return Ok(response);
        }

        [HttpGet("countries")]
        public async Task<ActionResult<ApiResponse<List<CountryResponse>>>> GetCountries()
        {
            var response = await _service.GetCountriesAsync();
            return Ok(response);
        }

        [HttpGet("preferences")]
        public async Task<ActionResult<ApiResponse<PreferenceResponse>>> GetPreferences(
            [FromQuery] string objectType,
            [FromQuery] string objectName,
            [FromQuery] string pageName)
        {
            var response = await _service.GetPreferencesAsync(objectType, objectName, pageName);
            return Ok(response);
        }
    }
}
