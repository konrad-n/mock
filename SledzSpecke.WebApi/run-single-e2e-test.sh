#!/bin/bash

# Simple script to run a single E2E test

set -e

echo "=== Running Single E2E Test ==="

cd tests/SledzSpecke.E2E.Tests

# Install Playwright browsers if needed
echo "Installing Playwright..."
dotnet tool restore
dotnet pwsh playwright.ps1 install chromium --with-deps

# Run a simple test
echo "Running test..."
export E2ETests__BaseUrl="https://sledzspecke.pl"
export E2ETests__ApiUrl="https://api.sledzspecke.pl"
export E2ETests__Browser="chromium"
export E2ETests__Headless="true"

# Create directories
mkdir -p Reports/Screenshots Reports/Videos Reports/Traces

# Run the login test
dotnet test --filter "FullyQualifiedName~LoginAfterRegistration" --logger "console;verbosity=normal"

echo "Test completed. Check Reports/ directory for outputs."