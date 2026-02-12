namespace ContractService.Models
{
    public class UserValidationResponse
    {
        public bool UserIsActive { get; set; }
        public string? TermsAcceptanceRedirectUrl { get; set; }
        public string? PolicySubCode { get; set; }
        public bool IsDnvUser { get; set; }
        public string? UserEmail { get; set; }
        public string? VeracityId { get; set; }
        public string? PortalLanguage { get; set; }
        public bool IsAdmin { get; set; }
    }
}
