namespace CertificateService.Models.Rest
{
    public class UpdateCertificateStatusRequest
    {
        public int CertificateId { get; set; }
        public string? NewStatus { get; set; }
        public string? Reason { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool NotifyClient { get; set; }
        public string? Comments { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class CertificateStatusUpdateResponse
    {
        public int CertificateId { get; set; }
        public string? Status { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
