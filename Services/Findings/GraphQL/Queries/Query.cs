// GraphQL/Queries/Query.cs
using FindingsAPI.Gateway.Services;
using HotChocolate.Authorization;

namespace FindingsAPI.Gateway.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class Query
    {
        private readonly IFindingService _findingService;
        private readonly ICompanyService _companyService;
        private readonly ISiteService _siteService;
        private readonly ILogger<Query> _logger;

        public Query(
            IFindingService findingService,
            ICompanyService companyService,
            ISiteService siteService,
            ILogger<Query> logger)
        {
            _findingService = findingService;
            _companyService = companyService;
            _siteService = siteService;
            _logger = logger;
        }

        [GraphQLDescription("Get all findings with optional filtering and pagination")]
        [UsePaging(IncludeTotalCount = true)]
        [UseFiltering]
        [UseSorting]
        [Authorize("CanViewFindings")]
        public async Task<IEnumerable<Finding>> GetFindings(
            [GraphQLDescription("Filter by company ID")] int? companyId = null,
            [GraphQLDescription("Filter by status")] string? status = null,
            [GraphQLDescription("Filter by category")] string? category = null,
            [Service] IHttpContextAccessor httpContextAccessor = null)
        {
            _logger.LogInformation("GraphQL Query: GetFindings called by {User}", 
                httpContextAccessor?.HttpContext?.User?.Identity?.Name);
            
            try
            {
                var query = new GetFindingsQuery
                {
                    CompanyId = companyId,
                    Status = status,
                    Category = category,
                    IncludeCompany = true,
                    IncludeSite = true
                };
                
                return await _findingService.GetFindingsAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFindings GraphQL query");
                throw new GraphQLException("Failed to retrieve findings", ex);
            }
        }

        [GraphQLDescription("Get a specific finding by ID")]
        [Authorize("CanViewFindings")]
        public async Task<Finding?> GetFinding(
            [GraphQLDescription("The ID of the finding")] int id,
            [Service] IHttpContextAccessor httpContextAccessor = null)
        {
            _logger.LogInformation("GraphQL Query: GetFinding {FindingId} called", id);
            
            try
            {
                return await _findingService.GetFindingByIdAsync(id, includeCompany: true);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFinding GraphQL query for ID {FindingId}", id);
                throw new GraphQLException($"Failed to retrieve finding with ID {id}", ex);
            }
        }

        [GraphQLDescription("Get findings statistics")]
        [Authorize("CanViewFindings")]
        public async Task<FindingsStatistics> GetFindingsStatistics(
            [GraphQLDescription("Company ID for statistics")] int? companyId = null)
        {
            var findings = await _findingService.GetFindingsAsync(new GetFindingsQuery
            {
                CompanyId = companyId
            });
            
            var stats = findings.ToList();
            
            return new FindingsStatistics
            {
                TotalCount = stats.Count,
                OpenCount = stats.Count(f => f.Status == "Open"),
                AcceptedCount = stats.Count(f => f.Status == "Accepted"),
                ClosedCount = stats.Count(f => f.Status == "Closed"),
                OverdueCount = stats.Count(f => 
                    f.DueDate.HasValue && 
                    f.DueDate.Value < DateTime.UtcNow && 
                    f.Status != "Closed"),
                ByCategory = stats
                    .GroupBy(f => f.Category)
                    .ToDictionary(g => g.Key ?? "Unknown", g => g.Count())
            };
        }

        [GraphQLDescription("Search findings by text")]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        [Authorize("CanViewFindings")]
        public async Task<IEnumerable<Finding>> SearchFindings(
            [GraphQLDescription("Search term")] string searchTerm,
            [GraphQLDescription("Search in title, description, or number")] 
            SearchField searchIn = SearchField.All)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Finding>();
            
            var query = new SearchFindingsQuery
            {
                SearchTerm = searchTerm,
                SearchIn = (FindingsAPI.Gateway.SearchField)searchIn,
                IncludeCompany = true
            };
            
            return await _findingService.SearchFindingsAsync(query);
        }

        [GraphQLDescription("Get all companies")]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        [Authorize]
        public async Task<IEnumerable<Company>> GetCompanies()
        {
            return await _companyService.GetCompaniesAsync();
        }

        [GraphQLDescription("Get company by ID")]
        [Authorize]
        public async Task<Company?> GetCompany(int id)
        {
            return await _companyService.GetCompanyByIdAsync(id);
        }

        [GraphQLDescription("Get sites for a company")]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        [Authorize]
        public async Task<IEnumerable<Site>> GetSites(int companyId)
        {
            return await _siteService.GetSitesByCompanyAsync(companyId);
        }
    }
    
    public class FindingsStatistics
    {
        public int TotalCount { get; set; }
        public int OpenCount { get; set; }
        public int AcceptedCount { get; set; }
        public int ClosedCount { get; set; }
        public int OverdueCount { get; set; }
        public Dictionary<string, int> ByCategory { get; set; } = new();
    }
    
    public enum SearchField
    {
        Title,
        Description,
        Number,
        All
    }
}