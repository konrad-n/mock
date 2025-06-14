#!/bin/bash

# Safe Build Script - Prevents MSBuild/Roslyn process accumulation

# Source environment variables
if [ -f .env ]; then
    export $(cat .env | grep -v '^#' | xargs)
fi

echo "=== Safe Build Script ==="
echo "Environment configured to prevent process accumulation"
echo ""

# Kill any existing build processes before starting
echo "1. Cleaning up any existing build processes..."
ps aux | grep -E "(MSBuild|VBCSCompiler)" | grep -v grep | awk '{print $2}' | xargs kill -9 2>/dev/null || true
sleep 1

# Show current memory
echo "2. Current memory status:"
free -h | grep -E "^(Mem|Swap):"
echo ""

# Run the build with resource limits
echo "3. Running build with resource limits..."
echo "   - MSBuild node reuse: DISABLED"
echo "   - Max MSBuild processes: 1"
echo "   - Compiler server: DISABLED"
echo ""

# Execute the build
dotnet build "$@"
BUILD_RESULT=$?

# Immediately kill build processes after completion
echo ""
echo "4. Cleaning up build processes..."
ps aux | grep -E "(MSBuild|VBCSCompiler)" | grep -v grep | awk '{print $2}' | xargs kill -9 2>/dev/null || true

# Show final memory
echo ""
echo "5. Final memory status:"
free -h | grep -E "^(Mem|Swap):"

exit $BUILD_RESULT