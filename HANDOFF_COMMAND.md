# Handoff Command for Next Claude Session

## Context
You are continuing work on the SledzSpecke SMK compliance implementation. Phases 1, 2, and 3 have been completed. The system is a medical specialization tracking system for Polish doctors that must be 100% compatible with SMK (System Monitorowania Kształcenia) for data export.

## Current Status
- ✅ **Phase 1 COMPLETED**: User and Specialization entities updated with SMK-required fields
- ✅ **Phase 2 COMPLETED**: Implemented missing entities (Procedure hierarchy already existed, AdditionalSelfEducationDays added)
- ✅ **Phase 3 COMPLETED**: Business rules implementation (ShiftDuration value object, Module progression service, Duration calculation service)
- ✅ **Build successful**: All code compiles without errors
- ⚠️ **Technical debt**: Many handlers still commented out due to User-Specialization relationship changes
- ⚠️ **EF Migration issue**: Unable to generate migration due to value object constructor issues
- 📍 **You are here**: Ready to start Phase 4 - XLSX Export Implementation

## Command to Send to New Claude Chat:

```
Continue implementing the SledzSpecke SMK compliance roadmap. Phases 1, 2, and 3 are complete. Start with Phase 4 - XLSX Export Implementation.

Current state:
- User entity has PESEL, PWZ, FirstName, LastName, CorrespondenceAddress
- Specialization entity has UserId, ProgramVariant, PlannedPesYear, ActualEndDate, Status
- Procedure entity hierarchy (ProcedureBase, ProcedureOldSmk, ProcedureNewSmk) is implemented
- AdditionalSelfEducationDays entity with 6-day/year validation is implemented
- ShiftDuration value object supports minutes > 59 as per SMK (updated from existing implementation)
- ModuleProgressionService enforces Basic -> Specialistic progression rules
- DurationCalculationService handles hour calculations and validations
- All entities and services registered in DI, build successful

Next tasks (Phase 4 - XLSX Export):
1. Implement ISpecializationExportService interface
2. Create SmkExportService with validation
3. Implement SmkExcelGenerator using ClosedXML
4. Create export DTOs matching SMK format exactly
5. Add ExportController with endpoints
6. Test XLSX output format against SMK requirements

Known issues to address:
- EF Core migrations fail due to value object constructors (workaround: manual SQL migration)
- Many application handlers are commented out (search for "TODO: User-Specialization relationship")
- IAbsenceRepository missing GetByInternshipIdAsync method (commented out in DurationCalculationService)

Key files to check:
- /home/ubuntu/projects/mock/IMPORTANT/sledzspecke-compliance-roadmap.md (master roadmap - see Phase 4)
- /home/ubuntu/projects/mock/IMPORTANT/sledzspecke-business-logic.md (business rules)
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Core/DomainServices/ModuleProgressionService.cs
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Core/DomainServices/DurationCalculationService.cs
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Core/ValueObjects/ShiftDuration.cs
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Core/Entities/AdditionalSelfEducationDays.cs

Remember:
- This is a PRODUCTION medical system
- Every field must match SMK exactly for successful import
- Date format: DD.MM.YYYY, Time format: HH:MM
- No approval workflows in our system (all approvals happen in SMK)
- Use clean architecture principles
- Install ClosedXML NuGet package for Excel generation
- Run "dotnet build" frequently
```

## Additional Context for Next Session

### Completed in This Session:

1. **Phase 2 - Missing Entities**:
   - Discovered Procedure entity hierarchy already existed with proper Old/New SMK support
   - Created AdditionalSelfEducationDays entity with:
     - 6-day annual limit validation  
     - Module and Internship associations
     - Full CRUD repository with year-based queries
     - EF Core configuration with proper indexes
     - Repository registered in DI container

2. **Phase 3 - Business Rules**:
   - Enhanced existing ShiftDuration value object:
     - Now stores total minutes internally to support minutes > 59
     - Added ToSmkFormat() for HH:MM export
     - Added ToDisplayFormat() for user-friendly display
     - Arithmetic and comparison operators
   - Created ModuleProgressionService:
     - Validates Basic module must complete before Specialistic
     - Tracks progression status and completion
     - Provides progression percentage calculations
   - Created DurationCalculationService:
     - Calculates internship and module durations
     - Validates 48h weekly maximum (SMK requirement)
     - Validates 160h monthly minimum (SMK requirement)
     - Handles self-education days in calculations

3. **Technical Decisions Made**:
   - Used existing entity properties (IsCompleted, IsApproved) instead of Status enums
   - ModuleType enum uses "Specialistic" not "Specialist" 
   - Repository methods don't accept CancellationToken parameter
   - Used .Equals() for enum comparisons to avoid operator issues
   - Commented out absence calculations pending repository method addition

4. **Files Created/Modified**:
   - Created: AdditionalSelfEducationDays.cs (entity)
   - Created: AdditionalSelfEducationDaysConfiguration.cs (EF config)
   - Created: IAdditionalSelfEducationDaysRepository.cs & implementation
   - Created: ModuleProgressionService.cs
   - Created: DurationCalculationService.cs
   - Modified: ShiftDuration.cs (enhanced existing)
   - Modified: SledzSpeckeDbContext.cs (added new entity)
   - Modified: Extensions.cs (registered repository)
   - Modified: Application/Extensions.cs (registered domain services)

### Development Environment:
- Working directory: `/home/ubuntu/projects/mock/SledzSpecke.WebApi`
- Production API: https://api.sledzspecke.pl
- Database: PostgreSQL (sledzspecke_db)
- Build command: `dotnet build`
- Test command: `dotnet test`

### Git Status:
- All Phase 2 and 3 changes need to be committed
- Branch: master
- Many files created and modified

### Next Priority:
Phase 4 (XLSX Export) is critical - this is the main deliverable for SMK compliance. The export must match SMK import format EXACTLY or it will be rejected. Use the roadmap's Phase 4 section for detailed requirements.