# SledzSpecke SMK Compliance Roadmap & Implementation Guide

## 🚨 Executive Summary

The current SledzSpecke implementation has significant gaps compared to SMK requirements. This roadmap provides a systematic approach to achieve 100% SMK compliance while maintaining the existing clean architecture.

### Critical Issues to Address:
- ✅ **User entity** missing essential medical fields (PESEL, PWZ) - **COMPLETED**
- ✅ **SMK version** stored as integer instead of string ("old"/"new") - **COMPLETED** 
- ✅ **Missing core fields in Specialization**: ProgramVariant, PlannedPesYear, ActualEndDate, Status - **COMPLETED**
- ⏳ **Missing core entities**: AdditionalSelfEducationDays
- ⏳ **No XLSX export** functionality
- ⏳ **Incomplete business rules** implementation

## 📋 Phase 1: Core Entity Alignment (Week 1-2) ✅ COMPLETED

### 1.1 Fix User Entity ✅ **COMPLETED**

**Current State:**
- ✅ Added: FirstName, LastName, Pesel, PwzNumber, PhoneNumber, DateOfBirth, CorrespondenceAddress
- ✅ Removed: Username, Bio, ProfilePicturePath, PreferredLanguage, PreferredTheme

**Implementation:**
- Created new value objects: Pesel (with checksum validation), PwzNumber, FirstName, LastName, Address
- Updated User entity with proper factory methods
- Implemented PESEL validation with full checksum verification
- Implemented PWZ validation (7 digits, cannot start with 0)

### 1.2 Fix SMK Version Type ✅ **COMPLETED**

**Current State:** Already correctly implemented as string type with values "old" or "new"

### 1.3 Update Specialization Entity ✅ **COMPLETED**

**Added fields:**
- UserId (to establish User-Specialization relationship)
- ProgramVariant (Wariant programu)
- PlannedPesYear (Planned PES exam year)  
- ActualEndDate
- SpecializationStatus (Active, Completed, Suspended, Cancelled)

## 📋 Phase 2: Complete Missing Entities (Week 2-3) ⏳ PENDING

### 2.1 Implement Procedure Entity Hierarchy

**Create discriminated entities for Old/New SMK:**

```csharp
// Base Procedure class
public abstract class Procedure : Entity
{
    public int Id { get; protected set; }
    public int ModuleId { get; protected set; }
    public string ProcedureCode { get; protected set; }
    public DateTime Date { get; protected set; }
    public string Location { get; protected set; }
    
    // Discriminator
    public abstract string SmkType { get; }
}

// Old SMK version
public sealed class ProcedureOldSmk : Procedure
{
    public override string SmkType => "old";
    public string ProcedureName { get; private set; }
    public int YearOfTraining { get; private set; }
    public string InternshipName { get; private set; }
    public string PatientInitials { get; private set; }
    public Gender PatientGender { get; private set; }
    public string FirstAssistantData { get; private set; }
    public string SecondAssistantData { get; private set; }
    public ProcedureRole Role { get; private set; } // A or B
}

// New SMK version
public sealed class ProcedureNewSmk : Procedure
{
    public override string SmkType => "new";
    public int ProcedureRequirementId { get; private set; }
    public string ProcedureName { get; private set; }
    public int CountA { get; private set; } // Performed count
    public int CountB { get; private set; } // Assisted count
    public string Supervisor { get; private set; }
}
```

### 2.2 Add AdditionalSelfEducationDays Entity

```csharp
public sealed class AdditionalSelfEducationDays : Entity
{
    public int Id { get; private set; }
    public int ModuleId { get; private set; }
    public int InternshipId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int NumberOfDays { get; private set; } // Max 6 per year
    public string Purpose { get; private set; }
    public string EventName { get; private set; }
    
    // Validation in factory method
    public static Result<AdditionalSelfEducationDays> Create(
        int moduleId, int internshipId, DateTime start, DateTime end, 
        string purpose, string eventName)
    {
        var days = (end - start).Days + 1;
        if (days > 6)
            return Result<AdditionalSelfEducationDays>.Failure(
                "Additional education cannot exceed 6 days", "EXCEEDED_DAYS_LIMIT");
                
        // Create instance
    }
}
```

## 📋 Phase 3: Business Rules Implementation (Week 3-4) ⏳ PENDING

### 3.1 Validation Rules ✅ PARTIALLY COMPLETED

**PESEL Validation:** ✅ **COMPLETED**
- Full checksum validation implemented
- Date of birth extraction from PESEL
- Gender extraction from PESEL

**PWZ Validation:** ✅ **COMPLETED**
- 7 digits validation
- Cannot start with 0

### 3.2 Duration Calculations ⏳ PENDING

**Medical Shift Duration (allows > 59 minutes):**
```csharp
public record ShiftDuration
{
    public int Minutes { get; }
    
    public ShiftDuration(int minutes)
    {
        if (minutes < 60)
            throw new InvalidShiftDurationException(
                "Minimum shift duration is 60 minutes");
        Minutes = minutes;
    }
    
    public string ToDisplayFormat()
    {
        var hours = Minutes / 60;
        var mins = Minutes % 60;
        return $"{hours}h {mins}min";
    }
}
```

### 3.3 Module Progression Rules ⏳ PENDING

```csharp
public class ModuleProgressionService : IModuleProgressionService
{
    public Result<Module> CanProgressToModule(
        Specialization specialization, 
        ModuleType targetType)
    {
        if (targetType == ModuleType.Specialist)
        {
            var basicModule = specialization.Modules
                .FirstOrDefault(m => m.Type == ModuleType.Basic);
                
            if (basicModule == null || !basicModule.IsCompleted)
            {
                return Result<Module>.Failure(
                    "Basic module must be completed before specialist module",
                    "BASIC_MODULE_NOT_COMPLETED");
            }
        }
        
        // Additional validation...
    }
}
```

## 📋 Phase 4: XLSX Export Implementation (Week 4-5) ⏳ PENDING

### 4.1 Export Service Architecture

```csharp
public interface ISpecializationExportService
{
    Task<Result<byte[]>> ExportToXlsxAsync(int specializationId);
    Task<Result<ExportPreview>> PreviewExportAsync(int specializationId);
    Task<Result<ValidationReport>> ValidateForExportAsync(int specializationId);
}

public class SmkExportService : ISpecializationExportService
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IExcelGenerator _excelGenerator;
    private readonly ISmkValidator _smkValidator;
    
    public async Task<Result<byte[]>> ExportToXlsxAsync(int specializationId)
    {
        // 1. Load complete specialization data
        var specialization = await LoadCompleteSpecialization(specializationId);
        
        // 2. Validate data
        var validationResult = await _smkValidator.ValidateAsync(specialization);
        if (!validationResult.IsValid)
            return Result<byte[]>.Failure(validationResult.Errors);
            
        // 3. Generate Excel file
        return await _excelGenerator.GenerateAsync(specialization);
    }
}
```

### 4.2 Excel Generator with ClosedXML

```csharp
public class SmkExcelGenerator : IExcelGenerator
{
    public async Task<byte[]> GenerateAsync(SpecializationExportData data)
    {
        using var workbook = new XLWorkbook();
        
        // Sheet 1: Basic Information
        var basicSheet = workbook.Worksheets.Add("Informacje podstawowe");
        GenerateBasicInfoSheet(basicSheet, data);
        
        // Sheet 2: Internships
        var internshipsSheet = workbook.Worksheets.Add("Staże");
        GenerateInternshipsSheet(internshipsSheet, data.Internships);
        
        // Sheet 3: Courses
        var coursesSheet = workbook.Worksheets.Add("Kursy");
        GenerateCoursesSheet(coursesSheet, data.Courses);
        
        // Sheet 4: Medical Shifts
        var shiftsSheet = workbook.Worksheets.Add("Dyżury");
        GenerateMedicalShiftsSheet(shiftsSheet, data.MedicalShifts);
        
        // Sheet 5: Procedures
        var proceduresSheet = workbook.Worksheets.Add("Procedury");
        GenerateProceduresSheet(proceduresSheet, data.Procedures);
        
        // Return as byte array
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
    
    private void GenerateInternshipsSheet(IXLWorksheet sheet, List<InternshipExport> internships)
    {
        // Headers
        sheet.Cell(1, 1).Value = "Nazwa stażu";
        sheet.Cell(1, 2).Value = "Nazwa podmiotu";
        sheet.Cell(1, 3).Value = "Komórka organizacyjna";
        sheet.Cell(1, 4).Value = "Data rozpoczęcia";
        sheet.Cell(1, 5).Value = "Data zakończenia";
        sheet.Cell(1, 6).Value = "Czas trwania (dni)";
        sheet.Cell(1, 7).Value = "Kierownik stażu";
        sheet.Cell(1, 8).Value = "PWZ kierownika";
        
        // Data
        int row = 2;
        foreach (var internship in internships)
        {
            sheet.Cell(row, 1).Value = internship.Name;
            sheet.Cell(row, 2).Value = internship.InstitutionName;
            sheet.Cell(row, 3).Value = internship.DepartmentName;
            sheet.Cell(row, 4).Value = internship.StartDate.ToString("dd.MM.yyyy");
            sheet.Cell(row, 5).Value = internship.EndDate.ToString("dd.MM.yyyy");
            sheet.Cell(row, 6).Value = internship.DaysCount;
            sheet.Cell(row, 7).Value = internship.SupervisorName;
            sheet.Cell(row, 8).Value = internship.SupervisorPwz;
            row++;
        }
    }
}
```

## 📋 Phase 5: API Adjustments (Week 5-6) ✅ PARTIALLY COMPLETED

### 5.1 Update DTOs to Match SMK Fields ✅ **COMPLETED**

Updated DTOs:
- UserRegistrationDto with all new fields
- UserProfileDto with new structure
- AddressDto for correspondence address

### 5.2 Add Export Endpoints ⏳ PENDING

```csharp
[ApiController]
[Route("api/export")]
public class ExportController : ControllerBase
{
    [HttpGet("specialization/{id}/xlsx")]
    [ProducesResponseType(typeof(FileResult), 200)]
    public async Task<IActionResult> ExportToXlsx(int id)
    {
        var result = await _exportService.ExportToXlsxAsync(id);
        if (result.IsFailure)
            return BadRequest(result.Error);
            
        return File(result.Value, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"EKS_{id}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
    
    [HttpGet("specialization/{id}/preview")]
    public async Task<IActionResult> PreviewExport(int id)
    {
        var result = await _exportService.PreviewExportAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
    
    [HttpPost("validate/{id}")]
    public async Task<IActionResult> ValidateForExport(int id)
    {
        var result = await _exportService.ValidateForExportAsync(id);
        return Ok(result);
    }
}
```

## 📋 Phase 6: Testing & Validation (Week 6-7) ⏳ PENDING

### 6.1 SMK Compatibility Tests

```csharp
[TestFixture]
public class SmkExportTests
{
    [Test]
    public async Task Export_ShouldGenerateValidSmkFormat()
    {
        // Arrange
        var specialization = CreateTestSpecialization();
        
        // Act
        var exportResult = await _exportService.ExportToXlsxAsync(specialization.Id);
        
        // Assert
        exportResult.IsSuccess.Should().BeTrue();
        
        // Validate Excel structure
        using var workbook = new XLWorkbook(new MemoryStream(exportResult.Value));
        workbook.Worksheets.Count.Should().Be(5);
        
        // Validate date formats
        var internshipSheet = workbook.Worksheet("Staże");
        var dateCell = internshipSheet.Cell(2, 4).Value.ToString();
        dateCell.Should().MatchRegex(@"\d{2}\.\d{2}\.\d{4}"); // DD.MM.YYYY
    }
}
```

### 6.2 Field Validation Tests ✅ **PARTIALLY COMPLETED**

- SmkVersion validation: Already correctly validates "old" or "new"
- PESEL validation with checksum: ✅ Implemented
- PWZ validation: ✅ Implemented

## 🚀 Migration Strategy

### Step 1: Database Backup
```bash
sudo -u postgres pg_dump sledzspecke_db > backup_$(date +%Y%m%d).sql
```

### Step 2: Create Migration Scripts ⏳ PENDING
1. User entity changes
2. Specialization entity changes
3. Add missing tables
4. Data migration for existing records

### Step 3: Deploy in Stages
1. Deploy entity changes with backward compatibility
2. Run data migrations
3. Deploy API changes
4. Remove deprecated fields

## ⚠️ Critical Success Factors

1. **Exact Field Matching**: Every field name, type, and format must match SMK exactly
2. **Date Format**: Always use DD.MM.YYYY format in exports
3. **SMK Version**: Must be string "old" or "new", never numeric
4. **No Approval Workflows**: Remove any approval states - all handled in SMK after import
5. **Complete Data**: All required fields must be present for successful import

## 📊 Progress Tracking

Create a tracking dashboard:
- [x] User entity aligned with SMK
- [x] SMK version converted to string
- [x] Specialization entity updated
- [ ] All entities implemented
- [ ] Business rules validated
- [ ] Export functionality complete
- [ ] E2E tests passing
- [ ] Production deployment

## 🔍 Validation Checklist

Before marking as complete, ensure:
- [x] PESEL validation with checksum
- [x] PWZ format validation (7 digits)
- [ ] Date formats: DD.MM.YYYY
- [ ] Time formats: HH:MM
- [ ] Duration in minutes (can be > 59)
- [x] SMK version as string
- [ ] All required fields present
- [ ] Excel export matches SMK import format exactly

## 📈 Monitoring Post-Deployment

1. Track export success rate
2. Monitor validation failures
3. Log SMK import issues
4. User feedback on missing fields

## 🔴 CURRENT STATUS (2025-06-16)

### Completed:
- ✅ Phase 1.1: User Entity Updates (PESEL, PWZ, names, address)
- ✅ Phase 1.2: SMK Version Verification
- ✅ Phase 1.3: Specialization Entity Updates
- ✅ Value object validations (PESEL with checksum, PWZ)
- ✅ API DTOs updated for new entity structure
- ✅ Build successful, core tests passing (135/135)

### Next Steps:
- ⏳ Phase 2: Implement missing entities (Procedure hierarchy, AdditionalSelfEducationDays)
- ⏳ Phase 3: Complete business rules implementation
- ⏳ Phase 4: XLSX Export Service
- ⏳ Database migrations
- ⏳ Update integration tests

### Technical Debt:
- User-Specialization relationship needs proper redesign (currently many handlers are commented out)
- Infrastructure repositories have commented methods awaiting new relationship model
- Integration tests need updating for new domain model

This roadmap ensures 100% SMK compliance while maintaining clean architecture and providing a superior user experience compared to the official SMK system.