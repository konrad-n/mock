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

## 🚨 CURRENT STATE - IMPORTANT FOR NEXT SESSION

### What Was Just Completed
1. **Unit Tests for Value Objects**: Created comprehensive test suite with 89 passing tests
2. **EF Core Duration Fix**: Configured Duration as owned entity, fixing database initialization
3. **Removed Over-Engineering**: Deleted EmployeeNumber value object (YAGNI violation)
4. **Updated Documentation**: Added critical lesson about avoiding over-engineering

### Known Issues to Address
1. **API Startup**: Development API conflicts with production on port 5000
   - Production API runs on https://api.sledzspecke.pl (port 5000)
   - For testing, use: `ASPNETCORE_URLS="http://localhost:5263" dotnet run`
2. **Points Value Object**: Not configured in EF Core (needs value converter)
3. **DateRange Value Object**: Not configured in EF Core (needs value converter)

### Next Immediate Steps
1. Complete Result pattern E2E testing (IN PROGRESS)
2. Configure missing EF Core value converters for Points and DateRange
3. Fix Procedure entity domain methods

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

### ✅ COMPLETED TASKS

#### 1. ~~Create Unit Tests for Value Objects~~ ✅ DONE
**Completed**: Created `/tests/SledzSpecke.Core.Tests/` project with comprehensive tests
- ✅ Duration: 13 tests covering all scenarios
- ✅ Points: 15 tests including operators and conversions  
- ✅ DateRange: 14 tests including overlap detection
- ✅ Email: 13 tests with validation scenarios
- ❌ EmployeeNumber: **DELETED** - violated YAGNI (no usage found)
**All 89 tests passing**

#### 2. ~~Configure EF Core Value Converters~~ ✅ DONE
**Completed**: Fixed Duration value object persistence
- ✅ Configured Duration as owned entity in MedicalShiftConfiguration
- ✅ Added parameterless constructor to MedicalShift for EF Core
- ✅ Database initialization now succeeds
- ✅ Application starts without EF Core errors

### 🔴 HIGH PRIORITY - Core Functionality

#### 1. Test Result Pattern End-to-End (IN PROGRESS)
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

#### 2. Fix Procedure Entity Domain Methods
**Why**: Currently bypassed with TODO comment
**Location**: `/src/SledzSpecke.Core/Entities/Internship.cs` line 270
**What to do**:
1. Check existing Procedure entity structure
2. Add proper factory methods to ProcedureOldSmk and ProcedureNewSmk
3. Implement AddProcedure domain method properly
4. Update AddProcedureHandler to use domain method

### 🟡 MEDIUM PRIORITY - Architecture Enhancements

#### 4. Implement Repository with Unit of Work
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

#### 5. Configure Missing EF Core Value Converters
**Why**: Points and DateRange aren't persisted correctly
**Location**: `/src/SledzSpecke.Infrastructure/DAL/Configurations/`
**What to do**:
1. Find entities using Points (likely in procedures)
2. Configure as owned entities or value converters
3. Test with actual database operations

#### 6. Complete Domain Event Implementation
**Why**: Events are raised but not dispatched
**Current**: `DomainEventDispatcher.cs.bak` is disabled
**Steps**:
1. Install MediatR package
2. Re-enable DomainEventDispatcher
3. Create event handlers for MedicalShiftCreatedEvent
4. Hook into SaveChangesAsync to dispatch events

### 🟢 LOW PRIORITY - Nice to Have

#### 7. Add Integration Tests
**Why**: Test database operations with real database
**Use**: TestContainers for PostgreSQL
**Test**: Repository methods, transactions, value object persistence

#### 8. Performance Monitoring
**Current**: PerformanceCommandHandlerDecorator logs but doesn't alert
**Enhancement**: Add threshold alerts for slow operations

#### 9. API Documentation
**Tool**: Swagger/OpenAPI
**Include**: Error codes, example requests/responses

## ⚡ CRITICAL LESSON: Avoiding Over-Engineering

### Real Example from This Project
We created an `EmployeeNumber` Value Object with full validation, exceptions, and tests. After implementing it, we discovered:
- **Zero usage** in any entity, handler, or API endpoint
- **No mention** in E2E tests or user scenarios
- **No field** in the database that would use it

**This was immediately deleted** as it violated YAGNI.

### How to Validate Before Creating New Abstractions
Before creating ANY new Value Object, pattern, or abstraction:

1. **Search for usage first**:
   ```bash
   # Check if the concept exists in entities
   grep -r "employee.*number\|employeeId" src/
   
   # Check E2E tests for real scenarios
   grep -r "employee" tests/SledzSpecke.E2E.Tests/
   ```

2. **Check the database schema**:
   - Look at entity configurations
   - Check migration files
   - Verify actual column names

3. **Review E2E test scenarios**:
   - These represent REAL user workflows
   - If it's not tested in E2E, users probably don't need it

4. **Ask yourself**:
   - Is this solving a problem that exists TODAY?
   - Is there primitive obsession that's causing bugs?
   - Will this abstraction be used in at least 2-3 places?

### Value Objects That ARE Justified
- `Email` - Used in User entity, has complex validation
- `Duration` - Used in MedicalShift, prevents invalid time values
- `Points` - Used in procedures, has business rules (0-1000 range)
- `DateRange` - Used for validations, has overlap logic

## 🛑 WHAT NOT TO DO

1. **DO NOT** add new patterns without clear justification
2. **DO NOT** refactor working code just to "make it cleaner"
3. **DO NOT** implement features not in current requirements
4. **DO NOT** use complex libraries when simple solutions work
5. **DO NOT** ignore existing patterns - follow what's established
6. **DO NOT** create Value Objects without confirming they solve real problems

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