using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;
using System.Text.Json;

namespace NotificationService.Repositories
{
    public class EfNotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public EfNotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<NotificationFilterItem>> GetCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites)
        {
            var parameters = new[]
            {
                new SqlParameter("@companies", ToJsonArray(companies)),
                new SqlParameter("@services", ToJsonArray(services)),
                new SqlParameter("@sites", ToJsonArray(sites))
            };

            return await _context.Set<NotificationFilterItem>()
                .FromSqlRaw("EXEC Sp_NotificationFilterCategories @companies, @services, @sites", parameters)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<NotificationFilterItem>> GetServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites)
        {
            var parameters = new[]
            {
                new SqlParameter("@companies", ToJsonArray(companies)),
                new SqlParameter("@categories", ToJsonArray(categories)),
                new SqlParameter("@sites", ToJsonArray(sites))
            };

            return await _context.Set<NotificationFilterItem>()
                .FromSqlRaw("EXEC Sp_NotificationFilterServices @companies, @categories, @sites", parameters)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<NotificationFilterItem>> GetCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites)
        {
            var parameters = new[]
            {
                new SqlParameter("@categories", ToJsonArray(categories)),
                new SqlParameter("@services", ToJsonArray(services)),
                new SqlParameter("@sites", ToJsonArray(sites))
            };

            return await _context.Set<NotificationFilterItem>()
                .FromSqlRaw("EXEC Sp_NotificationFilterCompanies @categories, @services, @sites", parameters)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<NotificationSiteNode>> GetSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services)
        {
            var parameters = new[]
            {
                new SqlParameter("@companies", ToJsonArray(companies)),
                new SqlParameter("@categories", ToJsonArray(categories)),
                new SqlParameter("@services", ToJsonArray(services))
            };

            var rows = await _context.Set<NotificationSiteRow>()
                .FromSqlRaw("EXEC Sp_NotificationFilterSites @companies, @categories, @services", parameters)
                .ToListAsync();

            return NotificationSiteTreeBuilder.BuildSiteTree(rows);
        }

        public async Task<NotificationPaginationResponse> GetNotificationsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, int pageNumber, int pageSize)
        {
            var parameters = new[]
            {
                new SqlParameter("@category", ToJsonArray(category)),
                new SqlParameter("@company", ToJsonArray(company)),
                new SqlParameter("@service", ToJsonArray(service)),
                new SqlParameter("@site", ToJsonArray(site)),
                new SqlParameter("@pageNumber", pageNumber),
                new SqlParameter("@pageSize", pageSize)
            };

            var rows = await _context.Set<NotificationRow>()
                .FromSqlRaw("EXEC Sp_Notifications @category, @company, @service, @site, @pageNumber, @pageSize", parameters)
                .ToListAsync();

            var firstRow = rows.FirstOrDefault();

            return new NotificationPaginationResponse
            {
                CurrentPage = firstRow?.CurrentPage ?? pageNumber,
                TotalItems = firstRow?.TotalItems ?? 0,
                TotalPages = firstRow?.TotalPages ?? 0,
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
