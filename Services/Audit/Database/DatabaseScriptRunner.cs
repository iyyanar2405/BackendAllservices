using AuditService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.RegularExpressions;

namespace AuditService.Database
{
    public class DatabaseScriptRunner
    {
        private static readonly Regex BatchSeparator = new Regex("^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DatabaseScriptRunner> _logger;
        private readonly DatabaseScriptOptions _options;
        private readonly IHostEnvironment _environment;

        public DatabaseScriptRunner(
            ApplicationDbContext dbContext,
            ILogger<DatabaseScriptRunner> logger,
            IOptions<DatabaseScriptOptions> options,
            IHostEnvironment environment)
        {
            _dbContext = dbContext;
            _logger = logger;
            _options = options.Value;
            _environment = environment;
        }

        public async Task ApplyAllAsync(CancellationToken cancellationToken)
        {
            var root = GetRootPath();
            if (!Directory.Exists(root))
            {
                _logger.LogWarning("Database script root path does not exist: {Root}", root);
                return;
            }

            var tablePath = System.IO.Path.Combine(root, "tables");
            var storedProcedurePath = System.IO.Path.Combine(root, "Stored-procedure");
            var insertScriptPath = System.IO.Path.Combine(root, "insert-scripts");

            await ApplyScriptsInFolderAsync(tablePath, "tables", cancellationToken);
            await ApplyScriptsInFolderAsync(storedProcedurePath, "stored procedures", cancellationToken);
            await ApplyScriptsInFolderAsync(insertScriptPath, "insert scripts", cancellationToken);
        }

        private async Task ApplyScriptsInFolderAsync(string folderPath, string label, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(folderPath))
            {
                _logger.LogWarning("Skipping {Label} because folder was not found: {Folder}", label, folderPath);
                return;
            }

            var scripts = Directory.GetFiles(folderPath, "*.sql", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var scriptPath in scripts)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ApplyScriptAsync(scriptPath, cancellationToken);
            }
        }

        private async Task ApplyScriptAsync(string scriptPath, CancellationToken cancellationToken)
        {
            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
            var batches = SplitIntoBatches(script);

            await using var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch))
                    {
                        continue;
                    }

                    await using var command = connection.CreateCommand();
                    command.CommandText = batch;
                    command.CommandType = CommandType.Text;
                    command.Transaction = transaction;
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Applied script: {Script}", scriptPath);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Failed to apply script: {Script}", scriptPath);
                throw;
            }
        }

        private static List<string> SplitIntoBatches(string script)
        {
            return BatchSeparator.Split(script)
                .Select(batch => batch.Trim())
                .Where(batch => !string.IsNullOrWhiteSpace(batch))
                .ToList();
        }

        private string GetRootPath()
        {
            if (!string.IsNullOrWhiteSpace(_options.RootPath))
            {
                return System.IO.Path.GetFullPath(_options.RootPath);
            }

            return _environment.ContentRootPath;
        }
    }
}
