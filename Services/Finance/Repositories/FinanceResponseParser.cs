using System.Text.Json;
using FinanceService.Models;

namespace FinanceService.Repositories
{
    internal static class FinanceResponseParser
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static ApiResponse<T>? ParseApiResponse<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions);
        }

        public static bool? ParseBooleanData(string? json)
        {
            var response = ParseApiResponse<JsonElement>(json);
            if (response?.Data.ValueKind == JsonValueKind.True)
            {
                return true;
            }

            if (response?.Data.ValueKind == JsonValueKind.False)
            {
                return false;
            }

            return null;
        }

        public static DownloadInvoiceResponse? ParseDownloadInvoiceResponse(string? json)
        {
            var response = ParseApiResponse<JsonElement>(json);
            if (response?.Data.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var data = response.Data;
            if (!data.TryGetProperty("content", out var contentElement))
            {
                return null;
            }

            var contentBytes = ExtractContentBytes(contentElement);
            if (contentBytes == null)
            {
                return null;
            }

            var fileName = data.TryGetProperty("fileName", out var fileNameElement)
                ? fileNameElement.GetString()
                : null;

            var isZipped = data.TryGetProperty("isZipped", out var zipElement) && zipElement.GetBoolean();

            return new DownloadInvoiceResponse
            {
                Content = contentBytes.Select(b => (int)b).ToList(),
                FileName = fileName,
                IsZipped = isZipped
            };
        }

        private static byte[]? ExtractContentBytes(JsonElement contentElement)
        {
            switch (contentElement.ValueKind)
            {
                case JsonValueKind.Array:
                    var bytes = new List<byte>();
                    foreach (var item in contentElement.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.Number && item.TryGetByte(out var value))
                        {
                            bytes.Add(value);
                        }
                    }

                    return bytes.Count > 0 ? bytes.ToArray() : null;

                case JsonValueKind.String:
                    var base64 = contentElement.GetString();
                    if (string.IsNullOrWhiteSpace(base64))
                    {
                        return null;
                    }

                    try
                    {
                        return Convert.FromBase64String(base64);
                    }
                    catch (FormatException)
                    {
                        return null;
                    }

                default:
                    return null;
            }
        }
    }
}
