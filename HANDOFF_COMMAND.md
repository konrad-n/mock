# Handoff Command for Next Claude Session

## Context
You are continuing work on the SledzSpecke SMK compliance implementation. Phase 1 (Core Entity Alignment) has been completed. The system is a medical specialization tracking system for Polish doctors that must be 100% compatible with SMK (System Monitorowania Kształcenia) for data export.

## Current Status
- ✅ **Phase 1 COMPLETED**: User and Specialization entities updated with SMK-required fields
- ✅ **Build successful**: Core tests passing (135/135)
- ⚠️ **Technical debt**: Many handlers commented out due to User-Specialization relationship changes
- 📍 **You are here**: Ready to start Phase 2 - Implement missing entities

## Command to Send to New Claude Chat:

```
Continue implementing the SledzSpecke SMK compliance roadmap. Phase 1 is complete. Start with Phase 2.

Current state:
- User entity has been updated with PESEL, PWZ, FirstName, LastName, CorrespondenceAddress
- Specialization entity has been updated with UserId, ProgramVariant, PlannedPesYear, ActualEndDate, Status
- Many handlers are temporarily commented out with TODO notes (search for "TODO: User-Specialization relationship")
- Build is successful, core tests passing

Next tasks (Phase 2):
1. Implement Procedure entity hierarchy for Old/New SMK:
   - Create abstract base Procedure class
   - Create ProcedureOldSmk and ProcedureNewSmk derived classes
   - Update repositories and configurations
   
2. Implement AdditionalSelfEducationDays entity:
   - Max 6 days per year validation
   - Associated with Module and Internship
   
3. Start implementing business rules (Phase 3):
   - ShiftDuration value object (allows minutes > 59)
   - Module progression rules
   - Duration calculations

Key files to check:
- /home/ubuntu/projects/mock/IMPORTANT/sledzspecke-compliance-roadmap.md (updated roadmap)
- /home/ubuntu/projects/mock/IMPORTANT/sledzspecke-business-logic.md (updated business logic)
- /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Core/Entities/ (entity definitions)

Remember:
- This is a PRODUCTION medical system
- Every field must match SMK exactly
- No approval workflows (all approvals happen in SMK)
- Use clean architecture principles
- Run "dotnet build" frequently to catch errors early
```

## Additional Notes for Next Session

### Key Technical Decisions Made:
1. **PESEL Validation**: Full checksum implementation with date/gender extraction
2. **PWZ Validation**: 7 digits, cannot start with 0
3. **Address**: Polish postal code format (XX-XXX)
4. **User-Specialization**: Now properly linked via UserId in Specialization entity

### Areas Needing Attention:
1. **Handler Restoration**: Once entities are complete, uncomment and fix handlers
2. **Database Migrations**: Create EF Core migrations for new entity structure
3. **Integration Tests**: Update to match new domain model
4. **XLSX Export**: High priority after entities are complete

### Development Environment:
- Working directory: `/home/ubuntu/projects/mock/SledzSpecke.WebApi`
- Production API: https://api.sledzspecke.pl
- Database: PostgreSQL (sledzspecke_db)
- Build command: `dotnet build`
- Test command: `dotnet test`

### Git Status:
- All changes committed and pushed
- Commit: "feat: Implement SMK compliance Phase 1 - Core entity alignment"
- Branch: master