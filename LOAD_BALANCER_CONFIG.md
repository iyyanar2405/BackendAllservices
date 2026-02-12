# Load Balancer Configuration Guide

## Overview
This guide provides ready-to-use configurations for deploying the Hadvida microservices platform with load balancing, rate limiting enforcement, and health monitoring.

---

## 1. NGINX Configuration

### Installation
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install nginx

# CentOS/RHEL
sudo yum install nginx

# macOS
brew install nginx
```

### Main Configuration File: `/etc/nginx/nginx.conf`

```nginx
# HADVIDA API GATEWAY - LOAD BALANCER CONFIGURATION

user nginx;
worker_processes auto;
error_log /var/log/nginx/error.log warn;
pid /var/run/nginx.pid;

events {
    worker_connections 4096;
    use epoll;
    multi_accept on;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    # Logging
    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                    '$status $body_bytes_sent "$http_referer" '
                    '"$http_user_agent" "$http_x_forwarded_for" '
                    'rt=$request_time uct="$upstream_connect_time" '
                    'uht="$upstream_header_time" urt="$upstream_response_time"';

    access_log /var/log/nginx/access.log main;

    # Performance
    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;
    types_hash_max_size 2048;
    client_max_body_size 100M;

    # Gzip Compression
    gzip on;
    gzip_vary on;
    gzip_min_length 1000;
    gzip_proxied any;
    gzip_types text/plain text/css text/xml text/javascript 
               application/x-javascript application/xml+rss 
               application/javascript application/json;

    # Rate Limiting (per IP)
    limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;
    limit_req_status 429;

    # API Gateway Upstream
    upstream api_gateway_cluster {
        least_conn;
        keepalive 32;

        server localhost:5011 max_fails=3 fail_timeout=30s weight=10;
        server localhost:5012 max_fails=3 fail_timeout=30s weight=10;
        server localhost:5013 max_fails=3 fail_timeout=30s weight=10;
    }

    # Findings Service Upstream (High Traffic)
    upstream findings_cluster {
        least_conn;
        server localhost:5000 max_fails=2 fail_timeout=20s;
        server localhost:5001 max_fails=2 fail_timeout=20s;
        server localhost:5002 max_fails=2 fail_timeout=20s;
    }

    # Action Service Upstream
    upstream action_cluster {
        least_conn;
        server localhost:5011 max_fails=2 fail_timeout=20s;
        server localhost:5012 max_fails=2 fail_timeout=20s;
    }

    # Health Check Configuration
    server {
        listen 8080;
        server_name localhost;
        location /nginx_status {
            stub_status on;
            access_log off;
            allow 127.0.0.1;
            deny all;
        }
    }

    # HTTP Server - Redirect to HTTPS
    server {
        listen 80;
        server_name api.hadvida.local;
        return 301 https://$server_name$request_uri;
    }

    # HTTPS Server - Main API Gateway
    server {
        listen 443 ssl http2;
        server_name api.hadvida.local;

        # SSL Configuration
        ssl_certificate /etc/ssl/certs/hadvida.crt;
        ssl_certificate_key /etc/ssl/private/hadvida.key;
        ssl_protocols TLSv1.2 TLSv1.3;
        ssl_ciphers HIGH:!aNULL:!MD5;
        ssl_prefer_server_ciphers on;
        ssl_session_cache shared:SSL:10m;
        ssl_session_timeout 10m;

        # Security Headers
        add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-Frame-Options "DENY" always;
        add_header X-XSS-Protection "1; mode=block" always;
        add_header Referrer-Policy "strict-origin-when-cross-origin" always;

        # Rate Limiting
        limit_req zone=api_limit burst=20 nodelay;

        # Health Check Endpoint
        location /health {
            access_log off;
            proxy_pass http://api_gateway_cluster;
            proxy_read_timeout 2s;
            proxy_connect_timeout 1s;

            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto https;
        }

        # API Routes to API Gateway Cluster
        location / {
            limit_req zone=api_limit burst=20 nodelay;

            proxy_pass http://api_gateway_cluster;
            proxy_http_version 1.1;
            
            # Connection Settings
            proxy_set_header Connection "";
            proxy_set_header Host $host;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection "upgrade";
            
            # Headers
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto https;
            proxy_set_header X-Request-ID $request_id;
            proxy_set_header Client-Id $http_client_id;

            # Timeouts
            proxy_connect_timeout 5s;
            proxy_send_timeout 60s;
            proxy_read_timeout 60s;
            
            # Buffering
            proxy_buffering on;
            proxy_buffer_size 4k;
            proxy_buffers 24 4k;
            proxy_busy_buffers_size 8k;

            # Error Handling
            proxy_next_upstream error timeout http_502 http_503 http_504;
            proxy_next_upstream_tries 2;
        }

        # Specific routes with custom rate limits
        location /api/findings/ {
            limit_req zone=api_limit burst=30 nodelay;
            proxy_pass http://findings_cluster;
            
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto https;
            proxy_set_header Client-Id $http_client_id;

            proxy_connect_timeout 5s;
            proxy_read_timeout 60s;
            proxy_send_timeout 60s;
        }

        # GraphQL endpoints (higher rate limits)
        location ~ ^/(.*)/graphql$ {
            limit_req zone=api_limit burst=25 nodelay;
            proxy_pass http://api_gateway_cluster;
            
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto https;
            
            proxy_connect_timeout 5s;
            proxy_read_timeout 90s;
            proxy_send_timeout 60s;
        }

        # Metrics Endpoint (Internal Only)
        location /nginx-metrics {
            access_log off;
            proxy_pass http://localhost:8080/nginx_status;
            allow 10.0.0.0/8;
            deny all;
        }

        # Error Pages
        error_page 502 503 504 /50x.html;
        location = /50x.html {
            root /usr/share/nginx/html;
        }
    }

    # Metrics/Status Server
    server {
        listen 9090;
        server_name localhost;

        location /metrics {
            access_log off;
            proxy_pass http://localhost:8080/nginx_status;
        }
    }
}
```

### Enable and Start NGINX
```bash
# Enable NGINX to start on boot
sudo systemctl enable nginx

# Start/Restart NGINX
sudo systemctl start nginx
sudo systemctl restart nginx

# Monitor logs
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log

# Test configuration
sudo nginx -t
```

---

## 2. HAProxy Configuration

### Installation
```bash
# Ubuntu/Debian
sudo apt-get install haproxy

# CentOS/RHEL
sudo yum install haproxy

# Verify
haproxy -v
```

### Configuration File: `/etc/haproxy/haproxy.cfg`

```haproxy
# HADVIDA MICROSERVICES - HAPROXY LOAD BALANCER

global
    log stdout local0
    log stdout local1 notice
    chroot /var/lib/haproxy
    stats socket /run/haproxy/admin.sock mode 660 level admin
    stats timeout 30s
    user haproxy
    group haproxy
    daemon

    # Performance
    maxconn 4096
    tune.ssl.default-dh-param 2048
    tune.maxconn 2000

defaults
    log     global
    mode    http
    option  httplog
    option  dontlognull
    option  forwardfor except 127.0.0.0/8
    option  redispatch
    option  http-server-close
    
    # Compression
    compression algo gzip
    compression type text/html text/plain text/xml text/css text/javascript application/json

    # Timeouts
    timeout connect 5000
    timeout client 60000
    timeout server 60000
    timeout http-request 60000
    timeout http-keep-alive 5000

# Stats Page
listen stats
    bind *:8404
    stats enable
    stats uri /stats
    stats refresh 30s
    stats show-legends
    stats auth admin:password

# Frontend - HTTP to HTTPS redirect
frontend http_in
    bind *:80
    redirect scheme https code 301 if !{ ssl_fc }

# Frontend - HTTPS with SSL
frontend https_in
    bind *:443 ssl crt /etc/ssl/private/hadvida.pem
    
    # Security Headers
    http-response set-header Strict-Transport-Security "max-age=31536000; includeSubDomains"
    http-response set-header X-Content-Type-Options "nosniff"
    http-response set-header X-Frame-Options "DENY"
    http-response set-header X-XSS-Protection "1; mode=block"

    # Rate Limiting - Stick Tables
    stick-table type ip size 100k expire 1m store http_req_rate(1m)
    http-request track-sc0 src
    http-request deny if { sc_http_req_rate(0) gt 100 }

    # Logging
    option httplog clf
    log global

    # ACLs based on path
    acl is_api_findings path_beg /api/findings
    acl is_graphql path_end /graphql
    acl is_health path /health

    # Health Check
    use_backend health if is_health

    # Route to specific backends
    use_backend findings_cluster if is_api_findings
    use_backend api_gateway if is_graphql
    default_backend api_gateway

# API Gateway Backend Cluster
backend api_gateway
    balance roundrobin
    option httpchk GET /health HTTP/1.1\r\nHost:\ localhost
    
    server ag1 127.0.0.1:5011 check inter 2000 rise 2 fall 3 weight 10
    server ag2 127.0.0.1:5012 check inter 2000 rise 2 fall 3 weight 10
    server ag3 127.0.0.1:5013 check inter 2000 rise 2 fall 3 weight 10

# Findings Service Cluster (High Traffic)
backend findings_cluster
    balance leastconn
    option httpchk GET /health HTTP/1.1\r\nHost:\ localhost
    timeout server 90000
    
    server findings1 127.0.0.1:5000 check inter 2000 rise 2 fall 3
    server findings2 127.0.0.1:5000 check inter 2000 rise 2 fall 3
    server findings3 127.0.0.1:5000 check inter 2000 rise 2 fall 3

# Action Service Cluster
backend action_cluster
    balance roundrobin
    option httpchk GET /health HTTP/1.1\r\nHost:\ localhost
    
    server action1 127.0.0.1:5001 check inter 2000 rise 2 fall 3
    server action2 127.0.0.1:5001 check inter 2000 rise 2 fall 3

# Health Check Backend
backend health
    option httpchk GET /health HTTP/1.1\r\nHost:\ localhost
    server check1 127.0.0.1:5000 check
    server check2 127.0.0.1:5001 check
    server check3 127.0.0.1:5002 check
```

### Enable and Start HAProxy
```bash
# Verify configuration
sudo haproxy -c -f /etc/haproxy/haproxy.cfg

# Enable on boot
sudo systemctl enable haproxy

# Start/Restart
sudo systemctl start haproxy
sudo systemctl restart haproxy

# Monitor
sudo journalctl -u haproxy -f
```

---

## 3. Windows Server IIS with URL Rewrite (Alternative)

### Configuration via ApplicationHost.config

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <system.webServer>
        <!-- Application Request Routing (ARR) -->
        <proxy>
            <enabled>true</enabled>
        </proxy>

        <!-- URL Rewrite Rules for Load Balancing -->
        <rewrite>
            <rules>
                <!-- API Gateway Routing -->
                <rule name="API Gateway Balancing" stopProcessing="true">
                    <match url="^(.*)$" />
                    <conditions>
                        <add input="{REQUEST_URI}" pattern="^/" />
                    </conditions>
                    <serverVariables>
                        <set name="HTTP_X_FORWARDED_FOR" value="{REMOTE_ADDR}" />
                        <set name="HTTP_X_FORWARDED_PROTO" value="https" />
                    </serverVariables>
                    <action type="Rewrite" url="http://LOCAL_SERVERS/{R:1}" />
                </rule>
            </rules>

            <!-- Server Farm Definition -->
            <outboundRules>
                <rule name="Rewrite Content-Length Header" stopProcessing="false">
                    <match serverVariable="RESPONSE_Content_Length" pattern=".+" />
                    <action type="Rewrite" value="0" />
                </rule>
            </outboundRules>
        </rewrite>

        <!-- Application Request Routing - Server Farm -->
        <webFarms>
            <webFarm name="ApiGatewayFarm" enabled="true">
                <server address="localhost" port="5011" enabled="true" />
                <server address="localhost" port="5012" enabled="true" />
                <server address="localhost" port="5013" enabled="true" />
                
                <healthCheck url="/health" interval="5" />
                <loadBalancing algorithm="LeastRequests" />
            </webFarm>
        </webFarms>
    </system.webServer>
</configuration>
```

---

## 4. Kubernetes Deployment (Enterprise)

### Kubernetes Service Definition

```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: hadvida

---
apiVersion: v1
kind: ConfigMap
metadata:
  name: api-gateway-config
  namespace: hadvida
data:
  ocelot.json: |
    {
      "Routes": [ ... ],
      "GlobalConfiguration": { ... }
    }

---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-gateway
  namespace: hadvida
spec:
  replicas: 3
  selector:
    matchLabels:
      app: api-gateway
  template:
    metadata:
      labels:
        app: api-gateway
    spec:
      containers:
      - name: api-gateway
        image: hadvida/api-gateway:latest
        ports:
        - containerPort: 5011
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: Production
        livenessProbe:
          httpGet:
            path: /health
            port: 5011
          initialDelaySeconds: 10
          periodSeconds: 5
        readinessProbe:
          httpGet:
            path: /health
            port: 5011
          initialDelaySeconds: 5
          periodSeconds: 5
        resources:
          requests:
            memory: "512Mi"
            cpu: "500m"
          limits:
            memory: "1Gi"
            cpu: "1000m"

---
apiVersion: v1
kind: Service
metadata:
  name: api-gateway-service
  namespace: hadvida
spec:
  type: LoadBalancer
  selector:
    app: api-gateway
  ports:
  - protocol: TCP
    port: 80
    targetPort: 5011

---
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: api-gateway-hpa
  namespace: hadvida
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: api-gateway
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

---

## 5. Monitoring & Alerts

### Prometheus Configuration

```yaml
# prometheus.yml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

scrape_configs:
  - job_name: 'nginx'
    static_configs:
      - targets: ['localhost:9090']

  - job_name: 'api_gateway'
    static_configs:
      - targets: 
        - 'localhost:5011'
        - 'localhost:5012'
        - 'localhost:5013'

  - job_name: 'services'
    static_configs:
      - targets:
        - 'localhost:5000'
        - 'localhost:5001'
        - 'localhost:5002'
        - 'localhost:5003'
        - 'localhost:5004'
        - 'localhost:5005'
        - 'localhost:5007'
        - 'localhost:5009'
        - 'localhost:5010'
```

### Alert Rules

```yaml
# alerts.yml
groups:
  - name: load_balancer_alerts
    rules:
      - alert: HighErrorRate
        expr: (rate(nginx_http_requests_total{status=~"5.."}[5m]) > 0.05)
        for: 5m
        annotations:
          summary: "High error rate detected"

      - alert: HighResponseTime
        expr: (nginx_request_duration_seconds > 1)
        for: 5m
        annotations:
          summary: "High response time detected"

      - alert: BackendDown
        expr: (up{job="api_gateway"} == 0)
        for: 1m
        annotations:
          summary: "Backend instance is down"
```

---

## 6. Deployment Commands

### Full Stack Startup Script

```powershell
# startup.ps1 - Start all services with load balancing

$ErrorActionPreference = "Stop"

Write-Host "Starting Hadvida Microservices Platform..." -ForegroundColor Green

# 1. Start Redis
Write-Host "`n[1/3] Starting Redis Cache..." -ForegroundColor Yellow
redis-server --daemonize yes

# 2. Start Core Services
$services = @{
    "Findings" = 5000
    "Action" = 5001
    "Audit" = 5002
}

Write-Host "`n[2/3] Starting Core Services..." -ForegroundColor Yellow
foreach ($service in $services.GetEnumerator()) {
    $name = $service.Name
    $port = $service.Value
    
    Write-Host "Starting $name on port $port..."
    
    Start-Job -Name $name -ScriptBlock {
        cd "d:\Dotnetfull_stack\BackEnd\src\Services\$using:name"
        $env:ASPNETCORE_URLS = "http://localhost:$using:port"
        dotnet run --configuration Release --no-build
    }
}

# 3. Start API Gateway Instances
Write-Host "`n[3/3] Starting API Gateway Instances..." -ForegroundColor Yellow
$gatewayPorts = @(5011, 5012, 5013)

foreach ($port in $gatewayPorts) {
    Write-Host "Starting API Gateway on port $port..."
    
    Start-Job -Name "Gateway-$port" -ScriptBlock {
        cd "d:\Dotnetfull_stack\BackEnd\src\ApiGateway"
        $env:ASPNETCORE_URLS = "http://localhost:$using:port"
        dotnet run --configuration Release --no-build
    }
}

# 4. Start Load Balancer (NGINX via WSL or native)
Write-Host "`n[4/4] Starting Load Balancer..." -ForegroundColor Yellow
# nginx.exe (if installed on Windows) or WSL nginx

Write-Host "`n✅ All services started!" -ForegroundColor Green
Write-Host "`nServices Status:" -ForegroundColor Cyan
Get-Job | Format-Table Name, State, PSBeginTime

Write-Host "`nAPI Gateway: http://localhost (via load balancer)" -ForegroundColor Cyan
Write-Host "Monitoring: http://localhost:8404 (HAProxy) or http://localhost:9090 (Prometheus)" -ForegroundColor Cyan
```

---

## 7. Verification Checklist

- [ ] Load balancer running and accepting connections
- [ ] Health checks passing for all backend instances
- [ ] Rate limiting active and enforcing limits
- [ ] SSL/TLS certificate valid
- [ ] All service instances responding to `/health` endpoint
- [ ] Metrics collection working
- [ ] Logs being generated correctly
- [ ] Failover working (stop one instance, verify traffic redirects)
- [ ] Security headers present in responses
- [ ] Compression working for text responses

---

**Configuration Version**: 1.0  
**Last Updated**: February 12, 2026  
**Status**: Ready for Deployment
