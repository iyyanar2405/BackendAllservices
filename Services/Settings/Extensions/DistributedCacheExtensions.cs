using Microsoft.Extensions.Caching.Distributed;

namespace SettingsService.Extensions
{
    public static class DistributedCacheExtensions { public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration) { services.AddStackExchangeRedisCache(options => { options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379"; options.InstanceName = "SettingsService:"; }); return services; } }
    public interface IRedisCacheService { Task<T?> GetAsync<T>(string key); Task SetAsync<T>(string key, T value, TimeSpan? exp = null); Task RemoveAsync(string key); }
    public class RedisCacheService : IRedisCacheService { private readonly IDistributedCache _c; public RedisCacheService(IDistributedCache c) => _c = c; public async Task<T?> GetAsync<T>(string k) { try { var v = await _c.GetStringAsync(k); return v == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(v); } catch { return default; } } public async Task SetAsync<T>(string k, T v, TimeSpan? e = null) { try { await _c.SetStringAsync(k, System.Text.Json.JsonSerializer.Serialize(v), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = e ?? TimeSpan.FromHours(1) }); } catch { } } public async Task RemoveAsync(string k) { try { await _c.RemoveAsync(k); } catch { } } }
}
