#!/bin/bash

# Quick test runner for development
# Only runs affected tests based on recent changes

echo "=== Quick Test Runner ==="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Get list of changed files
CHANGED_FILES=$(git diff --name-only HEAD~1 2>/dev/null || git diff --name-only)

if [ -z "$CHANGED_FILES" ]; then
    echo "No changes detected. Running smoke tests..."
    sudo dotnet test --filter "Category=Smoke" --no-build
else
    echo "Changed files detected:"
    echo "$CHANGED_FILES"
    echo ""
    
    # Determine which tests to run based on changes
    TESTS_TO_RUN=""
    
    if echo "$CHANGED_FILES" | grep -q "MedicalShift"; then
        echo -e "${YELLOW}Medical Shift changes detected${NC}"
        TESTS_TO_RUN="$TESTS_TO_RUN FullyQualifiedName~MedicalShift"
    fi
    
    if echo "$CHANGED_FILES" | grep -q "Procedure"; then
        echo -e "${YELLOW}Procedure changes detected${NC}"
        TESTS_TO_RUN="$TESTS_TO_RUN | FullyQualifiedName~Procedure"
    fi
    
    if echo "$CHANGED_FILES" | grep -q "User"; then
        echo -e "${YELLOW}User changes detected${NC}"
        TESTS_TO_RUN="$TESTS_TO_RUN | FullyQualifiedName~User"
    fi
    
    if echo "$CHANGED_FILES" | grep -q "Specialization"; then
        echo -e "${YELLOW}Specialization changes detected${NC}"
        TESTS_TO_RUN="$TESTS_TO_RUN | FullyQualifiedName~Specialization"
    fi
    
    if [ -z "$TESTS_TO_RUN" ]; then
        echo "No specific tests identified. Running all unit tests..."
        sudo dotnet test tests/SledzSpecke.Core.Tests/ --no-build
    else
        # Remove leading separator if present
        TESTS_TO_RUN=$(echo "$TESTS_TO_RUN" | sed 's/^ | //')
        
        echo -e "\n${YELLOW}Running tests matching: $TESTS_TO_RUN${NC}"
        sudo dotnet test --filter "$TESTS_TO_RUN" --no-build
    fi
fi

echo -e "\n${GREEN}Quick tests completed!${NC}"