#!/bin/bash

# Run E2E tests on VPS with proper isolation
# This script is designed to run on the production VPS

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}=== SledzSpecke E2E Tests on VPS ===${NC}"

# Configuration
BASE_DIR="/home/ubuntu/projects/mock/SledzSpecke.WebApi"
cd "$BASE_DIR"

# Check if running on VPS
if [[ ! -f "/var/www/sledzspecke-api/appsettings.json" ]]; then
    echo -e "${RED}This script is designed to run on the VPS only!${NC}"
    exit 1
fi

# Parse arguments
BROWSER="${1:-chromium}"
FILTER="${2:-}"

echo -e "${YELLOW}Configuration:${NC}"
echo "  Browser: $BROWSER"
echo "  Filter: ${FILTER:-All tests}"
echo ""

# Create test database
echo -e "${YELLOW}Creating isolated test database...${NC}"
TEST_DB="sledzspecke_e2e_test_$(date +%s)"
sudo -u postgres createdb "$TEST_DB"

# Copy schema from production
echo -e "${YELLOW}Copying production schema...${NC}"
sudo -u postgres pg_dump -s sledzspecke_db | sudo -u postgres psql "$TEST_DB"

# Create test user in database
sudo -u postgres psql "$TEST_DB" << EOF
-- Create test users with known credentials
INSERT INTO "Users" ("Email", "Password", "FirstName", "LastName", "SmkVersion", "Location", "University", "Year", "Phone", "RegistrationDate", "IsActive")
VALUES 
('test.user@sledzspecke.pl', '\$2a\$10\$8JqVb7SERQ2HCLKSMbfAkOSr1r2Ot.piVcAVZQQYjQxZX2x1B0XMO', 'Jan', 'Testowy', 'new', 'Warszawa', 'Warszawski Uniwersytet Medyczny', 3, '+48123456789', NOW(), true),
('anna.kowalska@sledzspecke.pl', '\$2a\$10\$8JqVb7SERQ2HCLKSMbfAkOSr1r2Ot.piVcAVZQQYjQxZX2x1B0XMO', 'Anna', 'Kowalska', 'old', 'Kraków', 'Uniwersytet Jagielloński', 5, '+48987654321', NOW(), true);
EOF

# Build test project
echo -e "${YELLOW}Building E2E test project...${NC}"
cd tests/SledzSpecke.E2E.Tests
dotnet build --configuration Release

# Install Playwright browsers
echo -e "${YELLOW}Installing Playwright browsers...${NC}"
dotnet tool restore
dotnet pwsh playwright.ps1 install $BROWSER --with-deps

# Run tests
echo -e "${YELLOW}Running E2E tests...${NC}"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
RESULTS_DIR="/var/www/sledzspecke-api/e2e-results/vps_$TIMESTAMP"
sudo mkdir -p "$RESULTS_DIR"

# Export test configuration
export E2ETests__BaseUrl="https://sledzspecke.pl"
export E2ETests__ApiUrl="https://api.sledzspecke.pl"
export E2ETests__Browser="$BROWSER"
export E2ETests__Headless="true"
export E2ETests__VideoPath="$RESULTS_DIR/videos"
export E2ETests__ScreenshotPath="$RESULTS_DIR/screenshots"
export E2ETests__TracePath="$RESULTS_DIR/traces"
export ConnectionStrings__DefaultConnection="Host=localhost;Database=$TEST_DB;Username=www-data"

# Create directories
mkdir -p "$RESULTS_DIR"/{videos,screenshots,traces,logs}

# Run tests
if [ -n "$FILTER" ]; then
    dotnet test --no-build --configuration Release \
        --logger "console;verbosity=normal" \
        --logger "html;LogFileName=$RESULTS_DIR/test-results.html" \
        --logger "trx;LogFileName=$RESULTS_DIR/test-results.trx" \
        --filter "$FILTER" \
        -- RunConfiguration.TestSessionTimeout=600000
else
    dotnet test --no-build --configuration Release \
        --logger "console;verbosity=normal" \
        --logger "html;LogFileName=$RESULTS_DIR/test-results.html" \
        --logger "trx;LogFileName=$RESULTS_DIR/test-results.trx" \
        -- RunConfiguration.TestSessionTimeout=600000
fi

TEST_RESULT=$?

# Create summary
cat > "$RESULTS_DIR/summary.json" << EOF
{
  "timestamp": "$(date -u +%Y-%m-%dT%H:%M:%SZ)",
  "browser": "$BROWSER",
  "filter": "${FILTER:-none}",
  "database": "$TEST_DB",
  "result": $([ $TEST_RESULT -eq 0 ] && echo '"passed"' || echo '"failed"'),
  "baseUrl": "https://sledzspecke.pl",
  "apiUrl": "https://api.sledzspecke.pl"
}
EOF

# Create HTML index
cat > "$RESULTS_DIR/index.html" << 'EOF'
<!DOCTYPE html>
<html>
<head>
    <title>VPS E2E Test Results</title>
    <meta charset="UTF-8">
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            background-color: #f5f5f5;
        }
        .container {
            max-width: 1000px;
            margin: 0 auto;
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        h1 { color: #333; }
        .info { background-color: #f0f8ff; padding: 15px; border-radius: 5px; margin: 20px 0; }
        .success { color: green; font-weight: bold; }
        .failure { color: red; font-weight: bold; }
        .links { margin-top: 30px; }
        .links a {
            display: inline-block;
            margin: 10px;
            padding: 10px 20px;
            background-color: #007bff;
            color: white;
            text-decoration: none;
            border-radius: 5px;
        }
        .links a:hover { background-color: #0056b3; }
    </style>
</head>
<body>
    <div class="container">
        <h1>🧪 VPS E2E Test Results</h1>
        <div class="info">
            <p><strong>Date:</strong> '"$(date)"'</p>
            <p><strong>Browser:</strong> '"$BROWSER"'</p>
            <p><strong>Environment:</strong> Production VPS</p>
            <p><strong>Status:</strong> <span class="'"$([ $TEST_RESULT -eq 0 ] && echo 'success' || echo 'failure')"'">'"$([ $TEST_RESULT -eq 0 ] && echo 'PASSED' || echo 'FAILED')"'</span></p>
        </div>
        <div class="links">
            <a href="test-results.html">📊 View Test Report</a>
            <a href="screenshots/">📸 Browse Screenshots</a>
            <a href="videos/">🎬 Watch Videos</a>
            <a href="traces/">🔍 Download Traces</a>
        </div>
    </div>
</body>
</html>
EOF

# Set permissions
sudo chown -R www-data:www-data "$RESULTS_DIR"

# Cleanup test database
echo -e "${YELLOW}Cleaning up test database...${NC}"
sudo -u postgres dropdb "$TEST_DB"

# Update latest symlink
sudo ln -sfn "$RESULTS_DIR" /var/www/sledzspecke-api/e2e-results/latest-vps

# Display results
echo ""
echo -e "${BLUE}=== Test Summary ===${NC}"
if [ $TEST_RESULT -eq 0 ]; then
    echo -e "${GREEN}✓ All tests passed!${NC}"
else
    echo -e "${RED}✗ Some tests failed!${NC}"
fi

echo ""
echo -e "${GREEN}Results available at:${NC}"
echo "  https://api.sledzspecke.pl/e2e-results/vps_$TIMESTAMP/"
echo "  https://api.sledzspecke.pl/e2e-results/latest-vps/"

exit $TEST_RESULT