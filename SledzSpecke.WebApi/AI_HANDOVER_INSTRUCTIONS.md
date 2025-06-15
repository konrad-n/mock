# 🚀 SledzSpecke Architecture Improvement - AI Handover Instructions

## 🎯 Project Overview
SledzSpecke is a medical education tracking system for Polish medical residents. It tracks internships, medical shifts, procedures, and other educational activities according to SMK (System Monitorowania Kształcenia) requirements.

**Current State**: We've implemented Value Objects, Domain-Driven Design patterns, and Result pattern for error handling. The architecture follows Clean Architecture with SOLID principles.

## ⚠️ CRITICAL PRINCIPLES TO FOLLOW

### 1. KISS (Keep It Simple, Stupid)
- **DO NOT** over-engineer solutions
- **DO NOT** add patterns/abstractions unless they solve REAL problems
- Every pattern must be justified by actual use cases in E2E tests
- Check `/tests/SledzSpecke.E2E.Tests/Scenarios/` for real user workflows

### 2. YAGNI (You Aren't Gonna Need It)
- **DO NOT** implement features "for the future"
- **DO NOT** add flexibility that isn't currently needed
- Focus on current SMK requirements ONLY
- Review government PDFs in `/docs/` for actual requirements

### 3. Test EVERY Change
```bash
# After EVERY code change:
dotnet build                                    # Must compile
dotnet run --project src/SledzSpecke.Api       # Must start without errors
curl http://localhost:5263/api/health          # Must return OK

# For significant changes:
./run-e2e-tests.sh                             # Must pass E2E tests
```

### 4. Frequent Git Commits
- Commit after EACH successful feature/fix
- Push frequently to trigger GitHub Actions
- Check build status: `./check-builds.sh latest`
- Monitor dashboard: https://api.sledzspecke.pl/monitoring/dashboard

## 📁 Key Files & Patterns

### Value Objects (Already Implemented)
- Location: `/src/SledzSpecke.Core/ValueObjects/`
- Examples: `Duration`, `Points`, `DateRange`, `Email`
- Pattern: Immutable, validated in constructor, no invalid state

### Domain Entities with Business Logic
- Location: `/src/SledzSpecke.Core/Entities/`
- Key Entity: `Internship.cs` - Shows proper DDD implementation
- Pattern: Private setters, factory methods, domain methods return Result

### Result Pattern
- Location: `/src/SledzSpecke.Core/Abstractions/Result.cs`
- Usage: ALL domain methods return Result, NO exceptions for business rules
- Example: `internship.UpdateDates()` returns `Result` not void

### Current Architecture Layers
```
SledzSpecke.Api         → Controllers, HTTP concerns
SledzSpecke.Application → CQRS handlers, DTOs
SledzSpecke.Core        → Domain entities, Value Objects, interfaces
SledzSpecke.Infrastructure → EF Core, repositories, external services
```

## 📋 PRIORITY TASK LIST (DO IN ORDER!)

### 🔴 HIGH PRIORITY - Core Functionality

#### 1. Create Unit Tests for Value Objects ⏱️ 2-3 hours
**Why**: Ensure validation rules work correctly
**Location**: Create `/tests/SledzSpecke.Core.Tests/ValueObjects/`
**What to test**:
```csharp
// Example for Duration tests:
[Fact]
public void Duration_WithNegativeHours_ShouldThrowException()
[Fact] 
public void Duration_WithValidValues_ShouldCalculateTotalMinutesCorrectly()
[Fact]
public void Duration_Equality_ShouldWorkCorrectly()
```
**Test EACH Value Object**: Duration, Points, DateRange, Email, EmployeeNumber

#### 2. Test Result Pattern End-to-End ⏱️ 1-2 hours
**Why**: Verify error handling flows from domain to API
**How**:
1. Create test endpoint that triggers domain validation failure
2. Verify HTTP response contains proper error code/message
3. Test with Postman/curl:
```bash
# Should return 400 with specific error code
curl -X POST http://localhost:5263/api/internships/1/complete \
  -H "Authorization: Bearer {token}"
```

#### 3. Fix Procedure Entity Domain Methods ⏱️ 3-4 hours
**Why**: Currently bypassed with TODO comment
**Location**: `/src/SledzSpecke.Core/Entities/Internship.cs` line 270
**What to do**:
1. Check existing Procedure entity structure
2. Add proper factory methods to ProcedureOldSmk and ProcedureNewSmk
3. Implement AddProcedure domain method properly
4. Update AddProcedureHandler to use domain method

### 🟡 MEDIUM PRIORITY - Architecture Enhancements

#### 4. Implement Repository with Unit of Work ⏱️ 4-5 hours
**Why**: Ensure transactional consistency
**Current Issue**: Each repository calls SaveChangesAsync independently
**Implementation**:
```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```
**Update handlers** to use single UnitOfWork.SaveChangesAsync()

#### 5. Configure Missing EF Core Value Converters ⏱️ 2 hours
**Why**: Points and DateRange aren't persisted correctly
**Location**: `/src/SledzSpecke.Infrastructure/DAL/Configurations/`
**What to do**:
1. Find entities using Points (likely in procedures)
2. Configure as owned entities or value converters
3. Test with actual database operations

#### 6. Complete Domain Event Implementation ⏱️ 3-4 hours
**Why**: Events are raised but not dispatched
**Current**: `DomainEventDispatcher.cs.bak` is disabled
**Steps**:
1. Install MediatR package
2. Re-enable DomainEventDispatcher
3. Create event handlers for MedicalShiftCreatedEvent
4. Hook into SaveChangesAsync to dispatch events

### 🟢 LOW PRIORITY - Nice to Have

#### 7. Add Integration Tests ⏱️ 4-5 hours
**Why**: Test database operations with real database
**Use**: TestContainers for PostgreSQL
**Test**: Repository methods, transactions, value object persistence

#### 8. Performance Monitoring ⏱️ 2-3 hours
**Current**: PerformanceCommandHandlerDecorator logs but doesn't alert
**Enhancement**: Add threshold alerts for slow operations

#### 9. API Documentation ⏱️ 2 hours
**Tool**: Swagger/OpenAPI
**Include**: Error codes, example requests/responses

## 🛑 WHAT NOT TO DO

1. **DO NOT** add new patterns without clear justification
2. **DO NOT** refactor working code just to "make it cleaner"
3. **DO NOT** implement features not in current requirements
4. **DO NOT** use complex libraries when simple solutions work
5. **DO NOT** ignore existing patterns - follow what's established

## 🔍 How to Verify Your Work

### After Each Task:
1. **Build**: `dotnet build` - MUST succeed
2. **Run**: `dotnet run --project src/SledzSpecke.Api` - MUST start
3. **Test API**: `curl http://localhost:5263/api/health` - MUST return OK
4. **Check Logs**: No errors in console output
5. **Run E2E**: `./run-e2e-tests.sh` - MUST pass

### Before Committing:
1. **Format**: `dotnet format`
2. **Analyze**: `dotnet build /p:TreatWarningsAsErrors=true`
3. **Test**: All tests pass
4. **Review**: Changes align with KISS/YAGNI

### After Pushing:
1. **Check Build**: `./check-builds.sh latest`
2. **Monitor**: https://api.sledzspecke.pl/monitoring/dashboard
3. **Verify**: No errors in production logs

## 📚 Essential Context Files

1. **CLAUDE.md** - Overall project guidelines and patterns
2. **E2E Test Scenarios** - Real user workflows to understand requirements
3. **Government SMK PDFs** - Official requirements (in Polish)
4. **Existing Entities** - Study Internship.cs for patterns to follow

## 🎯 Success Criteria

Your implementation is successful when:
- ✅ All builds pass (local and GitHub Actions)
- ✅ E2E tests pass without modification
- ✅ No new complexity without clear justification  
- ✅ Code follows existing patterns
- ✅ Each PR is small and focused
- ✅ Production app continues working

## 💡 Final Tips

1. **Read existing code first** - Understand patterns before changing
2. **Start small** - One task at a time
3. **Ask why** - Every change needs a reason
4. **Test often** - Don't accumulate untested changes
5. **Keep it simple** - Simplest solution that works is best

Remember: You're maintaining a production system used by medical professionals. Stability and simplicity are more important than architectural purity.

Good luck! 🚀