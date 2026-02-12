using Microsoft.Extensions.Caching.Distributed;

namespace ContractService.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redis = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            services.AddStackExchangeRedisCache(options => { options.Configuration = redis; options.InstanceName = "ContractService:"; });
            return services;
        }
    }
    public interface IRedisCacheService { Task<T?> GetAsync<T>(string key); Task SetAsync<T>(string key, T value, TimeSpan? expiration = null); Task RemoveAsync(string key); }
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger) { _cache = cache; _logger = logger; }
        public async Task<T?> GetAsync<T>(string key) { try { var v = await _cache.GetStringAsync(key); return string.IsNullOrEmpty(v) ? default : System.Text.Json.JsonSerializer.Deserialize<T>(v); } catch (Exception ex) { _logger.LogError(ex, "Cache error"); return default; } }
        public async Task SetAsync<T>(string key, T value, TimeSpan? exp = null) { try { var opt = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = exp ?? TimeSpan.FromHours(1) }; await _cache.SetStringAsync(key, System.Text.Json.JsonSerializer.Serialize(value), opt); } catch { } }
        public async Task RemoveAsync(string key) { try { await _cache.RemoveAsync(key); } catch { } }
    }
}
