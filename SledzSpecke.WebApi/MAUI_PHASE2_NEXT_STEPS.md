# MAUI Phase 2 - Next Steps Guide

## 🚀 Quick Start for Next Session

### Current State Summary
- **Core Layer**: ✅ Fully updated and compiling
- **Infrastructure Layer**: ✅ Fully updated and compiling  
- **Application Layer**: ❌ 177 compilation errors blocking migration
- **Migration**: ⏳ Cannot create until Application layer compiles

### Step 1: Fix Application Layer Errors

#### 1.1 Run Bulk Fixes First
```bash
cd /home/ubuntu/projects/mock/SledzSpecke.WebApi

# Fix ID property .Value accessors
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.UserId\.Value/.UserId/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.ModuleId\.Value/.ModuleId/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.ShiftId\.Value/.ShiftId/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.SpecializationId\.Value/.SpecializationId/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.InternshipId\.Value/.InternshipId/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.ProcedureId\.Value/.ProcedureId/g' {} \;

# Fix string property .Value accessors
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.Email\.Value/.Email/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.Name\.Value/.Name/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\.PhoneNumber\.Value/.PhoneNumber/g' {} \;

# Fix Duration references
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/shift\.Duration\.Hours/shift.Hours/g' {} \;
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/shift\.Duration\.Minutes/shift.Minutes/g' {} \;

# Fix User name references
find src/SledzSpecke.Application -name "*.cs" -exec sed -i 's/\${user\.FirstName} \${user\.LastName}/user.Name/g' {} \;

# Check error count after bulk fixes
dotnet build 2>&1 | grep -c "error CS"
```

#### 1.2 Fix Remaining Errors by Pattern

**Pattern 1: Enum Namespace Conflicts**
```csharp
// Add namespace qualifiers where needed
SledzSpecke.Core.Enums.ProcedureStatus
SledzSpecke.Core.Enums.SyncStatus
SledzSpecke.Core.Enums.ModuleType
SledzSpecke.Core.Enums.SmkVersion
```

**Pattern 2: Missing Properties**
- User entity no longer has FirstName/LastName - use Name
- MedicalShift no longer has Duration - use Hours and Minutes
- Entities use specific ID properties (UserId, ModuleId) not generic Id

**Pattern 3: Value Object Conversions**
```csharp
// Remove these patterns:
new Email(command.Email) → command.Email
new UserId(id) → id
SmkVersion.From(value) → (SmkVersion)value
```

### Step 2: Create the Migration

Once Application layer compiles:

```bash
# Create the migration
dotnet ef migrations add MAUIPhase2_EntityStructureUpdate \
  -p src/SledzSpecke.Infrastructure \
  -s src/SledzSpecke.Api

# Review the generated migration
cat src/SledzSpecke.Infrastructure/Migrations/*MAUIPhase2_EntityStructureUpdate.cs

# Key changes to verify in migration:
# 1. Column renames (Id → UserId, ModuleId, ShiftId, etc.)
# 2. New tables for split entities (RealizedMedicalShiftOldSMK, etc.)
# 3. Removed value object columns
# 4. Updated foreign key constraints
```

### Step 3: Update Repositories

Key repository changes needed:

```csharp
// Before (with Value Objects)
public async Task<User?> GetByIdAsync(UserId id)
{
    return await _context.Users
        .FirstOrDefaultAsync(u => u.Id == id.Value);
}

// After (without Value Objects)
public async Task<User?> GetByIdAsync(int id)
{
    return await _context.Users
        .FirstOrDefaultAsync(u => u.UserId == id);
}
```

### Step 4: Test the Migration

```bash
# Backup database first
sudo -u postgres pg_dump sledzspecke_db > backup_before_phase2.sql

# Apply migration
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api

# Verify schema changes
sudo -u postgres psql sledzspecke_db -c "\d+ Users"
sudo -u postgres psql sledzspecke_db -c "\d+ Modules"
sudo -u postgres psql sledzspecke_db -c "\dt"
```

## 🔍 Debugging Tips

### Check Current Error Types
```bash
# Group errors by type
dotnet build 2>&1 | grep "error CS" | cut -d: -f4 | sort | uniq -c | sort -nr

# Find specific error patterns
dotnet build 2>&1 | grep "does not contain a definition for 'Value'"
dotnet build 2>&1 | grep "cannot be applied to operands of type"
dotnet build 2>&1 | grep "Cannot implicitly convert type"
```

### Focus on High-Impact Files
```bash
# Find files with most errors
dotnet build 2>&1 | grep "error CS" | cut -d'(' -f1 | sort | uniq -c | sort -nr | head -20
```

## 📝 Important Notes

1. **Do NOT modify Core or Infrastructure layers** - they are complete and working
2. **Focus only on Application layer** - all errors are there
3. **Test compilation frequently** - after each batch of fixes
4. **Value Objects are completely removed** - don't try to use them
5. **Entity IDs are now plain integers** - no wrapper types

## 🎯 Success Criteria

Phase 2 is complete when:
1. ✅ Application layer compiles with 0 errors
2. ✅ EF Core migration is generated successfully
3. ✅ Migration applies cleanly to database
4. ✅ All unit tests pass (after updates)
5. ✅ API starts successfully

## 💡 Common Gotchas

1. **SmkVersion comparison** - Cast to enum when needed: `(SmkVersion)user.SmkVersion == SmkVersion.Old`
2. **Missing using statements** - Add `using SledzSpecke.Core.Enums;` where needed
3. **Null reference on IDs** - IDs are now non-nullable integers
4. **Repository interfaces** - May need updating to remove Value Object parameters

## 🔗 Related Documentation

- Original MAUI structure: `/home/ubuntu/projects/mock/MAUI.md`
- Phase 1 completion: `/home/ubuntu/projects/mock/SledzSpecke.WebApi/MAUI_PHASE1_COMPLETION_SUMMARY.md`
- Entity mappings: `/home/ubuntu/projects/mock/SledzSpecke.WebApi/MAUI_ENTITY_MAPPING.md`
- Implementation plan: `/home/ubuntu/projects/mock/SledzSpecke.WebApi/MAUI_IMPLEMENTATION_PLAN.md`