using Microsoft.Extensions.Options;

namespace AuditService.Database
{
    public class DatabaseScriptHostedService : BackgroundService
    {
        private readonly DatabaseScriptRunner _runner;
        private readonly DatabaseScriptOptions _options;
        private readonly ILogger<DatabaseScriptHostedService> _logger;

        public DatabaseScriptHostedService(
            DatabaseScriptRunner runner,
            IOptions<DatabaseScriptOptions> options,
            ILogger<DatabaseScriptHostedService> logger)
        {
            _runner = runner;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.AutoApply)
            {
                _logger.LogInformation("Database script auto-apply is disabled.");
                return;
            }

            await _runner.ApplyAllAsync(stoppingToken);
        }
    }
}
