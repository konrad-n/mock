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

### Overview
MCP servers provide Claude Code with direct access to tools and data sources, enabling powerful integrations without manual context switching. For SledzSpecke, this means direct access to databases, logs, and GitHub - all from within Claude Code.

### ⚡ Quick Decision Guide: Which MCP Config to Use?

#### Use `mcp-config.json` for:
- General development and debugging
- System health checks
- Log analysis
- GitHub integration
- Database queries (general)

#### Use `mcp-smk-config.json` for:
- **Analyzing official SMK PDFs**
- **Implementing Excel export features**
- **Ensuring 1:1 compliance with government SMK systems**
- **Extracting field mappings from PDFs**
- **Validating Excel format requirements**
- **Working with Chrome extension compatibility**

#### Use `mcp-cmkp-config.json` for:
- **Scraping CMKP specialization websites**
- **Downloading specialization requirement PDFs**
- **Converting PDFs to JSON templates**
- **Importing specialization data to database**
- **Managing all medical specializations (70+ types)**
- **Comparing Old vs New SMK specialization requirements**

### Configuration Files
- **General MCP**: `/home/ubuntu/projects/mock/mcp-config.json`
- **SMK Compliance**: `/home/ubuntu/projects/mock/mcp-smk-config.json`
- **CMKP Specializations**: `/home/ubuntu/projects/mock/mcp-cmkp-config.json`

### Available MCP Servers

#### 1. **Filesystem Server** - Direct file access
```bash
# Usage example - Read production logs directly
claude --mcp-config mcp-config.json -p "Analyze today's error logs from /var/log/sledzspecke" \
  --allowedTools "mcp__filesystem__read_file,mcp__filesystem__list_directory"

# Search for specific patterns in logs
claude --mcp-config mcp-config.json -p "Find all medical shift validation errors in logs" \
  --allowedTools "mcp__filesystem__read_file,mcp__filesystem__search_files"
```

**Available tools:**
- `mcp__filesystem__read_file` - Read any file content
- `mcp__filesystem__write_file` - Write to files (use with caution)
- `mcp__filesystem__list_directory` - List directory contents
- `mcp__filesystem__search_files` - Search file contents

#### 2. **PostgreSQL Server** - Database access
```bash
# Query the database directly
claude --mcp-config mcp-config.json -p "Show me all users registered in the last 7 days" \
  --allowedTools "mcp__postgres__query"

# Analyze database performance
claude --mcp-config mcp-config.json -p "Find slow queries affecting medical shifts" \
  --allowedTools "mcp__postgres__query,mcp__postgres__analyze"

# Debug specific user issues
claude --mcp-config mcp-config.json -p "Debug why user with email test@wum.edu.pl cannot log in" \
  --allowedTools "mcp__postgres__query"
```

**Available tools:**
- `mcp__postgres__query` - Execute SELECT queries
- `mcp__postgres__analyze` - Analyze query performance
- `mcp__postgres__schema` - View table schemas

#### 3. **GitHub Server** - Repository integration
```bash
# Check build status
claude --mcp-config mcp-config.json -p "Show me the status of the latest GitHub Actions builds" \
  --allowedTools "mcp__github__list_workflows,mcp__github__get_workflow_runs"

# Review recent PRs
claude --mcp-config mcp-config.json -p "Summarize open pull requests for SledzSpecke" \
  --allowedTools "mcp__github__list_pull_requests,mcp__github__get_pull_request"

# Create issues for bugs
claude --mcp-config mcp-config.json -p "Create a GitHub issue for the medical shift validation bug" \
  --allowedTools "mcp__github__create_issue"
```

**Available tools:**
- `mcp__github__list_workflows` - List GitHub Actions workflows
- `mcp__github__get_workflow_runs` - Get workflow run details
- `mcp__github__list_pull_requests` - List PRs
- `mcp__github__create_issue` - Create new issues
- `mcp__github__get_file_content` - Read files from repo

#### 4. **Memory Server** - Persistent context storage
```bash
# Store important SMK business rules
claude --mcp-config mcp-config.json -p "Remember: Medical shifts must not exceed 48 hours per week" \
  --allowedTools "mcp__memory__store"

# Recall stored information
claude --mcp-config mcp-config.json -p "What are the SMK weekly hour limits?" \
  --allowedTools "mcp__memory__recall"

# Store debugging context
claude --mcp-config mcp-config.json -p "Remember this user registration bug pattern for future debugging" \
  --allowedTools "mcp__memory__store"
```

**Available tools:**
- `mcp__memory__store` - Store information
- `mcp__memory__recall` - Retrieve stored information
- `mcp__memory__search` - Search stored memories
- `mcp__memory__list` - List all stored items

### Common MCP Workflows

#### 🔍 Production Debugging Workflow
```bash
# 1. Check application status and recent errors
claude --mcp-config mcp-config.json -p "Check SledzSpecke API health and find recent errors" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query"

# 2. Deep dive into specific error
claude --mcp-config mcp-config.json -p "Investigate the 'Invalid SMK version' error from user 12345" \
  --allowedTools "mcp__filesystem__search_files,mcp__postgres__query,mcp__memory__store"

# 3. Fix and verify
claude --mcp-config mcp-config.json -p "Fix the SMK version validation and verify with test data" \
  --allowedTools "mcp__filesystem__write_file,mcp__postgres__query"
```

#### 📊 Daily Operations Workflow
```bash
# Morning health check
claude --mcp-config mcp-config.json -p "Perform morning health check: API status, error count, new registrations" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__github__get_workflow_runs"

# User support
claude --mcp-config mcp-config.json -p "Help user jan.kowalski@uj.edu.pl who reports missing medical shifts" \
  --allowedTools "mcp__postgres__query,mcp__memory__recall"

# Performance monitoring
claude --mcp-config mcp-config.json -p "Analyze API performance for the last 24 hours" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__analyze"
```

#### 🚀 Development Workflow
```bash
# Feature development with context
claude --mcp-config mcp-config.json -p "Implement the new SMK module progression feature based on stored requirements" \
  --allowedTools "mcp__memory__recall,mcp__filesystem__write_file,mcp__postgres__schema"

# Code review with production data
claude --mcp-config mcp-config.json -p "Review the medical shift calculation logic against real production data" \
  --allowedTools "mcp__postgres__query,mcp__filesystem__read_file,mcp__github__get_file_content"

# Deployment verification
claude --mcp-config mcp-config.json -p "Verify the latest deployment succeeded and all services are healthy" \
  --allowedTools "mcp__github__get_workflow_runs,mcp__filesystem__read_file,mcp__postgres__query"
```

### Security & Best Practices

#### Tool Permissions
```bash
# ALWAYS specify exact tools needed
--allowedTools "mcp__postgres__query,mcp__filesystem__read_file"

# NEVER use write tools in production without careful consideration
--disallowedTools "mcp__filesystem__write_file,mcp__postgres__execute"

# For read-only operations
--allowedTools "mcp__postgres__query,mcp__filesystem__read_file,mcp__github__list*"
```

#### Environment Setup
```bash
# Set GitHub token for GitHub MCP server (required for GitHub integration)
export GITHUB_TOKEN="your-github-token"

# Add to ~/.bashrc for persistence
echo 'export GITHUB_TOKEN="your-github-token"' >> ~/.bashrc

# For SledzSpecke, create a token with these permissions:
# - repo (Full control of private repositories)
# - workflow (Update GitHub Action workflows)
# - read:org (Read org and team membership)

# Verify MCP configuration
claude --mcp-config mcp-config.json -p "List available MCP tools" --verbose

# Quick setup script
./setup-mcp.sh

# Use the helper script for common operations
./mcp-sledzspecke.sh health
```

### Troubleshooting MCP

#### Check MCP server status
```bash
claude --mcp-config mcp-config.json -p "Test MCP connections" --verbose
```

#### Common issues:
1. **"Tool not found"** - Ensure tool is in --allowedTools
2. **"Connection failed"** - Check server configuration in mcp-config.json
3. **"Permission denied"** - Verify file paths and database permissions
4. **"GitHub rate limit"** - Check GITHUB_TOKEN is set correctly

### Advanced MCP Usage

#### Batch Operations
```bash
# Analyze multiple aspects at once
claude --mcp-config mcp-config.json -p "Full system analysis: check logs, database health, GitHub builds, and stored issues" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__github__get_workflow_runs,mcp__memory__recall" \
  --max-turns 5
```

#### Custom Workflows
```bash
# Create a custom debugging session
claude --mcp-config mcp-config.json -p "Start debugging session for medical shift calculations" \
  --allowedTools "mcp__*" \
  --system-prompt "You are debugging the SledzSpecke medical shift system. Use all available MCP tools to investigate issues."
```

### MCP Integration with E2E Tests
```bash
# Run E2E tests and analyze results
claude --mcp-config mcp-config.json -p "Run E2E tests and analyze any failures using logs and database state" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__memory__store"

# Debug specific E2E test failures
claude --mcp-config mcp-config.json -p "Debug why the SMKWorkflow E2E test is failing" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__github__get_workflow_runs"
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
- ⚠️ **Authorization**: AdminOnly policy needs to be configured in Program.cs
- 📊 **Current Data**: 4 specializations seeded, 1 template imported (73 remaining)

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