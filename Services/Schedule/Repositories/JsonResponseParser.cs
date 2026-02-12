using System.Text.Json;
using ScheduleService.Models;

namespace ScheduleService.Repositories
{
    internal static class JsonResponseParser
    {
        public static ApiResponse<T> ParseFromRow<T>(object? row, string fallbackMessage)
        {
            if (row is IDictionary<string, object> dict)
            {
                foreach (var entry in dict)
                {
                    if (string.Equals(entry.Key, "JsonResponse", StringComparison.OrdinalIgnoreCase))
                    {
                        return ParseFromJson<T>(entry.Value?.ToString(), fallbackMessage);
                    }
                }
            }

            return BuildFallback<T>(fallbackMessage, "NOT_IMPLEMENTED");
        }

        public static ApiResponse<T> ParseFromJson<T>(string? json, string fallbackMessage)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return BuildFallback<T>(fallbackMessage, "EMPTY_RESPONSE");
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

            return BuildFallback<T>(fallbackMessage, "PARSE_ERROR");
        }

        private static ApiResponse<T> BuildFallback<T>(string fallbackMessage, string errorCode)
        {
            return new ApiResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = fallbackMessage,
                ErrorCode = errorCode
            };
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
