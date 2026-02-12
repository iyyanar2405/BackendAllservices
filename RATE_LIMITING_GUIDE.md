# Rate Limiting Implementation Guide

## Overview
Rate limiting has been successfully implemented in the Ocelot API Gateway to protect backend services from abuse, control traffic flow, and ensure fair resource allocation across clients.

---

## 1. Configuration Summary

### Rate Limits Applied by Route

#### Action Service
```json
// REST API: 100 requests/minute
// GraphQL: 50 requests/minute
"RateLimitOptions": {
  "ClientIdHeader": "Client-Id",
  "Period": "1m",
  "Limit": 100  // for REST
}
```

#### Audit Service
```json
// REST API: 80 requests/minute (strict for audit logging)
// GraphQL: 40 requests/minute
```

#### Certificate Service
```json
// REST API: 75 requests/minute
// GraphQL: 35 requests/minute
```

#### Contract Service
```json
// REST API: 60 requests/minute
// GraphQL: 30 requests/minute
```

#### Finance Service
```json
// REST API: 120 requests/minute (high volume)
// GraphQL: 60 requests/minute
```

#### Findings Service  
```json
// REST API: 150 requests/minute (main platform, highest limit)
// GraphQL: 80 requests/minute
```

#### Notification Service
```json
// REST API: 200 requests/minute (async, highest)
// GraphQL: 100 requests/minute
```

#### Schedule Service
```json
// REST API: 90 requests/minute
// GraphQL: 45 requests/minute
```

#### Settings Service
```json
// REST API: 70 requests/minute
// GraphQL: 35 requests/minute
```

---

## 2. Global Rate Limiting Settings

### ocelot.json GlobalConfiguration
```json
"GlobalConfiguration": {
  "RateLimitOptions": {
    "ClientIdHeader": "Client-Id",
    "QuotaExceededMessage": "API call quota exceeded. Contact support for rate limit increase.",
    "RateLimitCounterPrefix": "ocelot",
    "DisableRateLimitHeaders": false,
    "HttpStatusCode": 429
  },
  "QoSOptions": {
    "ExceptionsAllowedBeforeBreaking": 3,
    "DurationOfBreak": 30000,
    "TimeoutValue": 5000
  }
}
```

---

## 3. Rate Limit Response Headers

All rate-limited responses include headers for client tracking:

```http
HTTP/1.1 429 Too Many Requests
Content-Type: application/json
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1739370000
Retry-After: 60

{
  "message": "API call quota exceeded. Contact support for rate limit increase."
}
```

### Header Meanings
- **X-RateLimit-Limit**: Maximum requests allowed per period (60 seconds)
- **X-RateLimit-Remaining**: Requests remaining in current period
- **X-RateLimit-Reset**: Unix timestamp when the limit window resets
- **Retry-After**: Number of seconds to wait before retrying

---

## 4. Client Identification Methods

### Method 1: Client-Id Header (Recommended)
Each client should include a unique `Client-Id` header:

```bash
curl -H "Client-Id: mobile-app-v1" \
     -H "Authorization: Bearer {token}" \
     https://api.hadvida.local/api/findings/data
```

### Method 2: IP Address (Fallback)
If no `Client-Id` header is provided, rate limiting uses IP address:

```bash
# Rate limit tracked by IP address
curl https://api.hadvida.local/api/action/list
```

---

## 5. Implementing Client-Side Rate Limit Handling

### .NET HttpClient Example
```csharp
public class RateLimitAwareHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RateLimitAwareHttpClient> _logger;

    public RateLimitAwareHttpClient(HttpClient httpClient, 
        ILogger<RateLimitAwareHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> SendWithRetryAsync(
        HttpRequestMessage request, 
        string clientId)
    {
        // Add Client-Id header
        request.Headers.Add("Client-Id", clientId);

        var response = await _httpClient.SendAsync(request);

        // Handle 429 Too Many Requests
        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            // Extract retry-after header
            if (response.Headers.TryGetValues("Retry-After", out var retryValues))
            {
                int retryAfter = int.Parse(retryValues.First());
                _logger.LogWarning(
                    "Rate limited by gateway. Retry after {RetrySeconds} seconds",
                    retryAfter);

                // Wait and retry
                await Task.Delay(TimeSpan.FromSeconds(retryAfter));
                return await _httpClient.SendAsync(request);
            }
        }

        return response;
    }
}

// Usage
using var request = new HttpRequestMessage(HttpMethod.Get, 
    "https://api.hadvida.local/api/findings/data");
var response = await rateLimitClient.SendWithRetryAsync(request, "mobile-app");
```

### JavaScript/TypeScript Example
```typescript
class RateLimitAwareClient {
    async request(url: string, options: RequestInit = {}) {
        let retries = 0;
        const maxRetries = 3;

        while (retries < maxRetries) {
            try {
                const response = await fetch(url, {
                    ...options,
                    headers: {
                        ...options.headers,
                        'Client-Id': 'web-app-v1',
                        'Authorization': `Bearer ${this.token}`
                    }
                });

                if (response.status === 429) {
                    const retryAfter = parseInt(
                        response.headers.get('Retry-After') || '60'
                    );

                    console.log(
                        `Rate limited. Waiting ${retryAfter} seconds...`
                    );

                    await new Promise(resolve => 
                        setTimeout(resolve, retryAfter * 1000)
                    );

                    retries++;
                    continue;
                }

                return response;
            } catch (error) {
                console.error('Request failed:', error);
                throw error;
            }
        }

        throw new Error('Max retries exceeded');
    }
}

// Usage
const client = new RateLimitAwareClient();
const data = await client.request(
    'https://api.hadvida.local/api/findings/data'
);
```

---

## 6. Rate Limit Whitelisting (Future)

For trusted clients needing higher limits:

```json
"RateLimitOptions": {
  "ClientIdHeader": "Client-Id",
  "ClientWhitelist": ["internal-service-1", "reporting-system"],
  "Limit": 100
}
```

---

## 7. Monitoring Rate Limits

### Key Metrics to Track

```csharp
public class RateLimitMetrics
{
    public int TotalRequestsReceived { get; set; }
    public int RateLimitedRequests { get; set; }
    public double RateLimitPercentage => 
        (double)RateLimitedRequests / TotalRequestsReceived * 100;
    
    public Dictionary<string, int> RequestsByService { get; set; }
    public Dictionary<string, int> RateLimitsByService { get; set; }
    public Dictionary<string, int> RequestsByClientId { get; set; }
}
```

### Logging Implementation
```csharp
// Program.cs - Add rate limit logging middleware
app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    await next.Invoke();
    
    if (context.Response.StatusCode == 429)
    {
        var clientId = context.Request.Headers.TryGetValue(
            "Client-Id", out var clientIdValue
        ) ? clientIdValue.ToString() : context.Connection.RemoteIpAddress.ToString();

        logger.LogWarning(
            "Rate limit exceeded - ClientId: {ClientId}, Path: {Path}, Source: {Source}",
            clientId,
            context.Request.Path,
            context.Connection.RemoteIpAddress
        );
    }
});
```

---

## 8. Testing Rate Limits

### PowerShell Rate Limit Test Script
```powershell
# test-rate-limit.ps1
param(
    [string]$Endpoint = "http://localhost:5011/api/action/list",
    [int]$RequestCount = 150,
    [string]$ClientId = "test-client"
)

$results = @()
$rateLimitCount = 0

Write-Host "Testing rate limiting against: $Endpoint" -ForegroundColor Cyan
Write-Host "Sending $RequestCount requests with Client-Id: $ClientId`n" -ForegroundColor Yellow

for ($i = 1; $i -le $RequestCount; $i++) {
    try {
        $response = Invoke-WebRequest -Uri $Endpoint `
            -Headers @{ 
                "Client-Id" = $ClientId
                "Authorization" = "Bearer $token"
            } `
            -UseBasicParsing `
            -ErrorAction SilentlyContinue `
            -SkipHttpErrorCheck

        if ($response.StatusCode -eq 429) {
            $rateLimitCount++
            $retryAfter = $response.Headers['Retry-After']
            Write-Host "[$i] Rate Limited! Retry after: $retryAfter seconds" -ForegroundColor Red
        } elseif ($response.StatusCode -eq 200) {
            Write-Host "[$i] Success (200)" -ForegroundColor Green
        } else {
            Write-Host "[$i] Status: $($response.StatusCode)" -ForegroundColor Yellow
        }

        $results += @{
            Request = $i
            StatusCode = $response.StatusCode
            Timestamp = Get-Date
        }

        # Small delay to avoid overwhelming the server
        Start-Sleep -Milliseconds 50

    } catch {
        Write-Host "[$i] Error: $_" -ForegroundColor DarkRed
    }
}

Write-Host "`n=== Test Results ===" -ForegroundColor Cyan
Write-Host "Total Requests: $RequestCount"
Write-Host "Rate Limited: $rateLimitCount"
Write-Host "Success Rate: $(($RequestCount - $rateLimitCount) / $RequestCount * 100)%"

# Export results
$results | Export-Csv -Path "rate-limit-test-$(Get-Date -Format 'yyyyMMdd-HHmmss').csv"
```

### Running the Test
```bash
cd d:\Dotnetfull_stack\BackEnd\src

# Test Action Service (limit: 100/min)
.\test-rate-limit.ps1 -Endpoint "http://localhost:5011/api/action/list" `
                       -RequestCount 120 `
                       -ClientId "test-mobile-app"

# Test Findings Service (limit: 150/min)
.\test-rate-limit.ps1 -Endpoint "http://localhost:5011/api/findings/list" `
                       -RequestCount 180 `
                       -ClientId "test-web-app"

# Test GraphQL (lower limits)
.\test-rate-limit.ps1 -Endpoint "http://localhost:5011/findings/graphql" `
                       -RequestCount 100 `
                       -ClientId "test-graphql"
```

---

## 9. Best Practices

### For API Consumers
1. ✅ Always include `Client-Id` header for tracking
2. ✅ Implement exponential backoff on 429 responses
3. ✅ Monitor rate limit headers (#X-RateLimit- headers)
4. ✅ Cache responses when possible to reduce requests
5. ✅ Use batch endpoints when available

### For Operators
1. ✅ Monitor rate limit hit percentage
2. ✅ Track which clients are hitting limits
3. ✅ Adjust limits based on business needs
4. ✅ Alert on unusual spike patterns
5. ✅ Whitelist trusted internal services

### For Services
1. ✅ Design idempotent endpoints
2. ✅ Use database query optimization
3. ✅ Implement appropriate caching
4. ✅ Handle partial failures gracefully

---

## 10. Future Enhancements

### Planned Features
- [ ] Per-tier rate limiting (free/pro/enterprise)
- [ ] Time-based dynamic limits (peak/off-peak)
- [ ] Request credit system
- [ ] Rate limit marketplace/trading
- [ ] Advanced analytics dashboard
- [ ] OAuth scopes for fine-grained limits

### Configuration
```json
"Advanced RateLimitOptions": {
  "Tiers": [
    {
      "Name": "Free",
      "Limit": 100,
      "Period": "1h"
    },
    {
      "Name": "Professional",
      "Limit": 10000,
      "Period": "1h"
    },
    {
      "Name": "Enterprise",
      "Limit": -1,
      "Period": null
    }
  ],
  "TimeBasedLimits": {
    "PeakHours": "09:00-17:00",
    "OffPeakMultiplier": 1.5
  }
}
```

---

## 11. Troubleshooting

### Issue: Always Getting Rate Limited
**Solution**: 
1. Check if `Client-Id` header is being included
2. Verify limits aren't too strict for use case
3. Contact support for limit increase
4. Implement retry logic with backoff

### Issue: Different Clients Sharing Same Limit
**Solution**:
1. Ensure unique `Client-Id` per client
2. Check if requests are using same IP address
3. Add IP whitelisting if needed

### Issue: Legitimate Traffic Blocked
**Solution**:
1. Analyze traffic patterns using metrics
2. Increase limits for high-volume clients
3. Implement token bucket scaling for bursts
4. Consider horizontal scaling of backend

---

## 12. SLA & Support

### Rate Limit SLA
- **Free Tier**: 100-200 requests/minute (subject to availability)
- **Professional**: Custom negotiated limits
- **Enterprise**: Dedicated infrastructure

### Support
- **Email**: support@hadvida.inc
- **Priority**: Limits can be adjusted within 24 hours
- **Emergency**: 429 responses indicate immediate attention needed

---

**Implementation Date**: February 12, 2026  
**Status**: Active and Monitoring  
**Next Review**: February 19, 2026
