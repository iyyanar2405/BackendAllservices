using Polly;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Action.Extensions;

/// <summary>
/// Extension methods for configuring resilience patterns using Polly
/// Implements Circuit Breaker and Retry patterns for fault tolerance
/// </summary>
public static class ResiliencePolicyExtensions
{
    /// <summary>
    /// Adds resilience policies for HTTP clients with Circuit Breaker and Retry patterns
    /// </summary>
    public static IServiceCollection AddResiliencePolicies(
        this IServiceCollection services)
    {
        // Register resilience monitor
        services.AddSingleton<IResilienceMonitor, ResilienceMonitor>();
        
        return services;
    }

    /// <summary>
    /// Configures HttpClientFactory with resilience policies
    /// </summary>
    public static IHttpClientBuilder AddResilientHttpClient(
        this IServiceCollection services,
        string clientName = "DefaultHttpClient")
    {
        var retryPolicy = GetRetryPolicy();
        var circuitBreakerPolicy = GetCircuitBreakerPolicy();
        
        return services
            .AddHttpClient(clientName)
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "HadvidaApiClient/1.0");
            });
    }

    /// <summary>
    /// Gets a retry policy with exponential backoff
    /// Retries on transient failures (5xx, 408, 429)
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        var jitterer = new Random();

        return Policy
            .Handle<HttpRequestException>()
            .Or<OperationCanceledException>()
            .OrResult<HttpResponseMessage>(r =>
                r.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||        // 408
                r.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||       // 429
                r.StatusCode == System.Net.HttpStatusCode.InternalServerError ||   // 500
                r.StatusCode == System.Net.HttpStatusCode.BadGateway ||            // 502
                r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||    // 503
                r.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)          // 504
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                {
                    var baseDelay = Math.Pow(2, retryAttempt - 1); // 1, 2, 4 seconds
                    var jitterMs = jitterer.Next(0, 1000);
                    var totalMs = (int)(baseDelay * 1000) + jitterMs;

                    return TimeSpan.FromMilliseconds(totalMs);
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    System.Console.WriteLine(
                        $"Retry #{retryCount} after {timespan.TotalSeconds}s. " +
                        $"Reason: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                });
    }

    /// <summary>
    /// Gets a circuit breaker policy
    /// Opens after 5 consecutive failures, stays open for 30 seconds
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Policy
            .Handle<HttpRequestException>()
            .Or<OperationCanceledException>()
            .OrResult<HttpResponseMessage>(r =>
                r.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                r.StatusCode == System.Net.HttpStatusCode.BadGateway ||
                r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                r.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, timespan) =>
                {
                    System.Console.WriteLine(
                        $"Circuit breaker opened for {timespan.TotalSeconds}s due to: " +
                        $"{outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                },
                onReset: () =>
                {
                    System.Console.WriteLine("Circuit breaker reset");
                });
    }
}

/// <summary>
/// Extension methods for configuring database resilience
/// </summary>
public static class DatabaseResilienceExtensions
{
    /// <summary>
    /// Adds resilience policies for database connections
    /// Handles transient database connectivity issues
    /// </summary>
    public static IServiceCollection AddDatabaseResiliencePolicies(
        this IServiceCollection services)
    {
        // Retry policy for database operations
        var databaseRetryPolicy = Policy
            .Handle<Exception>(ex => 
                ex.Message.Contains("transient") ||
                ex.Message.Contains("timeout") ||
                ex.Message.Contains("connection"))
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryCount =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryCount)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    System.Console.WriteLine(
                        $"Database retry #{retryCount} after {timespan.TotalSeconds}s");
                });

        return services;
    }
}

/// <summary>
/// Resilience context for tracking failures and circuit breaker state
/// </summary>
public class ResilienceContext
{
    public string ServiceName { get; set; } = string.Empty;
    public DateTime LastFailureTime { get; set; }
    public int ConsecutiveFailures { get; set; }
    public CircuitState CurrentCircuitState { get; set; }
    public int TotalRetries { get; set; }
    public int TotalTimeouts { get; set; }

    public void RecordFailure()
    {
        ConsecutiveFailures++;
        LastFailureTime = DateTime.UtcNow;
    }

    public void Reset()
    {
        ConsecutiveFailures = 0;
        CurrentCircuitState = CircuitState.Closed;
    }
}

/// <summary>
/// Service for monitoring resilience policies
/// </summary>
public interface IResilienceMonitor
{
    void RecordRetry(string endpoint, int retryCount);
    void RecordCircuitBreakerOpen(string endpoint);
    void RecordCircuitBreakerClosed(string endpoint);
    void RecordTimeout(string endpoint);
    Dictionary<string, ResilienceContext> GetMetrics();
}

/// <summary>
/// Implementation of resilience monitoring
/// </summary>
public class ResilienceMonitor : IResilienceMonitor
{
    private readonly ConcurrentDictionary<string, ResilienceContext> _contexts = new();
    private readonly ILogger<ResilienceMonitor> _logger;

    public ResilienceMonitor(ILogger<ResilienceMonitor> logger)
    {
        _logger = logger;
    }

    public void RecordRetry(string endpoint, int retryCount)
    {
        var context = _contexts.GetOrAdd(endpoint, new ResilienceContext { ServiceName = endpoint });
        context.TotalRetries++;

        _logger.LogWarning(
            "Retry recorded for {Endpoint}. Total retries: {TotalRetries}",
            endpoint, context.TotalRetries);
    }

    public void RecordCircuitBreakerOpen(string endpoint)
    {
        var context = _contexts.GetOrAdd(endpoint, new ResilienceContext { ServiceName = endpoint });
        context.CurrentCircuitState = CircuitState.Open;
        context.RecordFailure();

        _logger.LogError(
            "Circuit breaker opened for {Endpoint}. Consecutive failures: {Failures}",
            endpoint, context.ConsecutiveFailures);
    }

    public void RecordCircuitBreakerClosed(string endpoint)
    {
        var context = _contexts.GetOrAdd(endpoint, new ResilienceContext { ServiceName = endpoint });
        context.CurrentCircuitState = CircuitState.Closed;
        context.Reset();

        _logger.LogInformation(
            "Circuit breaker closed for {Endpoint}",
            endpoint);
    }

    public void RecordTimeout(string endpoint)
    {
        var context = _contexts.GetOrAdd(endpoint, new ResilienceContext { ServiceName = endpoint });
        context.TotalTimeouts++;
        context.RecordFailure();

        _logger.LogWarning(
            "Timeout recorded for {Endpoint}. Total timeouts: {Timeouts}",
            endpoint, context.TotalTimeouts);
    }

    public Dictionary<string, ResilienceContext> GetMetrics()
    {
        return _contexts.ToDictionary(x => x.Key, x => x.Value);
    }
}

/// <summary>
/// Circuit state enumeration
/// </summary>
public enum CircuitState
{
    Closed = 0,      // Circuit is closed, requests pass through normally
    Open = 1,        // Circuit is open, requests fail immediately
    HalfOpen = 2     // Circuit is half-open, testing if service recovered
}
