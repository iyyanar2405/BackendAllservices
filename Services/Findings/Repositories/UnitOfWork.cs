// Repositories/UnitOfWork.cs
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using FindingsAPI.Gateway.Data;
using System.Data;

namespace FindingsAPI.Gateway.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private IDbTransaction _transaction;

        private IFindingRepository _findings;
        private IRepository<Company> _companies;
        private IRepository<Site> _sites;

        public UnitOfWork(
            ApplicationDbContext context,
            IConfiguration configuration,
            IDistributedCache distributedCache,
            IMemoryCache memoryCache)
        {
            _context = context;
            _configuration = configuration;
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
        }

        public IFindingRepository Findings => _findings ??= new DapperFindingRepository(
            _configuration,
            new EfRepository<Finding>(_context));
        public IRepository<Company> Companies => _companies ??= new DapperRepository<Company>(_configuration, _distributedCache, _memoryCache);
        public IRepository<Site> Sites => _sites ??= new DapperRepository<Site>(_configuration, _distributedCache, _memoryCache);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_context.Database.CurrentTransaction == null)
            {
                await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitAsync()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.CommitAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.RollbackAsync();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}