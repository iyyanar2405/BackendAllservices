using FindingsAPI.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindingsAPI.Gateway.Controllers
{
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly ISiteService _siteService;

        public SitesController(ISiteService siteService)
        {
            _siteService = siteService;
        }

        [HttpGet("api/companies/{companyId:int}/sites")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Site>>> GetSitesByCompany(int companyId)
        {
            var sites = await _siteService.GetSitesByCompanyAsync(companyId);
            return Ok(sites);
        }
    }
}
