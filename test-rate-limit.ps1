#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Rate Limit Testing Script for Hadvida API Gateway

.DESCRIPTION
    Tests rate limiting implementation across all microservices endpoints.
    Sends multiple requests to verify rate limits are properly enforced.

.PARAMETER Endpoint
    API endpoint to test

.PARAMETER RequestCount
    Number of requests to send

.PARAMETER ClientId
    Unique client identifier for rate limiting

.PARAMETER Delay
    Delay between requests in milliseconds

.EXAMPLE
    .\test-rate-limit.ps1 -Endpoint "http://localhost:5011/api/action/list" -RequestCount 120
    .\test-rate-limit.ps1 -Endpoint "http://localhost:5011/findings/graphql" -RequestCount 100 -ClientId "test-app"
#>

param(
    [string]$Endpoint = "http://localhost:5011/api/action/list",
    [int]$RequestCount = 150,
    [string]$ClientId = "test-client-$(Get-Random)",
    [int]$Delay = 50,
    [string]$Token = ""
)

# Initialize result tracking
$results = @{
    success = 0
    rateLimited = 0
    errors = 0
    responses = @()
    rateLimitHeaders = @()
}

Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║           HADVIDA API GATEWAY - RATE LIMIT TEST            ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan

Write-Host "`nTest Configuration:" -ForegroundColor Yellow
Write-Host "  Endpoint........: $Endpoint"
Write-Host "  Request Count...: $RequestCount"
Write-Host "  Client ID.......: $ClientId"
Write-Host "  Delay (ms)......: $Delay"
Write-Host "  Timestamp.......: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"

Write-Host "`nStarting test..." -ForegroundColor Green

$testStartTime = Get-Date
$requestTimes = @()

for ($i = 1; $i -le $RequestCount; $i++) {
    try {
        $headers = @{
            "Client-Id" = $ClientId
            "Content-Type" = "application/json"
        }
        
        if ($Token) {
            $headers["Authorization"] = "Bearer $Token"
        }

        $requestStart = Get-Date
        $response = Invoke-WebRequest -Uri $Endpoint `
            -Method Get `
            -Headers $headers `
            -UseBasicParsing `
            -ErrorAction SilentlyContinue `
            -SkipHttpErrorCheck `
            -TimeoutSec 10

        $requestEnd = Get-Date
        $requestTime = ($requestEnd - $requestStart).TotalMilliseconds
        $requestTimes += $requestTime

        # Parse response
        if ($response.StatusCode -eq 200) {
            $results.success++
            $statusColor = "Green"
            $statusIcon = "✓"
        } elseif ($response.StatusCode -eq 429) {
            $results.rateLimited++
            $statusColor = "Red"
            $statusIcon = "✗"
            
            # Extract rate limit headers
            if ($response.Headers) {
                $rateLimitInfo = @{
                    Request = $i
                    Limit = $response.Headers['X-RateLimit-Limit']
                    Remaining = $response.Headers['X-RateLimit-Remaining']
                    Reset = $response.Headers['X-RateLimit-Reset']
                    RetryAfter = $response.Headers['Retry-After']
                    Timestamp = Get-Date
                }
                $results.rateLimitHeaders += $rateLimitInfo
            }
        } else {
            $results.errors++
            $statusColor = "Yellow"
            $statusIcon = "!"
        }

        $results.responses += @{
            Request = $i
            StatusCode = $response.StatusCode
            Time = $requestTime
            Timestamp = Get-Date
        }

        # Display progress every 10 requests
        if ($i % 10 -eq 0 -or $i -eq 1 -or $i -eq $RequestCount) {
            Write-Host "[$i/$RequestCount] [$statusIcon] Status: $($response.StatusCode) | Time: ${requestTime}ms" `
                -ForegroundColor $statusColor
        }

        # Delay between requests
        if ($i -lt $RequestCount) {
            Start-Sleep -Milliseconds $Delay
        }

    } catch {
        $results.errors++
        Write-Host "[$i/$RequestCount] [E] Error: $($_.Exception.Message)" -ForegroundColor DarkRed
    }
}

$testEndTime = Get-Date
$totalTime = ($testEndTime - $testStartTime).TotalSeconds

# Display results summary
Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                    TEST RESULTS SUMMARY                     ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan

Write-Host "`nExecution Statistics:" -ForegroundColor Yellow
Write-Host "  Total Requests.......: $RequestCount"
Write-Host "  Successful (200).....: $($results.success) ($([math]::Round(($results.success / $RequestCount * 100), 2))%)"
Write-Host "  Rate Limited (429)...: $($results.rateLimited) ($([math]::Round(($results.rateLimited / $RequestCount * 100), 2))%)"
Write-Host "  Errors...............: $($results.errors) ($([math]::Round(($results.errors / $RequestCount * 100), 2))%)"
Write-Host "  Total Duration.......: ${totalTime}s"
Write-Host "  Requests/Second......: $([math]::Round(($RequestCount / $totalTime), 2))"

if ($requestTimes.Count -gt 0) {
    $avgTime = [System.Linq.Enumerable]::Average([double[]]$requestTimes)
    $minTime = [System.Linq.Enumerable]::Min([double[]]$requestTimes)
    $maxTime = [System.Linq.Enumerable]::Max([double[]]$requestTimes)
    
    Write-Host "`nResponse Time Analysis:" -ForegroundColor Yellow
    Write-Host "  Average.............: ${avgTime}ms"
    Write-Host "  Minimum.............: ${minTime}ms"
    Write-Host "  Maximum.............: ${maxTime}ms"
}

# Show rate limit details if hit
if ($results.rateLimitHeaders.Count -gt 0) {
    Write-Host "`nRate Limit Headers (First 5 occurrences):" -ForegroundColor Yellow
    
    $results.rateLimitHeaders | Select-Object -First 5 | ForEach-Object {
        Write-Host "  Request $($_.Request):"
        Write-Host "    - Limit.....: $($_.Limit)"
        Write-Host "    - Remaining.: $($_.Remaining)"
        Write-Host "    - Retry-After: $($_.RetryAfter)"
    }
}

# Health assessment
Write-Host "`nHealth Assessment:" -ForegroundColor Yellow

if ($results.rateLimited -eq 0) {
    Write-Host "  ✓ No rate limiting triggered" -ForegroundColor Green
    if ($results.success -eq $RequestCount) {
        Write-Host "  ✓ All requests successful" -ForegroundColor Green
        Write-Host "  → Rate limit is higher than tested load" -ForegroundColor Cyan
    }
}
else {
    $rateLimitPercentage = ($results.rateLimited / $RequestCount * 100)
    
    if ($rateLimitPercentage -eq 100) {
        Write-Host "  ⚠ All requests rate limited" -ForegroundColor Red
        Write-Host "  → Consider increasing rate limit or reducing request frequency" -ForegroundColor DarkYellow
    }
    elseif ($rateLimitPercentage -gt 50) {
        Write-Host "  ⚠ More than 50% of requests rate limited" -ForegroundColor Yellow
        Write-Host "  → May need adjustment depending on workload" -ForegroundColor DarkYellow
    }
    else {
        Write-Host "  → $rateLimitPercentage% of requests hit rate limit" -ForegroundColor Cyan
        Write-Host "  → This is expected behavior" -ForegroundColor Green
    }
}

if ($results.errors -gt 0) {
    Write-Host "  ✗ Errors occurred: $($results.errors)" -ForegroundColor Red
}

# Export results to CSV
$exportPath = "rate-limit-test-$(Get-Date -Format 'yyyyMMdd-HHmmss').csv"
$results.responses | Export-Csv -Path $exportPath -NoTypeInformation

Write-Host "`n✓ Test Results exported to: $exportPath" -ForegroundColor Green
Write-Host "`n"
