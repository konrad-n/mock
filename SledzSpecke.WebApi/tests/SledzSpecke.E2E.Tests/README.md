# SledzSpecke E2E Tests - World-Class Playwright.NET Implementation

## 🎯 Overview

This is a production-ready, enterprise-grade E2E testing framework built with Playwright.NET, following SOLID principles and clean architecture. The tests simulate real user interactions based on actual SMK (System Monitorowania Kształcenia) workflows documented in the system's user manuals.

## 🏗️ Architecture

### Design Principles
- **SOLID Principles**: Every component follows Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion
- **Page Object Model (POM)**: Clean separation of page structure from test logic
- **Builder Pattern**: Flexible test data creation
- **Factory Pattern**: Centralized browser and page creation
- **Clean Architecture**: Clear separation of concerns

### Project Structure
```
SledzSpecke.E2E.Tests/
├── Core/                      # Framework core components
│   ├── IPageObject.cs        # Base page object interface
│   ├── PageObjectBase.cs     # Base implementation with common helpers
│   ├── BrowserFactory.cs     # Browser lifecycle management
│   └── TestConfiguration.cs  # Configuration models
│
├── PageObjects/              # Page object implementations
│   ├── LoginPage.cs         # Login page interactions
│   ├── DashboardPage.cs     # Dashboard navigation
│   └── MedicalShiftsPage.cs # Medical shifts management
│
├── Scenarios/               # Test scenarios
│   ├── MedicalShiftsScenarios.cs      # Medical shifts workflows
│   └── CompleteSMKWorkflowScenario.cs # Full SMK simulation
│
├── Builders/               # Test data builders
│   └── TestDataBuilder.cs # Flexible test data creation
│
├── Fixtures/              # Test fixtures and base classes
│   └── E2ETestBase.cs    # Base test class with setup/teardown
│
└── Reports/              # Test execution reports
    ├── Screenshots/      # Test screenshots
    ├── Videos/          # Test execution videos
    ├── Traces/          # Playwright traces
    └── Logs/            # Detailed logs
```

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Playwright CLI (`dotnet tool install --global Microsoft.Playwright.CLI`)

### Installation
```bash
# Navigate to test project
cd SledzSpecke.WebApi/tests/SledzSpecke.E2E.Tests

# Restore packages
dotnet restore

# Install Playwright browsers
playwright install chromium firefox
```

### Running Tests

#### Using the Test Runner Script (Recommended)
```bash
# Run all tests
../run-e2e-tests.sh

# Run in headless mode
../run-e2e-tests.sh --headless

# Run with specific browser
../run-e2e-tests.sh --browser firefox

# Run specific tests
../run-e2e-tests.sh --filter "FullyQualifiedName~MedicalShifts"
```

#### Direct dotnet test
```bash
# Run all tests
dotnet test

# Run with specific configuration
dotnet test -- Playwright.LaunchOptions.Headless=true

# Run specific test category
dotnet test --filter "Category=SMKWorkflow"
```

## 📝 Test Scenarios

### 1. Medical Shifts Management
Based on SMK manual: "Dodanie informacji o zrealizowanych dyżurach medycznych"
- Add single shift
- Add monthly rotation
- Edit shift details
- Export shifts report
- Filter by date range

### 2. Complete SMK Workflow
Simulates full monthly workflow:
1. User login
2. Monthly shifts entry
3. Medical procedures documentation
4. Course registration
5. Self-education activities
6. Report generation
7. Supervisor approval submission

### 3. Performance Tests
- Load testing with years of data
- Response time verification
- Concurrent user simulation

## 🏆 Best Practices Implemented

### 1. Page Object Pattern
```csharp
public interface ILoginPage : IPageObject
{
    Task LoginAsync(string username, string password);
    Task<bool> IsLoginErrorDisplayedAsync();
}
```

### 2. Fluent Assertions
```csharp
isSuccess.Should().BeTrue("shift should be saved successfully");
shifts.Should().NotBeEmpty("at least one shift should be present");
```

### 3. Test Data Builders
```csharp
var shift = new MedicalShiftBuilder()
    .OnDate(DateTime.Today)
    .AsNightShift()
    .AtPlace("Szpital Wojewódzki")
    .Build();
```

### 4. Comprehensive Logging
- Structured logging with Serilog
- Correlation IDs for request tracking
- Screenshots on failures
- Performance metrics

### 5. Retry Logic
```csharp
protected async Task ClickWithRetryAsync(string selector, int maxRetries = 3)
```

## 🔧 Configuration

### appsettings.json
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

### Environment Variables
- `E2ETests__BaseUrl`: Override base URL
- `E2ETests__Headless`: Run in headless mode
- `E2ETests__TestUser__DefaultUsername`: Test user credentials

## 📊 Reporting

### Test Results
- Console output with detailed logs
- TRX files for CI/CD integration
- Screenshots for each key action
- Videos of test execution
- Playwright traces for debugging

### Viewing Results
```bash
# View trace files
playwright show-trace Reports/Traces/TestName_*.zip

# Open test report
start Reports/TestResults.html
```

## 🔍 Debugging

### Enable Debug Mode
```bash
# Set environment variable
export PWDEBUG=1
dotnet test
```

### Playwright Inspector
- Step through test execution
- Inspect page elements
- Generate selectors

### Trace Viewer
```bash
playwright show-trace trace.zip
```

## 🚦 CI/CD Integration

### GitHub Actions
```yaml
- name: Run E2E Tests
  run: |
    cd SledzSpecke.WebApi/tests/SledzSpecke.E2E.Tests
    dotnet test --logger "trx"
```

### Test Reports in CI
- Automatic artifact upload
- Test result annotations
- Failure screenshots
- Performance metrics

## 📈 Performance Considerations

1. **Parallel Execution**: Tests can run in parallel with proper isolation
2. **Browser Reuse**: Context isolation without browser restart
3. **Smart Waits**: Using Playwright's auto-waiting mechanisms
4. **Resource Cleanup**: Proper disposal of browser resources

## 🛡️ Security

- Test credentials stored in configuration
- No hardcoded sensitive data
- Secure credential management in CI/CD
- Data isolation between tests

## 🔄 Maintenance

### Adding New Page Objects
1. Create interface extending `IPageObject`
2. Implement class extending `PageObjectBase`
3. Define selectors as constants
4. Add page-specific methods

### Adding New Scenarios
1. Create test class extending `E2ETestBase`
2. Initialize page objects in `OnInitializeAsync`
3. Write test methods with `[Fact]` attribute
4. Use builders for test data

## 📚 Resources

- [Playwright .NET Documentation](https://playwright.dev/dotnet/)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Page Object Model Best Practices](https://martinfowler.com/bliki/PageObject.html)

## 🤝 Contributing

1. Follow existing patterns and conventions
2. Maintain test independence
3. Add appropriate logging
4. Update documentation
5. Ensure CI/CD passes

---

**Created with ❤️ following world-class software engineering standards**