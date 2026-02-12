# Circuit Breaker & Retry Resilience Patterns Implementation Guide

## Overview

This guide documents the implementation of circuit breaker and retry patterns across all microservices using the Polly resilience library. These patterns protect services from cascading failures and handle transient errors gracefully.

## Architecture

### Pattern: Retry with Exponential Backoff

**Purpose**: Automatically retry failed requests instead of immediately failing

**Configuration**:
- **Maximum Retries**: 3 attempts
- **Backoff Strategy**: Exponential (1s, 2s, 4s)
- **Jitter**: Random 0-1000ms added to prevent thundering herd
- **Handles**: 
  - 408 Timeout
  - 429 Too Many Requests (Rate Limiting)
  - 5xx Server Errors (500, 502, 503, 504)
  - HttpRequestException
  - OperationCanceledException

**Flow**:
```
Request Fails
    ↓
Retry #1 (wait 1-2s) → Success? Return
    ↓ Failure
Retry #2 (wait 2-3s) → Success? Return
    ↓ Failure
Retry #3 (wait 4-5s) → Success? Return
    ↓ Failure
Return Failure to Caller
```

### Pattern: Circuit Breaker

**Purpose**: Prevent cascading failures by failing fast when a service is unavailable

**Configuration**:
- **Failure Threshold**: 5 consecutive failures
- **Break Duration**: 30 seconds
- **States**:
  - **Closed**: Normal operation, requests pass through (initial state)
  - **Open**: Service unavailable, requests fail immediately without calling service
  - **Half-Open**: Testing recovery, allowing limited requests through

**Flow**:
```
               Closed
                 ↓ (5 failures)
            Open (30s) ← Half-Open (test request)
                 ↓ (timeout)
            Half-Open (allow test requests)
```

**Benefits**:
- Prevents resource exhaustion
- Allows dependent services to recover gracefully
- Provides clear failure signals vs timeouts

## Implementation Details

### File Structure

Each service now includes:
```
Services/[ServiceName]/
├── Extensions/
│   └── ResiliencePolicyExtensions.cs
├── Program.cs (updated)
└── [ServiceName].csproj (Polly package added)
```

### Code Example: Using Resilient HTTP Client

```csharp
// In Program.cs
builder.Services.AddResiliencePolicies();      // Register resilience monitor
builder.Services.AddResilientHttpClient();      // Configure HTTP client with policies

// In Controller or Service
public class OrderService
{
    private readonly HttpClient _httpClient;
    private readonly IResilienceMonitor _monitor;
    
    public OrderService(IHttpClientFactory factory, IResilienceMonitor monitor)
    {
        _httpClient = factory.CreateClient("DefaultHttpClient");
        _monitor = monitor;
    }
    
    public async Task<Order> GetOrderAsync(int id)
    {
        try
        {
            // Retry and Circuit Breaker policies applied automatically
            var response = await _httpClient.GetAsync($"https://order-service/api/orders/{id}");
            return await response.Content.ReadAsAsync<Order>();
        }
        catch (Exception ex)
        {
            _monitor.RecordTimeout("order-service");
            throw;
        }
    }
}
```

## Monitoring

### ResilienceMonitor Interface

```csharp
public interface IResilienceMonitor
{
    void RecordRetry(string endpoint, int retryCount);
    void RecordCircuitBreakerOpen(string endpoint);
    void RecordCircuitBreakerClosed(string endpoint);
    void RecordTimeout(string endpoint);
    Dictionary<string, ResilienceContext> GetMetrics();
}
```

### Usage

```csharp
// Get current resilience metrics
var metrics = _monitor.GetMetrics();
foreach (var (endpoint, context) in metrics)
{
    Console.WriteLine($"Endpoint: {endpoint}");
    Console.WriteLine($"  Total Retries: {context.TotalRetries}");
    Console.WriteLine($"  Total Timeouts: {context.TotalTimeouts}");
    Console.WriteLine($"  Circuit State: {context.CurrentCircuitState}");
    Console.WriteLine($"  Last Failure: {context.LastFailureTime}");
}
```

## Configuration Reference

### Retry Policy

| Setting | Value | Purpose |
|---------|-------|---------|
| Retry Count | 3 | Maximum number of retry attempts |
| Backoff Delay | Exponential | Delays increase: 1s, 2s, 4s |
| Jitter | 0-1000ms | Randomness to prevent synchronized retries |
| Timeout Handling | 408, 429, 5xx | Error codes that trigger retry |

### Circuit Breaker Policy

| Setting | Value | Purpose |
|---------|-------|---------|
| Failure Threshold | 5 | Open after 5 consecutive failures |
| Break Duration | 30 seconds | Time circuit stays open |
| Test Strategy | Half-Open | Allow gradual recovery testing |
| Failure Codes | 500-504 | Server errors that trigger break |

## Transient Error Handling

**Transient errors** (temporary, should be retried):
- Network timeouts
- Rate limiting (429)
- Service temporarily unavailable (503)
- Gateway errors (502, 504)

**Permanent errors** (won't be fixed by retry):
- 400 Bad Request
- 401 Unauthorized
- 403 Forbidden
- 404 Not Found

## Best Practices

### 1. **Idempotency**
Ensure operations are idempotent before implementing retry. Example:
```csharp
//❌ BAD - Not idempotent
async Task TransferMoney(int amount)
{
    await _db.DeductBalance(amount);  // Retrying will deduct multiple times
}

// ✅ GOOD - Idempotent
async Task TransferMoney(string transactionId, int amount)
{
    var existing = await _db.GetTransaction(transactionId);
    if (existing != null) return;  // Idempotent: rerun is safe
    await _db.DeductBalance(amount);
}
```

### 2. **Timeout Hierarchy**
```
Individual Request Timeout (10s)
    ↓
Client-Level Timeout (30s)
    ↓
Load Balancer Timeout (60s)
```

### 3. **Monitoring**
- Track retry rates to identify flaky endpoints
- Alert when circuit breaker opens
- Monitor circuit state transitions
- Set up metrics dashboards

### 4. **Gradual Rollout**
- Start with short timeout values
- Monitor real-world behavior
- Adjust retry counts based on metrics
- Tune circuit breaker thresholds

## Troubleshooting

### Issue: Circuit Breaker Stuck Open

**Symptoms**: Service fails immediately without attempting requests

**Resolution**:
1. Check if dependent service is running
2. Verify network connectivity
3. Check service logs for errors
4. Wait for 30-second break duration (or restart service)

### Issue: High Retry Rate

**Symptoms**: Many log entries showing "Retry #1", "Retry #2"

**Resolution**:
1. Investigate root cause (timeout? service overloaded?)
2. Consider increasing timeouts if service is slow but healthy
3. Add metrics dashboard to track by endpoint
4. Implement exponential backoff falloff if needed

### Issue: Circuit Breaker Too Sensitive

**Symptoms**: Breaker opens too frequently

**Resolution**:
1. Increase failure threshold from 5 to 10
2. Extend break duration from 30s to 60s
3. Only count specific error codes

## Integration Examples

### Example 1: Direct HttpClient Usage

```csharp
var factory = services.GetRequiredService<IHttpClientFactory>();
var client = factory.CreateClient("DefaultHttpClient");
var result = await client.GetAsync("https://api.example.com/data");
```

### Example 2: Dependency Injection

```csharp
public class DataService
{
    public DataService(IHttpClientFactory factory, IResilienceMonitor monitor)
    {
        _client = factory.CreateClient("DefaultHttpClient");
        _monitor = monitor;
    }
}
```

### Example 3: Custom Fallback

```csharp
try
{
    return await _client.GetAsync(endpoint);
}
catch (HttpRequestException) when (_monitor.IsCircuitOpen(endpoint))
{
    // Return cached value or default
    return GetCachedValue(endpoint) ?? DefaultValue;
}
```

## Related Documentation

- [Polly Library](https://github.com/App-vNext/Polly)
- [Rate Limiting Guide](../RATE_LIMITING_GUIDE.md)
- [API Gateway Configuration](../ApiGateway/ocelot.json)
- [Monitoring & Observability](./MONITORING_GUIDE.md)

## Deployment Notes

**Services Updated**:
- Action Service
- Audit Service
- Certificate Service
- Contract Service
- Finance Service
- Findings Service
- Notification Service
- Schedule Service
- Settings Service

**Backward Compatibility**:
- All changes are additive (no breaking changes)
- Existing endpoints continue to work
- Policies are transparent to callers

**Version Requirements**:
- .NET 10.0+
- Polly 8.2.1+

## Future Improvements

- [ ] Add Bulkhead isolation pattern
- [ ] Implement fallback policy with cached responses
- [ ] Add distributed tracing integration
- [ ] Implement adaptive timeout based on response times
- [ ] Add chaos engineering tests

---

**Last Updated**: 2024
**Maintained By**: [Team Name]
**Status**: Production Ready
