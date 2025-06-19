# SledzSpecke SMK Alignment Refactoring Plan & Tracking

**CRITICAL DEADLINE: Must be production-ready within 24 hours**
**Start Time: December 19, 2024**
**Target Completion: December 20, 2024**

## 🚨 CRITICAL REQUIREMENTS FROM UI MOCKUPS

### 1. Module Structure (MUST MATCH)
- ✅ Only TWO module types: "Podstawowy" (Basic) and "Specjalistyczny" (Specialist)
- ✅ Tab-based switching between modules
- ❌ Dynamic data loading based on selected module

### 2. Medical Shifts (Dyżury) Requirements
- ✅ **Nowy SMK**: Hours/minutes entry with date range (Od/Do)
- ✅ **Stary SMK**: Year-based shifts (Rok 1-5) with 24-hour blocks
- ✅ Location field (string) for department/unit
- ✅ NO approval workflow in UI

### 3. Procedures (Procedury) Requirements
- ❌ **Nowy SMK**: Simple counters (executed/required) 
- ❌ **Stary SMK**: A/B operator classification with counters
- ❌ Expandable list UI with "DODAJ REALIZACJĘ" button
- ❌ NO patient data tracking

### 4. Internships (Staże) Requirements
- ❌ Pre-defined internship names from specialization
- ❌ Simple day counting (X z Y dni)
- ❌ "SZCZEGÓŁY" button for realizations
- ❌ Status calculated from realizations

## 📋 IMPLEMENTATION TRACKING

### Phase 1: Domain Model Changes ✅ COMPLETED
- [x] Update ModuleType enum (Basic, Specialist)
- [x] Create ProcedureRequirement entity
- [x] Create ProcedureRealization entity
- [x] Create value objects (ProcedureRole, IDs)
- [x] Update MedicalShift entity (kept compatible)

### Phase 2: Database Layer ✅ COMPLETED
- [x] Add DbSet for ProcedureRequirement
- [x] Add DbSet for ProcedureRealization
- [x] Create EF configurations
- [x] Generate and run migration
- [ ] Migrate existing procedure data

### Phase 3: Repository Layer ⏳ PENDING
- [ ] Create IProcedureRequirementRepository
- [ ] Create IProcedureRealizationRepository  
- [ ] Implement SqlProcedureRequirementRepository
- [ ] Implement SqlProcedureRealizationRepository
- [ ] Remove old IProcedureRepository

### Phase 4: Application Layer ⏳ PENDING
#### Commands
- [ ] Create AddProcedureRealizationCommand
- [ ] Create UpdateProcedureRealizationCommand
- [ ] Create DeleteProcedureRealizationCommand
- [ ] Remove old procedure commands

#### Handlers
- [ ] Implement AddProcedureRealizationHandler
- [ ] Implement UpdateProcedureRealizationHandler
- [ ] Implement DeleteProcedureRealizationHandler
- [ ] Implement GetModuleProceduresHandler
- [ ] Remove old procedure handlers

#### DTOs
- [ ] Create ProcedureRequirementDto
- [ ] Create ProcedureRealizationDto
- [ ] Create ProcedureProgressDto
- [ ] Update ModuleProceduresDto

### Phase 5: API Layer ⏳ PENDING
- [ ] Create new ProceduresController endpoints
- [ ] GET /api/modules/{moduleId}/procedures
- [ ] POST /api/procedures/realizations
- [ ] PUT /api/procedures/realizations/{id}
- [ ] DELETE /api/procedures/realizations/{id}
- [ ] Update ModulesController

### Phase 6: Domain Services ⏳ PENDING
- [ ] Update ProcedureValidationService
- [ ] Update InternshipCompletionService
- [ ] Update ModuleProgressionService
- [ ] Update SmkComplianceValidator

### Phase 7: Infrastructure Services ⏳ PENDING
- [ ] Update SmkExportService for new structure
- [ ] Update SmkExcelGenerator
- [ ] Update StatisticsService
- [ ] Update SpecializationTemplateService

### Phase 8: Data Migration ⏳ PENDING
- [ ] Script to migrate ProcedureBase to ProcedureRequirement
- [ ] Script to migrate procedure instances to ProcedureRealization
- [ ] Populate requirements from templates
- [ ] Verify data integrity

### Phase 9: Testing ⏳ PENDING
- [ ] Unit tests for new entities
- [ ] Integration tests for new endpoints
- [ ] Update existing tests
- [ ] E2E tests for new workflow

### Phase 10: Deployment ⏳ PENDING
- [ ] Final build verification
- [ ] Run all migrations
- [ ] Deploy to production
- [ ] Smoke tests

## 🔍 DETAILED FILE CHANGES

### Entities to Remove
1. `/src/SledzSpecke.Core/Entities/ProcedureBase.cs`
2. `/src/SledzSpecke.Core/Entities/ProcedureOldSmk.cs`
3. `/src/SledzSpecke.Core/Entities/ProcedureNewSmk.cs`
4. `/src/SledzSpecke.Core/Entities/Procedure.cs`
5. `/src/SledzSpecke.Core/Entities/ProcedureBaseEnhanced.cs`

### New Entities Created
1. ✅ `/src/SledzSpecke.Core/Entities/ProcedureRequirement.cs`
2. ✅ `/src/SledzSpecke.Core/Entities/ProcedureRealization.cs`
3. ✅ `/src/SledzSpecke.Core/ValueObjects/ProcedureRole.cs`
4. ✅ `/src/SledzSpecke.Core/ValueObjects/ProcedureRequirementId.cs`
5. ✅ `/src/SledzSpecke.Core/ValueObjects/ProcedureRealizationId.cs`

### Repositories to Update
1. `/src/SledzSpecke.Core/Repositories/IProcedureRepository.cs` → Split into two
2. `/src/SledzSpecke.Infrastructure/DAL/Repositories/SqlProcedureRepository.cs` → Replace
3. `/src/SledzSpecke.Infrastructure/DAL/Repositories/RefactoredSqlProcedureRepository.cs` → Replace

### API Controllers to Update
1. `/src/SledzSpecke.Api/Controllers/ProceduresController.cs` - Complete rewrite
2. `/src/SledzSpecke.Api/Controllers/ModulesController.cs` - Add procedure endpoints
3. `/src/SledzSpecke.Api/Controllers/SmkController.cs` - Update exports

### Commands/Handlers to Replace
1. AddProcedure → AddProcedureRealization
2. UpdateProcedure → UpdateProcedureRealization
3. DeleteProcedure → DeleteProcedureRealization
4. GetUserProcedures → GetModuleProcedures
5. GetProcedureById → GetProcedureRealizationById

### DTOs to Create/Update
1. ProcedureDto → Split into RequirementDto and RealizationDto
2. ProcedureSummaryDto → ProcedureProgressDto
3. Update DashboardOverviewDto procedures section

### Domain Services Updates
1. ProcedureValidationService - Validate against requirements
2. InternshipCompletionService - Count realizations
3. ModuleProgressionService - Check procedure completion
4. SmkComplianceValidator - Validate requirements met

### Export Services Updates
1. SmkExportService - Export realizations by requirement
2. SmkExcelGenerator - Format for SMK import
3. ApplicationSmkExcelGenerator - Handle both versions

## 🚀 EXECUTION LOG

### Hour 1-2: Database Layer
- Creating EF configurations
- Updating DbContext
- Generating migration

### Hour 3-4: Repository Implementation
- Implementing new repositories
- Updating dependency injection

### Hour 5-8: Application Layer
- Commands and handlers
- DTOs and mappings
- Validation logic

### Hour 9-12: API Layer
- New endpoints
- Testing with Postman
- Swagger documentation

### Hour 13-16: Domain Services
- Business logic updates
- Validation rules
- Progress calculations

### Hour 17-20: Data Migration
- Migration scripts
- Data verification
- Integrity checks

### Hour 21-23: Testing & Fixes
- Run all tests
- Fix breaking changes
- Performance testing

### Hour 24: Deployment
- Final build
- Production deployment
- Smoke tests

## 🐛 ISSUES FOUND & FIXED

1. ModuleType enum values changed - Fixed all references
2. Build errors with Result pattern - Added proper using statements
3. SpecializationTemplate property mismatch - Updated to use nested objects

## 📊 PROGRESS METRICS

- **Total Files to Modify**: ~80 files
- **Files Modified**: 10/80 (12.5%)
- **Build Status**: ✅ SUCCESS (0 errors)
- **Tests Passing**: TBD
- **Time Remaining**: 23 hours

## 🎯 NEXT IMMEDIATE ACTIONS

1. Complete database configuration
2. Run migration
3. Implement repositories
4. Start on command handlers

---

**Last Updated**: December 19, 2024 - Starting intensive implementation phase