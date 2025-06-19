# CMKP Specialization Import Implementation Plan

## Overview
This plan outlines the implementation of a comprehensive system to import and manage 70+ medical specializations from CMKP (Centrum Medyczne Kształcenia Podyplomowego) into the SledzSpecke system.

## Current State
- Only 4 specializations are imported (cardiology and psychiatry for old/new SMK)
- Import happens automatically on first startup via `DataSeeder.cs`
- No mechanism to add new specializations after deployment
- JSON templates are hardcoded in the seeding process

## Implementation Plan

### Phase 1: Create Admin API Endpoints (Priority: High)

#### 1.1 Create SpecializationTemplateController
```csharp
// Location: src/SledzSpecke.Api/Controllers/Admin/SpecializationTemplateController.cs
[ApiController]
[Route("api/admin/specialization-templates")]
[Authorize(Policy = "AdminOnly")] // Create admin policy
public class SpecializationTemplateController : ControllerBase
{
    // GET: List all templates
    [HttpGet]
    public async Task<IActionResult> GetTemplates()
    
    // GET: Get specific template
    [HttpGet("{code}/{version}")]
    public async Task<IActionResult> GetTemplate(string code, string version)
    
    // POST: Import single template from JSON
    [HttpPost("import")]
    public async Task<IActionResult> ImportTemplate([FromBody] SpecializationTemplateDto template)
    
    // POST: Import multiple templates from directory
    [HttpPost("import-bulk")]
    public async Task<IActionResult> ImportBulkFromDirectory()
    
    // PUT: Update existing template
    [HttpPut("{code}/{version}")]
    public async Task<IActionResult> UpdateTemplate(string code, string version, [FromBody] SpecializationTemplateDto template)
    
    // DELETE: Remove template
    [HttpDelete("{code}/{version}")]
    public async Task<IActionResult> DeleteTemplate(string code, string version)
    
    // POST: Validate template before import
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateTemplate([FromBody] SpecializationTemplateDto template)
}
```

#### 1.2 Create Import Service
```csharp
// Location: src/SledzSpecke.Application/Services/SpecializationTemplateImportService.cs
public interface ISpecializationTemplateImportService
{
    Task<Result<int>> ImportTemplateAsync(SpecializationTemplateDto template);
    Task<Result<List<int>>> ImportFromDirectoryAsync(string directoryPath);
    Task<Result<bool>> ValidateTemplateAsync(SpecializationTemplateDto template);
    Task<Result<int>> UpdateTemplateAsync(string code, string version, SpecializationTemplateDto template);
}
```

### Phase 2: Enhance Database Schema (Priority: High)

#### 2.1 Add SpecializationTemplate Table
```sql
CREATE TABLE SpecializationTemplates (
    Id INT PRIMARY KEY IDENTITY,
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Version NVARCHAR(20) NOT NULL, -- "CMKP 2014" or "CMKP 2023"
    JsonContent NVARCHAR(MAX) NOT NULL, -- Store full JSON
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_SpecializationTemplate_Code_Version UNIQUE (Code, Version)
);
```

#### 2.2 Create Migration
```bash
dotnet ef migrations add AddSpecializationTemplateTable -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

### Phase 3: Implement Dynamic Template Loading (Priority: High)

#### 3.1 Create Template Repository
```csharp
// Location: src/SledzSpecke.Infrastructure/Repositories/SpecializationTemplateRepository.cs
public class SpecializationTemplateRepository : ISpecializationTemplateRepository
{
    Task<SpecializationTemplate> GetByCodeAndVersionAsync(string code, string version);
    Task<List<SpecializationTemplate>> GetAllActiveAsync();
    Task<int> CreateAsync(SpecializationTemplate template);
    Task UpdateAsync(SpecializationTemplate template);
    Task<bool> ExistsAsync(string code, string version);
}
```

#### 3.2 Update DataSeeder to Use Templates from Database
```csharp
// Modify DataSeeder.cs to:
// 1. Check SpecializationTemplates table first
// 2. If empty, import from JSON files
// 3. Allow re-seeding with force flag
```

### Phase 4: Create CMKP PDF Processing Pipeline (Priority: Medium)

#### 4.1 Create PDF Parser Service
```csharp
// Location: src/SledzSpecke.Application/Services/CmkpPdfParserService.cs
public interface ICmkpPdfParserService
{
    Task<Result<SpecializationTemplateDto>> ParsePdfAsync(string pdfPath);
    Task<Result<List<SpecializationTemplateDto>>> ParseDirectoryAsync(string directoryPath);
}
```

#### 4.2 Implement Parsing Logic
- Extract specialization name and code
- Parse module structure (basic + specialist)
- Extract courses with durations
- Extract internships with weeks/days
- Extract procedures with requirements
- Handle both old and new SMK formats

### Phase 5: Create Validation System (Priority: Medium)

#### 5.1 Template Validator
```csharp
// Location: src/SledzSpecke.Application/Validators/SpecializationTemplateValidator.cs
public class SpecializationTemplateValidator
{
    // Validate required fields
    // Check duration calculations
    // Verify module structure
    // Validate course/internship/procedure arrays
    // Check for duplicates
}
```

### Phase 6: Create Management UI (Priority: Low)

#### 6.1 Admin Dashboard Pages
- `/admin/specialization-templates` - List all templates
- `/admin/specialization-templates/import` - Import new templates
- `/admin/specialization-templates/{code}/{version}` - View/Edit template
- `/admin/specialization-templates/validate` - Validation tool

### Phase 7: Implement Automated Import Workflow (Priority: Medium)

#### 7.1 Create Scheduled Job
```csharp
// Check CMKP website for updates
// Download new/updated PDFs
// Parse and validate
// Import to database
// Send notification of changes
```

## Implementation Steps

### Step 1: Database Preparation
```bash
# Create migration for SpecializationTemplate table
dotnet ef migrations add AddSpecializationTemplateManagement -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api

# Update database
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

### Step 2: Create Core Services
1. Implement `SpecializationTemplateRepository`
2. Implement `SpecializationTemplateImportService`
3. Create `SpecializationTemplateValidator`
4. Update `DataSeeder` to use new system

### Step 3: Create API Endpoints
1. Implement `SpecializationTemplateController`
2. Add admin authorization policy
3. Create DTOs for template import/export
4. Add Swagger documentation

### Step 4: Import Existing Templates
```bash
# Use CMKP helper to download all PDFs
./cmkp-specialization-helper.sh download-all old
./cmkp-specialization-helper.sh download-all new

# Parse all PDFs to JSON
./cmkp-specialization-helper.sh parse-all old
./cmkp-specialization-helper.sh parse-all new

# Import via new API endpoint
curl -X POST https://api.sledzspecke.pl/api/admin/specialization-templates/import-bulk \
  -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json"
```

### Step 5: Testing
1. Unit tests for import service
2. Integration tests for API endpoints
3. E2E tests for full import workflow
4. Validation tests for all 70+ specializations

## MCP Commands for Implementation

### Initial Setup
```bash
# Create the implementation structure
claude --mcp-config mcp-cmkp-config.json -p "Create SpecializationTemplateController with all CRUD endpoints for managing specialization templates" \
  --allowedTools "mcp__cmkp-docs__write_file"

# Create the import service
claude --mcp-config mcp-cmkp-config.json -p "Implement SpecializationTemplateImportService with validation and bulk import capabilities" \
  --allowedTools "mcp__cmkp-docs__write_file,mcp__cmkp-docs__read_file"

# Generate migration
claude --mcp-config mcp-cmkp-config.json -p "Create EF Core migration for SpecializationTemplate table with proper constraints" \
  --allowedTools "mcp__cmkp-docs__write_file"
```

### Import All Specializations
```bash
# Download and process all specializations
claude --mcp-config mcp-cmkp-config.json -p "Download all specialization PDFs from CMKP for both old and new SMK versions" \
  --allowedTools "mcp__web-scraper__fetch,mcp__cmkp-docs__write_file" \
  --max-turns 50

# Convert all PDFs to JSON
claude --mcp-config mcp-cmkp-config.json -p "Convert all CMKP PDFs to JSON format matching our specialization template structure" \
  --allowedTools "mcp__cmkp-docs__read_file,mcp__cmkp-docs__write_file" \
  --max-turns 100

# Import to database
claude --mcp-config mcp-cmkp-config.json -p "Import all specialization JSON templates to database using the new import service" \
  --allowedTools "mcp__cmkp-docs__read_file,mcp__postgres__query" \
  --max-turns 50
```

## Success Criteria

1. ✅ All 70+ specializations imported for both SMK versions
2. ✅ Admin can add/update/remove specializations via API
3. ✅ Validation ensures data integrity
4. ✅ System can detect and import new specializations from CMKP
5. ✅ Existing functionality remains unchanged
6. ✅ Full audit trail of template changes

## Timeline

- **Week 1**: Database schema and core services
- **Week 2**: API endpoints and import logic
- **Week 3**: PDF parsing and validation
- **Week 4**: Import all specializations and testing

## Notes

- Maintain backward compatibility with existing 4 specializations
- Use transactions for bulk imports
- Add comprehensive logging for audit trail
- Consider caching frequently accessed templates
- Plan for future updates from CMKP