# SledzSpecke SMK Compliance Roadmap

## Executive Summary

This document provides a comprehensive roadmap for adjusting SledzSpecke to achieve 100% compliance with the SMK (System Monitorowania Kształcenia) system. Based on detailed analysis of official SMK documentation for both old and new versions, this roadmap ensures 1:1 field mapping for seamless data export and import via XLSX files.

**Critical Context:**
- SledzSpecke is for **DOCTORS ONLY** - no students or kierownik specjalizacji
- No approval workflows - data will be approved in SMK after export
- Must support both Old SMK (2014-2018) and New SMK (2023+) formats
- All fields must match SMK exactly for Chrome extension compatibility

---

## 1. Current State Analysis

### ✅ Already Implemented
- User entity with core fields
- Medical shifts (dyżury medyczne) tracking
- Procedures tracking with hierarchy
- Internships (staże) management
- Basic export functionality

### ❌ Missing/Incomplete
- SMK-specific user fields (PESEL, PWZ, correspondence address)
- Module structure (Basic/Specialized)
- CMKP course certificate tracking
- Self-education days tracking
- Procedure codes (A/B) differentiation
- Duration validation allowing minutes > 59

---

## 2. Entity Adjustments Required

### 2.1 User Entity Enhancement

**Current fields:** Id, Email, FirstName, LastName, etc.

**Required SMK fields:**
```csharp
public class User : BaseEntity
{
    // Existing fields...
    
    // REQUIRED SMK FIELDS
    public string Pesel { get; set; }  // 11-digit Polish ID
    public string Pwz { get; set; }    // Medical license number (7 digits)
    public string FirstName { get; set; }  // Already exists
    public string LastName { get; set; }   // Already exists
    public Address CorrespondenceAddress { get; set; }  // Value object
    
    // Optional but recommended
    public string PhoneNumber { get; set; }
    public string SecondName { get; set; }  // Middle name
}

// Value Object
public record Address(
    string Street,
    string HouseNumber,
    string ApartmentNumber,
    string PostalCode,
    string City,
    string Country = "Polska"
);
```

### 2.2 Specialization Entity Enhancement

**Required adjustments:**
```csharp
public class Specialization : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    // SMK Required
    public string Name { get; set; }  // e.g., "Kardiologia"
    public SmkVersion SmkVersion { get; set; }  // "old" or "new"
    public string ProgramVariant { get; set; }  // e.g., "wariant_1"
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int PlannedPesYear { get; set; }  // Year of planned PES exam
    public SpecializationStatus Status { get; set; }
    
    // Module tracking
    public bool HasBasicModule { get; set; }
    public bool HasSpecializedModule { get; set; }
    public DateTime? BasicModuleCompletionDate { get; set; }
    
    // Navigation
    public ICollection<Module> Modules { get; set; }
}

public enum SpecializationStatus
{
    Active,
    Completed,
    Suspended,
    Cancelled
}
```

### 2.3 Module Entity (NEW)

```csharp
public class Module : BaseEntity
{
    public int SpecializationId { get; set; }
    public Specialization Specialization { get; set; }
    
    public ModuleType Type { get; set; }  // Basic or Specialized
    public string Name { get; set; }  // e.g., "Moduł podstawowy - choroby wewnętrzne"
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCompleted { get; set; }
    
    // Navigation
    public ICollection<Internship> Internships { get; set; }
    public ICollection<Course> Courses { get; set; }
    public ICollection<MedicalShift> MedicalShifts { get; set; }
    public ICollection<Procedure> Procedures { get; set; }
}

public enum ModuleType
{
    Basic,      // Moduł podstawowy
    Specialized // Moduł specjalistyczny
}
```

### 2.4 Internship Entity Enhancement

```csharp
public class Internship : BaseEntity
{
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    
    // SMK Fields
    public string Name { get; set; }  // e.g., "Staż kierunkowy w zakresie kardiologii"
    public string InstitutionName { get; set; }
    public string Department { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // Duration tracking (SMK format)
    public int PlannedWeeks { get; set; }
    public int PlannedDays { get; set; }
    public int CompletedDays { get; set; }
    
    // Supervisor
    public string SupervisorName { get; set; }
    public string SupervisorPwz { get; set; }
    
    public InternshipStatus Status { get; set; }
}

public enum InternshipStatus
{
    Planned,
    InProgress,
    Completed,
    Cancelled
}
```

### 2.5 MedicalShift Entity Enhancement

```csharp
public class MedicalShift : BaseEntity
{
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    public int? InternshipId { get; set; }  // Optional link to internship
    public Internship Internship { get; set; }
    
    // SMK Fields
    public DateTime Date { get; set; }
    public ShiftDuration Duration { get; set; }  // Value object
    public ShiftType Type { get; set; }
    public string Location { get; set; }  // Hospital/department
    public string SupervisorName { get; set; }
    
    // Exemption tracking
    public bool IsExempt { get; set; }
    public string ExemptionReason { get; set; }
    public DateTime? ExemptionStartDate { get; set; }
    public DateTime? ExemptionEndDate { get; set; }
}

public enum ShiftType
{
    Accompanying,  // Towarzyszący
    Independent   // Samodzielny
}

// Value Object (already implemented)
public record ShiftDuration
{
    public int Hours { get; }
    public int Minutes { get; }
    
    // Allows minutes > 59 as per SMK requirements
}
```

### 2.6 Procedure Entity Enhancement

```csharp
// Keep existing hierarchy but add SMK fields
public abstract class ProcedureBase : BaseEntity
{
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    
    // SMK Common Fields
    public string Code { get; set; }  // Procedure code from program
    public string Name { get; set; }
    public DateTime PerformedDate { get; set; }
    public string Location { get; set; }
    public string PatientInfo { get; set; }  // Anonymized
    
    // SMK Specific
    public ProcedureExecutionType ExecutionType { get; set; }
    public string SupervisorName { get; set; }
    public string SupervisorPwz { get; set; }
}

public enum ProcedureExecutionType
{
    CodeA,  // Performed independently with assistance/supervision
    CodeB   // Assisted as first assistant
}

// Keep OldSmk and NewSmk specific classes
public class ProcedureOldSmk : ProcedureBase
{
    public int RequiredCountCodeA { get; set; }
    // No Code B in old SMK for most procedures
}

public class ProcedureNewSmk : ProcedureBase
{
    public int RequiredCountCodeA { get; set; }
    public int RequiredCountCodeB { get; set; }
}
```

### 2.7 Course Entity Enhancement

```csharp
public class Course : BaseEntity
{
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    
    // SMK Fields
    public string Name { get; set; }  // e.g., "Diagnostyka obrazowa"
    public string CmkpCertificateNumber { get; set; }  // REQUIRED
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public int DurationHours { get; set; }  // Didactic hours
    
    // CMKP Validation
    public bool IsVerifiedByCmkp { get; set; }
    public DateTime? CmkpVerificationDate { get; set; }
    
    // Location
    public string OrganizerName { get; set; }
    public string Location { get; set; }
    
    public CourseType Type { get; set; }
}

public enum CourseType
{
    Specialization,  // Kurs specjalizacyjny
    Improvement,     // Kurs doskonalący
    Scientific,      // Kurs naukowy
    Certification    // Kurs atestacyjny
}
```

### 2.8 SelfEducation Entity (NEW)

```csharp
public class SelfEducation : BaseEntity
{
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    
    // SMK Fields
    public SelfEducationType Type { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    
    // For publications
    public string PublicationTitle { get; set; }
    public string JournalName { get; set; }
    public bool IsPeerReviewed { get; set; }
    public PublicationRole Role { get; set; }
}

public enum SelfEducationType
{
    LiteratureStudy,     // Studiowanie piśmiennictwa
    Conference,          // Konferencja
    ScientificMeeting,   // Posiedzenie naukowe
    Publication,         // Publikacja
    Workshop            // Warsztaty
}

public enum PublicationRole
{
    Author,
    CoAuthor
}
```

### 2.9 AdditionalSelfEducationDays Entity (Already Implemented)

```csharp
public class AdditionalSelfEducationDays : BaseEntity
{
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    public int? InternshipId { get; set; }  // Links to specific internship
    public Internship Internship { get; set; }
    
    // SMK Fields (6 days per year since 2019)
    public DateTime Date { get; set; }
    public int Days { get; set; }  // 1-6 days
    public string EventName { get; set; }
    public string EventType { get; set; }  // Conference, Course, etc.
    public string Description { get; set; }
}
```

---

## 3. Business Rules Implementation

### 3.1 Module Progression Rules
- Basic module MUST be completed before starting Specialized module
- Each module has specific internships, courses, and requirements
- Module completion requires ALL elements to be completed

### 3.2 Duration Calculations
- **Medical Shifts**: Average 10h 5min per week
- **Working Days**: 5 days = 1 week (excluding holidays)
- **Minutes > 59**: Allowed for accurate time tracking
- **Internship Duration**: Track both planned and actual

### 3.3 CMKP Course Validation
- Courses MUST have CMKP certificate numbers
- Only courses from official CMKP list are valid
- Certificate verification required before course completion

### 3.4 Procedure Counting Rules
- Code A: Performed with supervision (counts toward requirement)
- Code B: Assisted only (separate requirement in new SMK)
- Each specialization has specific procedure requirements

---

## 4. API Endpoints Structure

### 4.1 Core Endpoints
```
GET    /api/users/{userId}/specializations
POST   /api/specializations
GET    /api/specializations/{id}/modules
POST   /api/modules/{id}/internships
POST   /api/modules/{id}/medical-shifts
POST   /api/modules/{id}/procedures
POST   /api/modules/{id}/courses
POST   /api/modules/{id}/self-education
POST   /api/modules/{id}/additional-days
```

### 4.2 SMK-Specific Endpoints
```
GET    /api/smk/validate/{specializationId}
GET    /api/smk/export/{specializationId}/xlsx
GET    /api/smk/export/{specializationId}/preview
GET    /api/smk/requirements/{specialization}/{smkVersion}
```

---

## 5. Data Validation Rules

### 5.1 User Validation
- **PESEL**: 11 digits, valid checksum
- **PWZ**: 7 digits, valid medical license format
- **Address**: All fields required except apartment number

### 5.2 Duration Validation
- **Medical Shifts**: Allow minutes > 59
- **Total Weekly**: Should average 10h 5min
- **Internships**: Cannot exceed planned duration

### 5.3 CMKP Validation
- Certificate numbers must match CMKP format
- Course dates must be within module dates
- Required courses must be completed

---

## 6. Export Format Specification

### 6.1 XLSX Structure
```
Sheet 1: User Data
- PESEL, PWZ, Name, Address

Sheet 2: Specialization
- Dates, Status, Modules

Sheet 3: Internships
- Per module, with durations

Sheet 4: Medical Shifts
- Date, Duration, Type, Location

Sheet 5: Procedures
- Code A/B counts, dates

Sheet 6: Courses
- CMKP certificates, dates

Sheet 7: Self-Education
- All activities, publications
```

### 6.2 Field Mapping
Each field must map exactly to SMK import format:
- Use Polish field names in headers
- Date format: DD.MM.YYYY
- Duration format: "Xh Ymin"
- Boolean: "Tak"/"Nie"

---

## 7. Implementation Phases

### Phase 1: Entity Updates (1 week)
- Update User entity with SMK fields
- Add Module entity and relationships
- Update all entities with SMK-specific fields
- Create necessary value objects

### Phase 2: Database Migration (3 days)
- Create EF Core migrations
- Update seed data with SMK examples
- Ensure backward compatibility

### Phase 3: Business Logic (1 week)
- Implement module progression rules
- Add CMKP validation service
- Update duration calculation services
- Add SMK-specific validators

### Phase 4: API Updates (1 week)
- Create module-based endpoints
- Add SMK validation endpoints
- Update existing endpoints
- Implement proper error handling

### Phase 5: Export Enhancement (1 week)
- Create comprehensive XLSX export
- Add all SMK sheets and fields
- Implement preview functionality
- Add export validation

### Phase 6: Testing & Validation (1 week)
- Test with real SMK data
- Validate all field mappings
- Test both old and new SMK formats
- E2E testing with Chrome extension

---

## 8. Critical Implementation Notes

### 8.1 No Approval Workflow
- Remove any approval-related fields
- Data is entered as "draft" in SledzSpecke
- Approval happens in SMK after import

### 8.2 SMK Version Handling
```csharp
public interface ISmkVersionStrategy
{
    bool ValidateProcedureRequirements(Module module);
    bool ValidateCourseRequirements(Module module);
    int CalculateRequiredProcedureCount(string procedureCode, ProcedureExecutionType type);
}

public class OldSmkStrategy : ISmkVersionStrategy { }
public class NewSmkStrategy : ISmkVersionStrategy { }
```

### 8.3 Chrome Extension Compatibility
- Field names must match exactly
- Use data attributes for field identification
- Maintain consistent ID patterns

---

## 9. Testing Checklist

### 9.1 Unit Tests
- [ ] PESEL validation
- [ ] PWZ validation
- [ ] Duration calculations (minutes > 59)
- [ ] Module progression rules
- [ ] CMKP certificate validation

### 9.2 Integration Tests
- [ ] Full specialization workflow
- [ ] Export all data types
- [ ] Import validation
- [ ] Multi-module scenarios

### 9.3 E2E Tests
- [ ] Create complete specialization
- [ ] Export to XLSX
- [ ] Validate with SMK requirements
- [ ] Test Chrome extension import

---

## 10. Post-Implementation

### 10.1 Documentation
- Update API documentation
- Create user guide for SMK fields
- Document export format
- Add validation rules guide

### 10.2 Monitoring
- Track export success rates
- Monitor validation failures
- Log CMKP verification issues
- Track Chrome extension compatibility

### 10.3 Future Enhancements
- Real-time CMKP API integration
- Automatic SMK synchronization
- Progress dashboards
- Requirement tracking

---

## Appendix A: SMK Field Mapping

| SledzSpecke Field | SMK Field (PL) | Format | Required |
|-------------------|----------------|---------|----------|
| User.Pesel | PESEL | 11 digits | Yes |
| User.Pwz | Numer PWZ | 7 digits | Yes |
| User.FirstName | Imię | Text | Yes |
| User.LastName | Nazwisko | Text | Yes |
| MedicalShift.Duration | Czas trwania | Xh Ymin | Yes |
| Course.CmkpCertificateNumber | Nr zaświadczenia | Text | Yes |
| Procedure.ExecutionType | Kod wykonania | A/B | Yes |

## Appendix B: Specialization Requirements

### Cardiology (Kardiologia)
- **Basic Module**: Internal Medicine (2 years)
- **Specialized Module**: Cardiology (3 years)
- **Required Procedures**: See official program PDFs
- **Required Courses**: 10-18 depending on version

### Key Differences Old vs New SMK
- **Old**: Single procedure count (Code A only)
- **New**: Separate Code A and Code B requirements
- **Old**: Fixed self-education days
- **New**: 6 additional days per year since 2019

---

**Document Version**: 1.0  
**Last Updated**: Based on SMK documentation v1.6.2 (2024) and CMKP programs (2023/2024)