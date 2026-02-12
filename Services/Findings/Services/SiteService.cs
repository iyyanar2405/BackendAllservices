// Services/SiteService.cs
using Microsoft.Extensions.Caching.Memory;
using FindingsAPI.Gateway.Repositories;

namespace FindingsAPI.Gateway.Services
{
    public interface ISiteService
    {
        Task<IEnumerable<Site>> GetSitesByCompanyAsync(int companyId);
        Task<IEnumerable<Site>> GetSitesByIdsAsync(IEnumerable<int> ids);
    }

    public class SiteService : ISiteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SiteService> _logger;
        private readonly IMemoryCache _cache;

        public SiteService(
            IUnitOfWork unitOfWork,
            ILogger<SiteService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<Site>> GetSitesByCompanyAsync(int companyId)
        {
            var cacheKey = $"sites:company:{companyId}";
            
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Site> cachedSites))
                return cachedSites;
            
            var sites = await _unitOfWork.Sites.FindAsync(s => s.CompanyId == companyId);
            _cache.Set(cacheKey, sites, TimeSpan.FromMinutes(10));
            
            return sites;
        }

        public async Task<IEnumerable<Site>> GetSitesByIdsAsync(IEnumerable<int> ids)
        {
            var sites = new List<Site>();
            foreach (var id in ids)
            {
                var site = await _unitOfWork.Sites.GetByIdAsync(id);
                if (site != null)
                {
                    sites.Add(site);
                }
            }
            return sites;
        }
    }
}