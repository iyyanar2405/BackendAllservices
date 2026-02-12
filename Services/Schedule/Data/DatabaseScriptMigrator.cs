using System.Data;
using System.Data.Common;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IOPath = System.IO.Path;

namespace ScheduleService.Data
{
    public class DatabaseScriptMigrator
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DatabaseScriptMigrator> _logger;
        private readonly DatabaseScriptOptions _options;

        public DatabaseScriptMigrator(
            ApplicationDbContext dbContext,
            ILogger<DatabaseScriptMigrator> logger,
            IOptions<DatabaseScriptOptions> options)
        {
            _dbContext = dbContext;
            _logger = logger;
            _options = options.Value;
        }

        public async Task ApplyAsync(CancellationToken cancellationToken = default)
        {
            if (!_options.ApplyOnStartup)
            {
                _logger.LogInformation("Database scripts are disabled.");
                return;
            }

            var baseDir = AppContext.BaseDirectory;
            var scriptGroups = new[]
            {
                new ScriptGroup("tables", IOPath.Combine(baseDir, "tables")),
                new ScriptGroup("Stored-procedure", IOPath.Combine(baseDir, "Stored-procedure")),
                new ScriptGroup("insert-scripts", IOPath.Combine(baseDir, "insert-scripts"))
            };

            await using var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await EnsureHistoryTableAsync(connection, cancellationToken);

            foreach (var group in scriptGroups)
            {
                if (!Directory.Exists(group.Path))
                {
                    _logger.LogWarning("Script folder not found: {Path}", group.Path);
                    continue;
                }

                var scripts = Directory.GetFiles(group.Path, "*.sql", SearchOption.AllDirectories)
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var scriptPath in scripts)
                {
                    var scriptName = IOPath.GetRelativePath(baseDir, scriptPath).Replace('\\', '/');
                    if (await IsAppliedAsync(connection, scriptName, cancellationToken))
                    {
                        continue;
                    }

                    var scriptText = await File.ReadAllTextAsync(scriptPath, cancellationToken);
                    var batches = SplitBatches(scriptText);

                    await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        foreach (var batch in batches)
                        {
                            if (string.IsNullOrWhiteSpace(batch))
                            {
                                continue;
                            }

                            await ExecuteBatchAsync(connection, transaction, batch, cancellationToken);
                        }

                        await MarkAppliedAsync(connection, transaction, scriptName, cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                        _logger.LogInformation("Applied script {Script}", scriptName);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogError(ex, "Failed applying script {Script}", scriptName);
                        throw;
                    }
                }
            }
        }

        private static async Task EnsureHistoryTableAsync(IDbConnection connection, CancellationToken cancellationToken)
        {
            const string sql = @"
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SchemaScriptHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SchemaScriptHistory]
    (
        [ScriptName] NVARCHAR(260) NOT NULL PRIMARY KEY,
        [AppliedOn] DATETIME NOT NULL DEFAULT GETDATE()
    );
END";

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 300;
            if (command is DbCommand dbCommand)
            {
                await dbCommand.ExecuteNonQueryAsync(cancellationToken);
            }
            else
            {
                command.ExecuteNonQuery();
            }
        }

        private static async Task<bool> IsAppliedAsync(IDbConnection connection, string scriptName, CancellationToken cancellationToken)
        {
            const string sql = "SELECT COUNT(1) FROM [dbo].[SchemaScriptHistory] WHERE [ScriptName] = @ScriptName";
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            var param = command.CreateParameter();
            param.ParameterName = "@ScriptName";
            param.Value = scriptName;
            command.Parameters.Add(param);

            if (command is DbCommand dbCommand)
            {
                var result = await dbCommand.ExecuteScalarAsync(cancellationToken);
                return Convert.ToInt32(result) > 0;
            }

            var value = command.ExecuteScalar();
            return Convert.ToInt32(value) > 0;
        }

        private static async Task ExecuteBatchAsync(IDbConnection connection, IDbTransaction transaction, string batch, CancellationToken cancellationToken)
        {
            using var command = connection.CreateCommand();
            command.CommandText = batch;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 300;
            command.Transaction = transaction;

            if (command is DbCommand dbCommand)
            {
                await dbCommand.ExecuteNonQueryAsync(cancellationToken);
            }
            else
            {
                command.ExecuteNonQuery();
            }
        }

        private static async Task MarkAppliedAsync(IDbConnection connection, IDbTransaction transaction, string scriptName, CancellationToken cancellationToken)
        {
            const string sql = "INSERT INTO [dbo].[SchemaScriptHistory] ([ScriptName]) VALUES (@ScriptName)";
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.Transaction = transaction;

            var param = command.CreateParameter();
            param.ParameterName = "@ScriptName";
            param.Value = scriptName;
            command.Parameters.Add(param);

            if (command is DbCommand dbCommand)
            {
                await dbCommand.ExecuteNonQueryAsync(cancellationToken);
            }
            else
            {
                command.ExecuteNonQuery();
            }
        }

        private static IEnumerable<string> SplitBatches(string script)
        {
            return Regex.Split(script, "^\\s*GO\\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                .Select(batch => batch.Trim())
                .Where(batch => !string.IsNullOrWhiteSpace(batch));
        }

        private sealed record ScriptGroup(string Name, string Path);
    }
}
