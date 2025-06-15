#!/bin/bash
# fix-devops.sh - Automated DevOps fixes for SledzSpecke
# Run this script on the VPS to implement critical DevOps improvements
# Usage: sudo bash fix-devops.sh

set -e

echo "🔧 SledzSpecke DevOps Fixer v1.0"
echo "================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Helper functions
log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

log_info() {
    echo -e "${YELLOW}ℹ️  $1${NC}"
}

# Check if running with sudo
if [ "$EUID" -ne 0 ]; then 
    log_error "Please run this script with sudo"
    exit 1
fi

# 1. Create and configure backup script
create_backup_script() {
    log_info "Setting up automatic database backups..."
    
    # Create backup script
    cat > /usr/local/bin/backup-sledzspecke.sh << 'EOF'
#!/bin/bash
set -e

# Configuration
BACKUP_DIR="/var/backups/postgresql"
DB_NAME="sledzspecke_db"
DB_USER="sledzspecke_user"
DB_PASSWORD="SledzSpecke123!"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
RETENTION_DAYS=7
LOG_FILE="/var/log/sledzspecke/backup.log"

# Create directories
mkdir -p "$BACKUP_DIR"
mkdir -p "$(dirname "$LOG_FILE")"

# Function to log
log_message() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

log_message "Starting backup of $DB_NAME"

# Set password
export PGPASSWORD="$DB_PASSWORD"

# Create backup
BACKUP_FILE="$BACKUP_DIR/sledzspecke_backup_${TIMESTAMP}.sql.gz"
if pg_dump -U "$DB_USER" -h localhost "$DB_NAME" | gzip > "$BACKUP_FILE"; then
    log_message "Backup created: $BACKUP_FILE ($(du -h "$BACKUP_FILE" | cut -f1))"
else
    log_message "ERROR: Backup failed!"
    exit 1
fi

# Clean old backups
find "$BACKUP_DIR" -name "sledzspecke_backup_*.sql.gz" -mtime +$RETENTION_DAYS -delete
log_message "Cleaned old backups"

# Count remaining backups
BACKUP_COUNT=$(find "$BACKUP_DIR" -name "sledzspecke_backup_*.sql.gz" -type f | wc -l)
log_message "Total backups: $BACKUP_COUNT"

unset PGPASSWORD
log_message "Backup completed successfully"
EOF

    chmod +x /usr/local/bin/backup-sledzspecke.sh
    
    # Create directories
    mkdir -p /var/backups/postgresql
    mkdir -p /var/log/sledzspecke
    chown postgres:postgres /var/backups/postgresql
    chown www-data:www-data /var/log/sledzspecke
    
    # Add to crontab
    (crontab -l 2>/dev/null | grep -v backup-sledzspecke || true; echo "0 2 * * * /usr/local/bin/backup-sledzspecke.sh >> /var/log/sledzspecke/backup-cron.log 2>&1") | crontab -
    
    # Test backup
    log_info "Testing backup script..."
    if /usr/local/bin/backup-sledzspecke.sh; then
        log_success "Backup script created and tested successfully"
    else
        log_error "Backup test failed!"
        return 1
    fi
}

# 2. Install and configure Seq
install_seq() {
    log_info "Installing Seq for centralized logging..."
    
    # Check if Docker is installed
    if ! command -v docker &> /dev/null; then
        log_info "Installing Docker..."
        curl -fsSL https://get.docker.com | sh
        usermod -aG docker ubuntu
        systemctl enable docker
        systemctl start docker
    fi
    
    # Check if Seq is already running
    if docker ps | grep -q seq; then
        log_info "Seq is already running"
    else
        # Run Seq container
        docker run -d \
            --name seq \
            --restart unless-stopped \
            -e ACCEPT_EULA=Y \
            -p 5341:80 \
            -v /opt/seq/data:/data \
            datalust/seq:latest
        
        log_success "Seq installed and running on port 5341"
        log_info "Access Seq at: http://$(hostname -I | awk '{print $1}'):5341"
    fi
}

# 3. Configure Nginx security headers and rate limiting
configure_nginx_security() {
    log_info "Configuring Nginx security..."
    
    # Backup current config
    cp /etc/nginx/sites-available/sledzspecke /etc/nginx/sites-available/sledzspecke.backup.$(date +%Y%m%d_%H%M%S)
    
    # Create rate limiting configuration
    cat > /etc/nginx/conf.d/rate-limiting.conf << 'EOF'
# Rate limiting zones
limit_req_zone $binary_remote_addr zone=api:10m rate=10r/s;
limit_req_zone $binary_remote_addr zone=login:10m rate=5r/m;
limit_req_zone $binary_remote_addr zone=register:10m rate=3r/m;
limit_req_status 429;

# Connection limiting
limit_conn_zone $binary_remote_addr zone=addr:10m;
limit_conn addr 100;
EOF

    # Update site configuration
    # Check if security headers already exist
    if ! grep -q "X-Frame-Options" /etc/nginx/sites-available/sledzspecke; then
        # Add security headers to location blocks
        sed -i '/location \/ {/a\
        # Security headers\
        add_header X-Frame-Options "SAMEORIGIN" always;\
        add_header X-Content-Type-Options "nosniff" always;\
        add_header X-XSS-Protection "1; mode=block" always;\
        add_header Referrer-Policy "strict-origin-when-cross-origin" always;\
        add_header Permissions-Policy "geolocation=(), microphone=(), camera=()" always;\
        add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;' \
        /etc/nginx/sites-available/sledzspecke
    fi
    
    # Add rate limiting to API location if not present
    if ! grep -q "limit_req zone=api" /etc/nginx/sites-available/sledzspecke; then
        sed -i '/location \/api {/a\
        limit_req zone=api burst=20 nodelay;' \
        /etc/nginx/sites-available/sledzspecke
    fi
    
    # Test and reload nginx
    if nginx -t; then
        nginx -s reload
        log_success "Nginx security headers and rate limiting configured"
    else
        log_error "Nginx configuration test failed!"
        return 1
    fi
}

# 4. Create health check script
create_health_check() {
    log_info "Creating health check script..."
    
    cat > /usr/local/bin/health-check-sledzspecke.sh << 'EOF'
#!/bin/bash
# Health check for SledzSpecke API

API_URL="http://localhost:5000/monitoring/health"
LOG_FILE="/var/log/sledzspecke/health-check.log"
MAX_RETRIES=3
RETRY_DELAY=10

# Function to log
log_message() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Check API health
check_api() {
    if curl -f -s "$API_URL" > /dev/null 2>&1; then
        return 0
    else
        return 1
    fi
}

# Main check
if ! check_api; then
    log_message "API health check failed, attempting restart..."
    systemctl restart sledzspecke-api
    
    # Wait and retry
    for i in $(seq 1 $MAX_RETRIES); do
        sleep $RETRY_DELAY
        if check_api; then
            log_message "API recovered after restart (attempt $i)"
            exit 0
        fi
    done
    
    log_message "ERROR: API still down after $MAX_RETRIES restart attempts!"
    # Here you could add notification (email/Slack)
    exit 1
else
    log_message "API health check passed"
fi
EOF

    chmod +x /usr/local/bin/health-check-sledzspecke.sh
    
    # Add to crontab (every 5 minutes)
    (crontab -l 2>/dev/null | grep -v health-check-sledzspecke || true; echo "*/5 * * * * /usr/local/bin/health-check-sledzspecke.sh > /dev/null 2>&1") | crontab -
    
    log_success "Health check script created"
}

# 5. Setup log rotation
setup_log_rotation() {
    log_info "Setting up log rotation..."
    
    cat > /etc/logrotate.d/sledzspecke << 'EOF'
/var/log/sledzspecke/*.log /var/log/sledzspecke/*.json {
    daily
    rotate 7
    compress
    delaycompress
    missingok
    notifempty
    create 0644 www-data www-data
    sharedscripts
    postrotate
        systemctl reload sledzspecke-api > /dev/null 2>&1 || true
    endscript
}
EOF

    log_success "Log rotation configured"
}

# 6. Install monitoring tools
install_monitoring_tools() {
    log_info "Installing monitoring tools..."
    
    # Update package list quietly
    apt-get update -qq
    
    # Install monitoring tools
    apt-get install -y -qq htop iotop nethogs ncdu
    
    # Create monitoring dashboard script
    cat > /usr/local/bin/sledzspecke-monitor << 'EOF'
#!/bin/bash
clear
echo "============================="
echo "SledzSpecke System Monitor"
echo "============================="
echo ""

# API Status
echo "📊 API Status:"
systemctl status sledzspecke-api --no-pager | head -5
echo ""

# Recent errors
echo "🚨 Recent Errors (last 10):"
grep -E "ERROR|CRITICAL" /var/log/sledzspecke/api-$(date +%Y-%m-%d).log 2>/dev/null | tail -10 || echo "No errors found today"
echo ""

# System resources
echo "💾 Disk Usage:"
df -h | grep -E "^/|Filesystem"
echo ""

echo "🧠 Memory Usage:"
free -h
echo ""

# Database info
echo "🗄️  Database Connections:"
sudo -u postgres psql -t -c "SELECT count(*) as connections FROM pg_stat_activity WHERE datname = 'sledzspecke_db';" sledzspecke_db 2>/dev/null || echo "Could not query database"
echo ""

# Last backup
echo "💿 Last Backup:"
ls -lht /var/backups/postgresql/sledzspecke_backup_*.sql.gz 2>/dev/null | head -1 || echo "No backups found"
echo ""

# Docker containers (if any)
if command -v docker &> /dev/null; then
    echo "🐳 Docker Containers:"
    docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | grep -E "sledzspecke|seq|NAME"
fi
EOF

    chmod +x /usr/local/bin/sledzspecke-monitor
    log_success "Monitoring tools installed"
}

# 7. Create migration helper script
create_migration_script() {
    log_info "Creating migration helper script..."
    
    cat > /var/www/sledzspecke-api/migrate.sh << 'EOF'
#!/bin/bash
# Database migration script for SledzSpecke

export ConnectionStrings__DefaultConnection="Host=localhost;Database=sledzspecke_db;Username=sledzspecke_user;Password=SledzSpecke123!"
cd /var/www/sledzspecke-api
dotnet SledzSpecke.Api.dll migrate-database
EOF

    chmod +x /var/www/sledzspecke-api/migrate.sh
    chown www-data:www-data /var/www/sledzspecke-api/migrate.sh
    
    log_success "Migration script created at /var/www/sledzspecke-api/migrate.sh"
}

# 8. Setup firewall
setup_firewall() {
    log_info "Configuring firewall..."
    
    # Install ufw if not present
    if ! command -v ufw &> /dev/null; then
        apt-get install -y -qq ufw
    fi
    
    # Configure firewall rules
    ufw --force enable
    ufw default deny incoming
    ufw default allow outgoing
    ufw allow ssh
    ufw allow 80/tcp
    ufw allow 443/tcp
    ufw allow 5341/tcp  # Seq
    
    log_success "Firewall configured"
}

# 9. Create quick deployment script
create_deployment_script() {
    log_info "Creating quick deployment script..."
    
    cat > /usr/local/bin/deploy-sledzspecke.sh << 'EOF'
#!/bin/bash
# Quick deployment script for SledzSpecke

echo "🚀 Deploying SledzSpecke..."

# Pull latest code
cd /home/ubuntu/sledzspecke
git pull origin master

# Build and deploy API
cd SledzSpecke.WebApi
dotnet publish -c Release -o /tmp/sledzspecke-api-new src/SledzSpecke.Api/SledzSpecke.Api.csproj

# Backup current version
cp -r /var/www/sledzspecke-api /var/www/sledzspecke-api-backup-$(date +%Y%m%d-%H%M%S)

# Stop service
systemctl stop sledzspecke-api

# Deploy new version
rm -rf /var/www/sledzspecke-api/*
cp -r /tmp/sledzspecke-api-new/* /var/www/sledzspecke-api/
chown -R www-data:www-data /var/www/sledzspecke-api

# Run migrations
sudo -u www-data /var/www/sledzspecke-api/migrate.sh

# Start service
systemctl start sledzspecke-api

# Build and deploy frontend
cd /home/ubuntu/sledzspecke/SledzSpecke-Frontend
npm ci
VITE_API_URL=https://api.sledzspecke.pl npm run build

# Deploy frontend
rm -rf /var/www/sledzspecke-web/dist/*
cp -r packages/web/dist/* /var/www/sledzspecke-web/dist/
chown -R www-data:www-data /var/www/sledzspecke-web

# Reload nginx
nginx -s reload

echo "✅ Deployment complete!"
EOF

    chmod +x /usr/local/bin/deploy-sledzspecke.sh
    log_success "Deployment script created"
}

# Main execution
main() {
    echo ""
    log_info "Starting DevOps improvements..."
    echo ""
    
    # Run all fixes
    create_backup_script
    echo ""
    
    install_seq
    echo ""
    
    configure_nginx_security
    echo ""
    
    create_health_check
    echo ""
    
    setup_log_rotation
    echo ""
    
    install_monitoring_tools
    echo ""
    
    create_migration_script
    echo ""
    
    setup_firewall
    echo ""
    
    create_deployment_script
    echo ""
    
    # Summary
    echo "================================"
    log_success "All DevOps fixes applied successfully!"
    echo ""
    echo "📋 Summary of changes:"
    echo "✅ Automatic database backups configured (daily at 2 AM)"
    echo "✅ Seq logging installed (http://$(hostname -I | awk '{print $1}'):5341)"
    echo "✅ Nginx security headers and rate limiting configured"
    echo "✅ Health checks running every 5 minutes"
    echo "✅ Log rotation configured"
    echo "✅ Monitoring tools installed"
    echo "✅ Migration helper script created"
    echo "✅ Firewall configured"
    echo "✅ Quick deployment script created"
    echo ""
    echo "🛠️  Useful commands:"
    echo "- View system monitor: sledzspecke-monitor"
    echo "- Run backup manually: /usr/local/bin/backup-sledzspecke.sh"
    echo "- Deploy updates: deploy-sledzspecke.sh"
    echo "- View logs: tail -f /var/log/sledzspecke/api-$(date +%Y-%m-%d).log"
    echo "- Check backups: ls -lh /var/backups/postgresql/"
    echo ""
    echo "⚠️  Important reminders:"
    echo "1. Update GitHub Actions workflow to use: sudo -u www-data /var/www/sledzspecke-api/migrate.sh"
    echo "2. Change default passwords in production"
    echo "3. Configure Seq authentication"
    echo "4. Set up external monitoring (UptimeRobot, Pingdom, etc.)"
    echo "5. Test the backup restore process"
    echo ""
}

# Run main function
main