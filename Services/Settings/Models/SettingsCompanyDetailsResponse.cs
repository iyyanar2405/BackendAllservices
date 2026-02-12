namespace SettingsService.Models
{
    public class SettingsCompanyDetailsResponse
    {
        public string? UserStatus { get; set; }
        public bool IsAdmin { get; set; }
        public CompanyDetail? ParentCompany { get; set; }
        public List<CompanyDetail> LegalEntities { get; set; } = new();
    }

    public class CompanyDetail
    {
        public int AccountId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public int CountryId { get; set; }
        public bool IsSerReqOpen { get; set; }
        public string? OrganizationName { get; set; }
        public bool PoNumberRequired { get; set; }
        public string? VatNumber { get; set; }
        public string? ZipCode { get; set; }
        public string? AccountDNVId { get; set; }
    }
}
