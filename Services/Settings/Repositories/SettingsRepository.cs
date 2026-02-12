using Dapper;
using SettingsService.Data;
using SettingsService.Models;
using System.Data;
using System.Text.Json;

namespace SettingsService.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DapperContext _context;

        public SettingsRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<SettingsCompanyDetailsResponse?> GetCompanyDetailsAsync(int? userId)
        {
            using var connection = _context.CreateConnection();
            var parameters = JsonSerializer.Serialize(new { userId });

            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetSettingsCompanyDetails",
                new { Parameters = parameters },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<SettingsCompanyDetailsResponse>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            return await connection.QueryFirstOrDefaultAsync<SettingsCompanyDetailsResponse>(
                "Sp_GetSettingsCompanyDetails",
                new { Parameters = parameters },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IReadOnlyList<AdminUserResponse>> GetAdminListAsync(int? userId, string? accountDNVId)
        {
            using var connection = _context.CreateConnection();
            var parameters = JsonSerializer.Serialize(new { userId, accountDNVId });

            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetSettingsAdminList",
                new { Parameters = parameters },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<List<AdminUserResponse>>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var rows = await connection.QueryAsync<AdminUserResponse>(
                "Sp_GetSettingsAdminList",
                new { Parameters = parameters },
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<MemberUserResponse>> GetMemberListAsync(int? userId, string? accountDNVId)
        {
            using var connection = _context.CreateConnection();
            var parameters = JsonSerializer.Serialize(new { userId, accountDNVId });

            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetSettingsMemberList",
                new { Parameters = parameters },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<List<MemberUserResponse>>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var rows = await connection.QueryAsync<MemberUserResponse>(
                "Sp_GetSettingsMemberList",
                new { Parameters = parameters },
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<CountryResponse>> GetCountriesAsync()
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetCountryList",
                new { Parameters = "{}" },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<List<CountryResponse>>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var rows = await connection.QueryAsync<CountryResponse>(
                "Sp_GetCountryList",
                new { Parameters = "{}" },
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<PreferenceResponse?> GetPreferencesAsync(string objectType, string objectName, string pageName)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PreferenceResponse>(
                "Sp_GetPreferences",
                new { objectType, objectName, pageName },
                commandType: CommandType.StoredProcedure);
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
