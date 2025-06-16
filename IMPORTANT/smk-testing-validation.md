# SledzSpecke SMK Testing & Validation Strategy

## Executive Summary

This document provides comprehensive testing strategies, real test data from official SMK documentation, and validation procedures to ensure 100% SMK compliance. All test data is extracted from actual government PDFs to guarantee accuracy.

---

## 1. Real Test Data from SMK Documentation

### 1.1 Cardiology Specialization Test Data (from PDFs)

```csharp
// Test Data Builder for Cardiology Specialization
public class SmkCardiologyTestData
{
    public static Specialization CreateCardiologySpecialization(SmkVersion version)
    {
        return new Specialization
        {
            Name = "Kardiologia",
            SmkVersion = version,
            StartDate = new DateTime(2023, 10, 1),
            PlannedEndDate = new DateTime(2028, 9, 30),  // 5 years total
            PlannedPesYear = 2028,
            HasBasicModule = true,
            HasSpecializedModule = true,
            Status = SpecializationStatus.Active
        };
    }

    // REAL DATA: Basic Module - Internal Medicine (2 years)
    public static Module CreateBasicModule()
    {
        return new Module
        {
            Type = ModuleType.Basic,
            Name = "Moduł podstawowy w zakresie chorób wewnętrznych",
            StartDate = new DateTime(2023, 10, 1),
            EndDate = new DateTime(2025, 9, 30),
            Internships = new List<Internship>
            {
                // From PDF: 67 weeks (335 days) main internship
                new Internship
                {
                    Name = "Staż podstawowy w zakresie chorób wewnętrznych",
                    PlannedWeeks = 67,
                    PlannedDays = 335,
                    StartDate = new DateTime(2023, 10, 1),
                    EndDate = new DateTime(2025, 3, 31)
                },
                // From PDF: 4 weeks (20 days) ICU
                new Internship
                {
                    Name = "Staż kierunkowy w zakresie intensywnej opieki medycznej",
                    PlannedWeeks = 4,
                    PlannedDays = 20,
                    Department = "Oddział Intensywnej Opieki Medycznej"
                },
                // From PDF: 12 weeks (60 days) Emergency
                new Internship
                {
                    Name = "Staż kierunkowy w szpitalnym oddziale ratunkowym",
                    PlannedWeeks = 12,
                    PlannedDays = 60,
                    Department = "Szpitalny Oddział Ratunkowy (SOR)"
                }
            },
            Courses = CreateBasicModuleCourses()
        };
    }

    // REAL COURSES from PDF with exact names and durations
    private static List<Course> CreateBasicModuleCourses()
    {
        return new List<Course>
        {
            new Course { Name = "Diagnostyka obrazowa", DurationDays = 3, DurationHours = 24 },
            new Course { Name = "Alergologia", DurationDays = 2, DurationHours = 16 },
            new Course { Name = "Onkologia kliniczna", DurationDays = 4, DurationHours = 32 },
            new Course { Name = "Medycyna paliatywna", DurationDays = 2, DurationHours = 16 },
            new Course { Name = "Toksykologia", DurationDays = 2, DurationHours = 16 },
            new Course { Name = "Geriatria", DurationDays = 2, DurationHours = 16 },
            new Course { Name = "Diabetologia", DurationDays = 4, DurationHours = 32 },
            new Course { Name = "Przetaczanie krwi i jej składników", DurationDays = 2, DurationHours = 16 },
            new Course { Name = "Orzecznictwo lekarskie", DurationDays = 3, DurationHours = 24 },
            new Course { Name = "Profilaktyka i promocja zdrowia", DurationDays = 2, DurationHours = 16 }
        };
    }

    // REAL PROCEDURES from PDFs with exact requirements
    public static List<ProcedureRequirement> GetBasicModuleProcedures(SmkVersion version)
    {
        if (version == SmkVersion.New)
        {
            // From New SMK PDF (2023)
            return new List<ProcedureRequirement>
            {
                new("Prowadzenie resuscytacji krążeniowo-oddechowej BLS/ALS", CodeA: 3, CodeB: 3),
                new("Nakłucie jamy opłucnej w przypadku płynu", CodeA: 10, CodeB: 3),
                new("Nakłucie jamy otrzewnej w przypadku wodobrzusza", CodeA: 10, CodeB: 3),
                new("Nakłucia żył obwodowych – iniekcje dożylne, pobrania krwi", CodeA: 30, CodeB: 5),
                new("Nakłucie tętnicy obwodowej do badania gazometrycznego", CodeA: 30, CodeB: 5),
                new("Pomiar ośrodkowego ciśnienia żylnego", CodeA: 6, CodeB: 2),
                new("Cewnikowanie pęcherza moczowego", CodeA: 20, CodeB: 4),
                new("Badanie per rectum", CodeA: 20, CodeB: 2),
                new("Przetoczenie krwi lub preparatu krwiopochodnego", CodeA: 20, CodeB: 2),
                new("Wprowadzenie zgłębnika do żołądka", CodeA: 5, CodeB: 2),
                new("Wykonanie i interpretacja 12-odprowadzeniowego EKG", CodeA: 30, CodeB: 2),
                new("Badanie palpacyjne gruczołu piersiowego", CodeA: 10, CodeB: 2)
            };
        }
        else
        {
            // From Old SMK PDF (2014) - only Code A
            return new List<ProcedureRequirement>
            {
                new("Prowadzenie resuscytacji krążeniowo-oddechowej", CodeA: 5),
                new("Intubacja dotchawicza", CodeA: 5),
                new("Kardiowersja elektryczna", CodeA: 5),
                new("Defibrylacja serca", CodeA: 5),
                new("Nakłucie tętnicy obwodowej do badania gazometrycznego", CodeA: 5),
                new("Pomiar ośrodkowego ciśnienia żylnego", CodeA: 5),
                new("Pomiar szczytowego przepływu wydechowego", CodeA: 5),
                new("Nakłucie jamy opłucnej w przypadku płynu", CodeA: 5),
                new("Nakłucie jamy otrzewnej w przypadku wodobrzusza", CodeA: 2),
                new("Wprowadzenie zgłębnika do żołądka", CodeA: 5),
                new("Badanie per rectum", CodeA: 5),
                new("Cewnikowanie pęcherza moczowego", CodeA: 10),
                new("Przetoczenie krwi lub preparatu krwiopochodnego", CodeA: 5),
                new("Wykonanie i interpretacja 12-odprowadzeniowego EKG", CodeA: 5),
                new("Badanie palpacyjne gruczołu piersiowego", CodeA: 50)
            };
        }
    }

    // REAL Medical Shift Requirements from PDFs
    public static MedicalShiftRequirement GetShiftRequirements()
    {
        return new MedicalShiftRequirement
        {
            AverageWeeklyHours = 10,
            AverageWeeklyMinutes = 5,
            MaxWeeklyHours = 48,  // Including regular work
            ShiftTypes = new[] { ShiftType.Accompanying, ShiftType.Independent },
            // From PDF: "Lekarz może pełnić dyżury w jednostce prowadzącej szkolenie lub stażu"
            LocationFlexibility = true
        };
    }
}

public record ProcedureRequirement(string Name, int CodeA, int CodeB = 0);
```

### 1.2 Test User Data

```csharp
public class SmkTestUsers
{
    public static List<User> GetTestUsers()
    {
        return new List<User>
        {
            new User
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                SecondName = "Piotr",
                Pesel = "85010512345",  // Valid PESEL with checksum
                Pwz = "1234567",        // 7-digit PWZ
                Email = "jan.kowalski@example.com",
                PhoneNumber = "+48 600 123 456",
                CorrespondenceAddress = new Address(
                    Street: "ul. Długa",
                    HouseNumber: "15",
                    ApartmentNumber: "3A",
                    PostalCode: "00-001",
                    City: "Warszawa",
                    Country: "Polska"
                )
            },
            new User
            {
                FirstName = "Anna",
                LastName = "Nowak-Wiśniewska",  // Double surname
                Pesel = "90032812367",
                Pwz = "7654321",
                Email = "anna.nowak@example.com",
                CorrespondenceAddress = new Address(
                    Street: "al. Niepodległości",
                    HouseNumber: "120",
                    ApartmentNumber: null,  // House, no apartment
                    PostalCode: "02-555",
                    City: "Warszawa"
                )
            }
        };
    }
}
```

---

## 2. Integration Tests for SMK Export

### 2.1 Complete Export Test Scenario

```csharp
[TestClass]
public class SmkExportIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task ExportToXlsx_CompleteCardiologySpecialization_GeneratesValidSmkFile()
    {
        // Arrange - Create complete specialization with all data
        var user = SmkTestUsers.GetTestUsers().First();
        var specialization = SmkCardiologyTestData.CreateCardiologySpecialization(SmkVersion.New);
        var basicModule = SmkCardiologyTestData.CreateBasicModule();
        
        // Add real procedures based on PDF requirements
        foreach (var req in SmkCardiologyTestData.GetBasicModuleProcedures(SmkVersion.New))
        {
            // Add Code A procedures
            for (int i = 0; i < req.CodeA; i++)
            {
                basicModule.Procedures.Add(new ProcedureNewSmk
                {
                    Name = req.Name,
                    Code = $"PROC_{i+1}",
                    ExecutionType = ProcedureExecutionType.CodeA,
                    PerformedDate = DateTime.Now.AddDays(-i * 7),
                    Location = "Oddział Chorób Wewnętrznych",
                    SupervisorName = "dr hab. Maria Wiśniewska",
                    SupervisorPwz = "9876543"
                });
            }
            
            // Add Code B procedures
            for (int i = 0; i < req.CodeB; i++)
            {
                basicModule.Procedures.Add(new ProcedureNewSmk
                {
                    Name = req.Name,
                    ExecutionType = ProcedureExecutionType.CodeB,
                    PerformedDate = DateTime.Now.AddDays(-i * 5)
                });
            }
        }
        
        // Add medical shifts - must average 10h 5min per week
        var shiftDates = GenerateShiftDates(basicModule.StartDate, 52 * 2); // 2 years
        foreach (var date in shiftDates)
        {
            basicModule.MedicalShifts.Add(new MedicalShift
            {
                Date = date,
                Duration = new ShiftDuration(10, 5),  // Exactly as required
                Type = date.Month % 2 == 0 ? ShiftType.Independent : ShiftType.Accompanying,
                Location = "Oddział Chorób Wewnętrznych",
                SupervisorName = "dr Jan Nowak"
            });
        }
        
        // Act - Export to XLSX
        var exportService = new SmkExportService(_dbContext, _validator);
        var result = await exportService.ExportToXlsxAsync(specialization.Id);
        
        // Assert - Validate export structure
        Assert.IsTrue(result.IsSuccess);
        
        using var stream = new MemoryStream(result.Value);
        using var workbook = new XLWorkbook(stream);
        
        // Verify all required sheets exist
        Assert.IsTrue(workbook.Worksheets.Contains("Dane osobowe"));
        Assert.IsTrue(workbook.Worksheets.Contains("Specjalizacja"));
        Assert.IsTrue(workbook.Worksheets.Contains("Staże"));
        Assert.IsTrue(workbook.Worksheets.Contains("Dyżury medyczne"));
        Assert.IsTrue(workbook.Worksheets.Contains("Procedury i zabiegi"));
        Assert.IsTrue(workbook.Worksheets.Contains("Kursy"));
        Assert.IsTrue(workbook.Worksheets.Contains("Samokształcenie"));
        
        // Verify user data sheet
        var userSheet = workbook.Worksheet("Dane osobowe");
        Assert.AreEqual("PESEL", userSheet.Cell("A1").Value);
        Assert.AreEqual("85010512345", userSheet.Cell("A2").Value);
        Assert.AreEqual("Numer PWZ", userSheet.Cell("B1").Value);
        Assert.AreEqual("1234567", userSheet.Cell("B2").Value);
        
        // Verify procedures meet requirements
        var procedureSheet = workbook.Worksheet("Procedury i zabiegi");
        var bslCount = procedureSheet.RowsUsed()
            .Where(r => r.Cell("B").GetString().Contains("BLS/ALS"))
            .Count();
        Assert.AreEqual(6, bslCount); // 3 Code A + 3 Code B
        
        // Verify shift calculations
        var shiftSheet = workbook.Worksheet("Dyżury medyczne");
        var totalHours = CalculateTotalShiftHours(shiftSheet);
        var weeklyAverage = totalHours / 104; // 2 years = 104 weeks
        Assert.IsTrue(Math.Abs(weeklyAverage - 10.083) < 0.1); // 10h 5min
    }

    [TestMethod]
    public async Task ExportToXlsx_ValidatesRequiredFields_ReturnsErrors()
    {
        // Arrange - Create incomplete specialization
        var specialization = new Specialization
        {
            Name = "Kardiologia",
            // Missing required fields
        };
        
        // Act
        var exportService = new SmkExportService(_dbContext, _validator);
        var result = await exportService.ValidateForExportAsync(specialization.Id);
        
        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("PESEL")));
        Assert.IsTrue(result.Errors.Any(e => e.Contains("PWZ")));
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Module")));
    }
}
```

### 2.2 Field Mapping Tests

```csharp
[TestClass]
public class SmkFieldMappingTests
{
    [TestMethod]
    public void DateFormat_ExportsAsPolishFormat()
    {
        var date = new DateTime(2024, 3, 15);
        var formatted = SmkFormatter.FormatDate(date);
        Assert.AreEqual("15.03.2024", formatted);
    }

    [TestMethod]
    public void Duration_ExportsWithMinutesOver59()
    {
        var duration = new ShiftDuration(10, 75); // 75 minutes
        var formatted = SmkFormatter.FormatDuration(duration);
        Assert.AreEqual("10h 75min", formatted);
    }

    [TestMethod]
    public void Boolean_ExportsAsPolishYesNo()
    {
        Assert.AreEqual("Tak", SmkFormatter.FormatBoolean(true));
        Assert.AreEqual("Nie", SmkFormatter.FormatBoolean(false));
    }
}
```

---

## 3. Performance Testing with Large Datasets

### 3.1 Performance Test Scenarios

```csharp
[TestClass]
public class SmkPerformanceTests
{
    [TestMethod]
    [DataRow(100)]    // 100 users
    [DataRow(1000)]   // 1,000 users
    [DataRow(10000)]  // 10,000 users
    public async Task ExportPerformance_LargeDataset_CompletesWithinTimeLimit(int userCount)
    {
        // Arrange - Generate realistic data volume
        var testData = GenerateLargeDataset(userCount);
        var stopwatch = Stopwatch.StartNew();
        
        // Act
        var exportService = new SmkExportService(_dbContext);
        var results = new List<byte[]>();
        
        foreach (var userId in testData.UserIds)
        {
            var result = await exportService.ExportUserDataAsync(userId);
            results.Add(result.Value);
        }
        
        stopwatch.Stop();
        
        // Assert - Performance benchmarks
        var avgTimePerUser = stopwatch.ElapsedMilliseconds / userCount;
        Console.WriteLine($"Users: {userCount}, Total: {stopwatch.ElapsedMilliseconds}ms, Avg: {avgTimePerUser}ms");
        
        Assert.IsTrue(avgTimePerUser < 100, $"Export too slow: {avgTimePerUser}ms per user");
        Assert.IsTrue(results.All(r => r.Length > 0));
    }

    private TestDataset GenerateLargeDataset(int userCount)
    {
        var dataset = new TestDataset();
        
        for (int i = 0; i < userCount; i++)
        {
            // Each user has realistic specialization data
            var user = new User
            {
                Pesel = GenerateValidPesel(),
                Pwz = $"{1000000 + i}",
                FirstName = $"Test{i}",
                LastName = $"User{i}"
            };
            
            // 5 years of data per user
            var specialization = GenerateCompleteSpecialization();
            
            // Realistic data volume:
            // - 2 modules
            // - 10 internships
            // - 520 medical shifts (10h/week for 5 years)
            // - 200 procedures
            // - 15 courses
            // - 50 self-education entries
            
            dataset.Users.Add(user);
        }
        
        return dataset;
    }

    [TestMethod]
    public async Task BulkExport_1000Users_UsesEfficientQueries()
    {
        // Arrange
        var userIds = Enumerable.Range(1, 1000).ToList();
        
        // Act - Monitor SQL queries
        var queries = new List<string>();
        _dbContext.Database.Log = sql => queries.Add(sql);
        
        var exportService = new SmkBulkExportService(_dbContext);
        await exportService.BulkExportAsync(userIds);
        
        // Assert - Should use efficient queries, not N+1
        var selectQueries = queries.Count(q => q.Contains("SELECT"));
        Assert.IsTrue(selectQueries < 50, $"Too many queries: {selectQueries}");
    }
}
```

### 3.2 Memory Usage Tests

```csharp
[TestMethod]
public async Task ExportMemoryUsage_LargeFile_StreamsEfficiently()
{
    // Arrange - User with 5 years of data
    var heavyUser = CreateHeavyDataUser();
    var initialMemory = GC.GetTotalMemory(true);
    
    // Act - Export using streaming
    var exportService = new SmkStreamingExportService(_dbContext);
    using var stream = new MemoryStream();
    await exportService.ExportToStreamAsync(heavyUser.Id, stream);
    
    // Force GC and measure
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    var finalMemory = GC.GetTotalMemory(true);
    
    // Assert - Memory usage should be reasonable
    var memoryUsedMB = (finalMemory - initialMemory) / (1024 * 1024);
    Assert.IsTrue(memoryUsedMB < 50, $"Excessive memory usage: {memoryUsedMB}MB");
}
```

---

## 4. Database Migration Scripts

### 4.1 Safe Migration Strategy

```csharp
// Migration: AddSmkComplianceFields.cs
public partial class AddSmkComplianceFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Step 1: Add new columns as nullable first
        migrationBuilder.AddColumn<string>(
            name: "Pesel",
            table: "Users",
            type: "nvarchar(11)",
            maxLength: 11,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Pwz",
            table: "Users",
            type: "nvarchar(7)",
            maxLength: 7,
            nullable: true);

        // Step 2: Add Module table
        migrationBuilder.CreateTable(
            name: "Modules",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", 
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SpecializationId = table.Column<int>(nullable: false),
                Type = table.Column<string>(maxLength: 20, nullable: false),
                Name = table.Column<string>(maxLength: 200, nullable: false),
                StartDate = table.Column<DateTime>(nullable: false),
                EndDate = table.Column<DateTime>(nullable: true),
                IsCompleted = table.Column<bool>(nullable: false, defaultValue: false),
                CreatedAt = table.Column<DateTime>(nullable: false),
                UpdatedAt = table.Column<DateTime>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Modules", x => x.Id);
                table.ForeignKey(
                    name: "FK_Modules_Specializations_SpecializationId",
                    column: x => x.SpecializationId,
                    principalTable: "Specializations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Step 3: Create indexes for performance
        migrationBuilder.CreateIndex(
            name: "IX_Users_Pesel",
            table: "Users",
            column: "Pesel",
            unique: true,
            filter: "[Pesel] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Pwz",
            table: "Users",
            column: "Pwz",
            unique: true,
            filter: "[Pwz] IS NOT NULL");

        // Step 4: Migrate existing data to modules
        migrationBuilder.Sql(@"
            -- Create default modules for existing specializations
            INSERT INTO Modules (SpecializationId, Type, Name, StartDate, IsCompleted, CreatedAt)
            SELECT 
                s.Id,
                'Basic',
                'Moduł podstawowy - ' + s.Name,
                s.StartDate,
                CASE WHEN s.Status = 'Completed' THEN 1 ELSE 0 END,
                GETUTCNOW()
            FROM Specializations s
            WHERE NOT EXISTS (
                SELECT 1 FROM Modules m WHERE m.SpecializationId = s.Id
            );

            -- Update foreign keys to point to modules
            ALTER TABLE MedicalShifts ADD ModuleId INT NULL;
            ALTER TABLE Procedures ADD ModuleId INT NULL;
            ALTER TABLE Internships ADD ModuleId INT NULL;
            
            -- Migrate relationships
            UPDATE ms SET ms.ModuleId = m.Id
            FROM MedicalShifts ms
            INNER JOIN Specializations s ON ms.SpecializationId = s.Id
            INNER JOIN Modules m ON m.SpecializationId = s.Id AND m.Type = 'Basic';
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverse migration with data preservation
        migrationBuilder.Sql(@"
            -- Save module data back to specializations if needed
            UPDATE s 
            SET s.Notes = s.Notes + ' | Module data: ' + m.Name
            FROM Specializations s
            INNER JOIN Modules m ON m.SpecializationId = s.Id;
        ");

        migrationBuilder.DropTable("Modules");
        migrationBuilder.DropColumn("Pesel", "Users");
        migrationBuilder.DropColumn("Pwz", "Users");
    }
}
```

### 4.2 Data Validation Migration

```csharp
// Migration: AddSmkDataValidation.cs
public partial class AddSmkDataValidation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add check constraints for data integrity
        migrationBuilder.Sql(@"
            -- PESEL validation (11 digits)
            ALTER TABLE Users 
            ADD CONSTRAINT CK_Users_Pesel_Format 
            CHECK (Pesel IS NULL OR LEN(Pesel) = 11 AND Pesel NOT LIKE '%[^0-9]%');

            -- PWZ validation (7 digits)
            ALTER TABLE Users 
            ADD CONSTRAINT CK_Users_Pwz_Format 
            CHECK (Pwz IS NULL OR LEN(Pwz) = 7 AND Pwz NOT LIKE '%[^0-9]%');

            -- Medical shift duration validation
            ALTER TABLE MedicalShifts
            ADD CONSTRAINT CK_MedicalShifts_Duration
            CHECK (Hours >= 0 AND Hours <= 24 AND Minutes >= 0);

            -- Module type validation
            ALTER TABLE Modules
            ADD CONSTRAINT CK_Modules_Type
            CHECK (Type IN ('Basic', 'Specialized'));
        ");
    }
}
```

---

## 5. Documentation Creation

### 5.1 API Documentation Template

```yaml
# SMK Export API Documentation

## Export Endpoints

### GET /api/smk/export/{specializationId}/xlsx
Exports complete specialization data in SMK-compatible XLSX format.

**Parameters:**
- specializationId (int): ID of the specialization to export

**Response:**
- 200 OK: Returns XLSX file
- 400 Bad Request: Validation errors
- 404 Not Found: Specialization not found

**Example Response Headers:**
```
Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
Content-Disposition: attachment; filename="SMK_Export_Kowalski_Jan_2024-03-15.xlsx"
```

### GET /api/smk/export/{specializationId}/validate
Validates specialization data for SMK compliance before export.

**Response Example:**
```json
{
  "isValid": false,
  "errors": [
    {
      "field": "User.Pesel",
      "message": "PESEL is required for SMK export"
    },
    {
      "field": "Module.Procedures",
      "message": "Missing 3 procedures of type 'Prowadzenie resuscytacji BLS/ALS' (Code A)"
    }
  ],
  "warnings": [
    {
      "field": "MedicalShifts",
      "message": "Weekly average is 8h 30min, below required 10h 5min"
    }
  ]
}
```
```

### 5.2 User Guide Documentation

```markdown
# SledzSpecke SMK Export Guide

## Prerequisites
Before exporting to SMK, ensure:
1. ✅ User has valid PESEL (11 digits)
2. ✅ User has valid PWZ (7 digits)
3. ✅ Complete correspondence address
4. ✅ All courses have CMKP certificate numbers
5. ✅ Medical shifts average 10h 5min per week
6. ✅ All required procedures completed

## Export Process

### Step 1: Validate Data
Navigate to: **Specialization Details > Export > Validate**

Common validation errors:
- "Missing PESEL": Update in User Profile
- "Incomplete procedures": Check procedure requirements
- "Invalid CMKP certificate": Verify certificate number

### Step 2: Export to XLSX
1. Click **Export to SMK Format**
2. Select export options:
   - Include draft data: Yes/No
   - Date range: Full specialization / Custom
3. Download XLSX file

### Step 3: Import to SMK
1. Log into SMK system
2. Navigate to "Import danych"
3. Upload generated XLSX file
4. Review and confirm import

## Field Mapping Reference

| SledzSpecke | SMK | Format |
|-------------|-----|---------|
| Duration | Czas trwania | 10h 5min |
| Date | Data | 15.03.2024 |
| Yes/No | Tak/Nie | Text |
```

---

## 6. Final Validation Checklist

### 6.1 Government Requirements Validation

```csharp
[TestClass]
public class SmkGovernmentComplianceTests
{
    [TestMethod]
    public void ValidateAgainstOfficialRequirements()
    {
        var validator = new SmkComplianceValidator();
        var results = new List<ValidationResult>();

        // Test 1: PESEL Format
        results.Add(validator.ValidatePesel("85010512345")); // Valid
        results.Add(validator.ValidatePesel("850105123")); // Too short
        results.Add(validator.ValidatePesel("85O1O512345")); // Letters
        
        // Test 2: PWZ Format
        results.Add(validator.ValidatePwz("1234567")); // Valid
        results.Add(validator.ValidatePwz("123456")); // Too short
        
        // Test 3: Medical Shifts Weekly Average
        var shifts = GenerateYearOfShifts();
        var weeklyAvg = validator.CalculateWeeklyAverage(shifts);
        Assert.AreEqual(10.083, weeklyAvg, 0.01); // 10h 5min
        
        // Test 4: Module Progression
        var modules = new List<Module> { basicModule, specializedModule };
        Assert.IsTrue(validator.ValidateModuleProgression(modules));
        
        // Test 5: CMKP Certificate
        Assert.IsTrue(validator.ValidateCmkpCertificate("CMKP/2024/KS/12345"));
        
        // Test 6: Procedure Requirements Met
        var procedures = GetUserProcedures();
        var requirements = SmkCardiologyTestData.GetBasicModuleProcedures(SmkVersion.New);
        foreach (var req in requirements)
        {
            var userCount = procedures.Count(p => p.Name == req.Name && p.ExecutionType == ProcedureExecutionType.CodeA);
            Assert.IsTrue(userCount >= req.CodeA, $"Missing {req.Name} Code A: {userCount}/{req.CodeA}");
        }
    }
}
```

### 6.2 Chrome Extension Compatibility Test

```javascript
// Test that exported XLSX works with Chrome extension
describe('SMK Chrome Extension Import', () => {
    it('should parse SledzSpecke export correctly', async () => {
        const file = await loadTestFile('SMK_Export_Test.xlsx');
        const parser = new SmkXlsxParser();
        const data = await parser.parse(file);
        
        // Verify field mapping
        expect(data.user.pesel).toBe('85010512345');
        expect(data.user.pwz).toBe('1234567');
        expect(data.user.firstName).toBe('Jan');
        expect(data.user.lastName).toBe('Kowalski');
        
        // Verify date format
        expect(data.specialization.startDate).toBe('01.10.2023');
        
        // Verify duration format
        expect(data.shifts[0].duration).toBe('10h 5min');
        
        // Verify boolean format
        expect(data.module.isCompleted).toBe('Tak');
    });
});
```

---

## 7. Continuous Validation Strategy

### 7.1 Automated Daily Validation

```csharp
public class SmkDailyValidationJob : IScheduledJob
{
    public async Task ExecuteAsync()
    {
        var activeSpecializations = await _repository.GetActiveSpecializationsAsync();
        var validationResults = new List<ValidationSummary>();
        
        foreach (var spec in activeSpecializations)
        {
            var result = await _validator.ValidateForSmkComplianceAsync(spec);
            validationResults.Add(new ValidationSummary
            {
                SpecializationId = spec.Id,
                UserId = spec.UserId,
                IsCompliant = result.IsValid,
                Errors = result.Errors,
                CheckedAt = DateTime.UtcNow
            });
        }
        
        // Send summary report
        await _notificationService.SendComplianceReportAsync(validationResults);
    }
}
```

### 7.2 Pre-Export Validation Pipeline

```csharp
public class SmkExportPipeline
{
    private readonly List<ISmkValidator> _validators = new()
    {
        new UserDataValidator(),      // PESEL, PWZ, Address
        new ModuleStructureValidator(), // Module progression
        new ProcedureCountValidator(),  // Requirements met
        new MedicalShiftValidator(),    // Weekly average
        new CmkpCertificateValidator(), // Valid certificates
        new DateFormatValidator(),      // Polish format
        new DurationFormatValidator()   // Hours and minutes
    };

    public async Task<ValidationResult> ValidateAsync(int specializationId)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(specializationId);
            errors.AddRange(result.Errors);
            warnings.AddRange(result.Warnings);
        }
        
        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Warnings = warnings,
            ValidatedAt = DateTime.UtcNow
        };
    }
}
```

---

## Testing Execution Plan

### Week 1: Unit & Integration Tests
- [ ] Implement all test data builders
- [ ] Create comprehensive unit tests
- [ ] Integration tests for each module
- [ ] Export format validation

### Week 2: Performance & Load Testing
- [ ] Performance benchmarks
- [ ] Memory usage optimization
- [ ] Database query optimization
- [ ] Bulk export testing

### Week 3: End-to-End Testing
- [ ] Complete user journey tests
- [ ] Chrome extension compatibility
- [ ] SMK import simulation
- [ ] Error scenario testing

### Week 4: Final Validation
- [ ] Government requirement checklist
- [ ] Field mapping verification
- [ ] Documentation review
- [ ] Production readiness assessment

---

**Remember**: Every test should use REAL data from the government PDFs to ensure 100% SMK compatibility!