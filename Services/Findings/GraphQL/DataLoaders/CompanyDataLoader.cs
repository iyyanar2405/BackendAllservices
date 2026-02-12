// GraphQL/DataLoaders/CompanyDataLoader.cs
using FindingsAPI.Gateway.Services;

namespace FindingsAPI.Gateway.GraphQL.DataLoaders
{
    public class CompanyDataLoader : BatchDataLoader<int, Company>
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyDataLoader> _logger;

        public CompanyDataLoader(
            ICompanyService companyService,
            IBatchScheduler batchScheduler,
            DataLoaderOptions options,
            ILogger<CompanyDataLoader> logger)
            : base(batchScheduler, options)
        {
            _companyService = companyService;
            _logger = logger;
        }

        protected override async Task<IReadOnlyDictionary<int, Company>> LoadBatchAsync(
            IReadOnlyList<int> keys, 
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Batch loading {Count} companies: {Keys}", 
                keys.Count, string.Join(",", keys));
            
            try
            {
                // Batch request to company service
                var companies = await _companyService.GetCompaniesByIdsAsync(keys);
                
                return companies.ToDictionary(c => c.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error batch loading companies");
                
                // Return empty dictionary to avoid breaking the whole query
                return keys.ToDictionary(key => key, _ => (Company)null);
            }
        }
    }
    
    public class SiteDataLoader : BatchDataLoader<int, Site>
    {
        private readonly ISiteService _siteService;
        private readonly ILogger<SiteDataLoader> _logger;

        public SiteDataLoader(
            ISiteService siteService,
            IBatchScheduler batchScheduler,
            DataLoaderOptions options,
            ILogger<SiteDataLoader> logger)
            : base(batchScheduler, options)
        {
            _siteService = siteService;
            _logger = logger;
        }

        protected override async Task<IReadOnlyDictionary<int, Site>> LoadBatchAsync(
            IReadOnlyList<int> keys, 
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Batch loading {Count} sites", keys.Count);
            
            var sites = await _siteService.GetSitesByIdsAsync(keys);
            return sites.ToDictionary(s => s.SiteId);
        }
    }
}