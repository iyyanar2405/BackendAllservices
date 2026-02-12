namespace SettingsService.Models
{
    public class AdminUserResponse
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? UserStatus { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool CanDelete { get; set; }
        public bool CanManagePermissions { get; set; }
        public List<CompanyRef> Companies { get; set; } = new();
    }

    public class MemberUserResponse
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? UserStatus { get; set; }
        public string? Roles { get; set; }
        public bool CanDelete { get; set; }
        public bool CanManagePermissions { get; set; }
        public List<CompanyRef> Companies { get; set; } = new();
        public List<ServiceRef> Services { get; set; } = new();
        public List<CountryNode> Countries { get; set; } = new();
    }

    public class CompanyRef
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class ServiceRef
    {
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
    }

    public class CountryNode
    {
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public List<CityNode> Cities { get; set; } = new();
    }

    public class CityNode
    {
        public string? CityName { get; set; }
        public List<SiteNode> Sites { get; set; } = new();
    }

    public class SiteNode
    {
        public int SiteId { get; set; }
        public string? SiteName { get; set; }
    }
}
