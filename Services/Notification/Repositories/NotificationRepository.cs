using Dapper;
using NotificationService.Data;
using NotificationService.Models;
using System.Data;
using System.Text.Json;

namespace NotificationService.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DapperContext _context;

        public NotificationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<NotificationFilterItem>> GetCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<NotificationFilterItem>(
                "Sp_NotificationFilterCategories",
                new
                {
                    companies = ToJsonArray(companies),
                    services = ToJsonArray(services),
                    sites = ToJsonArray(sites)
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<IReadOnlyList<NotificationFilterItem>> GetServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<NotificationFilterItem>(
                "Sp_NotificationFilterServices",
                new
                {
                    companies = ToJsonArray(companies),
                    categories = ToJsonArray(categories),
                    sites = ToJsonArray(sites)
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<IReadOnlyList<NotificationFilterItem>> GetCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<NotificationFilterItem>(
                "Sp_NotificationFilterCompanies",
                new
                {
                    categories = ToJsonArray(categories),
                    services = ToJsonArray(services),
                    sites = ToJsonArray(sites)
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<IReadOnlyList<NotificationSiteNode>> GetSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services)
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<NotificationSiteRow>(
                "Sp_NotificationFilterSites",
                new
                {
                    companies = ToJsonArray(companies),
                    categories = ToJsonArray(categories),
                    services = ToJsonArray(services)
                },
                commandType: CommandType.StoredProcedure);

            return NotificationSiteTreeBuilder.BuildSiteTree(rows);
        }

        public async Task<NotificationPaginationResponse> GetNotificationsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, int pageNumber, int pageSize)
        {
            using var connection = _context.CreateConnection();
            var rows = (await connection.QueryAsync<NotificationRow>(
                "Sp_Notifications",
                new
                {
                    category = ToJsonArray(category),
                    company = ToJsonArray(company),
                    service = ToJsonArray(service),
                    site = ToJsonArray(site),
                    pageNumber,
                    pageSize
                },
                commandType: CommandType.StoredProcedure)).ToList();

            return new NotificationPaginationResponse
            {
                CurrentPage = rows.FirstOrDefault()?.CurrentPage ?? pageNumber,
                TotalItems = rows.FirstOrDefault()?.TotalItems ?? 0,
                TotalPages = rows.FirstOrDefault()?.TotalPages ?? 0,
                Items = rows.Select(row => new NotificationItem
                {
                    CreatedTime = row.CreatedTime,
                    InfoId = row.InfoId,
                    Message = row.Message,
                    Language = row.Language,
                    NotificationCategory = row.NotificationCategory,
                    ReadStatus = row.ReadStatus,
                    Subject = row.Subject,
                    EntityType = row.EntityType,
                    EntityId = row.EntityId,
                    SnowLink = row.SnowLink
                }).ToList()
            };
        }

        private static string ToJsonArray(IEnumerable<int> values)
        {
            var list = values?.ToList() ?? new List<int>();
            return JsonSerializer.Serialize(list);
        }

    }
}
