using FindingsAPI.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindingsAPI.Gateway.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            var companies = await _companyService.GetCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }
    }
}
