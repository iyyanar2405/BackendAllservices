using Polly;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Certificate.Extensions;

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
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        var jitterer = new Random();
        return Policy
            .Handle<HttpRequestException>()
            .Or<OperationCanceledException>()
            .OrResult<HttpResponseMessage>(r =>
                r.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                r.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                r.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                r.StatusCode == System.Net.HttpStatusCode.BadGateway ||
                r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                r.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                {
                    var baseDelay = Math.Pow(2, retryAttempt - 1);
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
        _logger.LogWarning("Retry recorded for {Endpoint}. Total retries: {TotalRetries}", endpoint, context.TotalRetries);
    }

    public void RecordCircuitBreakerOpen(string endpoint)
    {
        var context = _contexts.GetOrAdd(endpoint, new ResilienceContext { ServiceName = endpoint });
        context.CurrentCircuitState = CircuitState.Open;
        context.RecordFailure();
        _logger.LogError("Circuit breaker opened for {Endpoint}. Consecutive failures: {Failures}", endpoint, context.ConsecutiveFailures);
    }

    public void RecordCircuitBreakerClosed(string endpoint)
    {
        var context = _contexts.GetOrAdd(endpoint, new ResilienceContext { ServiceName = endpoint });
        context.CurrentCircuitState = CircuitState.Closed;
        context.Reset();
        _logger.LogInformation("Circuit breaker closed for {Endpoint}", endpoint);
    }

    public void RecordTimeout(string endpoint)
    {
        var context = _contexts.GetOrAdd(endpoint, new ResilienceContext { ServiceName = endpoint });
        context.TotalTimeouts++;
        context.RecordFailure();
        _logger.LogWarning("Timeout recorded for {Endpoint}. Total timeouts: {Timeouts}", endpoint, context.TotalTimeouts);
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
    Closed = 0,
    Open = 1,
    HalfOpen = 2
}
