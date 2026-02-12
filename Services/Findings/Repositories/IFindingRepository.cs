namespace FindingsAPI.Gateway.Repositories
{
    public interface IFindingRepository
    {
        Task<IEnumerable<Finding>> GetFindingsAsync(GetFindingsQuery query);
        Task<Finding?> GetByIdAsync(int id);
        Task<IEnumerable<Finding>> SearchAsync(SearchFindingsQuery query);
        Task AddAsync(Finding entity);
        Task UpdateAsync(Finding entity);
        Task DeleteAsync(Finding entity);
        Task<int> SaveChangesAsync();
    }
}
