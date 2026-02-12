// Repositories/IUnitOfWork.cs
namespace FindingsAPI.Gateway.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IFindingRepository Findings { get; }
        IRepository<Company> Companies { get; }
        IRepository<Site> Sites { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}