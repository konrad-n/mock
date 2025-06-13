# SledzSpecke Web API Test Runner for Windows
# This script runs the automated API tests

Write-Host "SledzSpecke Web API Test Runner" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

# Check if Python is installed
try {
    $pythonVersion = python --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Python not found"
    }
    Write-Host "Found: $pythonVersion" -ForegroundColor Green
} catch {
    Write-Host "Python 3 is not installed. Please install Python 3 to run the tests." -ForegroundColor Red
    exit 1
}

# Check if requests module is installed
Write-Host "Checking for required Python packages..." -ForegroundColor Yellow
$pipList = pip list 2>&1
if ($pipList -notmatch "requests") {
    Write-Host "Installing requests module..." -ForegroundColor Yellow
    pip install requests
}

# Run the tests
Write-Host "`nStarting API tests...`n" -ForegroundColor Green
python test_api.py $args

# Capture the exit code
$exitCode = $LASTEXITCODE

# Display result
Write-Host ""
if ($exitCode -eq 0) {
    Write-Host "✓ All tests completed successfully!" -ForegroundColor Green
} else {
    Write-Host "✗ Some tests failed. Check the output above for details." -ForegroundColor Red
}

exit $exitCode