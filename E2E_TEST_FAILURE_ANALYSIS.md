# E2E Test Failure Analysis and Solutions

## Summary of Issues Found

### 1. **GitHub Actions Workflow Issue**
**Problem**: The workflow is filtering for a non-existent test `GuaranteedVideoTest`
- Location: `.github/workflows/e2e-tests-isolated.yml` line 230
- Impact: No tests run because the filter doesn't match any existing tests
- **Status**: ✅ FIXED - Changed filter to empty string to run all tests

### 2. **Local E2E Test Configuration**
**Problem**: E2E tests use production URLs when running locally
- Configuration file: `tests/SledzSpecke.E2E.Tests/appsettings.json`
- URLs point to https://sledzspecke.pl and https://api.sledzspecke.pl
- Tests fail because they expect local development servers

### 3. **Test Failures When Running Locally**
- `Frontend_HomePage_ShouldLoad`: Empty title returned
- `Frontend_LoginPage_ShouldBeAccessible`: No form found on page
- `API_HealthEndpoint_ShouldReturnOK`: This one passes (API is accessible)

## Root Causes

1. **Configuration Mismatch**: The E2E tests are configured for production URLs but need local URLs in CI/CD
2. **Missing Test Filter**: The workflow was looking for a test that doesn't exist
3. **Frontend Not Running**: In CI, the frontend needs to be built and served via nginx

## Solutions Implemented

### 1. Fixed GitHub Actions Workflow
Changed line 230 from:
```yaml
TEST_FILTER: ${{ github.event.inputs.test_filter || 'FullyQualifiedName~GuaranteedVideoTest' }}
```
To:
```yaml
TEST_FILTER: ${{ github.event.inputs.test_filter || '' }}
```

### 2. Environment-Specific Configuration
The workflow correctly sets environment variables to override the production URLs:
```yaml
E2ETests__BaseUrl: http://localhost:3000
E2ETests__ApiUrl: http://localhost:5000
```

## CI/CD Pipeline Process

The E2E test workflow does the following:
1. Creates an isolated PostgreSQL database for each test run
2. Runs migrations on the test database
3. Starts the API with the test database connection
4. Builds the frontend (React app)
5. Configures nginx to serve the frontend and proxy API requests
6. Runs E2E tests with Playwright
7. Captures screenshots, videos, and traces
8. Uploads artifacts and deploys results to VPS

## Available E2E Tests

The test suite includes:
- **HealthCheckScenarios**: Basic connectivity tests
- **MedicalShiftsScenarios**: Medical shift management workflows
- **UserJourneyScenarios**: Complete user workflows
- **PerformanceScenarios**: Load and performance tests
- **SMKComplianceScenarios**: SMK system compliance tests
- **ApiOnlyScenarios**: API-specific tests

## Next Steps

1. **Push the workflow fix** to trigger a new CI run
2. **Monitor the GitHub Actions** to see if tests now run properly
3. **Create environment-specific appsettings** if needed:
   - `appsettings.Development.json` for local development
   - `appsettings.E2E.json` for E2E test runs

## Running E2E Tests Locally

To run E2E tests locally with proper configuration:

```bash
# Set environment variables
export E2ETests__BaseUrl=http://localhost:3000
export E2ETests__ApiUrl=http://localhost:5000
export ASPNETCORE_ENVIRONMENT=Development

# Run tests
cd SledzSpecke.WebApi
dotnet test tests/SledzSpecke.E2E.Tests
```

Or use the isolated test runner:
```bash
./run-e2e-tests-isolated.sh --browser chromium
```

## Verification Steps

After pushing the fix:
1. Check GitHub Actions runs for successful E2E test execution
2. Verify test results are uploaded as artifacts
3. Check if results are deployed to https://api.sledzspecke.pl/e2e-results/latest/