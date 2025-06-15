#!/bin/bash

# Script to cleanup old SledzSpecke processes before starting API
# This prevents port conflicts and hanging processes

echo "=== SledzSpecke Process Cleanup ==="
echo "Checking for old processes..."

# Find all dotnet processes related to SledzSpecke
OLD_PROCESSES=$(ps aux | grep -E "dotnet.*SledzSpecke.*Api\.dll" | grep -v grep | awk '{print $2}')

if [ -n "$OLD_PROCESSES" ]; then
    echo "Found old SledzSpecke processes:"
    ps aux | grep -E "dotnet.*SledzSpecke.*Api\.dll" | grep -v grep
    
    echo ""
    echo "Killing old processes..."
    for PID in $OLD_PROCESSES; do
        echo "Killing process $PID..."
        sudo kill -9 $PID 2>/dev/null || true
    done
    
    echo "Cleanup completed."
else
    echo "No old processes found."
fi

# Also check specifically for processes on port 5000
echo ""
echo "Checking port 5000..."
PORT_PROCESS=$(sudo lsof -ti :5000)

if [ -n "$PORT_PROCESS" ]; then
    echo "Process $PORT_PROCESS is using port 5000. Killing it..."
    sudo kill -9 $PORT_PROCESS 2>/dev/null || true
    echo "Port 5000 cleared."
else
    echo "Port 5000 is free."
fi

echo ""
echo "Cleanup complete!"