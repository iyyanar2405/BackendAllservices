using FindingsAPI.Gateway.Data;
using Microsoft.EntityFrameworkCore;

namespace FindingsAPI.Gateway.Repositories
{
    public class EfFindingRepository : IFindingRepository
    {
        private readonly ApplicationDbContext _context;

        public EfFindingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Finding>> GetFindingsAsync(GetFindingsQuery query)
        {
            var findingsQuery = _context.Findings.AsNoTracking().AsQueryable();

            if (query.CompanyId.HasValue)
            {
                findingsQuery = findingsQuery.Where(f => f.CompanyId == query.CompanyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                findingsQuery = findingsQuery.Where(f => f.Status == query.Status);
            }

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                findingsQuery = findingsQuery.Where(f => f.Category == query.Category);
            }

            return await findingsQuery.ToListAsync();
        }

        public async Task<Finding?> GetByIdAsync(int id)
        {
            return await _context.Findings
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FindingsId == id);
        }

        public async Task<IEnumerable<Finding>> SearchAsync(SearchFindingsQuery query)
        {
            var findingsQuery = _context.Findings.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                switch (query.SearchIn)
                {
                    case SearchField.Title:
                        findingsQuery = findingsQuery.Where(f => f.Title.Contains(query.SearchTerm));
                        break;
                    case SearchField.Description:
                        findingsQuery = findingsQuery.Where(f => f.Description.Contains(query.SearchTerm));
                        break;
                    case SearchField.Number:
                        findingsQuery = findingsQuery.Where(f => f.FindingNumber.Contains(query.SearchTerm));
                        break;
                    case SearchField.All:
                    default:
                        findingsQuery = findingsQuery.Where(f =>
                            f.Title.Contains(query.SearchTerm) ||
                            f.Description.Contains(query.SearchTerm) ||
                            f.FindingNumber.Contains(query.SearchTerm));
                        break;
                }
            }

            return await findingsQuery.ToListAsync();
        }

        public async Task AddAsync(Finding entity)
        {
            await _context.Findings.AddAsync(entity);
        }

        public Task UpdateAsync(Finding entity)
        {
            _context.Findings.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Finding entity)
        {
            _context.Findings.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
