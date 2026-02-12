using ActionService.Data;
using ActionService.Data.Queries;
using ActionService.Models;
using Dapper;
using System.Data;
using System.Text.Json;

namespace ActionService.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private readonly DapperContext _context;

        public ActionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ActionFilterItem>> GetActionCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites)
        {
            using var connection = _context.CreateConnection();
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
            using var connection = _context.CreateConnection();
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
            using var connection = _context.CreateConnection();
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
            using var connection = _context.CreateConnection();
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
            using var connection = _context.CreateConnection();
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

            return new ActionsPaginationResponse
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
        }

        public async Task<ActionItem> CreateActionAsync(CreateActionRequest request)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "Sp_InsertAction",
                new
                {
                    id = request.Id,
                    action = request.Action,
                    dueDate = request.DueDate,
                    highPriority = request.HighPriority,
                    message = request.Message,
                    language = request.Language,
                    service = request.Service,
                    site = request.Site,
                    entityType = request.EntityType,
                    entityId = request.EntityId,
                    subject = request.Subject,
                    snowLink = request.SnowLink
                },
                commandType: CommandType.StoredProcedure);

            return new ActionItem
            {
                Id = request.Id,
                Action = request.Action,
                DueDate = request.DueDate,
                HighPriority = request.HighPriority ? 1 : 0,
                Message = request.Message,
                Language = request.Language,
                Service = request.Service,
                Site = request.Site,
                EntityType = request.EntityType,
                EntityId = request.EntityId?.ToString(),
                Subject = request.Subject,
                SnowLink = request.SnowLink
            };
        }

        private static string ToJsonArray(IEnumerable<int> values)
        {
            var list = values?.ToList() ?? new List<int>();
            return JsonSerializer.Serialize(list);
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

    }
}
