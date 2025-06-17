# Repository Migration Status

## Overview
This document tracks the status of repository migrations from direct EF Core implementations to the specification pattern using BaseRepository.

## Migration Status

### ✅ Already Refactored (Using BaseRepository + Specifications)
1. **RefactoredSqlMedicalShiftRepository** - Full implementation with specifications
2. **RefactoredSqlUserRepository** - Full implementation with specifications  
3. **RefactoredSqlProcedureRepository** - Full implementation with specifications
4. **RefactoredSqlInternshipRepository** - Full implementation with specifications

### ❌ Needs Migration (Direct EF Core Implementation)
1. **SqlCourseRepository** - Direct DbContext access, no specifications
2. **SqlRecognitionRepository** - Direct DbContext access, no specifications
3. **SqlPublicationRepository** - Direct DbContext access, no specifications
4. **SqlAbsenceRepository** - Direct DbContext access, no specifications
5. **SqlModuleRepository** - Direct DbContext access, no specifications
6. **SqlSpecializationRepository** - Direct DbContext access, no specifications
7. **SqlSelfEducationRepository** - Direct DbContext access, no specifications
8. **SqlAdditionalSelfEducationDaysRepository** - Direct DbContext access, no specifications

### ⚠️ Other Implementations (Check Status)
1. **EducationalActivityRepository** - Need to verify implementation
2. **FileMetadataRepository** - Need to verify implementation
3. **SqlInternshipRepositoryEnhanced** - Enhanced version (might be obsolete)
4. **SqlMedicalShiftRepositoryEnhanced** - Enhanced version (might be obsolete)

## Migration Pattern

### Before (Direct EF Core)
```csharp
internal sealed class SqlCourseRepository : ICourseRepository
{
    private readonly SledzSpeckeDbContext _context;
    private readonly DbSet<Course> _courses;

    public async Task<Course?> GetByIdAsync(CourseId id)
        => await _courses.SingleOrDefaultAsync(c => c.Id == id);
}
```

### After (BaseRepository + Specifications)
```csharp
internal sealed class RefactoredSqlCourseRepository : BaseRepository<Course>, ICourseRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Course?> GetByIdAsync(CourseId id)
    {
        return await GetByIdAsync(id.Value, default);
    }

    public async Task<IEnumerable<Course>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        var specification = new CourseBySpecializationSpecification(specializationId);
        return await GetBySpecificationAsync(specification);
    }
}
```

## Migration Priority (Based on Usage)
1. **SqlSpecializationRepository** - Core entity, high impact
2. **SqlCourseRepository** - Used in multiple handlers
3. **SqlPublicationRepository** - Complex queries benefit from specifications
4. **SqlAbsenceRepository** - Date range queries benefit from specifications
5. **SqlModuleRepository** - Part of core workflow
6. **SqlSelfEducationRepository** - Medium priority
7. **SqlRecognitionRepository** - Lower priority
8. **SqlAdditionalSelfEducationDaysRepository** - Lower priority

## Key Benefits of Migration
- **Testability**: Specifications can be unit tested independently
- **Reusability**: Common queries shared across repositories
- **Composability**: Combine specifications with AND/OR operations
- **Consistency**: All repositories follow the same pattern
- **Performance**: Optimized queries with proper indexing hints

## Notes
- Enhanced repositories (e.g., SqlInternshipRepositoryEnhanced) may be obsolete after refactoring
- All refactored repositories should remove SaveChangesAsync() calls (let UnitOfWork handle it)
- Need to create corresponding specification classes for each entity type