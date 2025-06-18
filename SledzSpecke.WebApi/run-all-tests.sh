#!/bin/bash

echo "=== SledzSpecke Complete Test Suite ==="
echo "Starting at: $(date)"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Create test results directory
mkdir -p TestResults

# Function to run tests and check results
run_test_category() {
    local category=$1
    local filter=$2
    
    echo -e "\n${YELLOW}Running $category tests...${NC}"
    
    if sudo dotnet test $filter --logger "trx;LogFileName=${category}.trx" --results-directory TestResults; then
        echo -e "${GREEN}✓ $category tests PASSED${NC}"
        return 0
    else
        echo -e "${RED}✗ $category tests FAILED${NC}"
        return 1
    fi
}

# Build solution first
echo -e "\n${YELLOW}Building solution...${NC}"
if sudo dotnet build; then
    echo -e "${GREEN}✓ Build successful${NC}"
else
    echo -e "${RED}✗ Build failed${NC}"
    exit 1
fi

# Run different test categories
FAILED=0

# Unit tests
run_test_category "Unit" "tests/SledzSpecke.Core.Tests/" || FAILED=1

# Integration tests
run_test_category "Integration" "tests/SledzSpecke.Tests.Integration/" || FAILED=1

# E2E tests (if not in CI)
if [ -z "$CI" ]; then
    run_test_category "E2E" "tests/SledzSpecke.E2E.Tests/" || FAILED=1
fi

# Generate consolidated report
echo -e "\n${YELLOW}Generating test report...${NC}"
sudo dotnet run --project tests/SledzSpecke.Tests.Common/ -- collect-results "Complete Test Run"

# Summary
echo -e "\n=== Test Summary ==="
if [ "$FAILED" -eq 0 ]; then
    echo -e "${GREEN}All tests passed!${NC}"
    
    # Show test statistics
    echo -e "\nTest Statistics:"
    find TestResults -name "*.trx" -exec grep -c "outcome=\"Passed\"" {} \; | awk '{total+=$1} END {print "  Passed: " total}'
    find TestResults -name "*.trx" -exec grep -c "outcome=\"Failed\"" {} \; | awk '{total+=$1} END {print "  Failed: " total}'
else
    echo -e "${RED}Some tests failed. Check TestResults/ for details.${NC}"
    exit 1
fi

echo -e "\nTest run completed at: $(date)"
echo "View detailed results in TestResults/*.html"