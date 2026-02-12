using System.Text.Json.Serialization;

namespace SettingsService.Models
{
    public class CompanyDetailsUpdateRequest
    {
        [JsonPropertyName("legalEntityId")]
        public int LegalEntityId { get; set; }

        [JsonPropertyName("organizationName")]
        public string? OrganizationName { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("countryId")]
        public int? CountryId { get; set; }

        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("vatNumber")]
        public string? VatNumber { get; set; }

        [JsonPropertyName("poNumberRequired")]
        public bool? PoNumberRequired { get; set; }

        [JsonPropertyName("contactDetails")]
        public ContactDetails? ContactDetails { get; set; }

        [JsonPropertyName("businessDetails")]
        public BusinessDetails? BusinessDetails { get; set; }

        [JsonPropertyName("preferences")]
        public CompanyPreferences? Preferences { get; set; }

        [JsonPropertyName("updatedBy")]
        public int? UpdatedBy { get; set; }
    }

    public class CompanyDetailsUpdateResponse
    {
        [JsonPropertyName("legalEntityId")]
        public int LegalEntityId { get; set; }

        [JsonPropertyName("organizationName")]
        public string? OrganizationName { get; set; }

        [JsonPropertyName("updatedFields")]
        public List<string> UpdatedFields { get; set; } = new();

        [JsonPropertyName("lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        [JsonPropertyName("updatedBy")]
        public string? UpdatedBy { get; set; }
    }

    public class ContactDetails
    {
        [JsonPropertyName("primaryContact")]
        public string? PrimaryContact { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }
    }

    public class BusinessDetails
    {
        [JsonPropertyName("industry")]
        public string? Industry { get; set; }

        [JsonPropertyName("employeeCount")]
        public int? EmployeeCount { get; set; }

        [JsonPropertyName("annualRevenue")]
        public decimal? AnnualRevenue { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

    public class CompanyPreferences
    {
        [JsonPropertyName("invoiceFrequency")]
        public string? InvoiceFrequency { get; set; }

        [JsonPropertyName("paymentTerms")]
        public string? PaymentTerms { get; set; }

        [JsonPropertyName("communicationLanguage")]
        public string? CommunicationLanguage { get; set; }

        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }
    }
}
