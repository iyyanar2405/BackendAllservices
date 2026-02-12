using Dapper;
using FindingsAPI.Gateway.Models.Actions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;

namespace FindingsAPI.Gateway.Repositories
{
    public class DapperActionRepository : IActionRepository
    {
        private readonly string _connectionString;

        public DapperActionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private static string ToJsonArray(IEnumerable<int> values)
        {
            var list = values?.ToList() ?? new List<int>();
            return JsonSerializer.Serialize(list);
        }

        public async Task<IReadOnlyList<ActionFilterItem>> GetActionCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<ActionFilterItem>(
                "Sp_ActionFilterCategories",
                new
                {
                    companies = ToJsonArray(companies),
                    services = ToJsonArray(services),
                    sites = ToJsonArray(sites)
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<IReadOnlyList<ActionFilterItem>> GetActionCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<ActionFilterItem>(
                "Sp_ActionFilterCompanies",
                new
                {
                    categories = ToJsonArray(categories),
                    services = ToJsonArray(services),
                    sites = ToJsonArray(sites)
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<IReadOnlyList<ActionFilterItem>> GetActionServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<ActionFilterItem>(
                "Sp_ActionFilterServices",
                new
                {
                    companies = ToJsonArray(companies),
                    categories = ToJsonArray(categories),
                    sites = ToJsonArray(sites)
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<IReadOnlyList<ActionSiteNode>> GetActionSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services)
        {
            using var connection = CreateConnection();
            var rows = await connection.QueryAsync<ActionSiteRow>(
                "Sp_ActionFilterSites",
                new
                {
                    companies = ToJsonArray(companies),
                    categories = ToJsonArray(categories),
                    services = ToJsonArray(services)
                },
                commandType: CommandType.StoredProcedure);

            return BuildSiteTree(rows);
        }

        public async Task<ActionsPaginationResponse> GetActionsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, bool isHighPriority, int pageNumber, int pageSize)
        {
            using var connection = CreateConnection();
            var rows = (await connection.QueryAsync<ActionRow>(
                "Sp_GetActions",
                new
                {
                    category = ToJsonArray(category),
                    company = ToJsonArray(company),
                    service = ToJsonArray(service),
                    site = ToJsonArray(site),
                    isHighPriority,
                    pageNumber,
                    pageSize
                },
                commandType: CommandType.StoredProcedure)).ToList();

            var response = new ActionsPaginationResponse
            {
                CurrentPage = rows.FirstOrDefault()?.CurrentPage ?? pageNumber,
                TotalItems = rows.FirstOrDefault()?.TotalItems ?? 0,
                TotalPages = rows.FirstOrDefault()?.TotalPages ?? 0,
                Items = rows.Select(row => new ActionItem
                {
                    Id = row.Id,
                    Action = row.Action,
                    DueDate = row.DueDate,
                    HighPriority = row.HighPriority ? 1 : 0,
                    Message = row.Message,
                    Language = row.Language,
                    Service = row.Service,
                    Site = row.Site,
                    EntityType = row.EntityType,
                    EntityId = row.EntityId,
                    Subject = row.Subject,
                    SnowLink = row.SnowLink
                }).ToList()
            };

            return response;
        }

        private static IReadOnlyList<ActionSiteNode> BuildSiteTree(IEnumerable<ActionSiteRow> rows)
        {
            var countries = new Dictionary<int, ActionSiteNode>();

            foreach (var row in rows)
            {
                if (!countries.TryGetValue(row.CountryId, out var countryNode))
                {
                    countryNode = new ActionSiteNode
                    {
                        Id = row.CountryId,
                        Label = row.CountryName
                    };
                    countries[row.CountryId] = countryNode;
                }

                var cityNode = countryNode.Children.FirstOrDefault(c => c.Id == row.CityId);
                if (cityNode == null)
                {
                    cityNode = new ActionSiteNode
                    {
                        Id = row.CityId,
                        Label = row.CityName
                    };
                    countryNode.Children.Add(cityNode);
                }

                cityNode.Children.Add(new ActionSiteNode
                {
                    Id = row.SiteId,
                    Label = row.SiteName
                });
            }

            return countries.Values.OrderBy(c => c.Label).ToList();
        }

        private sealed class ActionRow
        {
            public int Id { get; set; }
            public string? Action { get; set; }
            public DateTime? DueDate { get; set; }
            public bool HighPriority { get; set; }
            public string? Message { get; set; }
            public string? Language { get; set; }
            public string? Service { get; set; }
            public string? Site { get; set; }
            public string? EntityType { get; set; }
            public string? EntityId { get; set; }
            public string? Subject { get; set; }
            public string? SnowLink { get; set; }
            public int CurrentPage { get; set; }
            public int TotalItems { get; set; }
            public int TotalPages { get; set; }
        }

        private sealed class ActionSiteRow
        {
            public int CountryId { get; set; }
            public string CountryName { get; set; } = string.Empty;
            public int CityId { get; set; }
            public string CityName { get; set; } = string.Empty;
            public int SiteId { get; set; }
            public string SiteName { get; set; } = string.Empty;
        }
    }
}
