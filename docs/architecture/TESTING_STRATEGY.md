# SledzSpecke Testing Strategy - Comprehensive Guide

## 🎯 Overview

This document provides a complete guide to the testing infrastructure for the SledzSpecke medical education tracking system. Our testing approach follows world-class engineering standards with comprehensive coverage across unit, integration, and end-to-end testing levels.

## 📊 Current Test Status

### Overall Status
- **Unit Tests (Core)**: ✅ 134/134 passing (~200ms)
- **Integration Tests**: ❌ Build errors due to API changes
- **E2E Tests**: ✅ Infrastructure ready and operational
- **Total Coverage**: 97/106 tests passing

### Known Issues
- Integration tests need updating to match current entity structure
- Entity constructors and properties have changed significantly
- DTO structure mismatches in some controller tests

## 🏗️ Testing Architecture

### Design Principles
- **SOLID Principles**: Every test component follows Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion
- **Page Object Model (POM)**: Clean separation of page structure from test logic
- **Builder Pattern**: Flexible test data creation with Polish medical context
- **Factory Pattern**: Centralized browser and page creation
- **Clean Architecture**: Clear separation of concerns across all test levels

### Test Projects Structure
```
SledzSpecke.WebApi/
├── tests/
│   ├── SledzSpecke.Tests.Common/           # Shared test utilities
│   │   ├── Builders/                       # Test data builders
│   │   ├── Infrastructure/                 # Base test classes
│   │   └── Utilities/                      # Helper methods
│   │
│   ├── SledzSpecke.Core.Tests/             # Unit tests for domain
│   │   ├── Entities/                       # Entity tests
│   │   ├── ValueObjects/                   # Value object tests
│   │   └── Specifications/                 # Specification tests
│   │
│   ├── SledzSpecke.Integration.Tests/      # Integration tests
│   │   ├── Controllers/                    # API controller tests
│   │   ├── Handlers/                       # CQRS handler tests
│   │   └── Infrastructure/                 # Database tests
│   │
│   └── SledzSpecke.E2E.Tests/              # End-to-end tests
│       ├── Core/                           # Framework core
│       ├── PageObjects/                    # Page object implementations
│       ├── Scenarios/                      # Test scenarios
│       └── Reports/                        # Test execution reports
```

## 🚀 Test Execution

### Quick Start Commands

#### Run All Tests
```bash
# From project root
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
./run-all-tests.sh

# Components:
# - Unit tests
# - Integration tests (if fixed)
# - E2E tests
# - Test result collection
# - HTML report generation
```

#### Run Quick Tests (Based on Changes)
```bash
# Smart test runner that detects changed files
./run-quick-tests.sh

# Runs only relevant tests based on git diff
# Ideal for rapid development feedback
```

#### Run Tests with Coverage
```bash
# Generate code coverage report
./run-coverage-tests.sh

# Creates detailed coverage reports in:
# - coverage/report/index.html
# - coverage/Cobertura.xml
```

#### Run E2E Tests Only
```bash
# Standard E2E test run
./run-e2e-tests.sh

# With options:
./run-e2e-tests.sh --headless              # Run without browser UI
./run-e2e-tests.sh --browser firefox       # Use specific browser
./run-e2e-tests.sh --filter "MedicalShifts" # Run specific scenarios

# With database isolation (recommended)
./run-e2e-tests-isolated.sh
```

### Direct Test Execution

#### Unit Tests
```bash
cd tests/SledzSpecke.Core.Tests
dotnet test --logger "console;verbosity=detailed"
```

#### Integration Tests
```bash
cd tests/SledzSpecke.Integration.Tests
dotnet test -- "ConnectionStrings:DefaultConnection=Host=localhost;Database=sledzspecke_test;Username=postgres;Password=postgres"
```

#### E2E Tests
```bash
cd tests/SledzSpecke.E2E.Tests
dotnet test -- Playwright.LaunchOptions.Headless=true
```

## 📝 Test Data Builders

### Overview
Test data builders provide realistic Polish medical context data for all test levels. They use the Bogus library with Polish locale support.

### Available Builders

#### UserBuilder
```csharp
var user = new UserBuilder()
    .WithPolishMedicalUniversity()
    .WithValidPesel()
    .WithPwz()
    .Build();
```

#### MedicalShiftBuilder
```csharp
var shift = new MedicalShiftBuilder()
    .OnDate(DateTime.Today)
    .AsNightShift()
    .AtPolishHospital()
    .WithDuration(12, 0)
    .Build();
```

#### MedicalProcedureBuilder
```csharp
var procedure = new MedicalProcedureBuilder()
    .WithICD10Code()
    .WithPolishMedicalName()
    .AsAssisted()
    .Build();
```

#### InternshipBuilder
```csharp
var internship = new InternshipBuilder()
    .AtPolishHospital()
    .ForSpecialization("Kardiologia")
    .StartingOn(DateTime.Today)
    .Build();
```

### Polish Medical Context
All builders include:
- Real Polish hospital names
- Valid PESEL numbers
- PWZ (medical license) numbers
- Polish medical universities
- ICD-10 codes used in Poland
- Realistic specialization names

## 🧪 Unit Testing

### Approach
- Test domain logic in isolation
- Focus on value objects and entities
- Use specifications for query testing
- No database or external dependencies

### Example Test
```csharp
[Fact]
public void Email_Should_Not_Accept_Invalid_Format()
{
    // Arrange
    var invalidEmail = "not-an-email";
    
    // Act & Assert
    Action act = () => new Email(invalidEmail);
    act.Should().Throw<InvalidEmailException>();
}
```

### Best Practices
- One assertion per test
- Descriptive test names
- Arrange-Act-Assert pattern
- Use FluentAssertions for readability

## 🔄 Integration Testing

### Infrastructure
- Uses `IntegrationTestBase` for database isolation
- Each test gets a clean database
- Automatic migration and seeding
- Transaction rollback after each test

### Test Structure
```csharp
public class MedicalShiftsIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task Should_Create_Medical_Shift_With_Valid_Data()
    {
        // Arrange
        var shift = new MedicalShiftBuilder().Build();
        
        // Act
        var response = await Client.PostAsJsonAsync("/api/medical-shifts", shift);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

### Current Issues & Fixes
Integration tests need updating due to:
1. Entity constructor changes
2. Value object implementations
3. DTO structure updates

Priority fixes:
1. Update TestDataFactory methods
2. Fix command parameter mismatches
3. Update DTO expectations

## 🌐 E2E Testing with Playwright

### Architecture
- **Framework**: Playwright.NET with Page Object Model
- **Design**: SOLID principles throughout
- **Isolation**: Each test runs in isolated browser context
- **Reporting**: Screenshots, videos, and traces on failure

### Test Scenarios

#### 1. Medical Shifts Management
Based on SMK manual workflows:
```csharp
[Fact]
public async Task Should_Add_Monthly_Medical_Shifts()
{
    // Login
    await LoginPage.LoginAsync("test@example.com", "Test123!");
    
    // Navigate to shifts
    await DashboardPage.NavigateToMedicalShiftsAsync();
    
    // Add shifts for entire month
    var shifts = GenerateMonthlyShifts();
    foreach (var shift in shifts)
    {
        await MedicalShiftsPage.AddShiftAsync(shift);
    }
    
    // Verify
    var count = await MedicalShiftsPage.GetShiftCountAsync();
    count.Should().Be(shifts.Count);
}
```

#### 2. Complete SMK Workflow
Simulates full monthly user journey:
1. User login
2. Monthly shifts entry
3. Medical procedures documentation
4. Course registration
5. Self-education activities
6. Report generation
7. Supervisor approval

#### 3. SMK Compliance Testing
```csharp
[Fact]
public async Task Should_Complete_Cardiology_Module_With_SMK_Requirements()
{
    // Validates:
    // - 160 hour monthly minimum
    // - 48 hour weekly maximum
    // - Required procedures
    // - Module progression
}
```

#### 4. Performance Testing
```csharp
[Fact]
public async Task Should_Handle_Large_Dataset_Performance()
{
    // Load test with 100 medical shifts
    // Dashboard performance with full dataset
    // Export performance testing
    // Concurrent user simulation
}
```

### Page Object Implementation
```csharp
public interface IMedicalShiftsPage : IPageObject
{
    Task<bool> AddShiftAsync(MedicalShiftData shift);
    Task<int> GetShiftCountAsync();
    Task<bool> EditShiftAsync(int shiftId, MedicalShiftData updatedShift);
    Task<bool> DeleteShiftAsync(int shiftId);
    Task<List<MedicalShiftData>> GetShiftsAsync();
}
```

### E2E Test Configuration
```json
{
  "E2ETests": {
    "BaseUrl": "https://sledzspecke.pl",
    "Browser": "Chromium",
    "Headless": false,
    "RecordVideo": true,
    "SmkSimulation": {
      "SimulateRealUserSpeed": true,
      "DelayBetweenActions": 500
    }
  }
}
```

## 📊 Test Reporting

### Test Results Collection
The `TestResultsCollector` aggregates results from all test runs:

```bash
# After test execution
dotnet run --project tools/TestResultsCollector

# Generates HTML report at:
# TestResults/TestReport_YYYY-MM-DD_HH-mm-ss.html
```

### Report Features
- Summary statistics (passed/failed/skipped)
- Filterable test results
- Duration visualization
- Error details with stack traces
- Performance metrics

### E2E Test Reports
- Screenshots for each key action
- Videos of test execution
- Playwright traces for debugging
- Performance metrics

### Viewing E2E Results
```bash
# View trace files
playwright show-trace Reports/Traces/TestName_*.zip

# View test screenshots
ls Reports/Screenshots/

# Open HTML report
open Reports/TestResults.html
```

## 🚦 CI/CD Integration

### GitHub Actions Workflow
```yaml
name: Run Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      
      - name: Install Playwright
        run: |
          dotnet tool install --global Microsoft.Playwright.CLI
          playwright install chromium firefox
      
      - name: Run All Tests
        run: ./run-all-tests.sh
        
      - name: Upload Test Results
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: test-results
          path: |
            **/TestResults/*.trx
            **/TestResults/*.html
            **/Reports/**
```

### E2E Dashboard Access
- **URL**: https://api.sledzspecke.pl/e2e-dashboard
- **Features**:
  - Mobile-friendly interface
  - Real-time test execution
  - Visual results with screenshots
  - Test history and trends
  - One-click test runs

## 🛡️ Security in Testing

### Credential Management
- Test credentials in configuration files
- No hardcoded sensitive data
- Environment-specific settings
- Secure CI/CD variable storage

### Test User Accounts
```json
{
  "TestUsers": {
    "DefaultUser": {
      "Username": "test@example.com",
      "Password": "Test123!"
    },
    "AdminUser": {
      "Username": "admin@example.com",
      "Password": "Admin123!"
    }
  }
}
```

### Data Isolation
- Each test uses isolated database
- No cross-test data pollution
- Automatic cleanup after tests
- Sensitive data masking in logs

## 📈 Performance Testing

### Load Testing Approach
1. **Baseline Establishment**: Normal user workflow timing
2. **Load Simulation**: Multiple concurrent users
3. **Stress Testing**: System limits identification
4. **Endurance Testing**: Extended period performance

### Performance Metrics
- Response time (p50, p95, p99)
- Throughput (requests/second)
- Error rate
- Resource utilization
- Memory leaks detection

### Performance Test Example
```csharp
[Fact]
public async Task Dashboard_Should_Load_Within_2_Seconds_With_1000_Records()
{
    // Arrange
    await SeedLargeDataset(1000);
    
    // Act
    var stopwatch = Stopwatch.StartNew();
    await DashboardPage.NavigateAsync();
    await DashboardPage.WaitForDataLoadAsync();
    stopwatch.Stop();
    
    // Assert
    stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000);
}
```

## 🔧 Troubleshooting

### Common Issues

#### Integration Tests Failing
```bash
# Check database connection
psql -h localhost -U postgres -d sledzspecke_test

# Reset test database
dotnet ef database drop -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

#### E2E Tests Timeout
```bash
# Increase timeout in test
PageObject.SetDefaultTimeout(30000); // 30 seconds

# Check if application is running
curl https://sledzspecke.pl/health
```

#### Flaky Tests
1. Add retry logic for network operations
2. Use proper wait conditions
3. Ensure test isolation
4. Check for race conditions

### Debug Mode
```bash
# Enable Playwright debug mode
export PWDEBUG=1
dotnet test

# Enable verbose logging
export DEBUG=pw:api
dotnet test
```

## 🏆 Best Practices

### General Testing Principles
1. **Test Independence**: Each test must run in isolation
2. **Deterministic**: Same input always produces same output
3. **Fast Feedback**: Unit tests < 1ms, Integration < 100ms, E2E < 10s
4. **Clear Naming**: Test name describes what and why
5. **Single Assertion**: One logical assertion per test

### Code Quality in Tests
```csharp
// Good test example
[Fact]
public async Task AddMedicalShift_WithValidData_ReturnsCreatedStatus()
{
    // Arrange - Clear setup
    var shift = new MedicalShiftBuilder()
        .WithValidData()
        .Build();
    
    // Act - Single action
    var result = await _handler.HandleAsync(new AddMedicalShift(shift));
    
    // Assert - Clear expectation
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().BeGreaterThan(0);
}
```

### Test Maintenance
1. **Keep Tests Updated**: Update tests when requirements change
2. **Remove Obsolete Tests**: Delete tests for removed features
3. **Refactor Test Code**: Apply same quality standards as production
4. **Monitor Test Performance**: Track test execution time
5. **Review Test Coverage**: Aim for meaningful coverage, not 100%

## 🚀 Future Enhancements

### Planned Improvements
1. **Mutation Testing**: Add Stryker.NET for mutation testing
2. **Contract Testing**: API contract validation with Pact
3. **Visual Regression**: Screenshot comparison for UI changes
4. **Chaos Engineering**: Resilience testing with controlled failures
5. **Security Testing**: OWASP ZAP integration for security scans

### Performance Baselines
Establish and monitor:
- API response time baselines
- Database query performance
- Frontend rendering metrics
- Memory usage patterns
- Concurrent user limits

## 📚 Resources

### Documentation
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Guide](https://fluentassertions.com/)
- [Playwright .NET Docs](https://playwright.dev/dotnet/)
- [Bogus Documentation](https://github.com/bchavez/Bogus)

### Internal Guides
- Architecture Documentation: `/docs/architecture/`
- API Documentation: https://api.sledzspecke.pl/swagger
- SMK Requirements: `/docs/smk-requirements/`

---

**Remember**: Quality tests are as important as quality code. They are your safety net for confident refactoring and your documentation for system behavior.