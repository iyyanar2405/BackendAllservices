using Microsoft.Extensions.Options;

namespace ScheduleService.Data
{
    public class DatabaseScriptHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseScriptHostedService> _logger;
        private readonly DatabaseScriptOptions _options;

        public DatabaseScriptHostedService(
            IServiceProvider serviceProvider,
            ILogger<DatabaseScriptHostedService> logger,
            IOptions<DatabaseScriptOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_options.ApplyOnStartup)
            {
                return;
            }

            _logger.LogInformation("Applying database scripts on startup.");
            using var scope = _serviceProvider.CreateScope();
            var migrator = scope.ServiceProvider.GetRequiredService<DatabaseScriptMigrator>();
            await migrator.ApplyAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
