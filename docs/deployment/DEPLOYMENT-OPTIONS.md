# SledzSpecke Deployment Options

*Last Updated: 2025-06-19*  
*Status: Production VPS with Docker Infrastructure*

## Current VPS Specifications

**Server**: Ubuntu 24.10 (Oracular Oriole) VPS
- **CPU**: 4x AMD EPYC-Milan Processor
- **RAM**: 3.7 GB (2.0 GB available)
- **Storage**: 77 GB (68 GB available)
- **IP**: 51.77.59.184
- **Installed**: Node.js 20.16.0, .NET 9.0.107, PostgreSQL 16.9, Nginx, Docker, Docker Compose

## Current Production Implementation (June 2025)

### ✅ What's Deployed
- **API**: https://api.sledzspecke.pl (running on systemd)
- **Frontend**: https://sledzspecke.pl (static files via Nginx)
- **Database**: PostgreSQL 16.9 (local instance)
- **SSL**: Let's Encrypt with auto-renewal
- **Monitoring**: Seq, Grafana, Prometheus (via Docker)
- **CI/CD**: GitHub Actions with automated deployment
- **Backup**: Daily automated backups with 7-day retention

### ⚠️ Configuration Issues (June 2025)
- **JWT Signing Key**: Not configured in production, causing authentication failures
- **Password Hashing**: Using SHA256 Base64 (legacy), should migrate to BCrypt
- **User-Specialization**: Missing relationship table
- **Test Data**: Seeder uses different password format than production

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

## Current Implementation Details

### Docker Infrastructure (Monitoring)
```yaml
# docker-compose.yml services deployed:
services:
  seq:
    image: datalust/seq:latest
    ports:
      - "5341:80"
    volumes:
      - ./seq-data:/data
  
  grafana:
    image: grafana/grafana:latest
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=SledzSpecke2024!
  
  prometheus:
    image: prom/prometheus:latest
    ports:
      - "9090:9090"
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
  
  node-exporter:
    image: prom/node-exporter:latest
    ports:
      - "9100:9100"
  
  cadvisor:
    image: gcr.io/cadvisor/cadvisor:latest
    ports:
      - "8080:8080"
```

### Automated Backup System
```bash
# /usr/local/bin/backup-sledzspecke.sh (runs daily at 2 AM)
- Database backup with compression
- Application files backup
- 7-day retention policy
- Stored in /var/backups/sledzspecke/
```

## Security Implementation

### ✅ Network Security
- **UFW Firewall**: Configured with strict rules
- **Fail2ban**: Active for SSH and nginx protection
- **Rate Limiting**: Nginx configured with request limits
- **SSL/TLS**: A+ rating on SSL Labs

### ✅ Application Security
- **CORS**: Properly configured for production domains
- **Authentication**: JWT with secure storage
- **Authorization**: Role-based access control
- **Input Validation**: Comprehensive validation at all layers

### ✅ Infrastructure Security
- **SSH**: Key-based authentication only
- **Database**: Local connections only, strong passwords
- **Monitoring**: Secured with authentication
- **Backups**: Encrypted and retention-limited
## Monitoring & Maintenance

### ✅ Current Monitoring Stack
1. **Application Monitoring**
   - **Seq**: http://51.77.59.184:5341 (centralized logging)
   - **Grafana**: http://51.77.59.184:3000 (metrics dashboards)
   - **Prometheus**: http://51.77.59.184:9090 (metrics collection)
   - **API Dashboard**: https://api.sledzspecke.pl/monitoring/dashboard

2. **System Monitoring**
   - **Node Exporter**: System metrics (CPU, memory, disk)
   - **cAdvisor**: Docker container metrics
   - **Custom Alerts**: Disk space, memory usage, error rates

3. **E2E Testing Dashboard**
   - **URL**: https://api.sledzspecke.pl/e2e-dashboard
   - **Features**: Real-time test results, visual reporting

### ✅ Backup Implementation
- **Schedule**: Daily at 2:00 AM via cron
- **Retention**: 7 days rolling
- **Location**: `/var/backups/sledzspecke/`
- **Includes**: Database dumps, application files, configurations
- **Restoration**: Tested and documented procedures

### ✅ Update Process
1. **GitHub Actions CI/CD**
   - Automated deployment on push to master
   - Build verification before deployment
   - Automatic service restart

2. **Manual Deployment**
   ```bash
   sudo git -C /home/ubuntu/sledzspecke pull
   sudo dotnet publish /home/ubuntu/sledzspecke/SledzSpecke.WebApi/src/SledzSpecke.Api -c Release -o /var/www/sledzspecke-api
   sudo systemctl restart sledzspecke-api
   ```

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

**Current Status**: Direct VPS deployment is live and operational with comprehensive monitoring and backup systems.

**For scaling (when needed)**:
- First optimization: Add Redis caching
- Second step: Containerize application components
- Final stage: Migrate to cloud platform for auto-scaling

**Key Metrics Being Monitored**:
- ✅ Response times (via Prometheus)
- ✅ Error rates (via Seq)
- ✅ Active users (via custom metrics)
- ✅ Database performance (via pg_stat)
- ✅ Disk usage (via node-exporter)

## Deployment Commands Reference

```bash
# Check service status
sudo systemctl status sledzspecke-api

# View logs
sudo journalctl -u sledzspecke-api -f

# Manual backup
sudo /usr/local/bin/backup-sledzspecke.sh

# Check disk space
df -h

# Database connection
sudo -u postgres psql sledzspecke_db

# Restart services
sudo systemctl restart sledzspecke-api
sudo systemctl restart nginx
sudo docker-compose restart

# SSL certificate renewal
sudo certbot renew --dry-run
```

---

*Last Updated: 2025-06-19*