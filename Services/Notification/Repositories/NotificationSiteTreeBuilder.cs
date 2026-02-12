using NotificationService.Models;
using NotificationService.Data;

namespace NotificationService.Repositories
{
    public static class NotificationSiteTreeBuilder
    {
        public static IReadOnlyList<NotificationSiteNode> BuildSiteTree(IEnumerable<NotificationSiteRow> rows)
        {
            var countries = new Dictionary<int, NotificationSiteNode>();

            foreach (var row in rows)
            {
                if (!countries.TryGetValue(row.CountryId, out var countryNode))
                {
                    countryNode = new NotificationSiteNode
                    {
                        Id = row.CountryId,
                        Label = row.CountryName
                    };
                    countries[row.CountryId] = countryNode;
                }

                var cityNode = countryNode.Children.FirstOrDefault(c => c.Id == row.CityId);
                if (cityNode == null)
                {
                    cityNode = new NotificationSiteNode
                    {
                        Id = row.CityId,
                        Label = row.CityName
                    };
                    countryNode.Children.Add(cityNode);
                }

                cityNode.Children.Add(new NotificationSiteNode
                {
                    Id = row.SiteId,
                    Label = row.SiteName
                });
            }

            return countries.Values.OrderBy(c => c.Label).ToList();
        }
    }
}
