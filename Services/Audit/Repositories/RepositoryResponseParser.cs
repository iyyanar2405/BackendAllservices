using AuditService.Models;
using System.Text.Json;

namespace AuditService.Repositories
{
    public static class RepositoryResponseParser
    {
        public static ApiResponse<T> ParseJsonResponse<T>(object? row, string fallbackMessage)
        {
            if (row is IDictionary<string, object> dict)
            {
                if (TryParseStructuredResponse<T>(dict, out var structuredResponse) && structuredResponse != null)
                {
                    return structuredResponse;
                }

                if (dict.TryGetValue("JsonResponse", out var jsonValue) && jsonValue != null)
                {
                    return DeserializeApiResponse<T>(jsonValue.ToString(), fallbackMessage);
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
                            Message = dict.TryGetValue("message", out var message) ? message?.ToString() : string.Empty,
                            ErrorCode = dict.TryGetValue("errorCode", out var errorCode) ? errorCode?.ToString() : string.Empty
                        };
                    }
                }
            }

            return new ApiResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = fallbackMessage,
                ErrorCode = "NOT_IMPLEMENTED"
            };
        }

        public static ApiResponse<T> DeserializeApiResponse<T>(string? json, string fallbackMessage)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new ApiResponse<T>
                {
                    Data = default,
                    IsSuccess = false,
                    Message = fallbackMessage,
                    ErrorCode = "EMPTY_RESPONSE"
                };
            }

            try
            {
                var response = JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions());
                if (response != null)
                {
                    return response;
                }
            }
            catch
            {
            }

            return new ApiResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = fallbackMessage,
                ErrorCode = "PARSE_ERROR"
            };
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private static bool TryParseStructuredResponse<T>(IDictionary<string, object> dict, out ApiResponse<T>? response)
        {
            response = null;

            if (typeof(T) == typeof(AuditDaysByServiceResponse))
            {
                if (dict.TryGetValue("pieChartData", out var pieChartData) && dict.TryGetValue("totalServiceAuditsDayCount", out var totalValue))
                {
                    var items = DeserializeList<AuditDaysByServiceItem>(pieChartData);
                    var total = ConvertToDecimal(totalValue);
                    response = new ApiResponse<T>
                    {
                        Data = (T)(object)new AuditDaysByServiceResponse
                        {
                            PieChartData = items,
                            TotalServiceAuditsDayCount = total
                        },
                        IsSuccess = true,
                        Message = dict.TryGetValue("message", out var message) ? message?.ToString() : string.Empty,
                        ErrorCode = dict.TryGetValue("errorCode", out var errorCode) ? errorCode?.ToString() : string.Empty
                    };
                    return true;
                }
            }

            if (typeof(T) == typeof(AuditDaysByMonthAndServiceResponse))
            {
                if (dict.TryGetValue("chartData", out var chartData))
                {
                    var items = DeserializeList<AuditDaysMonthData>(chartData);
                    response = new ApiResponse<T>
                    {
                        Data = (T)(object)new AuditDaysByMonthAndServiceResponse
                        {
                            ChartData = items
                        },
                        IsSuccess = true,
                        Message = dict.TryGetValue("message", out var message) ? message?.ToString() : string.Empty,
                        ErrorCode = dict.TryGetValue("errorCode", out var errorCode) ? errorCode?.ToString() : string.Empty
                    };
                    return true;
                }
            }

            if (typeof(T) == typeof(AuditDaysGridResponse))
            {
                if (dict.TryGetValue("data", out var dataValue))
                {
                    var items = DeserializeList<AuditDaysGridNode>(dataValue);
                    response = new ApiResponse<T>
                    {
                        Data = (T)(object)new AuditDaysGridResponse
                        {
                            Data = items
                        },
                        IsSuccess = true,
                        Message = dict.TryGetValue("message", out var message) ? message?.ToString() : string.Empty,
                        ErrorCode = dict.TryGetValue("errorCode", out var errorCode) ? errorCode?.ToString() : string.Empty
                    };
                    return true;
                }
            }

            return false;
        }

        private static List<T> DeserializeList<T>(object? value)
        {
            if (value == null)
            {
                return new List<T>();
            }

            if (value is string json)
            {
                var trimmed = json.TrimStart();
                if (trimmed.StartsWith("[") || trimmed.StartsWith("{"))
                {
                    var parsed = JsonSerializer.Deserialize<List<T>>(json, JsonOptions());
                    return parsed ?? new List<T>();
                }
            }

            return new List<T>();
        }

        private static decimal ConvertToDecimal(object? value)
        {
            if (value == null)
            {
                return 0;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue;
            }

            if (decimal.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }

            return 0;
        }
    }
}
