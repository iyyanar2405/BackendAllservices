using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Core;
using Microsoft.Extensions.Configuration;

namespace Shared.Logging;

public static class SerilogExtensions
{
    public static LoggerConfiguration AddCustomSerilog(this LoggerConfiguration config, IConfiguration configuration)
    {
        var logLevel = configuration["Logging:LogLevel:Default"] ?? "Information";
        var minimumLevel = Enum.Parse<LogEventLevel>(logLevel);

        return config
            .MinimumLevel.Is(minimumLevel)
            .WriteTo.Console();
    }
}

public class CorrelationIdEnricher : ILogEventEnricher
{
    private readonly string _correlationId;

    public CorrelationIdEnricher(string correlationId)
    {
        _correlationId = correlationId;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", _correlationId));
    }
}

public class RequestPathEnricher : ILogEventEnricher
{
    private readonly string _requestPath;

    public RequestPathEnricher(string requestPath)
    {
        _requestPath = requestPath;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", _requestPath));
    }
}
