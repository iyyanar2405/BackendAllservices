# Microservices Architecture: Scaling Strategy

## Overview
This document outlines horizontal and vertical scaling strategies for the Hadvida Inc. microservices platform with 9 business services plus API Gateway and Authentication Provider.

---

## 1. Rate Limiting Configuration

### Current Implementation
All routes in ocelot.json now include per-route rate limiting:

```json
"RateLimitOptions": {
  "ClientIdHeader": "Client-Id",
  "QuotaExceededMessage": "API call quota exceeded. Contact support for rate limit increase.",
  "RateLimitCounterPrefix": "ocelot",
  "DisableRateLimitHeaders": false,
  "HttpStatusCode": 429
}
```

### Rate Limits by Service (per minute)

| Service | REST API | GraphQL | Rationale |
|---------|----------|---------|-----------|
| **Action** | 100 | 50 | Core service - moderate limits |
| **Audit** | 80 | 40 | Synchronous audit logging - strict |
| **Certificate** | 75 | 35 | Medium throughput service |
| **Contract** | 60 | 30 | Lower complexity operations |
| **Finance** | 120 | 60 | High-volume financial transactions |
| **Findings** | 150 | 80 | Core read-heavy service (main platform) |
| **Notification** | 200 | 100 | Async high-throughput service |
| **Schedule** | 90 | 45 | Scheduled task execution |
| **Settings** | 70 | 35 | Configuration service |

### Rate Limit Headers
Responses include Retry-After headers:
- `X-RateLimit-Limit`: Maximum requests allowed per period
- `X-RateLimit-Remaining`: Requests remaining in current period
- `X-RateLimit-Reset`: Timestamp when limit resets

### Client Identification
Clients identified via:
- `Client-Id` header (custom identifier for rate limit tracking)
- IP address fallback if Client-Id not provided

---

## 2. Horizontal Scaling Strategy

### Architecture Overview

```
┌─────────────────────────────────────────┐
│      Load Balancer (Entry Point)        │
│      - NGINX / HAProxy / Azure LB       │
│      - SSL Termination                  │
│      - Session Persistence              │
└──────────────┬──────────────────────────┘
               │
       ┌───────┼───────┐
       │       │       │
   ┌───▼──┐ ┌──▼──┐ ┌──▼──┐
   │ AG-1 │ │ AG-2│ │ AG-3│  API Gateway Instances
   └───┬──┘ └──┬──┘ └──┬──┘
       │       │       │
   ┌───┼───────┼───────┼───┐
   │   │   Redis Cache │   │  Shared Infrastructure
   │   │   (5.2+)      │   │
   └───┼───────────────┼───┘
       │               │
   ┌───┴──────────┬────┴───┐
   │   Service    │ Service │  Multiple Instances Per Service
   │  Cluster-1   │Cluster-2│
```

### Load Balancer Configuration

#### NGINX Example
```nginx
upstream api_gateway {
    least_conn;  # Load balancing algorithm
    server localhost:5011 fail_timeout=30s;
    server localhost:5012 fail_timeout=30s;
    server localhost:5013 fail_timeout=30s;
    keepalive 32;
}

server {
    listen 80;
    server_name api.hadvida.local;
    
    location / {
        proxy_pass http://api_gateway;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_connect_timeout 5s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
}
```

#### HAProxy Example
```haproxy
global
    maxconn 4096
    log stdout local0

defaults
    log     global
    mode    http
    timeout connect 5000
    timeout client 50000
    timeout server 50000
    compression enabled

frontend api_frontend
    bind *:80
    option forwardfor
    default_backend api_backend

backend api_backend
    balance leastconn
    option httpchk GET /health
    server ag1 localhost:5011 check inter 2s
    server ag2 localhost:5012 check inter 2s
    server ag3 localhost:5013 check inter 2s
```

### Horizontal Scaling Implementation

#### 1. API Gateway Scaling
```powershell
# Run multiple gateway instances on different ports
$ports = 5011, 5012, 5013

$ports | ForEach-Object {
    $env:ASPNETCORE_URLS = "http://localhost:$_"
    Start-Job {
        cd "d:\Dotnetfull_stack\BackEnd\src\ApiGateway"
        dotnet run --configuration Release --no-build
    }
}

# Monitor instances
Get-Job | Format-Table Id, Name, State, PSBeginTime
```

#### 2. Service Scaling
Scale individual services to multiple instances:

```powershell
# Example: Scale Action Service to 3 instances
$servicePath = "d:\Dotnetfull_stack\BackEnd\src\Services\Action"
$basePorts = 5011, 5012, 5013

$basePorts | ForEach-Object {
    $port = 5001 + ($_-5011)
    $env:ASPNETCORE_URLS = "http://localhost:$port"
    Start-Job {
        cd $servicePath
        dotnet run --configuration Release --no-build
    }
}
```

#### 3. Health Check Implementation
Each service should expose health endpoint:

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom_check");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// CustomHealthCheck.cs
public class CustomHealthCheck : IHealthCheck
{
    private readonly IRedisCacheService _cache;
    
    public CustomHealthCheck(IRedisCacheService cache)
    {
        _cache = cache;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.GetAsync("health_check_key");
            return HealthCheckResult.Healthy("Service is healthy");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Service is unhealthy");
        }
    }
}
```

### Horizontal Scaling Benefits
| Feature | Benefit |
|---------|---------|
| **Load Distribution** | Spreads traffic across instances |
| **High Availability** | Service continues if one instance fails |
| **Auto-Recovery** | Failed instances can be restarted |
| **Cost Efficient** | Use smaller instances, scale as needed |
| **Performance** | Linear scaling with load |

### Horizontal Scaling Challenges
- **Data Consistency**: Redis cache coordination across instances
- **Session State**: Use distributed session storage (Redis)
- **Database Connections**: Connection pooling needed
- **Operational Complexity**: More instances to manage

---

## 3. Vertical Scaling Strategy

### Resource Upgrades

#### Current Configuration
```
Per Service Instance:
├── CPU: 2 cores (baseline)
├── RAM: 4 GB
├── Disk: 50 GB SSD
└── Network: 1 Gbps
```

#### Recommended Tiers

| Tier | CPU | RAM | Use Case | Cost Impact |
|------|-----|-----|----------|------------|
| **Tier 1** (Current) | 2 | 4 GB | Development/Light Load | Baseline |
| **Tier 2** | 4 | 8 GB | Production Load | 2x |
| **Tier 3** | 8 | 16 GB | High Throughput | 4x |
| **Tier 4** | 16+ | 32+ GB | Enterprise Scale | 8x+ |

### Vertical Scaling Implementation

#### Memory Optimization
```csharp
// Startup optimization
builder.Services.AddOptions()
    .Configure<MemoryPoolOptions>(options =>
    {
        options.MaxBufferSize = 8192;
    })
    .Configure<KestrelServerOptions>(options =>
    {
        options.Limits.MaxRequestBodySize = 100 * 1024;
        options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(30);
    });
```

#### Database Connection Pooling
```csharp
// Optimize for higher memory instance
builder.Services.AddScoped<IDbConnection>(_ =>
{
    var connection = new SqlConnection(_connectionString)
    {
        // Connection pooling with larger pool
        ConnectionString = _connectionString + 
            ";Min Pool Size=10;Max Pool Size=100;Pooling=true;"
    };
    return connection;
});
```

#### Garbage Collection Configuration
```csharp
// Startup - Configure GC for larger memory instances
AppContext.SetSwitch("System.GC.HeapCount", 4);
AppContext.SetSwitch("System.GC.HeapAffinitizeMask", 0xF);
```

### Vertical Scaling Benefits
| Feature | Benefit |
|---------|---------|
| **Simplicity** | Single instance to manage |
| **State Preservation** | In-memory caching stays local |
| **Reduced Network** | Less inter-instance communication |
| **Easier Debugging** | Single process to troubleshoot |
| **Lower Latency** | No load balancer overhead |

### Vertical Scaling Challenges
| Challenge | Risk |
|-----------|------|
| **Single Point of Failure** | No redundancy if instance fails |
| **Diminishing Returns** | CPU/Memory improvements plateau |
| **Downtime for Upgrades** | Service interruption needed |
| **Resource Waste** | Peak capacity may be unused at off-peak |
| **License Costs** | Higher vCPU licensing for cloud |

---

## 4. Hybrid Scaling Strategy (Recommended)

### Recommended Approach for Hadvida

```
Production Environment Layout:
├── Load Balancer (Tier 2: 4 CPU, 8 GB)
├── API Gateway (3x instances, Tier 2 each)
├── Redis Cache (Tier 3: 8 CPU, 16 GB, dedicated)
├── Business Services
│   ├── Findings (3x Tier 2) - High traffic core service
│   ├── Action (2x Tier 2) - Medium traffic
│   ├── Notification (2x Tier 2) - Async processing
│   ├── Finance (2x Tier 3) - Single instance for consistency
│   ├── Audit (1x Tier 2) - Single instance, important
│   └── Others (1x Tier 1 each) - Low traffic
├── AuthProvider (2x Tier 1) - Lightweight, critical
└── Message Queue (1x dedicated - if implemented)
```

### Implementation Plan

#### Phase 1: Foundation (Week 1-2)
1. Implement health checks across all services
2. Set up load balancer (NGINX/HAProxy)
3. Configure rate limiting (completed)
4. Implement Redis cache scaling

#### Phase 2: Horizontal Scaling (Week 3-4)
1. Run 2-3 instances of high-traffic services
2. Test load distribution
3. Monitor performance metrics

#### Phase 3: Vertical Scaling (Week 5-6)
1. Identify bottleneck services (CPU/Memory profiling)
2. Upgrade to higher tiers incrementally
3. Benchmark and fine-tune

#### Phase 4: Monitoring & Auto-scaling (Week 7-8)
1. Implement metrics collection (Prometheus/ELK)
2. Set up alerting
3. Implement auto-scaling policies

### Scaling Decision Matrix

| Metric | Action | Type |
|--------|--------|------|
| **CPU > 70%** | Add instance OR upgrade | Depends on headroom |
| **Memory > 80%** | Upgrade tier | Vertical |
| **Response Time > 500ms** | Investigate bottleneck | Investigate |
| **Rate Limit Hits > 10%** | Review limits OR scale | Evaluate business need |
| **Queue Depth Growing** | Add workers | Horizontal |

---

## 5. Monitoring & Metrics

### Key Performance Indicators

```csharp
public class PerformanceMetrics
{
    public double AverageResponseTime { get; set; }      // ms
    public double P95ResponseTime { get; set; }          // ms
    public double P99ResponseTime { get; set; }          // ms
    public int RequestsPerSecond { get; set; }           // RPS
    public double CpuUtilization { get; set; }           // %
    public double MemoryUtilization { get; set; }        // %
    public int ActiveConnections { get; set; }
    public int RateLimitExceededCount { get; set; }
    public double CacheHitRatio { get; set; }            // %
    public int DatabaseConnections { get; set; }
}
```

### Monitoring Implementation
```csharp
// Prometheus metrics
var requestCounter = new Counter("http_requests_total", 
    "Total HTTP requests");
var responseHistogram = new Histogram("http_request_duration_seconds",
    "HTTP request duration");

app.Use(async (context, next) =>
{
    using (responseHistogram.WithLabels(context.Request.Path).NewTimer())
    {
        requestCounter.Inc();
        await next();
    }
});
```

---

## 6. Cost Analysis

### Monthly Cost Estimation (AWS Example)

| Component | Tier | Monthly Cost | Notes |
|-----------|------|--------------|-------|
| **API Gateway** | 3x t3.medium | $90 | $30/instance |
| **Services** | 12x t3.medium | $360 | Mixed tier |
| **Redis** | r6g.xlarge | $250 | 16GB in-memory |
| **Load Balancer** | Network LB | $45 | Pay per LCU |
| **Data Transfer** | 1TB/month | $85 | Inter-region |
| ****Total** | | **$830** | Baseline production |

### Cost Optimization
- Use reserved instances for 30% savings
- Auto-scale down during off-peak hours
- Use spot instances for non-critical services
- Consolidate small services on shared tier

---

## 7. Deployment Strategy

### Rolling Deployment with Scaling
```bash
#!/bin/bash
# Deploy multiple instances with health check

SERVICES=("Action" "Audit" "Certificate")
INSTANCES=2

for service in "${SERVICES[@]}"; do
    for i in $(seq 1 $INSTANCES); do
        port=$((5000 + $i))
        echo "Starting $service instance $i on port $port"
        
        # Start instance with health check
        dotnet run --project Services/$service/${service}Service.csproj \
            --urls "http://localhost:$port" &
            
        # Wait for health check to pass
        sleep 2
        curl -f http://localhost:$port/health || exit 1
    done
done
```

---

## 8. Recommendations for Hadvida

### Immediate (Month 1)
- ✅ Implement rate limiting (COMPLETED)
- Implement service health checks
- Set up monitoring infrastructure
- Configure load balancer

### Short-term (Month 2-3)
- Deploy 2x instances for high-traffic services (Findings, Notification)
- Implement Redis scaling
- Set up automated backup strategy

### Medium-term (Month 4-6)
- Scale Finance/Audit to multi-instance
- Implement auto-scaling policies
- Upgrade to Tier 2 for core services

### Long-term (6+ months)
- Move to Tier 3 for critical services
- Implement cross-zone redundancy
- Disaster recovery setup
- Multi-region deployment

---

## 9. Troubleshooting Guide

### High CPU Usage
1. Profile application with JetBrains profiler
2. Check for memory leaks in application
3. Add more instances (horizontal) or upgrade CPU (vertical)
4. Review database query performance

### High Memory Usage
1. Configure heap size limits
2. Implement aggressive caching eviction
3. Check for memory leaks
4. Increase available RAM

### Rate Limit Exceeded
1. Review `RateLimitOptions` in ocelot.json
2. Check Client-Id header implementation
3. Consider whitelisting high-volume clients
4. Increase limits if business justified

### Connection Pool Exhaustion
```csharp
// Monitor and adjust
var poolSize = ConnectionPool.GetStatistics();
logger.LogWarning(
    "Active: {Active}, Idle: {Idle}, Total: {Total}",
    poolSize.Active, poolSize.Idle, poolSize.Total);
```

---

## 10. Summary

| Aspect | Horizontal | Vertical | Recommendation |
|--------|-----------|----------|-----------------|
| **Scalability** | Unlimited | Limited | Use both |
| **Complexity** | High | Low | Start vertical |
| **Cost** | Variable | Fixed to tier | Monitor both |
| **Redundancy** | Built-in | Required separately | Horizontal first |
| **For Hadvida** | High-traffic services | Baseline/critical | Hybrid approach |

---

**Document Version**: 1.0  
**Last Updated**: February 12, 2026  
**Status**: Ready for Implementation
