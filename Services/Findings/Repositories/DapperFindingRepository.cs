using Dapper;
using FindingsAPI.Gateway;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FindingsAPI.Gateway.Repositories
{
    public class DapperFindingRepository : IFindingRepository
    {
        private readonly string _connectionString;
        private readonly IRepository<Finding> _writeRepository;

        public DapperFindingRepository(IConfiguration configuration, IRepository<Finding> writeRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _writeRepository = writeRepository;
        }

        public async Task<IEnumerable<Finding>> GetFindingsAsync(GetFindingsQuery query)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
SELECT
    f.*, 
    a.CompanyId AS CompanyId,
    fs.StatusName AS Status,
    fc.CategoryName AS Category,
    f.IdentifiedDate AS OpenDate
FROM Findings f
LEFT JOIN Audits a ON f.AuditId = a.AuditId
LEFT JOIN FindingStatuses fs ON f.FindingStatusId = fs.FindingStatusId
LEFT JOIN FindingCategories fc ON f.FindingCategoryId = fc.FindingCategoryId
WHERE (@CompanyId IS NULL OR a.CompanyId = @CompanyId)
  AND (@Status IS NULL OR fs.StatusName = @Status)
  AND (@Category IS NULL OR fc.CategoryName = @Category)
";

            return await connection.QueryAsync<Finding>(sql, new
            {
                CompanyId = query.CompanyId,
                Status = string.IsNullOrWhiteSpace(query.Status) ? null : query.Status,
                Category = string.IsNullOrWhiteSpace(query.Category) ? null : query.Category
            });
        }

        public async Task<Finding?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
SELECT
    f.*, 
    a.CompanyId AS CompanyId,
    fs.StatusName AS Status,
    fc.CategoryName AS Category,
    f.IdentifiedDate AS OpenDate
FROM Findings f
LEFT JOIN Audits a ON f.AuditId = a.AuditId
LEFT JOIN FindingStatuses fs ON f.FindingStatusId = fs.FindingStatusId
LEFT JOIN FindingCategories fc ON f.FindingCategoryId = fc.FindingCategoryId
WHERE f.FindingId = @Id
";

            return await connection.QueryFirstOrDefaultAsync<Finding>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Finding>> SearchAsync(SearchFindingsQuery query)
        {
            using var connection = new SqlConnection(_connectionString);

            var whereClause = query.SearchIn switch
            {
                SearchField.Title => "f.Title LIKE @Term",
                SearchField.Description => "f.Description LIKE @Term",
                SearchField.Number => "f.FindingNumber LIKE @Term",
                _ => "(f.Title LIKE @Term OR f.Description LIKE @Term OR f.FindingNumber LIKE @Term)"
            };

            var sql = $@"
SELECT
    f.*, 
    a.CompanyId AS CompanyId,
    fs.StatusName AS Status,
    fc.CategoryName AS Category,
    f.IdentifiedDate AS OpenDate
FROM Findings f
LEFT JOIN Audits a ON f.AuditId = a.AuditId
LEFT JOIN FindingStatuses fs ON f.FindingStatusId = fs.FindingStatusId
LEFT JOIN FindingCategories fc ON f.FindingCategoryId = fc.FindingCategoryId
WHERE {whereClause}
";

            return await connection.QueryAsync<Finding>(sql, new
            {
                Term = $"%{query.SearchTerm}%"
            });
        }

        public async Task AddAsync(Finding entity)
        {
            await _writeRepository.AddAsync(entity);
        }

        public async Task UpdateAsync(Finding entity)
        {
            await _writeRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Finding entity)
        {
            await _writeRepository.DeleteAsync(entity);
        }

        public Task<int> SaveChangesAsync()
        {
            return _writeRepository.SaveChangesAsync();
        }
    }
}
