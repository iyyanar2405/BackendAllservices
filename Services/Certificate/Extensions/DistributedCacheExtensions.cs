using Microsoft.Extensions.Caching.Distributed;

namespace CertificateService.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "CertificateService:";
            });
            return services;
        }
    }

    public interface IRedisCacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }

    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger) { _cache = cache; _logger = logger; }
        public async Task<T?> GetAsync<T>(string key)
        {
            try { var value = await _cache.GetStringAsync(key); return string.IsNullOrEmpty(value) ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value); }
            catch (Exception ex) { _logger.LogError(ex, "Error getting cache: {Key}", key); return default; }
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try { var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1) }; await _cache.SetStringAsync(key, System.Text.Json.JsonSerializer.Serialize(value), options); }
            catch (Exception ex) { _logger.LogError(ex, "Error setting cache: {Key}", key); }
        }
        public async Task RemoveAsync(string key) { try { await _cache.RemoveAsync(key); } catch (Exception ex) { _logger.LogError(ex, "Error removing cache: {Key}", key); } }
    }
}
