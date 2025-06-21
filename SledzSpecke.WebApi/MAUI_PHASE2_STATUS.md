# MAUI Phase 2 Migration Status

## Current Status: Core Layer Being Fixed, Infrastructure and Application Next

### ✅ Completed Tasks

#### 1. Core Layer Entity Updates
- ✅ All entities updated to match MAUI structure
- ✅ Value Objects removed from entity properties
- ✅ ID properties renamed (UserId, ModuleId, ShiftId, ProcedureId, etc.)
- ✅ Enum properties now use direct enum types instead of Value Objects
- ✅ Core project builds successfully with 0 errors

#### 2. Infrastructure Layer Updates
- All DbContext configurations updated for new entity structure
- Created configurations for split tables:
  - RealizedMedicalShiftOldSMKConfiguration
  - RealizedMedicalShiftNewSMKConfiguration
  - RealizedInternshipOldSMKConfiguration
  - RealizedInternshipNewSMKConfiguration
  - ProcedureOldSmkConfiguration
  - ProcedureNewSmkConfiguration
- Fixed all property mappings to use new names
- Infrastructure project builds successfully with 0 errors

#### 3. Core Layer Domain Services & Policies Updates
- ✅ MedicalShift policies updated to use Hours/Minutes instead of Duration
- ✅ Domain services updated for new property names (Id→ModuleId, etc.)
- ✅ Specifications updated for new property types and enums
- ✅ Fixed ambiguous references between ValueObjects and Enums namespaces
- ✅ SmkVersion comparisons updated for enum usage
- ✅ Core project builds successfully with 0 errors

#### 4. Key Entity Changes Made
- `User`: Id → UserId
- `Module`: Id → ModuleId
- `MedicalShift`: Id → ShiftId
- `Specialization`: Id → SpecializationId
- `Internship`: Id → InternshipId
- `Procedure`: Split into ProcedureOldSmk and ProcedureNewSmk
- All enum references fixed (SyncStatus, SmkVersion, ModuleType, etc.)

### ❌ Blocked Tasks

#### 1. EF Core Migration Creation
**Status**: Cannot create migration due to Application layer compilation errors
**Command to run when ready**:
```bash
dotnet ef migrations add MAUIPhase2_EntityStructureUpdate -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

#### 2. Application Layer Fixes Needed (384 errors)
Major issues to fix:
- Remove `.Value` accessors from ID properties (e.g., `user.UserId.Value` → `user.UserId`)
- Fix Value Object property access (e.g., `user.Email.Value` → `user.Email`)
- Update FirstName/LastName references to use Name property
- Fix Duration property references (use Hours/Minutes instead)
- Fix enum namespace conflicts
- Update all repository method calls

### 📋 Common Error Patterns to Fix

#### 1. ID Property Access
```csharp
// Wrong
var id = user.Id.Value;
var moduleId = module.Id;

// Correct
var id = user.UserId;
var moduleId = module.ModuleId;
```

#### 2. Value Object Properties
```csharp
// Wrong
var email = user.Email.Value;
var duration = shift.Duration.Hours;

// Correct
var email = user.Email;
var hours = shift.Hours;
```

#### 3. User Name Properties
```csharp
// Wrong
var fullName = $"{user.FirstName} {user.LastName}";

// Correct
var fullName = user.Name;
```

#### 4. Enum Comparisons
```csharp
// Wrong
if (procedure.Status == ProcedureStatus.Completed)

// Correct
if (procedure.Status == SledzSpecke.Core.Enums.ProcedureStatus.Completed)
```

### 🔧 Quick Fix Script Created
A script was created to fix common property references:
```bash
#!/bin/bash
# fix_property_refs.sh - Already executed
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/module\.Id/module.ModuleId/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/user\.Id/user.UserId/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/specialization\.Id/specialization.SpecializationId/g' {} \;
```

### 📁 Files Modified in Phase 2

#### Core Layer
- `/src/SledzSpecke.Core/Entities/Internship.cs` - Fixed SyncStatus references
- `/src/SledzSpecke.Core/Entities/ProcedureBase.cs` - Fixed enum references
- `/src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs` - Fixed enum namespaces
- `/src/SledzSpecke.Core/DomainServices/ModuleProgressionService.cs` - Fixed type conversions

#### Infrastructure Layer
- `/src/SledzSpecke.Infrastructure/DAL/Configurations/UserConfiguration.cs` - Updated for UserId
- `/src/SledzSpecke.Infrastructure/DAL/Configurations/ModuleConfiguration.cs` - Updated for ModuleId
- `/src/SledzSpecke.Infrastructure/DAL/Configurations/MedicalShiftConfiguration.cs` - Updated for ShiftId
- `/src/SledzSpecke.Infrastructure/DAL/Configurations/SpecializationConfiguration.cs` - Updated for SpecializationId
- Created 6 new configuration files for split tables

#### Application Layer (Partially Fixed)
- Various handlers and services have been partially updated
- Still requires comprehensive fixes to compile

### 🎯 Next Steps Priority Order

1. **Fix Application Layer Compilation** (High Priority)
   - Focus on fixing the 177 compilation errors
   - Use find/replace for common patterns
   - Test compilation frequently

2. **Create EF Core Migration** (High Priority)
   - Once Application compiles, create the migration
   - Review the migration file for correctness

3. **Update Repository Implementations** (Medium Priority)
   - Remove Value Object conversions
   - Update method signatures

4. **Test Migration** (Medium Priority)
   - Apply migration to test database
   - Verify schema changes

### 💡 Tips for Next Session

1. Start with a bulk find/replace for common patterns:
   ```bash
   # Remove .Value from ID properties
   find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.UserId\.Value/.UserId/g' {} \;
   find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.ModuleId\.Value/.ModuleId/g' {} \;
   
   # Fix FirstName/LastName
   find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/user\.FirstName/user.Name/g' {} \;
   find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/user\.LastName/""/g' {} \;
   ```

2. Focus on one error type at a time across all files

3. Build frequently to track progress:
   ```bash
   dotnet build 2>&1 | grep -c "error CS"
   ```

4. Once errors drop below 50, switch to fixing individual files

### 📊 Progress Metrics
- Phase 1: ✅ Complete (Entities updated)
- Phase 2 Core: ✅ Complete (Domain services & policies updated)
- Phase 2 Infrastructure: ✅ Complete (Configurations updated)
- Phase 2 Application: 🔄 In Progress (384 errors remaining)
- Phase 2 Migration: ⏳ Blocked (Waiting for Application fixes)