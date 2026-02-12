namespace ContractService.Models
{
    public class ContractListResponse
    {
        public int ContractId { get; set; }
        public string? ContractName { get; set; }
        public string? ContractType { get; set; }
        public string? CompanyId { get; set; }
        public string? Company { get; set; }
        public string? Service { get; set; }
        public string? Sites { get; set; }
        public DateTime? DateAdded { get; set; }
        public string? CurrentSecurity { get; set; }
    }
}
