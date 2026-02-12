using FinanceService.Models;

namespace FinanceService.Services
{
    public interface IFinanceService
    {
        Task<ApiResponse<InvoiceListPageData>> GetInvoiceListAsync(int pageNumber, int pageSize, string? status, string? companyFilter, DateTime? startDate, DateTime? endDate);
        Task<ApiResponse<DownloadInvoiceResponse>> DownloadInvoiceAsync(List<string> invoiceNumbers, int? userId);
        Task<ApiResponse<bool>> UpdatePlannedPaymentDateAsync(List<string> invoiceNumbers, DateTime plannedPaymentDate);
    }
}
