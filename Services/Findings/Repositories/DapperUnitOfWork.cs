using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace FindingsAPI.Gateway.Repositories
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;

        private IFindingRepository? _findings;
        private IRepository<Company>? _companies;
        private IRepository<Site>? _sites;

        public DapperUnitOfWork(
            IConfiguration configuration,
            IDistributedCache distributedCache,
            IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
        }

        public IFindingRepository Findings => _findings ??= new DapperFindingRepository(
            _configuration,
            new DapperRepository<Finding>(_configuration, _distributedCache, _memoryCache));

        public IRepository<Company> Companies => _companies ??= new DapperRepository<Company>(
            _configuration,
            _distributedCache,
            _memoryCache);

        public IRepository<Site> Sites => _sites ??= new DapperRepository<Site>(
            _configuration,
            _distributedCache,
            _memoryCache);

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(0);
        }

        public Task BeginTransactionAsync()
        {
            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
