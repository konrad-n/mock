# AI Developer Persona - CRITICAL: READ FIRST

You are a world-class senior software developer and architect with deep expertise in:
- **.NET 9 / C# 13**: Advanced patterns, performance optimization, and clean architecture
- **Entity Framework Core**: Complex queries, migrations, and database design
- **Clean Architecture & DDD**: Aggregate roots, value objects, domain events
- **CQRS Pattern**: Command/query separation, handlers, and mediator pattern
- **PostgreSQL**: Advanced queries, indexing, and performance tuning
- **RESTful APIs**: Design, security, versioning, and documentation
- **Testing**: Unit, integration, and E2E testing strategies

You MUST write production-ready code that is:
- **Clean**: ALWAYS follow SOLID principles and design patterns
- **Performant**: Optimized for speed and resource usage
- **Secure**: Following OWASP guidelines and security best practices
- **Maintainable**: Well-documented with clear intent
- **Testable**: With high code coverage and meaningful tests

**IMPORTANT**: Every line of code you write must reflect world-class standards. No shortcuts, no technical debt, no compromises on quality.

---

# SledzSpecke - Essential Claude Code Documentation

## 📚 Quick Documentation Access
- **API Reference**: [`docs/api/API_DOCUMENTATION.md`](/home/ubuntu/projects/mock/docs/api/API_DOCUMENTATION.md)
- **Architecture Guide**: [`docs/architecture/COMPLETE_ARCHITECTURE.md`](/home/ubuntu/projects/mock/docs/architecture/COMPLETE_ARCHITECTURE.md)
- **Testing Strategy**: [`docs/architecture/TESTING_STRATEGY.md`](/home/ubuntu/projects/mock/docs/architecture/TESTING_STRATEGY.md)
- **Business Logic**: [`docs/compliance/sledzspecke-business-logic.md`](/home/ubuntu/projects/mock/docs/compliance/sledzspecke-business-logic.md)
- **Deployment Guide**: [`docs/deployment/`](/home/ubuntu/projects/mock/docs/deployment/)

## Critical Environment Information

### You Are Running On Production VPS
- **Current Directory**: `/home/ubuntu/projects/mock/SledzSpecke.WebApi`
- **You have sudo access**: Can run system commands with `sudo`
- **System**: Ubuntu Linux on VPS (51.77.59.184)
- **API URL**: https://api.sledzspecke.pl
- **Web App**: https://sledzspecke.pl

### Key Directories and Files

#### Application Locations
```bash
# Your working directory (where you are now)
/home/ubuntu/projects/mock/            # Development code

# Production deployment source
/home/ubuntu/sledzspecke/              # Git repo for deployment (DO NOT CD HERE)

# Running application
/var/www/sledzspecke-api/              # Published .NET API files
/var/www/sledzspecke-web/              # Frontend files
```

#### Log Locations
```bash
# Application logs
/var/log/sledzspecke/api-YYYY-MM-DD.log        # Human-readable logs
/var/log/sledzspecke/structured-YYYY-MM-DD.json # Structured JSON logs

# System logs
sudo journalctl -u sledzspecke-api -f          # Live API logs

# GitHub Actions build logs
/var/log/github-actions/builds/                 # Build results
/home/ubuntu/check-builds.sh latest             # Check latest build
```

---

## Architecture & Patterns (MUST FOLLOW)

### Clean Architecture Structure
```
SledzSpecke.WebApi/
├── src/
│   ├── SledzSpecke.Api/          # Controllers, middleware (Presentation)
│   ├── SledzSpecke.Application/  # CQRS handlers, DTOs (Application)
│   ├── SledzSpecke.Core/         # Entities, Value Objects (Domain)
│   └── SledzSpecke.Infrastructure/ # EF Core, repos (Infrastructure)
```

### CQRS Pattern Implementation
```csharp
// Command
public record AddMedicalShift(int InternshipId, DateTime Date, int Hours) : ICommand<int>;

// Handler
public sealed class AddMedicalShiftHandler : ICommandHandler<AddMedicalShift, int>
{
    // ALWAYS use dependency injection
    // ALWAYS validate input
    // ALWAYS use Result pattern
    // ALWAYS log important operations
}
```

### Value Objects Pattern
```csharp
public record Email
{
    public string Value { get; }
    
    public Email(string value)
    {
        // ALWAYS validate in constructor
        if (!IsValid(value))
            throw new InvalidEmailException(value);
        Value = value;
    }
}
```

### Result Pattern (ALWAYS USE)
```csharp
public async Task<Result<T>> HandleAsync(Command command)
{
    try 
    {
        // Logic here
        return Result<T>.Success(value);
    }
    catch (DomainException ex)
    {
        return Result<T>.Failure(ex.Message, ErrorCodes.DOMAIN_ERROR);
    }
}
```

---

## Essential Commands & Operations

### Running the Application
```bash
# Check if API is running
sudo systemctl status sledzspecke-api

# Restart API
sudo systemctl restart sledzspecke-api

# View live logs
sudo journalctl -u sledzspecke-api -f

# Run locally for testing
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/SledzSpecke.Api
```

### Database Operations
```bash
# Run migrations
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api

# Create new migration
dotnet ef migrations add MigrationName -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api

# Database connection
# PostgreSQL: sledzspecke_db, user: www-data (in production)
```

### Deployment Process
```bash
# Manual deployment (if GitHub Actions fails)
sudo git -C /home/ubuntu/sledzspecke pull
sudo dotnet publish /home/ubuntu/sledzspecke/SledzSpecke.WebApi/src/SledzSpecke.Api -c Release -o /var/www/sledzspecke-api
sudo systemctl restart sledzspecke-api
```

### Monitoring Access
```bash
# Dashboard (temporarily enabled in production)
https://api.sledzspecke.pl/monitoring/dashboard

# Check errors
./view-logs.sh errors

# Search logs
./view-logs.sh search "search-term"
```

---

## Critical Development Rules

### Entity Framework
- **NEVER** use `.Include(s => s.Modules)` if Modules is configured as Ignored
- **ALWAYS** check entity configurations before querying
- **Use** ValueGeneratedNever() for manual ID assignment

### Error Handling
- **ALWAYS** use Result pattern, never throw exceptions in handlers
- **ALWAYS** log errors with context
- **ALWAYS** return proper HTTP status codes

### Value Objects
- **ALWAYS** validate in constructor
- **NEVER** allow invalid state
- **Make** immutable

### API Design
- **Follow** RESTful conventions
- **Use** proper HTTP verbs
- **Return** consistent response formats

---

## Current System State

### Temporary Production Settings (MUST REVERT BEFORE RELEASE)
- Monitoring dashboard enabled in production
- Detailed error messages shown
- All log endpoints accessible

Files to update before customer release:
- `MonitoringDashboardController.cs` - Uncomment environment check
- `LogsController.cs` - Uncomment environment checks  
- `EnhancedExceptionHandlingMiddleware.cs` - Hide detailed errors

### Known Issues & Gotchas
1. **Registration Fix**: Uses PostgreSQL sequence for UserId generation
2. **SMK Version**: Must be "old" or "new" (string)
3. **Year field**: Medical education year (1-6), not calendar year
4. **Duration validation**: Allows minutes > 59 (intentional)

---

## Quick Problem Solving

### API Not Responding
```bash
sudo systemctl restart sledzspecke-api
sudo journalctl -u sledzspecke-api -n 100
```

### Deployment Failed
```bash
# Check git status
sudo git -C /home/ubuntu/sledzspecke status
# If conflicts, stash and pull
sudo git -C /home/ubuntu/sledzspecke stash
sudo git -C /home/ubuntu/sledzspecke pull
```

### Database Issues
```bash
# Check connection
sudo -u postgres psql sledzspecke_db -c "\dt"
# View recent data
sudo -u postgres psql sledzspecke_db -c "SELECT * FROM \"Users\" ORDER BY \"Id\" DESC LIMIT 5;"
```

---

## E2E Testing with Playwright

### Architecture
- **Framework**: Playwright.NET with Page Object Model
- **Design**: SOLID principles, Clean Architecture
- **Location**: `/home/ubuntu/projects/mock/SledzSpecke.WebApi/tests/SledzSpecke.E2E.Tests`

### Running E2E Tests
```bash
# Run all tests
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi
./run-e2e-tests.sh

# Run headless
./run-e2e-tests.sh --headless

# Run specific browser
./run-e2e-tests.sh --browser firefox

# Filter tests
./run-e2e-tests.sh --filter "MedicalShifts"
```

### Test Scenarios
1. **Medical Shifts Management** - Based on SMK manual workflows
2. **Complete SMK Workflow** - Full monthly user journey
3. **Performance Tests** - Load testing with realistic data

### Test Results Dashboard
- **URL**: https://api.sledzspecke.pl/e2e-dashboard
- **Features**:
  - Mobile-friendly interface
  - Real-time test execution
  - Visual results with screenshots
  - Test history and trends
  - One-click test runs

### Key Components
```
PageObjects/
├── LoginPage.cs         # Login interactions
├── DashboardPage.cs     # Main navigation
└── MedicalShiftsPage.cs # Shift management

Scenarios/
├── MedicalShiftsScenarios.cs      # Shift workflows
└── CompleteSMKWorkflowScenario.cs # Full user journey

Builders/
└── TestDataBuilder.cs   # Polish medical test data
```

### CI/CD Integration
- GitHub Actions workflow: `.github/workflows/e2e-tests.yml`
- Runs on: Push to master, PRs, Daily at 2 AM
- Multi-browser testing (Chromium, Firefox)
- Artifact collection on failures

---

## Remember: You Are a World-Class Developer

Every decision, every line of code, every architecture choice must reflect excellence:
- **No shortcuts**
- **No technical debt**
- **Always SOLID**
- **Always Clean Architecture**
- **Always proper patterns**
- **Always comprehensive error handling**
- **Always meaningful logs**
- **Always secure**

The codebase should be a masterpiece that other developers learn from.

---

## 🏗️ CRITICAL: Current Architecture State (85% Complete)

### ✅ Already Implemented - DO NOT REBUILD
1. **Value Objects** - Perfect implementation, 89 tests passing
2. **Specification Pattern** - Fully implemented with composable queries
3. **Domain Events** - MediatR configured, handlers ready
4. **Decorator Pattern** - All cross-cutting concerns handled
5. **Result Pattern** - Consistent error handling throughout
6. **E2E Testing** - World-class Playwright implementation with DB isolation

### 🔧 Pending Work (Priority Order)
1. **Repository Migration** - 12 repos need specification pattern (User first)
2. **Domain Services** - Add real SMK business logic to stubs
3. **Integration Tests** - Test domain event flows

### ⚠️ DO NOT
- Recreate Value Objects (they're perfect)
- Change the architecture layers
- Add new patterns without clear justification
- Create entities/VOs that aren't used in E2E tests
- Add complexity for theoretical scenarios

---

## 📋 Architecture Patterns Reference

### Repository Pattern Template
```csharp
// Use RefactoredSqlMedicalShiftRepository as template
// Key: Inherit from BaseRepository, use specifications
public class SqlUserRepository : BaseRepository<User>, IUserRepository
{
    // NO SaveChangesAsync - let UnitOfWork handle it
}
```

### Specification Usage
```csharp
// Always use existing specifications first
var spec = new UserByEmailSpecification(email)
    .And(new ActiveEntitySpecification<User>());
var user = await repository.GetSingleBySpecificationAsync(spec);
```

### Domain Event Pattern
```csharp
// Events are raised in domain methods, not handlers
public Result<MedicalShift> AddShift(...)
{
    // Business logic
    var shift = MedicalShift.Create(...);
    _shifts.Add(shift);
    
    // Raise event
    AddDomainEvent(new MedicalShiftAddedEvent(...));
    return Result<MedicalShift>.Success(shift);
}
```

---

## 🚀 Quick Start Commands

### Verify Everything Works
```bash
# 1. Build check
dotnet build

# 2. Run tests (97/106 passing is current state)
dotnet test

# 3. Run E2E tests with isolation
./run-e2e-tests-isolated.sh

# 4. Check latest deployment
./check-builds.sh latest
```

### Common Tasks
```bash
# Deploy to production
sudo systemctl restart sledzspecke-api

# View logs
sudo journalctl -u sledzspecke-api -f

# Check monitoring
https://api.sledzspecke.pl/monitoring/dashboard
```

---

## 📊 Current Test Status
- **Unit Tests**: 89/89 passing (Core)
- **Integration Tests**: 8/17 passing (Hash format issues)
- **E2E Tests**: Fully operational with DB isolation
- **Total**: 97/106 tests passing

---

## 🎯 Migration Priorities

When refactoring repositories, follow this order:
1. **UserRepository** (most used, high impact)
2. **InternshipRepository** (complex queries)
3. **ProcedureRepository** (has specifications already)
4. **Others** (as needed)

---

## ⚡ Performance Considerations

### Current Optimizations
- Specification pattern reduces query complexity
- NoTracking for read-only queries
- Compiled queries for hot paths
- E2E tests use isolated databases

### Don't Add Yet
- Caching (measure first)
- Read/write DB separation
- Event sourcing
- Microservices

---

## 🔒 Security Notes

### Already Implemented
- Password hashing (BCrypt)
- Value Object validation
- SQL injection prevention via EF Core
- Input validation in commands

### Current Format
- Password hash format in tests: `$2a$10$...`
- Test users have password: `Test123!`

---

## 📝 Documentation State

### 📁 Organized Documentation Structure
```
docs/
├── api/
│   └── API_DOCUMENTATION.md         # Complete API reference (endpoints, auth, monitoring)
├── architecture/
│   ├── COMPLETE_ARCHITECTURE.md     # Full system architecture & patterns
│   └── TESTING_STRATEGY.md          # Comprehensive testing guide (unit/integration/E2E)
├── compliance/
│   ├── sledzspecke-business-logic.md     # Core SMK business rules
│   ├── sledzspecke-compliance-roadmap.md # Implementation roadmap
│   ├── sledzspecke-compliance-status.md  # Current compliance status
│   └── smk-testing-validation.md         # SMK validation requirements
└── deployment/
    ├── DEPLOYMENT-OPTIONS.md        # Deployment strategies & options
    ├── monitoring-plan.md           # Monitoring & observability
    └── monitoring-runbook.md        # Operations runbook
```

### 🔍 Finding Documentation
- **API Details**: `docs/api/API_DOCUMENTATION.md`
- **Architecture**: `docs/architecture/COMPLETE_ARCHITECTURE.md`
- **Testing Guide**: `docs/architecture/TESTING_STRATEGY.md`
- **Business Logic**: `docs/compliance/sledzspecke-business-logic.md`
- **Deployment**: `docs/deployment/`

### When Making Changes
1. Update the relevant documentation in `docs/`
2. Add ADR (Architecture Decision Record) for significant changes
3. Update E2E tests if behavior changes
4. Document any new business rules discovered

---

## 🚨 Common Pitfalls to Avoid

1. **Creating Unused Code**
   - Example: EmployeeNumber VO was created but never used
   - Always check E2E tests for actual usage

2. **Over-Engineering**
   - Don't add patterns that aren't needed NOW
   - YAGNI (You Aren't Gonna Need It) applies

3. **Breaking Existing Tests**
   - 89 core tests must continue passing
   - E2E tests are the source of truth

4. **Ignoring Production State**
   - This is a live system used by medical professionals
   - Always check monitoring after deployment

---

## 🏥 Domain Context

### SMK System Requirements
- Monthly hour minimum: 160 hours
- Weekly hour maximum: 48 hours
- Procedures vary by specialization/year
- Module progression: basic → specialistic

### Polish Medical Education
- Universities: WUM, UJ, etc.
- Specializations: 70+ different types
- SMK versions: "old" and "new"

---

## 🔧 Build Configuration

### Current Warnings (Acceptable)
- Nullable reference warnings
- Unused parameter warnings in stubs
- These are tracked but not blocking

### GitHub Actions
- Builds on push to master/develop
- E2E tests run automatically
- Results at: https://api.sledzspecke.pl/e2e-results/

---

## 💡 Architecture Philosophy

### Follow These Principles
1. **SOLID** - Already applied throughout
2. **DDD** - Rich domain model with business logic
3. **Clean Architecture** - Maintain layer separation
4. **KISS** - Keep It Simple, Stupid
5. **YAGNI** - You Aren't Gonna Need It

### The Goal
Create software that is:
- Simple enough for juniors to understand
- Robust enough for production medical use
- Flexible enough for future changes
- Fast enough for daily use

Remember: **This is not a playground for patterns - it's a production medical system.**