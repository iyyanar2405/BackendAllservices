using FinanceService.Models;
using FinanceService.Repositories;

namespace FinanceService.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IFinanceRepository _repository;
        private readonly ILogger<FinanceService> _logger;

        public FinanceService(IFinanceRepository repository, ILogger<FinanceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<InvoiceListPageData>> GetInvoiceListAsync(int pageNumber, int pageSize, string? status, string? companyFilter, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var data = await _repository.GetInvoiceListAsync(pageNumber, pageSize, status, companyFilter, startDate, endDate);
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load invoice list");
                return Failure<InvoiceListPageData>("Failed to load invoice list");
            }
        }

        public async Task<ApiResponse<DownloadInvoiceResponse>> DownloadInvoiceAsync(List<string> invoiceNumbers, int? userId)
        {
            try
            {
                var data = await _repository.DownloadInvoiceAsync(invoiceNumbers, userId);
                if (data == null)
                {
                    return Failure<DownloadInvoiceResponse>("Invoice not found", "INVOICE_NOT_FOUND");
                }

                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download invoice");
                return Failure<DownloadInvoiceResponse>("Failed to download invoice");
            }
        }

        public async Task<ApiResponse<bool>> UpdatePlannedPaymentDateAsync(List<string> invoiceNumbers, DateTime plannedPaymentDate)
        {
            try
            {
                var data = await _repository.UpdatePlannedPaymentDateAsync(invoiceNumbers, plannedPaymentDate);
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update planned payment date");
                return Failure<bool>("Failed to update planned payment date");
            }
        }

        private static ApiResponse<T> Success<T>(T data)
        {
            return new ApiResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = "Success",
                ErrorCode = string.Empty
            };
        }

        private static ApiResponse<T> Failure<T>(string message, string? errorCode = null)
        {
            return new ApiResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode ?? "ERR_FINANCE"
            };
        }
    }
}
