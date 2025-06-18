# SledzSpecke Deployment Options

## Current VPS Specifications

**Server**: Ubuntu 24.10 (Oracular Oriole) VPS
- **CPU**: 4x AMD EPYC-Milan Processor
- **RAM**: 3.7 GB (2.0 GB available)
- **Storage**: 77 GB (68 GB available)
- **IP**: 51.77.59.184
- **Installed**: Node.js 20.16.0, .NET 9.0.107, PostgreSQL 16.9, Nginx

## Deployment Architecture Options

### Option 1: Direct VPS Deployment (Recommended for MVP)
**Best for**: Quick deployment, cost-effective, simple management

```
┌─────────────────────────────────────────┐
│           VPS (51.77.59.184)            │
├─────────────────────────────────────────┤
│  Nginx (Port 80/443)                    │
│    ├── / → React App (static files)     │
│    └── /api → .NET API (port 5000)     │
├─────────────────────────────────────────┤
│  PostgreSQL (localhost:5432)            │
│  .NET API Service (systemd)             │
│  Static Files (/var/www/sledzspecke)   │
└─────────────────────────────────────────┘
```

**Pros**:
- Simple setup and maintenance
- Low cost (single VPS)
- Good for 100-1000 concurrent users
- Easy SSL with Let's Encrypt

**Cons**:
- Limited scalability
- Single point of failure
- Manual updates required

**Setup Steps**:
1. Configure Nginx as reverse proxy
2. Set up systemd service for .NET API
3. Build and deploy React static files
4. Configure SSL with certbot
5. Set up PostgreSQL backups

### Option 2: Docker Containerized Deployment
**Best for**: Consistent environments, easier updates

```
┌─────────────────────────────────────────┐
│           VPS (Docker Host)             │
├─────────────────────────────────────────┤
│  Docker Compose                         │
│    ├── nginx:alpine                    │
│    ├── sledzspecke-api:latest         │
│    ├── sledzspecke-web:latest         │
│    └── postgres:16-alpine              │
└─────────────────────────────────────────┘
```

**Pros**:
- Consistent dev/prod environment
- Easy rollbacks
- Better resource isolation
- Simplified deployment with docker-compose

**Cons**:
- Slight performance overhead
- Requires Docker knowledge
- Still single server limitation

### Option 3: Cloud Platform Deployment
**Best for**: Scalability, reliability, managed services

#### 3a. AWS Architecture
```
┌─────────────────────────────────────────┐
│  CloudFront CDN                         │
│    └── S3 (React Static Files)         │
├─────────────────────────────────────────┤
│  Application Load Balancer              │
│    └── ECS Fargate (.NET API)         │
├─────────────────────────────────────────┤
│  RDS PostgreSQL (Multi-AZ)             │
└─────────────────────────────────────────┘
```

#### 3b. Azure Architecture
```
┌─────────────────────────────────────────┐
│  Azure CDN                              │
│    └── Storage Account (React)         │
├─────────────────────────────────────────┤
│  Azure App Service                      │
│    └── .NET API                        │
├─────────────────────────────────────────┤
│  Azure Database for PostgreSQL          │
└─────────────────────────────────────────┘
```

**Pros**:
- Auto-scaling capabilities
- High availability
- Managed database with backups
- Global CDN for frontend

**Cons**:
- Higher cost ($50-200/month)
- More complex setup
- Vendor lock-in

### Option 4: Kubernetes Deployment
**Best for**: Maximum scalability, microservices future

```
┌─────────────────────────────────────────┐
│  Kubernetes Cluster (k3s/k8s)           │
├─────────────────────────────────────────┤
│  Ingress Controller (nginx)             │
├─────────────────────────────────────────┤
│  Deployments:                           │
│    ├── sledzspecke-api (3 replicas)    │
│    ├── sledzspecke-web (2 replicas)    │
│    └── postgres (StatefulSet)          │
└─────────────────────────────────────────┘
```

**Pros**:
- Horizontal scaling
- Self-healing
- Rolling updates
- Future microservices ready

**Cons**:
- Complex setup and maintenance
- Overkill for current needs
- Requires Kubernetes expertise

## Recommended Approach

### Phase 1: MVP Launch (Current VPS)
1. **Deploy directly on current VPS**
   - Use Nginx reverse proxy
   - systemd for API service management
   - PostgreSQL with daily backups
   - Let's Encrypt SSL

2. **Monitoring Setup**
   - Prometheus + Grafana for metrics
   - Sentry for error tracking
   - ELK stack for logs (optional)

### Phase 2: Growth (3-6 months)
1. **Dockerize the application**
   - Create Dockerfile for API
   - Create Dockerfile for frontend
   - Use docker-compose for orchestration

2. **Add redundancy**
   - Database replication
   - Regular automated backups
   - CDN for static assets (Cloudflare)

### Phase 3: Scale (6-12 months)
1. **Move to cloud platform**
   - Migrate to AWS/Azure/GCP
   - Use managed database service
   - Implement auto-scaling
   - Add monitoring and alerting

## Deployment Scripts

### 1. Nginx Configuration
```nginx
server {
    listen 80;
    server_name sledzspecke.com www.sledzspecke.com;
    
    # Redirect to HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name sledzspecke.com www.sledzspecke.com;
    
    ssl_certificate /etc/letsencrypt/live/sledzspecke.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/sledzspecke.com/privkey.pem;
    
    # Frontend
    location / {
        root /var/www/sledzspecke;
        try_files $uri $uri/ /index.html;
    }
    
    # API
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 2. Systemd Service
```ini
[Unit]
Description=SledzSpecke API
After=network.target postgresql.service

[Service]
Type=notify
WorkingDirectory=/opt/sledzspecke/api
ExecStart=/usr/bin/dotnet /opt/sledzspecke/api/SledzSpecke.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=sledzspecke-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

### 3. Deployment Script
```bash
#!/bin/bash
# deploy.sh - Deployment script for SledzSpecke

set -e

echo "🚀 Deploying SledzSpecke..."

# Build frontend
cd SledzSpecke-Frontend/packages/web
npm ci
npm run build

# Deploy frontend
sudo rm -rf /var/www/sledzspecke/*
sudo cp -r dist/* /var/www/sledzspecke/

# Build API
cd ../../../SledzSpecke.WebApi
dotnet publish -c Release -o /tmp/sledzspecke-api

# Stop service
sudo systemctl stop sledzspecke-api

# Deploy API
sudo rm -rf /opt/sledzspecke/api/*
sudo cp -r /tmp/sledzspecke-api/* /opt/sledzspecke/api/

# Run migrations
cd /opt/sledzspecke/api
sudo -u www-data dotnet SledzSpecke.Api.dll --migrate

# Start service
sudo systemctl start sledzspecke-api

# Reload nginx
sudo nginx -s reload

echo "✅ Deployment complete!"
```

## Security Considerations

1. **Network Security**
   - Configure firewall (ufw/iptables)
   - Use fail2ban for SSH protection
   - Implement rate limiting in Nginx

2. **Application Security**
   - Enable CORS properly
   - Use HTTPS everywhere
   - Implement API rate limiting
   - Regular security updates

3. **Database Security**
   - Use strong passwords
   - Enable SSL for connections
   - Regular backups to external storage
   - Restrict network access

## Monitoring & Maintenance

1. **Health Checks**
   - API health endpoint
   - Database connection monitoring
   - Disk space alerts
   - Memory usage tracking

2. **Backup Strategy**
   - Daily PostgreSQL backups
   - Weekly full system backups
   - Test restore procedures
   - Off-site backup storage

3. **Update Process**
   - Blue-green deployment
   - Database migration testing
   - Rollback procedures
   - Downtime notifications

## Cost Estimates

### Current VPS Only
- **VPS**: €10-20/month
- **Domain**: €10-15/year
- **SSL**: Free (Let's Encrypt)
- **Total**: ~€15-25/month

### Cloud Platform (AWS/Azure)
- **Compute**: €30-50/month
- **Database**: €20-40/month
- **Storage/CDN**: €5-10/month
- **Total**: ~€55-100/month

### Recommendations Summary

**For immediate deployment**: Use Option 1 (Direct VPS) with the provided Nginx and systemd configurations. This is the most cost-effective and straightforward approach.

**For future growth**: Plan migration to Option 2 (Docker) or Option 3 (Cloud) based on user growth and requirements.

**Key metrics to monitor**:
- Response times
- Error rates
- Active users
- Database performance
- Disk usage