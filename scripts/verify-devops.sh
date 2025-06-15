#!/bin/bash
# verify-devops.sh - Verify all DevOps components are working

echo "🔍 SledzSpecke DevOps Verification"
echo "=================================="

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Check function
check() {
    if eval "$2"; then
        echo -e "${GREEN}✅ $1${NC}"
        return 0
    else
        echo -e "${RED}❌ $1${NC}"
        return 1
    fi
}

echo ""
echo "1. Checking Services..."
check "API Service" "systemctl is-active --quiet sledzspecke-api"
check "PostgreSQL" "systemctl is-active --quiet postgresql"
check "Nginx" "systemctl is-active --quiet nginx"
check "Docker" "systemctl is-active --quiet docker"

echo ""
echo "2. Checking Endpoints..."
check "API Health" "curl -sf http://localhost:5000/monitoring/health > /dev/null"
check "Frontend" "curl -sf http://localhost > /dev/null"
check "API HTTPS" "curl -sf https://api.sledzspecke.pl/monitoring/health > /dev/null"
check "Frontend HTTPS" "curl -sf https://sledzspecke.pl > /dev/null"

echo ""
echo "3. Checking Monitoring..."
check "Seq Running" "docker ps | grep -q seq"
check "Seq Accessible" "curl -sf http://localhost:5341 > /dev/null"

echo ""
echo "4. Checking Backups..."
check "Backup Script Exists" "test -f /usr/local/bin/backup-sledzspecke.sh"
check "Backup Directory" "test -d /var/backups/postgresql"
check "Backup Cron Job" "crontab -l | grep -q backup-sledzspecke"

echo ""
echo "5. Checking Security..."
check "UFW Active" "ufw status | grep -q 'Status: active'"
check "Fail2ban Active" "systemctl is-active --quiet fail2ban"
check "SSL Certificate" "test -d /etc/letsencrypt/live/sledzspecke.pl"

echo ""
echo "6. Checking Logs..."
check "API Log Directory" "test -d /var/log/sledzspecke"
if [ -f "/var/log/sledzspecke/api-$(date +%Y-%m-%d).log" ]; then
    echo -e "${GREEN}✅ Today's API logs exist${NC}"
    echo "   Recent entries:"
    tail -5 /var/log/sledzspecke/api-$(date +%Y-%m-%d).log | sed 's/^/   /'
else
    echo -e "${YELLOW}⚠️  No logs for today yet${NC}"
fi

echo ""
echo "7. Checking Database..."
DB_CONN=$(sudo -u postgres psql -t -c "SELECT count(*) FROM pg_stat_activity WHERE datname = 'sledzspecke_db';" sledzspecke_db 2>/dev/null | tr -d ' ')
if [ -n "$DB_CONN" ]; then
    echo -e "${GREEN}✅ Database connections: $DB_CONN${NC}"
else
    echo -e "${RED}❌ Cannot query database${NC}"
fi

echo ""
echo "8. System Resources..."
echo "Disk Usage:"
df -h / | grep -v Filesystem | awk '{print "   " $0}'
echo ""
echo "Memory Usage:"
free -h | grep Mem | awk '{print "   Total: " $2 ", Used: " $3 ", Free: " $4}'

echo ""
echo "=================================="
echo "Verification complete!"
echo ""
echo "Quick commands:"
echo "  View API logs: tail -f /var/log/sledzspecke/api-$(date +%Y-%m-%d).log"
echo "  View Seq: http://$(hostname -I | awk '{print $1}'):5341"
echo "  System monitor: sledzspecke-monitor"
echo "  Manual backup: sudo /usr/local/bin/backup-sledzspecke.sh"