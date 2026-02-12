using FinanceService.Models;
using FinanceService.Services;

namespace FinanceService.GraphQL.Queries
{
    public class Query
    {
        private readonly IFinanceService _service;

        public Query(IFinanceService service)
        {
            _service = service;
        }

        [GraphQLName("InvoiceListPage")]
        public Task<ApiResponse<InvoiceListPageData>> InvoiceListPage(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? companyFilter = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return _service.GetInvoiceListAsync(pageNumber, pageSize, status, companyFilter, startDate, endDate);
        }

        [GraphQLName("DownloadInvoice")]
        public Task<ApiResponse<DownloadInvoiceResponse>> DownloadInvoice(List<string> invoiceNumber, int? userId)
        {
            return _service.DownloadInvoiceAsync(invoiceNumber, userId);
        }
    }
}
