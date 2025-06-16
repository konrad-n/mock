# Handoff Command for Next Claude Session

## Context
You are continuing work on the SledzSpecke SMK compliance implementation. Phases 1-4 have been completed. The system is a medical specialization tracking system for Polish doctors that must be 100% compatible with SMK (System Monitorowania Kształcenia) for data export.

## Current Status
- ✅ **Phase 1 COMPLETED**: User and Specialization entities updated with SMK-required fields
- ✅ **Phase 2 COMPLETED**: Implemented missing entities (Procedure hierarchy already existed, AdditionalSelfEducationDays added)
- ✅ **Phase 3 COMPLETED**: Business rules implementation (ShiftDuration value object, Module progression service, Duration calculation service)
- ✅ **Phase 4 COMPLETED**: XLSX Export Implementation (built but needs testing)
- ✅ **Build successful**: All code compiles without errors
- ⚠️ **Technical debt**: Many handlers still commented out due to User-Specialization relationship changes
- 🔄 **You are here**: Ready to test Phase 4 Export functionality

## Command to Send to New Claude Chat:

```
Continue implementing the SledzSpecke SMK compliance roadmap. Phases 1-4 are complete. The XLSX Export functionality is implemented but needs testing.

Current state:
- User entity has PESEL, PWZ, FirstName, LastName, CorrespondenceAddress
- Specialization entity has UserId, ProgramVariant, PlannedPesYear, ActualEndDate, Status
- Procedure entity hierarchy (ProcedureBase, ProcedureOldSmk, ProcedureNewSmk) is implemented
- AdditionalSelfEducationDays entity with 6-day/year validation is implemented
- ShiftDuration value object supports minutes > 59 as per SMK
- ModuleProgressionService enforces Basic -> Specialistic progression rules
- DurationCalculationService handles hour calculations and validations
- XLSX Export Service with validation and Excel generation using ClosedXML
- ExportController with /xlsx, /preview, and /validate endpoints
- All code builds successfully

Next tasks (Phase 4 Testing):
1. Test the export endpoints with sample data
2. Verify XLSX format matches SMK requirements exactly
3. Validate date formats (DD.MM.YYYY) and time formats (HH:MM)
4. Ensure all 6 sheets are generated correctly (Basic Info, Internships, Courses, Shifts, Procedures, Self-education)
5. Test with both old and new SMK versions
6. Fix any runtime issues or data mapping problems

Known implementation details:
- Internship doesn't have a Name property - using "Institution - Department" format
- MedicalShift only tracks duration, not start/end times - using "00:00" as start time
- Course entity only has CompletionDate, not separate start/end dates
- Many entities use value objects that need .Value property access
- ModuleId comparisons need .Value for EF queries
- Module entity doesn't have navigation properties - data loaded separately

Key files to check:
- /home/ubuntu/projects/mock/IMPORTANT/sledzspecke-compliance-roadmap.md (master roadmap)
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Infrastructure/Export/SmkExportService.cs
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Infrastructure/Export/SmkExcelGenerator.cs
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Infrastructure/Export/SmkValidator.cs
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Api/Controllers/ExportController.cs

Test endpoints:
- GET /api/export/specialization/{id}/xlsx - Download Excel file
- GET /api/export/specialization/{id}/preview - Preview export data
- POST /api/export/specialization/{id}/validate - Validate data for export

Remember:
- This is a PRODUCTION medical system
- Every field must match SMK exactly for successful import
- Date format: DD.MM.YYYY, Time format: HH:MM
- No approval workflows in our system (all approvals happen in SMK)
- Use clean architecture principles
- Run "dotnet build" frequently
```

## Additional Context for Next Session

### Completed in Phase 4:

1. **Export Service Architecture**:
   - ISpecializationExportService interface with 3 methods
   - SmkExportService implementation with complete data loading
   - Separate queries for each entity type due to missing navigation properties
   - Export preview and validation report DTOs

2. **Excel Generation**:
   - SmkExcelGenerator using ClosedXML library
   - 6 sheets: Basic Info, Internships, Courses, Medical Shifts, Procedures, Self-education
   - Proper Polish formatting for dates and times
   - Different procedure sheet structure for old vs new SMK

3. **Validation**:
   - SmkValidator with comprehensive field validation
   - Date/time format validation using regex
   - Business rule validation (6-day self-education limit, 160h monthly minimum)
   - Cross-entity validation (module consistency, internship references)

4. **API Layer**:
   - ExportController with authorization
   - File download with proper MIME type
   - Export preview for UI integration
   - Validation endpoint for pre-export checks

### Data Mapping Decisions:
- **Internship Name**: Concatenated as `{InstitutionName} - {DepartmentName}`
- **Medical Shift Times**: Start time hardcoded as "00:00", end time calculated from duration
- **Course Dates**: Both start and end date use CompletionDate
- **Supervisor Info**: MedicalShift uses ApproverName, Internship uses SupervisorName
- **Notes**: MedicalShift uses AdditionalFields property
- **Procedure Names**: Old SMK uses ProcedureGroup or Code, New SMK has ProcedureName property

### Development Environment:
- Working directory: `/home/ubuntu/projects/mock/SledzSpecke.WebApi`
- Production API: https://api.sledzspecke.pl
- Database: PostgreSQL (sledzspecke_db)
- Build command: `dotnet build`
- Test command: `dotnet test`

### Git Status:
- All Phase 4 changes need to be committed
- Branch: master
- Many files created and modified

### Next Priority:
1. Test Phase 4 Export functionality thoroughly
2. Fix any runtime issues
3. Validate against actual SMK import requirements
4. Then proceed to Phase 5 (API Adjustments) and Phase 6 (Testing & Validation)