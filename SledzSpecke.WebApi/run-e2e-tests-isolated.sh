#!/bin/bash

# E2E Test Runner with Database Isolation
# This script runs E2E tests with proper database isolation
# Each test run gets its own database instance

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Default values
BROWSER="chromium"
HEADLESS="true"
FILTER=""
PARALLEL="false"
RECORD_VIDEO="true"
USE_TEST_DB="true"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --browser)
            BROWSER="$2"
            shift 2
            ;;
        --headed)
            HEADLESS="false"
            shift
            ;;
        --filter)
            FILTER="$2"
            shift 2
            ;;
        --parallel)
            PARALLEL="true"
            shift
            ;;
        --no-video)
            RECORD_VIDEO="false"
            shift
            ;;
        --production-db)
            USE_TEST_DB="false"
            shift
            ;;
        --help)
            echo "Usage: $0 [options]"
            echo "Options:"
            echo "  --browser <chromium|firefox|webkit>  Browser to use (default: chromium)"
            echo "  --headed                             Run tests in headed mode"
            echo "  --filter <pattern>                   Filter tests by pattern"
            echo "  --parallel                           Run tests in parallel"
            echo "  --no-video                          Disable video recording"
            echo "  --production-db                     Use production database (dangerous!)"
            echo "  --help                              Show this help message"
            exit 0
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            exit 1
            ;;
    esac
done

echo -e "${BLUE}=== SledzSpecke E2E Tests with Database Isolation ===${NC}"
echo -e "${YELLOW}Configuration:${NC}"
echo "  Browser: $BROWSER"
echo "  Headless: $HEADLESS"
echo "  Video Recording: $RECORD_VIDEO"
echo "  Database: $(if [ "$USE_TEST_DB" = "true" ]; then echo "Isolated Test DB"; else echo "Production DB (⚠️ DANGEROUS!)"; fi)"
[ -n "$FILTER" ] && echo "  Filter: $FILTER"
echo ""

# Check if PostgreSQL is running
if ! systemctl is-active --quiet postgresql; then
    echo -e "${RED}PostgreSQL is not running. Starting...${NC}"
    sudo systemctl start postgresql
fi

# Create test results directory
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
RESULTS_DIR="Reports/E2E_$TIMESTAMP"
mkdir -p "$RESULTS_DIR"
mkdir -p "$RESULTS_DIR/Videos"
mkdir -p "$RESULTS_DIR/Screenshots"
mkdir -p "$RESULTS_DIR/Traces"
mkdir -p "$RESULTS_DIR/Logs"

# Export environment variables for tests
export ASPNETCORE_ENVIRONMENT="E2E"
export E2E_BROWSER="$BROWSER"
export E2E_HEADLESS="$HEADLESS"
export E2E_VIDEO_PATH="$RESULTS_DIR/Videos"
export E2E_SCREENSHOT_PATH="$RESULTS_DIR/Screenshots"
export E2E_TRACE_PATH="$RESULTS_DIR/Traces"
export E2E_RECORD_VIDEO="$RECORD_VIDEO"

# If using test database, create a test database instance
if [ "$USE_TEST_DB" = "true" ]; then
    echo -e "${YELLOW}Creating isolated test database...${NC}"
    
    # Generate unique test database name
    TEST_DB_NAME="sledzspecke_e2e_$(date +%s)"
    export E2E_TEST_DATABASE="$TEST_DB_NAME"
    
    # Create test database
    sudo -u postgres createdb "$TEST_DB_NAME" || {
        echo -e "${RED}Failed to create test database${NC}"
        exit 1
    }
    
    # Copy schema from production database
    sudo -u postgres pg_dump -s sledzspecke_db | sudo -u postgres psql "$TEST_DB_NAME" || {
        echo -e "${RED}Failed to copy schema to test database${NC}"
        sudo -u postgres dropdb "$TEST_DB_NAME"
        exit 1
    }
    
    echo -e "${GREEN}Test database created: $TEST_DB_NAME${NC}"
    
    # Update connection string for tests
    export ConnectionStrings__DefaultConnection="Host=localhost;Database=$TEST_DB_NAME;Username=postgres;Password="
fi

# Function to cleanup test database
cleanup_test_db() {
    if [ "$USE_TEST_DB" = "true" ] && [ -n "$TEST_DB_NAME" ]; then
        echo -e "${YELLOW}Cleaning up test database...${NC}"
        sudo -u postgres dropdb "$TEST_DB_NAME" 2>/dev/null || true
    fi
}

# Set trap to cleanup on exit
trap cleanup_test_db EXIT

# Install browsers if needed
echo -e "${YELLOW}Installing Playwright browsers...${NC}"
cd tests/SledzSpecke.E2E.Tests
dotnet tool restore
dotnet pwsh playwright.ps1 install $BROWSER --with-deps

# Build the test project
echo -e "${YELLOW}Building E2E test project...${NC}"
dotnet build --configuration Release

# Construct test command
TEST_CMD="dotnet test --no-build --configuration Release"
TEST_CMD="$TEST_CMD --logger \"console;verbosity=detailed\""
TEST_CMD="$TEST_CMD --logger \"html;LogFileName=$RESULTS_DIR/test-results.html\""
TEST_CMD="$TEST_CMD --logger \"trx;LogFileName=$RESULTS_DIR/test-results.trx\""

# Add filter if specified
if [ -n "$FILTER" ]; then
    TEST_CMD="$TEST_CMD --filter \"$FILTER\""
fi

# Add parallel execution if requested
if [ "$PARALLEL" = "true" ]; then
    TEST_CMD="$TEST_CMD -- NUnit.NumberOfTestWorkers=4"
fi

# Run the tests
echo -e "${YELLOW}Running E2E tests...${NC}"
echo "Command: $TEST_CMD"
echo ""

if eval $TEST_CMD; then
    TEST_RESULT=0
    echo -e "${GREEN}✓ All E2E tests passed!${NC}"
else
    TEST_RESULT=$?
    echo -e "${RED}✗ Some E2E tests failed!${NC}"
fi

# Generate summary report
echo -e "${YELLOW}Generating test summary...${NC}"
cat > "$RESULTS_DIR/summary.txt" << EOF
E2E Test Run Summary
===================
Date: $(date)
Browser: $BROWSER
Headless: $HEADLESS
Database: $(if [ "$USE_TEST_DB" = "true" ]; then echo "$TEST_DB_NAME (isolated)"; else echo "Production"; fi)
Filter: ${FILTER:-"None"}
Result: $(if [ $TEST_RESULT -eq 0 ]; then echo "PASSED"; else echo "FAILED"; fi)

Test Results Location: $RESULTS_DIR
EOF

# Copy results to web-accessible location
if [ -d "/var/www/sledzspecke-api/e2e-results" ]; then
    echo -e "${YELLOW}Copying results to web server...${NC}"
    sudo cp -r "$RESULTS_DIR" "/var/www/sledzspecke-api/e2e-results/"
    sudo chown -R www-data:www-data "/var/www/sledzspecke-api/e2e-results/$RESULTS_DIR"
    
    # Update latest symlink
    sudo ln -sfn "/var/www/sledzspecke-api/e2e-results/$RESULTS_DIR" "/var/www/sledzspecke-api/e2e-results/latest"
    
    echo -e "${GREEN}Results available at: https://api.sledzspecke.pl/e2e-results/latest/${NC}"
fi

# Display summary
echo ""
echo -e "${BLUE}=== Test Summary ===${NC}"
cat "$RESULTS_DIR/summary.txt"

# Open results in browser if running locally and not headless
if [ "$HEADLESS" = "false" ] && command -v xdg-open &> /dev/null; then
    xdg-open "$RESULTS_DIR/test-results.html" 2>/dev/null || true
fi

exit $TEST_RESULT