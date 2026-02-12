using FinanceService.Models;

namespace FinanceService.Repositories
{
    public interface IFinanceRepository
    {
        Task<InvoiceListPageData> GetInvoiceListAsync(int pageNumber, int pageSize, string? status, string? companyFilter, DateTime? startDate, DateTime? endDate);
        Task<DownloadInvoiceResponse?> DownloadInvoiceAsync(List<string> invoiceNumbers, int? userId);
        Task<bool> UpdatePlannedPaymentDateAsync(List<string> invoiceNumbers, DateTime plannedPaymentDate);
    }
}
