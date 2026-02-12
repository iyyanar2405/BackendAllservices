using System.Collections.Generic;

namespace AuthProvider.ViewModel
{
    public enum ProviderAssociationTypes
    {
        None=0,
        Self=1,
        Delegate=2,
        Case_Manager=3
    }

    public enum RoleTypes
    {
        None = 0,
        HCP = 1,
        Location_Director = 2,
        Site_Director = 3,
        Staff = 4
    }

    public class ChorusAccessViewModel
    {
        public bool IsDefault { get; set; }
        public int SiteId { get; set; }
        public List<RoleAssocation> UserRoles { get; set; }
        public List<ProviderAssocation> ProviderAssocations { get; set; }
    }

    public class ProviderAssocation
    {
        public int ProviderId { get; set; }
        public bool IsPrimary { get; set; } = false;
        public ProviderAssociationTypes Type { get; set; }
        public int?[] LocationId { get; set; }
    }



    public class RoleAssocation
    {
        public RoleTypes? Role { get; set; }
        public int?[] LocationId { get; set; }

    }

}
