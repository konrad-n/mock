#!/bin/bash

# E2E Tests Runner Script
# This script runs the Playwright-based E2E tests for SledzSpecke

set -e

echo "🧪 SledzSpecke E2E Tests Runner"
echo "================================"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Configuration
TEST_PROJECT="tests/SledzSpecke.E2E.Tests"
REPORTS_DIR="$TEST_PROJECT/Reports"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Parse command line arguments
HEADLESS=false
BROWSER="Chromium"
FILTER=""

while [[ $# -gt 0 ]]; do
    case $1 in
        --headless)
            HEADLESS=true
            shift
            ;;
        --browser)
            BROWSER="$2"
            shift 2
            ;;
        --filter)
            FILTER="$2"
            shift 2
            ;;
        --help)
            echo "Usage: $0 [options]"
            echo "Options:"
            echo "  --headless       Run tests in headless mode"
            echo "  --browser NAME   Browser to use (Chromium, Firefox, Safari)"
            echo "  --filter FILTER  Test filter expression"
            echo "  --help           Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Create reports directory structure
echo "📁 Creating reports directories..."
mkdir -p "$REPORTS_DIR"/{Screenshots,Videos,Traces,Logs,Downloads}

# Install Playwright browsers if needed
echo "🌐 Ensuring Playwright browsers are installed..."
cd "$TEST_PROJECT"
dotnet tool install --global Microsoft.Playwright.CLI || true
playwright install $BROWSER

# Update test configuration
if [ "$HEADLESS" = true ]; then
    echo "🤖 Running in headless mode..."
    jq '.E2ETests.Headless = true' appsettings.json > tmp.json && mv tmp.json appsettings.json
fi

# Build the test project
echo "🔨 Building test project..."
dotnet build --configuration Release

# Run the tests
echo "🚀 Running E2E tests..."
echo "   Browser: $BROWSER"
echo "   Headless: $HEADLESS"
echo "   Filter: ${FILTER:-'All tests'}"
echo ""

if [ -n "$FILTER" ]; then
    dotnet test --no-build --configuration Release --logger "console;verbosity=normal" --filter "$FILTER" -- Playwright.LaunchOptions.Headless=$HEADLESS
else
    dotnet test --no-build --configuration Release --logger "console;verbosity=normal" -- Playwright.LaunchOptions.Headless=$HEADLESS
fi

TEST_RESULT=$?

# Generate test report
echo ""
echo "📊 Generating test report..."

# Archive current test results
if [ -d "$REPORTS_DIR" ]; then
    ARCHIVE_DIR="$REPORTS_DIR/Archive/$TIMESTAMP"
    mkdir -p "$ARCHIVE_DIR"
    
    # Move screenshots, videos, and traces to archive
    [ -d "$REPORTS_DIR/Screenshots" ] && mv "$REPORTS_DIR/Screenshots" "$ARCHIVE_DIR/"
    [ -d "$REPORTS_DIR/Videos" ] && mv "$REPORTS_DIR/Videos" "$ARCHIVE_DIR/"
    [ -d "$REPORTS_DIR/Traces" ] && mv "$REPORTS_DIR/Traces" "$ARCHIVE_DIR/"
    
    echo "📦 Test artifacts archived to: $ARCHIVE_DIR"
fi

# Display results
echo ""
echo "================================"
if [ $TEST_RESULT -eq 0 ]; then
    echo -e "${GREEN}✅ All E2E tests passed!${NC}"
else
    echo -e "${RED}❌ Some E2E tests failed!${NC}"
    echo ""
    echo "Debug artifacts available in:"
    echo "  - Screenshots: $REPORTS_DIR/Archive/$TIMESTAMP/Screenshots/"
    echo "  - Videos: $REPORTS_DIR/Archive/$TIMESTAMP/Videos/"
    echo "  - Traces: $REPORTS_DIR/Archive/$TIMESTAMP/Traces/"
    echo ""
    echo "To view traces, run:"
    echo "  playwright show-trace $REPORTS_DIR/Archive/$TIMESTAMP/Traces/*.zip"
fi

exit $TEST_RESULT