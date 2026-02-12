using ActionService.Data;
using ActionService.Data.Entities;
using ActionService.Data.Queries;
using ActionService.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ActionService.Repositories
{
    public class EfActionRepository : IActionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EfActionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ActionFilterItem>> GetActionCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites)
        {
            var parameters = new[]
            {
                CreateJsonParameter("@companies", companies),
                CreateJsonParameter("@services", services),
                CreateJsonParameter("@sites", sites)
            };

            var result = await _dbContext.ActionFilterItems
                .FromSqlRaw("EXEC Sp_ActionFilterCategories @companies, @services, @sites", parameters)
                .ToListAsync();

            return result;
        }

        public async Task<IReadOnlyList<ActionFilterItem>> GetActionCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites)
        {
            var parameters = new[]
            {
                CreateJsonParameter("@categories", categories),
                CreateJsonParameter("@services", services),
                CreateJsonParameter("@sites", sites)
            };

            var result = await _dbContext.ActionFilterItems
                .FromSqlRaw("EXEC Sp_ActionFilterCompanies @categories, @services, @sites", parameters)
                .ToListAsync();

            return result;
        }

        public async Task<IReadOnlyList<ActionFilterItem>> GetActionServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites)
        {
            var parameters = new[]
            {
                CreateJsonParameter("@companies", companies),
                CreateJsonParameter("@categories", categories),
                CreateJsonParameter("@sites", sites)
            };

            var result = await _dbContext.ActionFilterItems
                .FromSqlRaw("EXEC Sp_ActionFilterServices @companies, @categories, @sites", parameters)
                .ToListAsync();

            return result;
        }

        public async Task<IReadOnlyList<ActionSiteNode>> GetActionSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services)
        {
            var parameters = new[]
            {
                CreateJsonParameter("@companies", companies),
                CreateJsonParameter("@categories", categories),
                CreateJsonParameter("@services", services)
            };

            var rows = await _dbContext.ActionSiteRows
                .FromSqlRaw("EXEC Sp_ActionFilterSites @companies, @categories, @services", parameters)
                .ToListAsync();

            return BuildSiteTree(rows);
        }

        public async Task<ActionsPaginationResponse> GetActionsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, bool isHighPriority, int pageNumber, int pageSize)
        {
            var parameters = new[]
            {
                CreateJsonParameter("@category", category),
                CreateJsonParameter("@company", company),
                CreateJsonParameter("@service", service),
                CreateJsonParameter("@site", site),
                new SqlParameter("@isHighPriority", isHighPriority),
                new SqlParameter("@pageNumber", pageNumber),
                new SqlParameter("@pageSize", pageSize)
            };

            var rows = await _dbContext.ActionRows
                .FromSqlRaw("EXEC Sp_GetActions @category, @company, @service, @site, @isHighPriority, @pageNumber, @pageSize", parameters)
                .ToListAsync();

            return new ActionsPaginationResponse
            {
                CurrentPage = rows.FirstOrDefault()?.CurrentPage ?? pageNumber,
                TotalItems = rows.FirstOrDefault()?.TotalItems ?? 0,
                TotalPages = rows.FirstOrDefault()?.TotalPages ?? 0,
                Items = rows.Select(MapActionItem).ToList()
            };
        }

        public async Task<ActionItem> CreateActionAsync(CreateActionRequest request)
        {
            var entity = new ActionEntity
            {
                Id = request.Id,
                Action = request.Action,
                DueDate = request.DueDate,
                HighPriority = request.HighPriority,
                Message = request.Message,
                Language = request.Language,
                Service = request.Service,
                Site = request.Site,
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                Subject = request.Subject,
                SnowLink = request.SnowLink
            };

            _dbContext.Actions.Add(entity);
            await _dbContext.SaveChangesAsync();

            return MapActionItem(entity);
        }

        private static SqlParameter CreateJsonParameter(string name, IEnumerable<int> values)
        {
            return new SqlParameter(name, JsonSerializer.Serialize(values?.ToList() ?? new List<int>()));
        }

        private static ActionItem MapActionItem(ActionRow row)
        {
            return new ActionItem
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
            };
        }

        private static ActionItem MapActionItem(ActionEntity entity)
        {
            return new ActionItem
            {
                Id = entity.Id,
                Action = entity.Action,
                DueDate = entity.DueDate,
                HighPriority = entity.HighPriority ? 1 : 0,
                Message = entity.Message,
                Language = entity.Language,
                Service = entity.Service,
                Site = entity.Site,
                EntityType = entity.EntityType,
                EntityId = entity.EntityId?.ToString(),
                Subject = entity.Subject,
                SnowLink = entity.SnowLink
            };
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
