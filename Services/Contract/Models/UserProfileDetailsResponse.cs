namespace ContractService.Models
{
    public class UserProfileDetailsResponse
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string? Region { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CommunicationLanguage { get; set; }
        public string? JobTitle { get; set; }
        public string? PortalLanguage { get; set; }
        public string? VeracityId { get; set; }
        public List<AccessRoleDetail> AccessLevel { get; set; } = new();
    }

    public class AccessRoleDetail
    {
        public List<int> RoleLevel { get; set; } = new();
        public string? RoleName { get; set; }
    }
}
