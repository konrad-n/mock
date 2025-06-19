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

## 🚨 CRITICAL: SMK Excel Export Compliance
**If the user mentions:**
- Excel export functionality
- SMK (System Monitorowania Kształcenia) compliance  
- Chrome extension for data import
- Official government PDF requirements

**→ USE THE SMK MCP CONFIG:** Jump to [SMK Compliance MCP Integration](#-smk-compliance-mcp-integration) section and use `mcp-smk-config.json` instead of the regular config.

## 🎓 CRITICAL: CMKP Specialization Requirements
**If the user mentions:**
- Medical specialization requirements
- CMKP (Centrum Medyczne Kształcenia Podyplomowego) programs
- Importing specialization PDFs
- Creating specialization templates

**→ USE THE CMKP MCP CONFIG:** Jump to [CMKP Specialization Management](#-cmkp-specialization-management) section and use `mcp-cmkp-config.json`.

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
- **You have GitHub CLI access**: Can use `gh` command for GitHub operations
- **System**: Ubuntu Linux on VPS (51.77.59.184)
- **API URL**: https://api.sledzspecke.pl
- **Web App**: https://sledzspecke.pl
- **Working Branch**: Always work on master branch and push directly (no feature branches needed)

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
/var/log/github-actions/builds/                 # Build results (saved on this VPS)
/home/ubuntu/check-builds.sh latest             # Check latest build
# Note: GitHub Actions logs are automatically saved to this VPS for easy debugging
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

## 🔌 MCP (Model Context Protocol) Integration

### ⚡ Quick Decision Guide: Which MCP Config to Use?

| Use Case | Config File | When to Use |
|----------|------------|-------------|
| **General Development** | `mcp-config.json` | Debugging, logs, database queries, GitHub |
| **SMK/Excel Export** | `mcp-smk-config.json` | SMK PDFs, Excel compliance, Chrome extension |
| **CMKP Specializations** | `mcp-cmkp-config.json` | Medical specializations, PDFs to JSON |

### Configuration Files
- **General**: `/home/ubuntu/projects/mock/mcp-config.json`
- **SMK**: `/home/ubuntu/projects/mock/mcp-smk-config.json`
- **CMKP**: `/home/ubuntu/projects/mock/mcp-cmkp-config.json`

### Available MCP Tools

#### 1. **Filesystem Server**
- `mcp__filesystem__read_file` - Read files
- `mcp__filesystem__write_file` - Write files
- `mcp__filesystem__list_directory` - List directories
- `mcp__filesystem__search_files` - Search content

#### 2. **PostgreSQL Server**
- `mcp__postgres__query` - SELECT queries
- `mcp__postgres__analyze` - Query performance
- `mcp__postgres__schema` - Table schemas

#### 3. **GitHub Server**
- `mcp__github__list_workflows` - GitHub Actions
- `mcp__github__get_workflow_runs` - Build status
- `mcp__github__list_pull_requests` - PRs
- `mcp__github__create_issue` - Create issues

#### 4. **Memory Server**
- `mcp__memory__store` - Store context
- `mcp__memory__recall` - Retrieve info
- `mcp__memory__search` - Search memories

### Quick Commands
```bash
# General debugging
claude --mcp-config mcp-config.json -p "Check API health and recent errors" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query"

# SMK compliance work
claude --mcp-config mcp-smk-config.json -p "Analyze SMK PDFs for Excel export" \
  --allowedTools "mcp__smk-docs__read_file,mcp__memory__store"

# CMKP specializations
claude --mcp-config mcp-cmkp-config.json -p "Import cardiology specialization" \
  --allowedTools "mcp__cmkp-docs__read_file,mcp__postgres__query"
```

### Setup
```bash
# GitHub token (required)
export GITHUB_TOKEN="your-github-token"
echo 'export GITHUB_TOKEN="your-github-token"' >> ~/.bashrc

# Test connection
claude --mcp-config mcp-config.json -p "Test MCP connections" --verbose
```

### 📋 SMK Compliance MCP Integration

#### 🎯 When to Use This Section
**USE THIS SECTION WHEN:**
- User mentions Excel export functionality
- User asks about SMK compliance
- User provides SMK PDF documentation
- User mentions Chrome extension compatibility
- Working on export features to match government systems

#### Quick Start Example
```bash
# If user says: "I need to export data to Excel format that matches SMK system"
# You should use:
claude --mcp-config mcp-smk-config.json -p "Analyze SMK PDFs and implement Excel export" \
  --allowedTools "mcp__smk-docs__read_file,mcp__smk-docs__write_file,mcp__memory__store"
```

#### Overview
For ensuring 1:1 compliance with official government SMK systems, use the specialized `mcp-smk-config.json` configuration. This provides direct access to:
- Official SMK PDF documentation (Old and New versions)
- Excel export specifications and templates
- Database validation for SMK field mappings

#### SMK PDF Analysis
```bash
# Analyze official SMK PDFs for field requirements
claude --mcp-config mcp-smk-config.json -p "Analyze the official Old SMK PDF and extract all Excel field mappings for medical shifts" \
  --allowedTools "mcp__smk-docs__read_file,mcp__memory__store"

# Compare Old vs New SMK requirements
claude --mcp-config mcp-smk-config.json -p "Compare procedure requirements between Old and New SMK PDFs" \
  --allowedTools "mcp__smk-docs__read_file,mcp__memory__store"

# Extract exact Excel column specifications
claude --mcp-config mcp-smk-config.json -p "Extract the exact Excel column headers and formats from New SMK PDF for internships (staże)" \
  --allowedTools "mcp__smk-docs__read_file,mcp__memory__store"
```

#### Excel Export Implementation
```bash
# Implement Excel generator based on PDF specs
claude --mcp-config mcp-smk-config.json -p "Implement Excel export for medical shifts matching the exact format in the New SMK PDF" \
  --allowedTools "mcp__smk-docs__read_file,mcp__smk-docs__write_file,mcp__memory__recall"

# Validate Excel format against SMK requirements
claude --mcp-config mcp-smk-config.json -p "Validate our Excel export format against the official SMK import specifications" \
  --allowedTools "mcp__smk-docs__read_file,mcp__postgres__query"

# Generate test Excel files
claude --mcp-config mcp-smk-config.json -p "Generate test Excel files with sample data matching SMK format for Chrome extension testing" \
  --allowedTools "mcp__smk-docs__write_file,mcp__postgres__query,mcp__memory__recall"
```

#### SMK Field Mapping
```bash
# Store official field mappings
claude --mcp-config mcp-smk-config.json -p "Extract and store all field mappings from SMK PDFs: database fields to Excel columns" \
  --allowedTools "mcp__smk-docs__read_file,mcp__memory__store"

# Validate database schema against SMK
claude --mcp-config mcp-smk-config.json -p "Validate our database schema has all required fields for SMK export" \
  --allowedTools "mcp__postgres__schema,mcp__memory__recall"

# Check for missing SMK fields
claude --mcp-config mcp-smk-config.json -p "Identify any missing fields in our system that are required by SMK" \
  --allowedTools "mcp__smk-docs__read_file,mcp__postgres__schema,mcp__memory__recall"
```

#### Common SMK Compliance Tasks
```bash
# 1. Initial PDF analysis and storage
claude --mcp-config mcp-smk-config.json -p "Analyze all SMK PDFs and store key requirements: field names, formats, validation rules" \
  --allowedTools "mcp__smk-docs__read_file,mcp__memory__store" \
  --max-turns 10

# 2. Implement complete Excel export
claude --mcp-config mcp-smk-config.json -p "Implement complete Excel export service for Old SMK format based on PDF specifications" \
  --allowedTools "mcp__smk-docs__read_file,mcp__smk-docs__write_file,mcp__memory__recall,mcp__postgres__schema"

# 3. Create validation tests
claude --mcp-config mcp-smk-config.json -p "Create comprehensive tests to validate Excel exports match SMK requirements exactly" \
  --allowedTools "mcp__smk-docs__read_file,mcp__smk-docs__write_file,mcp__memory__recall"

# 4. Chrome extension compatibility
claude --mcp-config mcp-smk-config.json -p "Generate Excel test files that can be imported by SMK Chrome extension" \
  --allowedTools "mcp__smk-docs__write_file,mcp__postgres__query,mcp__memory__recall"
```

#### Important SMK Compliance Notes
1. **PDF Location**: Place official SMK PDFs in `/home/ubuntu/projects/mock/docs/smk-pdfs/`
   - `old-smk-official.pdf` - Old SMK system documentation
   - `new-smk-official.pdf` - New SMK system documentation

2. **Excel Format Requirements**:
   - Date format: `DD.MM.YYYY` (Polish format)
   - Time format: `HH:MM` (24-hour)
   - Duration: Can have minutes > 59 (e.g., `10h 75min`)
   - Boolean: `Tak`/`Nie` (Polish Yes/No)
   - Encoding: UTF-8 with Polish characters

3. **Field Mapping Storage**:
   Use memory server to store extracted mappings:
   ```bash
   # Example: Store medical shift field mapping
   claude --mcp-config mcp-smk-config.json -p "Remember: SMK medical shifts Excel columns: A=Data dyżuru (DD.MM.YYYY), B=Godzina rozpoczęcia (HH:MM), C=Godzina zakończenia (HH:MM), D=Czas trwania (min), E=Miejsce, F=Typ dyżuru" \
     --allowedTools "mcp__memory__store"
   ```

4. **Validation Checklist**:
   - All required fields present
   - Correct data formats
   - Polish language for enums/booleans
   - Proper sheet names in Excel
   - Column headers match exactly

---

## 🎓 CMKP Specialization Management

### 🎯 When to Use This Section
**USE THIS SECTION WHEN:**
- User needs to import specialization requirements
- User mentions CMKP website PDFs
- User wants to add new medical specializations
- Working with specialization templates (JSON files)
- User mentions 70+ medical specializations

### Quick Start
```bash
# List all available specializations from CMKP
./cmkp-specialization-helper.sh list-specializations new

# Download and process a specialization
./cmkp-specialization-helper.sh download-pdf new alergologia
./cmkp-specialization-helper.sh parse-pdf /home/ubuntu/projects/mock/docs/cmkp-pdfs/new-smk/alergologia.pdf

# Import to database
./cmkp-specialization-helper.sh import-json /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Api/Data/SpecializationTemplates/alergologia_new.json
```

### CMKP URLs
- **New SMK (2023)**: https://www.cmkp.edu.pl/ksztalcenie/podyplomowe/lekarze-i-lekarze-dentysci/modulowe-programy-specjalizacji-lekarskich-2023
- **Old SMK (2014-2018)**: https://www.cmkp.edu.pl/ksztalcenie/podyplomowe/lekarze-i-lekarze-dentysci/modulowe-programy-specjalizacji-od-1-10-2014-aktualizacja-2018

### Specialization JSON Structure
```json
{
  "name": "Alergologia",
  "code": "alergologia",
  "version": "CMKP 2023",
  "totalDuration": { "years": 5, "months": 0, "days": 0 },
  "modules": [
    {
      "name": "Moduł podstawowy",
      "moduleType": "Basic",
      "courses": [...],
      "internships": [...],
      "procedures": [...]
    }
  ]
}
```

### Common CMKP Tasks

#### 1. Import All Specializations
```bash
# Download all PDFs for both versions
./cmkp-specialization-helper.sh download-all old
./cmkp-specialization-helper.sh download-all new

# Parse all PDFs to JSON
./cmkp-specialization-helper.sh parse-all old
./cmkp-specialization-helper.sh parse-all new

# Import all to database
./cmkp-specialization-helper.sh import-all
```

#### 2. Add New Specialization
```bash
# Download specific specialization
claude --mcp-config mcp-cmkp-config.json -p "Download Anestezjologia specialization PDF from new CMKP website" \
  --allowedTools "mcp__web-scraper__fetch,mcp__cmkp-docs__write_file"

# Generate JSON template
claude --mcp-config mcp-cmkp-config.json -p "Parse Anestezjologia PDF and create JSON template matching cardiology structure" \
  --allowedTools "mcp__cmkp-docs__read_file,mcp__cmkp-docs__write_file" \
  --max-turns 10

# Import to database
claude --mcp-config mcp-cmkp-config.json -p "Import anestezjologia_new.json to database specialization templates" \
  --allowedTools "mcp__cmkp-docs__read_file,mcp__postgres__query"
```

#### 3. Update Existing Specialization
```bash
# Compare versions
./cmkp-specialization-helper.sh compare-versions kardiologia

# Update JSON with changes
claude --mcp-config mcp-cmkp-config.json -p "Update cardiology_new.json with latest CMKP requirements" \
  --allowedTools "mcp__cmkp-docs__read_file,mcp__cmkp-docs__write_file,mcp__web-scraper__fetch"
```

#### 4. Find Missing Specializations
```bash
# Check what's missing
./cmkp-specialization-helper.sh missing-specializations

# Bulk download missing ones
claude --mcp-config mcp-cmkp-config.json -p "Download all missing specializations from CMKP and create JSON templates" \
  --allowedTools "mcp__web-scraper__fetch,mcp__cmkp-docs__write_file,mcp__memory__store" \
  --max-turns 50
```

### Directory Structure
```
SledzSpecke.WebApi/src/SledzSpecke.Api/Data/SpecializationTemplates/
├── alergologia_new.json
├── alergologia_old.json
├── anestezjologia_new.json
├── anestezjologia_old.json
├── cardiology_new.json      # Already exists
├── cardiology_old.json      # Already exists
├── psychiatry_new.json      # Already exists
├── psychiatry_old.json      # Already exists
└── ... (70+ specializations × 2 versions)

docs/cmkp-pdfs/
├── old-smk/
│   ├── alergologia.pdf
│   ├── anestezjologia.pdf
│   └── ...
└── new-smk/
    ├── alergologia.pdf
    ├── anestezjologia.pdf
    └── ...
```

### Important Notes
1. **Always download the FIRST PDF link** for each specialization (contains full program)
2. **Use exact CMKP naming** for consistency
3. **Both versions needed** - Old SMK (2014) and New SMK (2023)
4. **Validate JSON structure** before importing to database
5. **UTF-8 encoding** required for Polish characters

### 🚀 Current Implementation Status (December 2024)
- ✅ **Database Schema**: SpecializationTemplates table created with proper constraints
- ✅ **Entity & Repository**: SpecializationTemplateDefinition entity and repository implemented
- ✅ **Import Service**: Full CRUD operations with validation
- ✅ **Admin API**: Complete REST endpoints at `/api/admin/specialization-templates`
- ✅ **DataSeeder**: Updated to use dynamic template loading
- ✅ **Helper Scripts**: CMKP import scripts ready for use
- ✅ **Authorization**: AdminOnly policy configured and working
- 📊 **Current Data**: 5 specializations in templates (cardiology, psychiatry, alergologia, anestezjologia, chirurgia) - 72+ remaining to import

### 🔧 Quick Setup for Full Import
```bash
# 1. Configure AdminOnly policy in Program.cs
# Add: options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

# 2. Run the API
dotnet run --project src/SledzSpecke.Api

# 3. Import all specializations
./cmkp-specialization-helper.sh import-all

# 4. Verify import
curl https://api.sledzspecke.pl/api/admin/specialization-templates | jq length
# Should return: 77
```

### Database Import Process
```bash
# The import process will:
# 1. Create SpecializationTemplate records
# 2. Create Module records (Basic + Specialist)
# 3. Create Course templates
# 4. Create Internship templates  
# 5. Create Procedure requirements

# Verify import
claude --mcp-config mcp-cmkp-config.json -p "List all imported specialization templates from database" \
  --allowedTools "mcp__postgres__query"
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

## 📊 Current Implementation Details (December 2024)

### Entities & Value Objects
- **23 Domain Entities** - All with enhanced versions using value objects
- **71 Value Objects** - Complete elimination of primitive obsession
- **Specification Classes** - 30+ reusable specifications for complex queries

### Repository Implementation (100% Complete)
All repositories now use the specification pattern with BaseRepository:
```csharp
// Example: RefactoredSqlUserRepository
public class RefactoredSqlUserRepository : BaseRepository<User>, IUserRepository
{
    public RefactoredSqlUserRepository(SledzSpeckeDbContext context, ILogger<RefactoredSqlUserRepository> logger)
        : base(context, logger) { }
}
```

### API Endpoints
**Controllers (Traditional REST):**
- Auth, Users, MedicalShifts, Procedures, Courses
- Modules, Internships, Specializations, Publications
- Dashboard, SMK, Export, Monitoring
- Admin/SpecializationTemplate

**Minimal APIs (Modern):**
- MedicalShiftEndpoints
- InternshipEndpoints
- ModuleEndpoints
- Users endpoints
- SMKIntegration endpoints

### Recent Architectural Improvements
1. **Feature Folders** - `/Application/Features/*` organized by feature
2. **Message Pipeline** - Enhanced with decorators for cross-cutting concerns
3. **Outbox Pattern** - For reliable event publishing
4. **CMKP Integration** - Full specialization template management
5. **Monitoring** - Comprehensive logging and dashboards

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

## 🏗️ CRITICAL: Current Architecture State (95% Complete)

### ✅ Already Implemented - DO NOT REBUILD
1. **Value Objects** - Perfect implementation, 71 value objects eliminating primitive obsession
2. **Specification Pattern** - Fully implemented with composable queries
3. **Domain Events** - MediatR configured, handlers ready
4. **Decorator Pattern** - All cross-cutting concerns handled
5. **Result Pattern** - Consistent error handling throughout
6. **E2E Testing** - World-class Playwright implementation with DB isolation
7. **Repository Pattern** - ALL 13 repositories refactored with specification support ✅
8. **Feature Folders** - Complete migration to vertical slice architecture
9. **Minimal APIs** - Modern endpoints implemented alongside controllers
10. **Outbox Pattern** - Reliable messaging infrastructure in place

### 🔧 Remaining Work (Priority Order)
1. **Domain Services** - Add real SMK business logic to stubs
2. **Integration Tests** - Fix remaining test failures (authentication issues)
3. **E2E Tests** - Fix frontend connectivity issues (27/31 tests failing)

### ⚠️ DO NOT
- Recreate Value Objects (they're perfect)
- Change the architecture layers
- Add new patterns without clear justification
- Create entities/VOs that aren't used in E2E tests
- Add complexity for theoretical scenarios
- Refactor repositories again (ALL are complete)

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
- **Unit Tests**: 132/134 passing (98.5% pass rate)
- **Integration Tests**: Need updates due to API structure changes
- **E2E Tests**: 4/31 passing (frontend connectivity issues)
- **Total**: 64+ tests across all projects
- **Architecture**: Rock-solid with proper patterns implemented

---

## 🎯 Development Priorities

### ✅ Completed (December 2024)
- **ALL Repositories Migrated** - 13/13 using specification pattern
- **Feature Folder Structure** - Complete vertical slice architecture
- **Minimal APIs** - Modern endpoints implemented
- **Outbox Pattern** - Reliable messaging ready

### 📋 Next Focus Areas
1. **Domain Logic** - Implement real SMK business rules in domain services
2. **Test Fixes** - Update integration tests for new API structure
3. **E2E Connectivity** - Fix frontend URL issues in E2E tests
4. **Performance Monitoring** - Add metrics to new endpoints

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

## 📝 Documentation Structure

```
docs/
├── api/
│   └── API_DOCUMENTATION.md         # Complete API reference
├── architecture/
│   ├── COMPLETE_ARCHITECTURE.md     # System architecture & patterns
│   └── TESTING_STRATEGY.md          # Testing guide (unit/integration/E2E)
├── compliance/
│   ├── sledzspecke-business-logic.md     # Core SMK business rules
│   ├── sledzspecke-compliance-roadmap.md # Implementation roadmap
│   ├── sledzspecke-compliance-status.md  # Current compliance status
│   └── smk-testing-validation.md         # SMK validation requirements
└── deployment/
    ├── DEPLOYMENT-OPTIONS.md        # Deployment strategies
    ├── monitoring-plan.md           # Monitoring & observability
    └── monitoring-runbook.md        # Operations runbook
```

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

### Build Frequency & Verification
- **IMPORTANT**: Build the project frequently during development to catch errors early
- **Use**: `dotnet build` after making changes to verify compilation
- **Before pushing**: Always run `dotnet build` to ensure no build errors
- **Strategy**: Write code incrementally and build often to avoid accumulating errors

### Build Commands
```bash
# Quick build check
dotnet build

# Build with detailed output
dotnet build -v detailed

# Clean and rebuild
dotnet clean && dotnet build

# Build specific project
dotnet build src/SledzSpecke.Api
```

### Current Warnings (Acceptable)
- Nullable reference warnings
- Unused parameter warnings in stubs
- These are tracked but not blocking

### GitHub Actions
- Builds on push to master/develop
- E2E tests run automatically
- Results at: https://api.sledzspecke.pl/e2e-results/
- Build logs saved locally at: `/var/log/github-actions/builds/`

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

---

## 📌 Quick MCP Reference

### For General Development:
```bash
claude --mcp-config mcp-config.json -p "Your task here"
```

### For SMK/Excel Export Work:
```bash
claude --mcp-config mcp-smk-config.json -p "Your SMK-related task here"
```

### For CMKP Specialization Management:
```bash
claude --mcp-config mcp-cmkp-config.json -p "Your CMKP specialization task here"
```

**Remember**: 
- Excel export/SMK compliance → use `mcp-smk-config.json`
- Specialization requirements/CMKP → use `mcp-cmkp-config.json`