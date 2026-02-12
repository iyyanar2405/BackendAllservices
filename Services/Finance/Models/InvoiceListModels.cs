namespace FinanceService.Models
{
    public class InvoiceListPageData
    {
        public List<InvoiceListItem> Items { get; set; } = new();
    }

    public class InvoiceListItem
    {
        public string? Amount { get; set; }
        public string? BillingAddress { get; set; }
        public string? Company { get; set; }
        public string? ContactPerson { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Invoice { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? OriginalInvoice { get; set; }
        public DateTime? PlannedPaymentDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Status { get; set; }
        public string? ReportingCountry { get; set; }
        public string? ProjectNumber { get; set; }
        public string? AccountDNVId { get; set; }
    }

    public class DownloadInvoiceResponse
    {
        public List<int> Content { get; set; } = new();
        public string? FileName { get; set; }
        public bool IsZipped { get; set; }
    }

    public class PlannedPaymentDateRequest
    {
        public List<string> InvoiceNumbers { get; set; } = new();
        public DateTime PlannedPaymentDate { get; set; }
    }
}
