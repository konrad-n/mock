# Repository Pattern Refactoring Guide

## Overview

This document outlines the refactoring strategy for improving the repository pattern implementation in the SledzSpecke application. The goal is to reduce code duplication, improve testability, and make queries more maintainable using the Specification pattern.

## Current State

### Problems with Current Implementation
1. **No inheritance from BaseRepository** - All repositories implement everything from scratch
2. **Duplicate query logic** - Same queries repeated across repositories
3. **Direct SaveChangesAsync calls** - Should be handled by Unit of Work
4. **Complex joins in repositories** - Makes testing difficult
5. **ID generation duplicated** - Same logic in multiple repositories

## Target Architecture

### 1. Generic Repository Interface
```csharp
public interface IGenericRepository<TEntity> where TEntity : AggregateRoot
{
    // Basic CRUD
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    
    // Specification-based queries
    Task<IEnumerable<TEntity>> GetBySpecificationAsync(ISpecification<TEntity> specification);
    Task<TEntity?> GetSingleBySpecificationAsync(ISpecification<TEntity> specification);
    Task<int> CountBySpecificationAsync(ISpecification<TEntity> specification);
}
```

### 2. Specification Pattern
```csharp
// Instead of:
public async Task<IEnumerable<MedicalShift>> GetByDateRangeAsync(DateTime start, DateTime end, int userId)
{
    // Complex query logic here
}

// Use:
var specification = new MedicalShiftByInternshipIdsSpecification(internshipIds)
    .And(new MedicalShiftByDateRangeSpecification(start, end));
var shifts = await repository.GetBySpecificationAsync(specification);
```

### 3. Repository Implementation
```csharp
public class SqlMedicalShiftRepository : BaseRepository<MedicalShift>, IMedicalShiftRepository
{
    // Only implement domain-specific methods
    // Let BaseRepository handle common operations
}
```

## Refactoring Steps

### Step 1: Create Specifications (✅ Completed)
- Created specifications for MedicalShift, User, Internship
- Added composite specifications for common queries
- Location: `/src/SledzSpecke.Core/Specifications/`

### Step 2: Update BaseRepository (✅ Completed)
- Added IGenericRepository interface implementation
- Added pagination and count methods for specifications
- Enhanced with specification support

### Step 3: Refactor Individual Repositories (🔧 In Progress)
1. **MedicalShiftRepository** - Example created as `RefactoredSqlMedicalShiftRepository`
2. **UserRepository** - Next priority
3. **InternshipRepository** - After UserRepository
4. **ProcedureRepository** - Already has specifications
5. Others - Lower priority

### Step 4: Remove SaveChangesAsync from Repositories
- Move all SaveChangesAsync calls to Unit of Work
- Update command handlers to use UnitOfWork.SaveChangesAsync()

### Step 5: Create ID Generation Service
```csharp
public interface IIdGenerationService
{
    Task<TId> GenerateNextIdAsync<TEntity, TId>() where TEntity : AggregateRoot;
}
```

## Migration Strategy

### For Each Repository:

1. **Create new repository class inheriting from BaseRepository**
```csharp
internal sealed class SqlUserRepository : BaseRepository<User>, IUserRepository
{
    public SqlUserRepository(SledzSpeckeDbContext context) : base(context) { }
    
    // Only implement methods not covered by BaseRepository
}
```

2. **Replace complex queries with specifications**
```csharp
// Old way
public async Task<User?> GetByUsernameAsync(string username)
{
    return await _context.Users.FirstOrDefaultAsync(u => u.Username.Value == username);
}

// New way
public async Task<User?> GetByUsernameAsync(Username username)
{
    var spec = new UserByUsernameSpecification(username);
    return await GetSingleBySpecificationAsync(spec);
}
```

3. **Remove SaveChangesAsync calls**
```csharp
// Remove this from repository methods:
await _context.SaveChangesAsync();

// Add to command handler instead:
await _unitOfWork.SaveChangesAsync();
```

4. **Test thoroughly before switching**
- Create unit tests for new specifications
- Integration test the refactored repository
- Ensure backward compatibility

## Example: Refactoring UserRepository

### Before:
```csharp
public async Task<IEnumerable<User>> GetBySpecializationIdAsync(int specializationId)
{
    return await _context.Users
        .Where(u => u.SpecializationId == specializationId)
        .ToListAsync();
}
```

### After:
```csharp
public async Task<IEnumerable<User>> GetBySpecializationIdAsync(SpecializationId specializationId)
{
    var spec = new UserBySpecializationSpecification(specializationId);
    return await GetBySpecificationAsync(spec);
}
```

## Benefits

1. **Reduced Code Duplication** - Common logic in BaseRepository
2. **Better Testability** - Specifications can be unit tested
3. **Composable Queries** - Combine specifications with And/Or/Not
4. **Cleaner Repositories** - Focus on domain logic, not infrastructure
5. **Consistent Pattern** - All repositories follow same structure

## Testing Strategy

### Unit Tests for Specifications
```csharp
[Fact]
public void UserByEmailSpecification_Should_Match_User_With_Email()
{
    // Arrange
    var email = new Email("test@example.com");
    var spec = new UserByEmailSpecification(email);
    var user = new User { Email = email };
    
    // Act
    var result = spec.IsSatisfiedBy(user);
    
    // Assert
    result.Should().BeTrue();
}
```

### Integration Tests for Repositories
```csharp
[Fact]
public async Task GetBySpecification_Should_Return_Matching_Entities()
{
    // Arrange
    var spec = new ActiveInternshipSpecification();
    
    // Act
    var internships = await repository.GetBySpecificationAsync(spec);
    
    // Assert
    internships.Should().OnlyContain(i => i.IsActive && !i.IsCompleted);
}
```

## Timeline

1. **Week 1**: Refactor high-traffic repositories (User, MedicalShift, Internship)
2. **Week 2**: Refactor remaining repositories
3. **Week 3**: Remove SaveChangesAsync, implement ID generation service
4. **Week 4**: Testing and optimization

## Checklist

- [x] Create specification classes
- [x] Update BaseRepository with IGenericRepository
- [x] Create example refactored repository
- [ ] Refactor UserRepository
- [ ] Refactor InternshipRepository
- [ ] Refactor ProcedureRepository
- [ ] Refactor remaining repositories
- [ ] Create ID generation service
- [ ] Update all command handlers to use UnitOfWork
- [ ] Remove SaveChangesAsync from all repositories
- [ ] Update repository registrations in DI
- [ ] Comprehensive testing
- [ ] Performance benchmarking

## Notes

- Keep old repositories until new ones are fully tested
- Run repositories side-by-side initially
- Monitor performance impact
- Document any breaking changes

---
*Last Updated: 2025-06-16*