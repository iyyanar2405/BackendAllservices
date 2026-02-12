using CertificateService.Models;
using CertificateService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CertificateService.Controllers
{
    [ApiController]
    [Route("api/certificates")]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificateService _service;

        public CertificatesController(ICertificateService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CertificateListResponse>>>> GetCertificates()
        {
            var response = await _service.GetCertificateListAsync();
            return Ok(response);
        }

        [HttpGet("{certificateId:int}")]
        public async Task<ActionResult<ApiResponse<CertificateDetailResponse>>> GetCertificateDetails(int certificateId)
        {
            var response = await _service.GetCertificateDetailsAsync(certificateId);
            return Ok(response);
        }

        [HttpGet("{certificateId:int}/sites")]
        public async Task<ActionResult<ApiResponse<List<CertificateSiteResponse>>>> GetCertificateSites(int certificateId)
        {
            var response = await _service.GetCertificateSitesAsync(certificateId);
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
