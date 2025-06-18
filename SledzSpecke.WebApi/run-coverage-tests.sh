#!/bin/bash

# Test runner with code coverage reporting

echo "=== SledzSpecke Tests with Coverage ==="
echo "Starting at: $(date)"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Create directories
mkdir -p TestResults
mkdir -p TestResults/Coverage

# Build first
echo -e "\n${YELLOW}Building solution...${NC}"
sudo dotnet build

# Run tests with coverage
echo -e "\n${YELLOW}Running tests with coverage...${NC}"

# Unit tests with coverage
sudo dotnet test tests/SledzSpecke.Core.Tests/ \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=opencover \
    /p:CoverletOutput=../../TestResults/Coverage/unit.coverage.xml \
    --logger "trx;LogFileName=unit-coverage.trx" \
    --results-directory TestResults

# Integration tests with coverage
sudo dotnet test tests/SledzSpecke.Tests.Integration/ \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=opencover \
    /p:CoverletOutput=../../TestResults/Coverage/integration.coverage.xml \
    /p:MergeWith=../../TestResults/Coverage/unit.coverage.xml \
    --logger "trx;LogFileName=integration-coverage.trx" \
    --results-directory TestResults

# Check if reportgenerator is installed
if ! command -v reportgenerator &> /dev/null; then
    echo -e "\n${YELLOW}Installing ReportGenerator...${NC}"
    sudo dotnet tool install -g dotnet-reportgenerator-globaltool
    export PATH="$PATH:$HOME/.dotnet/tools"
fi

# Generate coverage report
echo -e "\n${YELLOW}Generating coverage report...${NC}"
reportgenerator \
    -reports:TestResults/Coverage/*.coverage.xml \
    -targetdir:TestResults/CoverageReport \
    -reporttypes:Html

# Display coverage summary
echo -e "\n${GREEN}=== Coverage Summary ===${NC}"
if [ -f "TestResults/CoverageReport/index.html" ]; then
    # Extract coverage percentage from HTML report
    COVERAGE=$(grep -oP 'Line coverage: \K[0-9.]+' TestResults/CoverageReport/index.html | head -1)
    
    if [ -n "$COVERAGE" ]; then
        echo "Overall Coverage: ${COVERAGE}%"
        
        # Check if coverage meets threshold
        if (( $(echo "$COVERAGE >= 80" | bc -l) )); then
            echo -e "${GREEN}✓ Coverage meets 80% threshold${NC}"
        else
            echo -e "${RED}✗ Coverage below 80% threshold${NC}"
        fi
    fi
    
    echo -e "\nDetailed report available at: TestResults/CoverageReport/index.html"
else
    echo -e "${RED}Coverage report generation failed${NC}"
fi

echo -e "\nCoverage test run completed at: $(date)"