#!/bin/bash

echo "=== SledzSpecke Resource Cleanup Script ==="
echo ""

# Kill lingering dotnet processes
echo "1. Killing lingering dotnet build processes..."
ps aux | grep MSBuild | grep -v grep | awk '{print $2}' | xargs kill -9 2>/dev/null || true
ps aux | grep VBCSCompiler | grep -v grep | awk '{print $2}' | xargs kill -9 2>/dev/null || true
ps aux | grep "dotnet run" | grep -v grep | awk '{print $2}' | xargs kill -9 2>/dev/null || true
echo "   ✓ Dotnet processes cleaned"

# Show memory before
echo ""
echo "2. Memory status before cleanup:"
free -h

# Clean build artifacts (optional)
if [ "$1" == "--full" ]; then
    echo ""
    echo "3. Cleaning build artifacts (--full mode)..."
    cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
    find . -type d -name "bin" -o -name "obj" | grep -E "src/.*/(bin|obj)$" | xargs rm -rf 2>/dev/null || true
    echo "   ✓ Build artifacts cleaned"
fi

# Clear dotnet temp files
echo ""
echo "4. Clearing dotnet temporary files..."
rm -rf /tmp/NuGetScratch* 2>/dev/null || true
rm -rf /tmp/dotnet-* 2>/dev/null || true
echo "   ✓ Temp files cleaned"

# Clear system cache (if running as sudo)
if [ "$EUID" -eq 0 ]; then
    echo ""
    echo "5. Clearing system cache..."
    sync && echo 3 > /proc/sys/vm/drop_caches
    echo "   ✓ System cache cleared"
fi

# Show memory after
echo ""
echo "6. Memory status after cleanup:"
free -h

echo ""
echo "=== Cleanup complete ==="
echo ""
echo "Tips for development:"
echo "- Use 'dotnet build --no-restore' when dependencies haven't changed"
echo "- Use 'dotnet build -c Release' for smaller binaries"
echo "- Run this script periodically during heavy development"
echo "- Use './cleanup-resources.sh --full' to also clean build artifacts"