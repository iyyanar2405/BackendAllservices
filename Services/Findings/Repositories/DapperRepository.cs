// Repositories/DapperRepository.cs
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq.Expressions;
using System.Text.Json;

namespace FindingsAPI.Gateway.Repositories
{
    public class DapperRepository<T> : IRepository<T> where T : class
    {
        private readonly string _connectionString;
        private readonly IDistributedCache _cache;
        private readonly IMemoryCache _memoryCache;

        public DapperRepository(IConfiguration configuration, IDistributedCache cache, IMemoryCache memoryCache)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _cache = cache;
            _memoryCache = memoryCache;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<T?> GetByIdAsync(int id)
        {
            var cacheKey = $"{typeof(T).Name}_{id}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<T>(cached);
            }

            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {typeof(T).Name}s WHERE {typeof(T).Name}Id = @Id", new { Id = id });
            if (result != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }
            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var cacheKey = $"{typeof(T).Name}_All";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<IEnumerable<T>>(cached);
            }

            using var connection = CreateConnection();
            var result = await connection.QueryAsync<T>($"SELECT * FROM {typeof(T).Name}s");
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            return result;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            // For simplicity, get all and filter in memory
            var all = await GetAllAsync();
            return all.AsQueryable().Where(predicate);
        }

        public async Task AddAsync(T entity)
        {
            using var connection = CreateConnection();
            var properties = typeof(T).GetProperties().Where(p => p.Name != $"{typeof(T).Name}Id");
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var sql = $"INSERT INTO {typeof(T).Name}s ({columns}) VALUES ({values}); SELECT SCOPE_IDENTITY();";

            var id = await connection.ExecuteScalarAsync<int>(sql, entity);
            typeof(T).GetProperty($"{typeof(T).Name}Id")?.SetValue(entity, id);

            // Invalidate cache
            await InvalidateCacheAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            using var connection = CreateConnection();
            var properties = typeof(T).GetProperties();
            var setClause = string.Join(", ", properties.Where(p => p.Name != $"{typeof(T).Name}Id").Select(p => $"{p.Name} = @{p.Name}"));
            var sql = $"UPDATE {typeof(T).Name}s SET {setClause} WHERE {typeof(T).Name}Id = @{typeof(T).Name}Id";

            await connection.ExecuteAsync(sql, entity);

            // Invalidate cache
            await InvalidateCacheAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync($"DELETE FROM {typeof(T).Name}s WHERE {typeof(T).Name}Id = @{typeof(T).Name}Id", entity);

            // Invalidate cache
            await InvalidateCacheAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper doesn't have SaveChanges, so return 0
            return 0;
        }

        private async Task InvalidateCacheAsync()
        {
            var keys = new[] { $"{typeof(T).Name}_All" };
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key);
            }
        }
    }
}