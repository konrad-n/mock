#!/bin/bash

# SledzSpecke Web API Test Runner
# This script runs the automated API tests

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}SledzSpecke Web API Test Runner${NC}"
echo "================================"

# Check if Python is installed
if ! command -v python3 &> /dev/null; then
    echo -e "${RED}Python 3 is not installed. Please install Python 3 to run the tests.${NC}"
    exit 1
fi

# Check if requests module is installed
if ! python3 -c "import requests" &> /dev/null; then
    echo -e "${YELLOW}Installing required Python packages...${NC}"
    pip3 install requests || pip install requests
fi

# Make the Python script executable
chmod +x test_api.py

# Run the tests
echo -e "\n${GREEN}Starting API tests...${NC}\n"
python3 test_api.py "$@"

# Capture the exit code
EXIT_CODE=$?

# Display result
echo ""
if [ $EXIT_CODE -eq 0 ]; then
    echo -e "${GREEN}✓ All tests completed successfully!${NC}"
else
    echo -e "${RED}✗ Some tests failed. Check the output above for details.${NC}"
fi

exit $EXIT_CODE