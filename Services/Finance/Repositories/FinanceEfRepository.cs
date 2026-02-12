using System.Text.Json;
using FinanceService.Data;
using FinanceService.Data.Entities;
using FinanceService.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Repositories
{
    public class FinanceEfRepository : IFinanceRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FinanceEfRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<InvoiceListPageData> GetInvoiceListAsync(int pageNumber, int pageSize, string? status, string? companyFilter, DateTime? startDate, DateTime? endDate)
        {
            var items = await _dbContext.InvoiceListItems
                .FromSqlInterpolated($"EXEC Sp_GetInvoiceList @pageSize={pageSize}, @pageNumber={pageNumber}, @status={status}, @companyFilter={companyFilter}, @startDate={startDate}, @endDate={endDate}")
                .AsNoTracking()
                .ToListAsync();

            return new InvoiceListPageData
            {
                Items = items
            };
        }

        public async Task<DownloadInvoiceResponse?> DownloadInvoiceAsync(List<string> invoiceNumbers, int? userId)
        {
            var parameters = JsonSerializer.Serialize(new
            {
                userId,
                invoiceNumber = invoiceNumbers
            });

            var row = await _dbContext.JsonResponses
                .FromSqlInterpolated($"EXEC Sp_DownloadInvoice @Parameters={parameters}")
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var parsed = FinanceResponseParser.ParseDownloadInvoiceResponse(row?.JsonResponse);
            if (parsed != null)
            {
                return parsed;
            }

            return null;
        }

        public async Task<bool> UpdatePlannedPaymentDateAsync(List<string> invoiceNumbers, DateTime plannedPaymentDate)
        {
            var jsonNumbers = JsonSerializer.Serialize(invoiceNumbers);
            var row = await _dbContext.JsonResponses
                .FromSqlInterpolated($"EXEC Sp_UpdatePlannedPaymentDate @invoiceNumbers={jsonNumbers}, @plannedPaymentDate={plannedPaymentDate}")
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var parsed = FinanceResponseParser.ParseBooleanData(row?.JsonResponse);
            return parsed ?? false;
        }
    }
}
