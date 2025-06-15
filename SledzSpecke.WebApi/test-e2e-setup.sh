#!/bin/bash

# Simple E2E test setup verification script

set -e

echo "=== E2E Test Setup Verification ==="
echo ""

# Check if we're in the right directory
if [ ! -f "SledzSpecke.WebApi.sln" ]; then
    echo "❌ Not in the SledzSpecke.WebApi directory"
    exit 1
fi

echo "✅ In correct directory"

# Check if E2E test project exists
if [ -d "tests/SledzSpecke.E2E.Tests" ]; then
    echo "✅ E2E test project exists"
else
    echo "❌ E2E test project not found"
    exit 1
fi

# Check if test project builds
echo ""
echo "Building E2E test project..."
cd tests/SledzSpecke.E2E.Tests
if dotnet build --configuration Release > /dev/null 2>&1; then
    echo "✅ E2E test project builds successfully"
else
    echo "❌ E2E test project build failed"
    dotnet build --configuration Release
    exit 1
fi

# Check Playwright installation
echo ""
echo "Checking Playwright..."
if command -v playwright &> /dev/null; then
    echo "✅ Playwright CLI is installed"
else
    echo "⚠️  Playwright CLI not found globally, checking local installation..."
    if [ -f ".playwright/node/linux-x64/playwright" ]; then
        echo "✅ Playwright found locally"
    else
        echo "❌ Playwright not installed"
    fi
fi

# Check database connectivity
echo ""
echo "Checking PostgreSQL..."
if systemctl is-active --quiet postgresql; then
    echo "✅ PostgreSQL is running"
else
    echo "❌ PostgreSQL is not running"
fi

# Check if we can connect to database
if sudo -u postgres psql -c "SELECT 1" > /dev/null 2>&1; then
    echo "✅ Can connect to PostgreSQL"
else
    echo "❌ Cannot connect to PostgreSQL"
fi

# Check API endpoints
echo ""
echo "Checking API endpoints..."
if curl -s -o /dev/null -w "%{http_code}" https://api.sledzspecke.pl/health | grep -q "200"; then
    echo "✅ Production API is healthy"
else
    echo "⚠️  Production API health check failed"
fi

# Check web app
if curl -s -o /dev/null -w "%{http_code}" https://sledzspecke.pl | grep -q "200\|304"; then
    echo "✅ Production web app is accessible"
else
    echo "⚠️  Production web app not accessible"
fi

# Check E2E results directory
echo ""
echo "Checking E2E results directory..."
if [ -d "/var/www/sledzspecke-api/e2e-results" ]; then
    echo "✅ E2E results directory exists"
    ls -la /var/www/sledzspecke-api/e2e-results | head -5
else
    echo "❌ E2E results directory not found"
fi

echo ""
echo "=== Setup verification complete ==="