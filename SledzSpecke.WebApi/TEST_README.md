# SledzSpecke Web API Automated Testing

This directory contains automated testing scripts for the SledzSpecke Web API.

## Features

- **Automatic API startup**: Checks if the API is running and starts it if needed
- **Comprehensive endpoint testing**: Tests all major endpoints including:
  - Authentication (sign-up, sign-in)
  - Internships (CRUD operations)
  - Procedures (CRUD operations for both Old and New SMK)
  - Medical Shifts (CRUD operations for both Old and New SMK)
- **Clear reporting**: Shows pass/fail status for each test with timing
- **Summary report**: Displays overall test statistics at the end
- **Cross-platform**: Works on Linux, macOS, and Windows

## Prerequisites

- Python 3.x installed
- .NET SDK (for running the API)
- `requests` Python package (will be installed automatically if missing)

## Usage

### Linux/macOS

```bash
# Make the script executable (first time only)
chmod +x test_api.sh

# Run all tests
./test_api.sh

# Run tests with custom API URL
./test_api.sh --url http://localhost:5001/api

# Run tests without auto-starting the API
./test_api.sh --no-start
```

### Windows

```powershell
# Run all tests
.\test_api.ps1

# Run tests with custom API URL
.\test_api.ps1 --url http://localhost:5001/api

# Run tests without auto-starting the API
.\test_api.ps1 --no-start
```

### Direct Python execution

```bash
# Run all tests
python test_api.py

# Run tests with custom API URL
python test_api.py --url http://localhost:5001/api

# Run tests without auto-starting the API
python test_api.py --no-start
```

## Test Coverage

The test suite covers:

1. **Authentication**
   - User registration (sign-up)
   - User login (sign-in)
   - JWT token retrieval

2. **Internships**
   - Create new internship
   - Retrieve internship list
   - Update internship (if implemented)
   - Delete internship (if implemented)

3. **Procedures (Both Old and New SMK)**
   - Create procedure with all required fields
   - Retrieve procedures by internship
   - Update procedure details
   - Delete procedure

4. **Medical Shifts (Both Old and New SMK)**
   - Create medical shift with duration
   - Retrieve shifts by internship
   - Update shift details
   - Delete shift

## Test Output

The script provides:
- Real-time test execution status
- Duration for each test
- Color-coded pass/fail indicators
- Final summary with:
  - Total tests run
  - Number passed/failed/skipped
  - Success rate percentage
  - List of failed tests (if any)

## Exit Codes

- `0`: All tests passed
- `1`: One or more tests failed

## Customization

You can modify the test configuration by editing the following variables in `test_api.py`:

```python
API_BASE_URL = "http://localhost:5000/api"  # API endpoint
TEST_USERNAME = "testuser"                    # Test user credentials
TEST_PASSWORD = "Test123!"
```

## Troubleshooting

1. **API won't start**: Ensure you have .NET SDK installed and the database is configured
2. **Authentication fails**: Check that the test user can be created (database is writable)
3. **Tests timeout**: Increase timeout values in the script or check API performance
4. **Python not found**: Install Python 3.x from python.org