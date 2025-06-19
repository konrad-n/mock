#!/bin/bash

# SledzSpecke Health Monitoring Script
# Comprehensive health checks for production

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

API_URL="https://api.sledzspecke.pl"
FRONTEND_URL="https://sledzspecke.pl"
LOG_FILE="/var/log/sledzspecke/health-monitor.log"

# Function to log with timestamp
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Function to check endpoint
check_endpoint() {
    local url=$1
    local expected_status=$2
    local description=$3
    
    response=$(curl -s -o /dev/null -w "%{http_code}" "$url" || echo "000")
    
    if [ "$response" -eq "$expected_status" ]; then
        echo -e "${GREEN}✓${NC} $description: OK (HTTP $response)"
        log "✓ $description: OK (HTTP $response)"
        return 0
    else
        echo -e "${RED}✗${NC} $description: FAIL (HTTP $response, expected $expected_status)"
        log "✗ $description: FAIL (HTTP $response, expected $expected_status)"
        return 1
    fi
}

# Function to check CORS
check_cors() {
    local origin=$1
    local should_allow=$2
    
    headers=$(curl -s -H "Origin: $origin" -I "$API_URL/api/health" 2>/dev/null)
    
    if echo "$headers" | grep -q "Access-Control-Allow-Origin"; then
        if [ "$should_allow" = "true" ]; then
            echo -e "${GREEN}✓${NC} CORS for $origin: Allowed (as expected)"
            log "✓ CORS for $origin: Allowed (as expected)"
        else
            echo -e "${RED}✗${NC} CORS for $origin: Allowed (should be blocked)"
            log "✗ CORS for $origin: Allowed (should be blocked)"
            return 1
        fi
    else
        if [ "$should_allow" = "false" ]; then
            echo -e "${GREEN}✓${NC} CORS for $origin: Blocked (as expected)"
            log "✓ CORS for $origin: Blocked (as expected)"
        else
            echo -e "${RED}✗${NC} CORS for $origin: Blocked (should be allowed)"
            log "✗ CORS for $origin: Blocked (should be allowed)"
            return 1
        fi
    fi
}

# Function to check security headers
check_security_headers() {
    headers=$(curl -s -I "$API_URL/api/health" 2>/dev/null)
    
    security_headers=(
        "X-Content-Type-Options: nosniff"
        "X-Frame-Options: DENY"
        "Strict-Transport-Security"
        "X-XSS-Protection"
        "Referrer-Policy"
    )
    
    all_good=true
    for header in "${security_headers[@]}"; do
        if echo "$headers" | grep -q "$header"; then
            echo -e "${GREEN}✓${NC} Security header present: $header"
        else
            echo -e "${RED}✗${NC} Security header missing: $header"
            log "✗ Security header missing: $header"
            all_good=false
        fi
    done
    
    if [ "$all_good" = true ]; then
        log "✓ All security headers present"
    fi
}

# Function to check API response time
check_response_time() {
    local start_time=$(date +%s%N)
    curl -s "$API_URL/api/health" > /dev/null
    local end_time=$(date +%s%N)
    
    local response_time=$(( ($end_time - $start_time) / 1000000 ))
    
    if [ $response_time -lt 500 ]; then
        echo -e "${GREEN}✓${NC} API response time: ${response_time}ms (excellent)"
        log "✓ API response time: ${response_time}ms"
    elif [ $response_time -lt 1000 ]; then
        echo -e "${YELLOW}⚠${NC} API response time: ${response_time}ms (acceptable)"
        log "⚠ API response time: ${response_time}ms"
    else
        echo -e "${RED}✗${NC} API response time: ${response_time}ms (too slow)"
        log "✗ API response time: ${response_time}ms (too slow)"
    fi
}

# Function to check database connectivity through health endpoint
check_database() {
    health_response=$(curl -s "$API_URL/api/health")
    
    if echo "$health_response" | grep -q '"database": "connected"'; then
        echo -e "${GREEN}✓${NC} Database connectivity: OK"
        log "✓ Database connectivity: OK"
    else
        echo -e "${RED}✗${NC} Database connectivity: FAIL"
        log "✗ Database connectivity: FAIL"
        return 1
    fi
}

# Function to check system resources
check_system_resources() {
    # Check disk space
    disk_usage=$(df -h / | awk 'NR==2 {print $5}' | sed 's/%//')
    if [ $disk_usage -lt 80 ]; then
        echo -e "${GREEN}✓${NC} Disk usage: ${disk_usage}% (OK)"
        log "✓ Disk usage: ${disk_usage}%"
    elif [ $disk_usage -lt 90 ]; then
        echo -e "${YELLOW}⚠${NC} Disk usage: ${disk_usage}% (warning)"
        log "⚠ Disk usage: ${disk_usage}% (warning)"
    else
        echo -e "${RED}✗${NC} Disk usage: ${disk_usage}% (critical)"
        log "✗ Disk usage: ${disk_usage}% (critical)"
    fi
    
    # Check memory
    mem_usage=$(free | grep Mem | awk '{print int($3/$2 * 100)}')
    if [ $mem_usage -lt 80 ]; then
        echo -e "${GREEN}✓${NC} Memory usage: ${mem_usage}% (OK)"
        log "✓ Memory usage: ${mem_usage}%"
    elif [ $mem_usage -lt 90 ]; then
        echo -e "${YELLOW}⚠${NC} Memory usage: ${mem_usage}% (warning)"
        log "⚠ Memory usage: ${mem_usage}% (warning)"
    else
        echo -e "${RED}✗${NC} Memory usage: ${mem_usage}% (critical)"
        log "✗ Memory usage: ${mem_usage}% (critical)"
    fi
}

# Function to check recent errors in logs
check_recent_errors() {
    if [ -f "/var/log/sledzspecke/api-$(date +%Y-%m-%d).log" ]; then
        error_count=$(grep -c "ERROR\|CRITICAL" "/var/log/sledzspecke/api-$(date +%Y-%m-%d).log" || true)
        if [ $error_count -eq 0 ]; then
            echo -e "${GREEN}✓${NC} No errors in today's logs"
            log "✓ No errors in today's logs"
        else
            echo -e "${YELLOW}⚠${NC} Found $error_count errors in today's logs"
            log "⚠ Found $error_count errors in today's logs"
            # Show last 3 errors
            echo "Recent errors:"
            grep "ERROR\|CRITICAL" "/var/log/sledzspecke/api-$(date +%Y-%m-%d).log" | tail -3
        fi
    fi
}

# Main monitoring function
main() {
    echo "================================================"
    echo "SledzSpecke Health Monitor - $(date)"
    echo "================================================"
    
    log "Starting health check"
    
    # Basic connectivity
    echo -e "\n${YELLOW}Basic Connectivity:${NC}"
    check_endpoint "$API_URL/api/health" 200 "Health endpoint"
    check_endpoint "$API_URL/swagger/index.html" 200 "Swagger UI"
    check_endpoint "$FRONTEND_URL" 200 "Frontend"
    
    # CORS checks
    echo -e "\n${YELLOW}CORS Configuration:${NC}"
    check_cors "https://sledzspecke.pl" true
    check_cors "https://www.sledzspecke.pl" true
    check_cors "http://localhost:3000" true
    check_cors "https://malicious-site.com" false
    
    # Security
    echo -e "\n${YELLOW}Security Headers:${NC}"
    check_security_headers
    
    # Performance
    echo -e "\n${YELLOW}Performance:${NC}"
    check_response_time
    
    # Database
    echo -e "\n${YELLOW}Database:${NC}"
    check_database
    
    # System resources
    echo -e "\n${YELLOW}System Resources:${NC}"
    check_system_resources
    
    # Recent errors
    echo -e "\n${YELLOW}Application Logs:${NC}"
    check_recent_errors
    
    # API service status
    echo -e "\n${YELLOW}Service Status:${NC}"
    if systemctl is-active --quiet sledzspecke-api; then
        echo -e "${GREEN}✓${NC} API service is running"
        log "✓ API service is running"
    else
        echo -e "${RED}✗${NC} API service is not running!"
        log "✗ API service is not running!"
    fi
    
    echo -e "\n================================================"
    log "Health check completed"
}

# Run main function
main

# Exit with appropriate code
if grep -q "✗" "$LOG_FILE" | tail -1; then
    exit 1
else
    exit 0
fi