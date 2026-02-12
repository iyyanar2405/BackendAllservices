// Services/CompanyService.cs
using Microsoft.Extensions.Caching.Memory;
using FindingsAPI.Gateway.Repositories;

namespace FindingsAPI.Gateway.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetCompaniesAsync();
        Task<Company> GetCompanyByIdAsync(int id);
        Task<IEnumerable<Company>> GetCompaniesByIdsAsync(IEnumerable<int> ids);
    }

    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompanyService> _logger;
        private readonly IMemoryCache _cache;

        public CompanyService(
            IUnitOfWork unitOfWork,
            ILogger<CompanyService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync()
        {
            var cacheKey = "companies:all";
            
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Company> cachedCompanies))
                return cachedCompanies;
            
            var companies = await _unitOfWork.Companies.FindAsync(c => true);
            _cache.Set(cacheKey, companies, TimeSpan.FromMinutes(10));
            
            return companies;
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            var cacheKey = $"company:{id}";
            
            if (_cache.TryGetValue(cacheKey, out Company cachedCompany))
                return cachedCompany;
            
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company != null)
            {
                _cache.Set(cacheKey, company, TimeSpan.FromMinutes(5));
            }
            
            return company;
        }

        public async Task<IEnumerable<Company>> GetCompaniesByIdsAsync(IEnumerable<int> ids)
        {
            var companies = new List<Company>();
            foreach (var id in ids)
            {
                var company = await GetCompanyByIdAsync(id);
                if (company != null)
                {
                    companies.Add(company);
                }
            }
            return companies;
        }
    }
}