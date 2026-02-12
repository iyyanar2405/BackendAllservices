using Dapper;
using FinanceService.Data;
using FinanceService.Models;
using System.Data;
using System.Text.Json;

namespace FinanceService.Repositories
{
    public class FinanceRepository : IFinanceRepository
    {
        private readonly DapperContext _context;

        public FinanceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<InvoiceListPageData> GetInvoiceListAsync(int pageNumber, int pageSize, string? status, string? companyFilter, DateTime? startDate, DateTime? endDate)
        {
            using var connection = _context.CreateConnection();
            var items = await connection.QueryAsync<InvoiceListItem>(
                "Sp_GetInvoiceList",
                new { pageSize, pageNumber, status, companyFilter, startDate, endDate },
                commandType: CommandType.StoredProcedure);

            return new InvoiceListPageData
            {
                Items = items.ToList()
            };
        }

        public async Task<DownloadInvoiceResponse?> DownloadInvoiceAsync(List<string> invoiceNumbers, int? userId)
        {
            using var connection = _context.CreateConnection();
            var parameters = JsonSerializer.Serialize(new
            {
                userId,
                invoiceNumber = invoiceNumbers
            });

            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_DownloadInvoice",
                new { Parameters = parameters },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<DownloadInvoiceResponse>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            return await connection.QueryFirstOrDefaultAsync<DownloadInvoiceResponse>(
                "Sp_DownloadInvoice",
                new { Parameters = parameters },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdatePlannedPaymentDateAsync(List<string> invoiceNumbers, DateTime plannedPaymentDate)
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_UpdatePlannedPaymentDate",
                new
                {
                    invoiceNumbers = JsonSerializer.Serialize(invoiceNumbers),
                    plannedPaymentDate
                },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<bool>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var result = await connection.QueryFirstOrDefaultAsync<bool?>(
                "Sp_UpdatePlannedPaymentDate",
                new
                {
                    invoiceNumbers = JsonSerializer.Serialize(invoiceNumbers),
                    plannedPaymentDate
                },
                commandType: CommandType.StoredProcedure);

            return result ?? false;
        }

        private static ApiResponse<T>? TryParseJsonResponse<T>(object? row)
        {
            if (row is IDictionary<string, object> dict)
            {
                if (dict.TryGetValue("JsonResponse", out var jsonValue) && jsonValue != null)
                {
                    return DeserializeApiResponse<T>(jsonValue.ToString());
                }

                if (dict.TryGetValue("data", out var dataValue) && dataValue != null)
                {
                    var dataJson = dataValue.ToString();
                    if (!string.IsNullOrWhiteSpace(dataJson) && dataJson.TrimStart().StartsWith("{"))
                    {
                        var data = JsonSerializer.Deserialize<T>(dataJson, JsonOptions());
                        return new ApiResponse<T>
                        {
                            Data = data,
                            IsSuccess = true,
                            Message = string.Empty,
                            ErrorCode = string.Empty
                        };
                    }
                }
            }

            return null;
        }

        private static ApiResponse<T>? DeserializeApiResponse<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions());
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }
}
