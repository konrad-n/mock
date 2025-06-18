# SledzSpecke - Comprehensive Business Logic & Implementation Guide

## Executive Summary

SledzSpecke is a medical specialization tracking system designed for doctors to monitor their training progress. The system must maintain 1:1 field compatibility with SMK (System Monitorowania Kształcenia) to enable seamless data export to XLSX files that can be imported into SMK using a Chrome extension.

**Critical Requirements:**
- **FOR DOCTORS ONLY** - No student or specialization director functionality
- **NO APPROVAL WORKFLOWS** - All approvals happen later in SMK after data import
- **EXACT FIELD MATCHING** - Every field must match SMK exactly for successful data migration

## System Architecture Overview

### Core Entities

#### 1. User (Doctor) ✅ UPDATED
```csharp
public sealed class User
{
    public int Id { get; private set; } // Auto-generated using PostgreSQL sequence
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Pesel { get; private set; } // With checksum validation
    public string PwzNumber { get; private set; } // Prawo Wykonywania Zawodu (7 digits)
    public string PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Address CorrespondenceAddress { get; private set; }
    
    // Removed fields: Username, Bio, ProfilePicturePath, PreferredLanguage, PreferredTheme
}
```

#### 2. Specialization (EKS - Elektroniczna Karta Specjalizacji) ✅ UPDATED
```csharp
public sealed class Specialization : Entity
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Name { get; private set; } // e.g., "Kardiologia"
    public string ProgramCode { get; private set; }
    public string SmkVersion { get; private set; } // "old" or "new"
    public string ProgramVariant { get; private set; } // Numer wariantu programu
    public DateTime StartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public int PlannedPesYear { get; private set; } // Planned exam year
    public SpecializationStatus Status { get; private set; }
    public string ProgramStructure { get; private set; }
    public List<Module> Modules { get; private set; }
}

public enum SpecializationStatus
{
    Active,
    Completed,
    Suspended,
    Cancelled
}
```

#### 3. Module
```csharp
public sealed class Module : Entity
{
    public int Id { get; private set; }
    public int SpecializationId { get; private set; }
    public ModuleType Type { get; private set; }
    public string Name { get; private set; }
    public int YearOfTraining { get; private set; } // 1-6
    public bool IsCompleted { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    public List<Internship> Internships { get; private set; }
    public List<Course> Courses { get; private set; }
    public List<MedicalShift> MedicalShifts { get; private set; }
    public List<Procedure> Procedures { get; private set; }
}

public enum ModuleType
{
    Basic,      // Moduł podstawowy
    Specialist  // Moduł specjalistyczny
}
```

#### 4. Internship (Staż)
```csharp
public sealed class Internship : Entity
{
    public int Id { get; private set; }
    public int ModuleId { get; private set; }
    public string Name { get; private set; } // Nazwa stażu
    public string InstitutionName { get; private set; } // Nazwa podmiotu
    public string DepartmentName { get; private set; } // Nazwa komórki organizacyjnej
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int PlannedDurationWeeks { get; private set; }
    public int PlannedDurationDays { get; private set; }
    public int ActualDurationDays { get; private set; }
    public bool IsPartialRealization { get; private set; } // Realizacja częściowa
    public string SupervisorName { get; private set; }
    public string SupervisorPwz { get; private set; }
}
```

#### 5. Course (Kurs)
```csharp
public sealed class Course : Entity
{
    public int Id { get; private set; }
    public int ModuleId { get; private set; }
    public string CourseName { get; private set; }
    public string CourseNumber { get; private set; } // From SMK/CMKP
    public string OrganizingInstitution { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal DurationWeeks { get; private set; } // Can be 0.6, 0.8, etc.
    public int DurationDays { get; private set; }
    public string CertificateNumber { get; private set; }
    public int YearOfTraining { get; private set; }
    public int SequenceNumber { get; private set; } // Numer kolejny kursu
    public CourseRecognitionType? Recognition { get; private set; }
}

public enum CourseRecognitionType
{
    None,
    RecognizedByDecision,  // Uznanie na podstawie decyzji
    ExemptFromRealization  // Zwolnienie z realizacji
}
```

#### 6. Medical Shift (Dyżur medyczny)
```csharp
public sealed class MedicalShift : Entity
{
    public int Id { get; private set; }
    public int ModuleId { get; private set; }
    public int InternshipId { get; private set; } // Associated internship
    public DateTime ShiftDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int DurationMinutes { get; private set; } // Allows > 59
    public string Location { get; private set; } // Miejsce realizacji
    public ShiftType Type { get; private set; }
}

public enum ShiftType
{
    Accompanying,  // Towarzyszący
    Independent    // Samodzielny
}
```

#### 7. Procedure (Zabieg/Procedura medyczna) ⏳ NEEDS HIERARCHY
```csharp
public sealed class Procedure : Entity
{
    public int Id { get; private set; }
    public int ModuleId { get; private set; }
    public string ProcedureCode { get; private set; } // Kod zabiegu
    public string ProcedureName { get; private set; }
    public string ProcedureGroup { get; private set; } // If from group
    public DateTime Date { get; private set; }
    public int YearOfTraining { get; private set; }
    public string Location { get; private set; } // Miejsce wykonania
    public string InternshipName { get; private set; } // Associated internship
    public string PatientInitials { get; private set; }
    public Gender PatientGender { get; private set; }
    public string FirstAssistantData { get; private set; }
    public string SecondAssistantData { get; private set; }
    public ProcedureRole Role { get; private set; }
}

public enum ProcedureRole
{
    Performed,    // Code A - wykonanie samodzielne
    Assisted      // Code B - asysta
}

public enum Gender
{
    Male,
    Female,
    Other
}
```

#### 8. Self Education (Samokształcenie)
```csharp
public sealed class SelfEducation : Entity
{
    public int Id { get; private set; }
    public int SpecializationId { get; private set; }
    public SelfEducationType Type { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime Date { get; private set; }
    public string Organizer { get; private set; } // For conferences/events
    public string Journal { get; private set; } // For publications
    public bool IsAuthor { get; private set; } // For publications
    public bool IsCoAuthor { get; private set; }
}

public enum SelfEducationType
{
    ScientificPublication,     // Praca naukowa
    ReviewPublication,         // Praca poglądowa
    LiteratureStudy,          // Studiowanie piśmiennictwa
    ConferenceParticipation,  // Udział w konferencji
    Workshop,                  // Warsztat
    Seminar                   // Seminarium
}
```

#### 9. Additional Self Education Days ⏳ NOT IMPLEMENTED
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
}
```

## Business Rules & Validation

### 1. User Registration & Authentication ✅ UPDATED
- Email must be unique and valid
- PWZ number must be valid (7 digits, cannot start with 0)
- PESEL must be valid (11 digits, checksum validation)
- Date of birth must match PESEL
- Password must meet security requirements (min 8 chars, mixed case, numbers, special chars)

### 2. Specialization Management
- A doctor can have multiple active specializations
- SMK version must be either "old" or "new" (string values)
- Planned end date is calculated based on specialization duration
- No approval states - all data is in "draft" until exported

### 3. Module Rules
- Basic module (moduł podstawowy) must be completed before specialist module
- Modules are year-based (1-6 years of training)
- Each module contains planned elements from the specialization program

### 4. Internship Rules
- Duration tracked in both weeks and working days
- Can span multiple training years (partial realization)
- Non-medical institutions use the main training institution name
- Supervisor information required (name and PWZ)

### 5. Course Rules
- Courses must be confirmed with CMKP certificate number
- Duration can be fractional weeks (0.6, 0.8, etc.)
- Recognition types for previously completed courses
- Sequence number within training year

### 6. Medical Shift Rules
- Duration in minutes (allows values > 59 for calculation purposes)
- Must be associated with an internship
- Location can be different from internship location
- Average 10h 5min per week requirement

### 7. Procedure Rules
- Must use SMK procedure codes
- Patient privacy: only initials stored
- Differentiate between performed (Code A) and assisted (Code B)
- Group procedures need additional specification

### 8. Self Education Rules
- 6 additional days per year for conferences/training
- Publications require author/co-author specification
- Unused days don't carry over to next year

## API Endpoints

### Authentication
```
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh
POST   /api/auth/logout
```

### User Management
```
GET    /api/users/profile
PUT    /api/users/profile
PUT    /api/users/change-password
```

### Specializations
```
GET    /api/specializations
POST   /api/specializations
GET    /api/specializations/{id}
PUT    /api/specializations/{id}
DELETE /api/specializations/{id}
```

### Modules
```
GET    /api/specializations/{specId}/modules
POST   /api/specializations/{specId}/modules
GET    /api/modules/{id}
PUT    /api/modules/{id}
```

### Internships
```
GET    /api/modules/{moduleId}/internships
POST   /api/modules/{moduleId}/internships
GET    /api/internships/{id}
PUT    /api/internships/{id}
DELETE /api/internships/{id}
```

### Courses
```
GET    /api/modules/{moduleId}/courses
POST   /api/modules/{moduleId}/courses
GET    /api/courses/{id}
PUT    /api/courses/{id}
DELETE /api/courses/{id}
```

### Medical Shifts
```
GET    /api/modules/{moduleId}/shifts
POST   /api/modules/{moduleId}/shifts
GET    /api/shifts/{id}
PUT    /api/shifts/{id}
DELETE /api/shifts/{id}
GET    /api/shifts/summary/{moduleId} // Weekly/monthly summaries
```

### Procedures
```
GET    /api/modules/{moduleId}/procedures
POST   /api/modules/{moduleId}/procedures
GET    /api/procedures/{id}
PUT    /api/procedures/{id}
DELETE /api/procedures/{id}
GET    /api/procedures/codes // Get SMK procedure codes
```

### Self Education
```
GET    /api/specializations/{specId}/self-education
POST   /api/specializations/{specId}/self-education
GET    /api/self-education/{id}
PUT    /api/self-education/{id}
DELETE /api/self-education/{id}
```

### Export ⏳ NOT IMPLEMENTED
```
GET    /api/export/specialization/{id}/xlsx
GET    /api/export/specialization/{id}/preview
POST   /api/export/validate/{id} // Validate before export
```

## Data Export Requirements

### XLSX Export Format
The exported Excel file must match SMK import format exactly:

#### Sheet 1: Basic Information
- Doctor's personal data
- Specialization details
- Module information

#### Sheet 2: Internships (Staże)
| Column | Field | Format |
|--------|-------|--------|
| A | Nazwa stażu | Text |
| B | Nazwa podmiotu | Text |
| C | Komórka organizacyjna | Text |
| D | Data rozpoczęcia | DD.MM.YYYY |
| E | Data zakończenia | DD.MM.YYYY |
| F | Czas trwania (dni) | Number |
| G | Kierownik stażu | Text |
| H | PWZ kierownika | Text |

#### Sheet 3: Courses (Kursy)
| Column | Field | Format |
|--------|-------|--------|
| A | Nazwa kursu | Text |
| B | Numer kursu | Text |
| C | Podmiot prowadzący | Text |
| D | Data rozpoczęcia | DD.MM.YYYY |
| E | Data ukończenia | DD.MM.YYYY |
| F | Nr zaświadczenia | Text |
| G | Rok szkolenia | Number |

#### Sheet 4: Medical Shifts (Dyżury)
| Column | Field | Format |
|--------|-------|--------|
| A | Data dyżuru | DD.MM.YYYY |
| B | Godzina rozpoczęcia | HH:MM |
| C | Godzina zakończenia | HH:MM |
| D | Czas trwania (min) | Number |
| E | Miejsce | Text |
| F | Typ dyżuru | Text |
| G | Nazwa stażu | Text |

#### Sheet 5: Procedures (Procedury)
| Column | Field | Format |
|--------|-------|--------|
| A | Kod procedury | Text |
| B | Nazwa procedury | Text |
| C | Data wykonania | DD.MM.YYYY |
| D | Miejsce | Text |
| E | Inicjały pacjenta | Text |
| F | Płeć | M/K |
| G | Rola | A/B |
| H | Asysta | Text |

## Integration Requirements

### SMK Compatibility
1. **Field Mapping**: Every field must map exactly to SMK fields
2. **Data Formats**: Dates in DD.MM.YYYY, times in HH:MM
3. **Encoding**: UTF-8 with Polish characters support
4. **Validation**: Pre-export validation to ensure SMK compatibility

### Chrome Extension Requirements
The Chrome extension will:
1. Read the exported XLSX file
2. Parse data according to the format
3. Fill SMK forms automatically
4. Handle multi-page forms and navigation

## Security Considerations

1. **Data Privacy**
   - Patient data anonymized (only initials)
   - Secure storage of medical license numbers
   - GDPR compliance for EU citizens

2. **Authentication**
   - JWT tokens with refresh mechanism
   - Session timeout after inactivity
   - Multi-factor authentication optional

3. **Authorization**
   - Doctors can only access their own data
   - No admin override capabilities
   - Audit trail for all data modifications

## Performance Requirements

1. **Response Times**
   - API responses < 200ms for simple queries
   - Export generation < 5 seconds
   - Real-time updates for shift tracking

2. **Scalability**
   - Support for 10,000+ concurrent users
   - Horizontal scaling capability
   - Database indexing on key fields

## Mobile/Web UI Requirements

### Key Features
1. **Dashboard**
   - Overview of all active specializations
   - Progress indicators
   - Upcoming deadlines

2. **Data Entry**
   - Quick entry forms for shifts
   - Bulk import for procedures
   - Auto-save functionality

3. **Reports**
   - Monthly/yearly summaries
   - Export preview
   - Validation warnings

### Mobile Specific
- Offline capability with sync
- Quick shift timer
- Photo capture for documents
- Push notifications for deadlines

## Implementation Priority

### Phase 1 (MVP) ✅ COMPLETED
1. User authentication & profile
2. Basic specialization management
3. Medical shifts tracking
4. Basic export functionality

### Phase 2 ⏳ IN PROGRESS
1. Complete internship management
2. Course tracking
3. Procedure recording
4. Advanced export with validation

### Phase 3 ⏳ PENDING
1. Self-education tracking
2. Mobile apps (iOS/Android)
3. Chrome extension
4. Advanced reporting

## Testing Strategy

### Unit Tests ✅ PASSING
- All business logic in domain layer
- Value object validation
- Entity state transitions

### Integration Tests ⚠️ NEEDS UPDATE
- API endpoint testing
- Database operations
- Export file generation

### E2E Tests ⚠️ NEEDS UPDATE
- Complete user workflows
- Export and validation
- Performance benchmarks

## Deployment Considerations

1. **Environment Variables**
   - Database connections
   - JWT secrets
   - API keys
   - SMK endpoints (if any)

2. **Database Migrations** ⏳ PENDING
   - Version-controlled migrations
   - Rollback procedures
   - Data seeding for testing

3. **Monitoring**
   - Application performance
   - Error tracking
   - User analytics
   - Export success rates

## 🔴 CURRENT STATUS (2025-06-16)

### Technical Debt:
- User-Specialization relationship refactored but many handlers are commented out
- Procedure entity needs hierarchy implementation for Old/New SMK
- AdditionalSelfEducationDays entity not implemented
- XLSX export service not implemented
- Database migrations pending for new entity structure

### Next Immediate Steps:
1. Create database migrations for User and Specialization changes
2. Implement Procedure entity hierarchy
3. Implement AdditionalSelfEducationDays entity
4. Begin XLSX export service implementation
5. Update integration and E2E tests