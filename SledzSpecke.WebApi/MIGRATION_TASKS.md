# SledzSpecke Web API Migration Tasks

## IMPORTANT: AI Assistant Requirements

You are a **Senior .NET Architect and Clean Code Expert** with deep expertise in:
- **Domain-Driven Design (DDD)** and Clean Architecture principles
- **SOLID principles** applied rigorously in real-world scenarios
- **CQRS pattern** implementation without over-engineering
- **Value Objects** and fighting primitive obsession
- **Test-Driven Development (TDD)** with comprehensive test coverage
- **Migration strategies** from mobile (MAUI) to web architectures
- **Pattern recognition** to identify and apply best practices from reference projects

### Your Mindset for This Migration:
1. **Quality First**: Never compromise code quality for speed. Every line of code should be maintainable, testable, and follow clean code principles.
2. **Learn from MySpot**: Study the MySpot project patterns deeply - it's your gold standard for implementation.
3. **Preserve Business Logic**: The MAUI app (SledzSpecke.App) contains critical business rules that must be preserved during migration.
4. **Incremental Refactoring**: Make small, tested changes. Build frequently. Never break the build.
5. **No Hacks or Shortcuts**: Implement proper solutions. If something seems hacky, it probably is - find the clean way.
6. **Documentation Matters**: Update documentation as you go. Future maintainers (including AI) need to understand your decisions.

### Key Principles to Follow:
- **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **KISS**: Keep It Simple, Stupid - but not at the expense of proper architecture
- **YAGNI**: You Aren't Gonna Need It - don't over-engineer, but do engineer properly
- **DRY**: Don't Repeat Yourself - but don't create wrong abstractions

### What Makes You Perfect for This Task:
- You understand that **clean code is not optional** - it's the foundation of maintainable software
- You see **patterns, not just code** - you recognize when to apply specific patterns from MySpot
- You have **attention to detail** - every value object validation, every error message, every test case matters
- You think in **layers and boundaries** - keeping concerns separated and dependencies flowing correctly
- You are **cautious but confident** - you test everything but move forward decisively when code is clean

## Migration Focus

This document tracks the migration from SledzSpecke.App (MAUI) to SledzSpecke.WebApi with emphasis on **code quality** and **clean architecture patterns** from the MySpot reference project. 

**Key Principle**: Quality over features. We must ensure all migrated code follows SOLID principles, clean architecture, and best practices from MySpot before adding any new functionality.

## Current Migration Phase: Quality Refactoring

Before continuing with new features, we must:
1. Apply MySpot's clean architecture patterns to all existing code
2. Refactor existing implementations to follow SOLID principles
3. Ensure proper separation of concerns
4. Implement comprehensive error handling
5. Add integration tests for all endpoints

## Original Requirements

- Migrate all business logic from SledzSpecke.App (MAUI) to Web API
- Apply clean code principles from MySpot example project
- Maintain government-required specialization JSON files unchanged
- Test after each change to ensure API stability
- Follow SOLID, KISS, YAGNI principles throughout

## Detailed Task List

### ✅ Completed Tasks
1. **analyze-projects** - Analyze MAUI app (SledzSpecke.App) to identify all business logic, features, and data models
2. **review-myspot** - Review MySpot project for clean architecture patterns and SOLID principles to apply
3. **identify-missing-features** - Compare MAUI app features with current Web API to identify missing functionality
4. **verify-specialization-files** - Ensure specialization JSON files are properly integrated and unchanged
5. **fix-internship-endpoints** - Uncomment and implement DELETE internship endpoint
   - Created DeleteInternship command and handler
   - Created GetInternshipById query and handler
   - Fixed SmkVersion mapping issue in ModuleConfiguration
6. **add-dashboard-endpoint** - Create dashboard/progress endpoints for user statistics
   - Created DashboardController with overview, progress, and statistics endpoints
   - Created DTOs: DashboardOverviewDto, ModuleProgressDto, SpecializationInfoDto, DashboardProgressDtos
   - Created queries: GetDashboardOverview, GetModuleProgress
   - Created IProgressCalculationService and implementation with weighted progress calculation
   - Created query handlers: GetDashboardOverviewHandler, GetModuleProgressHandler
   - Fixed all compilation issues (ModuleType conflicts, DTO property mismatches, type conversions)
   - Dashboard endpoints ready:
     - GET /api/dashboard/overview
     - GET /api/dashboard/progress/{specializationId}
     - GET /api/dashboard/statistics/{specializationId}
7. **add-courses-endpoints** - Complete Courses controller with all CRUD operations
   - Created missing commands: UpdateCourse, DeleteCourse, CompleteCourse
   - Created missing query: GetCourseById
   - Created all handlers: GetCourseByIdHandler, UpdateCourseHandler, DeleteCourseHandler, CompleteCourseHandler
   - Updated CoursesController with all CRUD endpoints:
     - GET /api/courses/{courseId}
     - PUT /api/courses/{courseId}
     - DELETE /api/courses/{courseId}
     - POST /api/courses/{courseId}/complete
   - Note: Some Course entity methods are missing (UpdateName, UpdateInstitution, UpdateCompletionDate) - workaround implemented

8. **migrate-viewmodels-logic** - Extract business logic from MAUI ViewModels and convert to API handlers/services
   - Created module management functionality (SwitchModule command, GetAvailableModules query)
   - Added calculation endpoints for days, time normalization, and required shift hours
   - ModulesController with endpoints for module switching and listing
   - CalculationsController with various calculation endpoints
   - Successfully built the project with all new additions
9. **complete-publications-endpoints** - Complete all TODO endpoints in Publications controller
   - Created UpdatePublication command and handler
   - Created DeletePublication command and handler
   - Created queries: GetPeerReviewedPublications, GetFirstAuthorPublications, GetPublicationImpactScore
   - Created all query handlers with business logic
   - Added UpdateDetails method to Publication entity
   - Added ImpactFactor property to Publication entity and DTO
   - Updated controller with all endpoints:
     - GET /api/publications/user/{userId}/peer-reviewed
     - GET /api/publications/user/{userId}/first-author
     - GET /api/publications/user/{userId}/specialization/{specializationId}/impact-score
     - PUT /api/publications/{id}
     - DELETE /api/publications/{id}
   - Fixed all compilation errors related to Guid/int conversions
   - Build succeeded
10. **complete-recognitions-endpoints** - Complete all TODO endpoints in Recognitions controller
   - Created ApproveRecognition command and handler
   - Created GetTotalReductionDays query and handler
   - Created UpdateRecognition, DeleteRecognition commands and handlers
   - Added all endpoints to controller:
     - PUT /api/recognitions/{id}/approve
     - GET /api/recognitions/user/{userId}/specialization/{specializationId}/total-reduction
     - PUT /api/recognitions/{id}
     - DELETE /api/recognitions/{id}
   - Build succeeded
11. **complete-selfeducation-endpoints** - Complete all TODO endpoints in SelfEducation controller
   - Created queries: GetSelfEducationByYear, GetCompletedSelfEducation, GetTotalCreditHours, GetTotalQualityScore
   - Created commands: CompleteSelfEducation, UpdateSelfEducation, DeleteSelfEducation
   - Created all query and command handlers
   - Added UpdateDetails method and QualityScore property to SelfEducation entity
   - Updated controller with all endpoints:
     - GET /api/selfeducation/user/{userId}/year/{year}
     - GET /api/selfeducation/user/{userId}/specialization/{specializationId}/completed
     - GET /api/selfeducation/user/{userId}/specialization/{specializationId}/credit-hours
     - GET /api/selfeducation/user/{userId}/specialization/{specializationId}/quality-score
     - PUT /api/selfeducation/{id}/complete
     - PUT /api/selfeducation/{id}
     - DELETE /api/selfeducation/{id}
   - Fixed type conversion errors (decimal to double)
   - Build succeeded
12. **complete-absences-endpoints** - Complete all TODO endpoints in Absences controller
   - Created ApproveAbsence command and handler
   - Created UpdateAbsence, DeleteAbsence commands and handlers
   - Created AbsenceNotFoundException
   - Updated controller with all endpoints:
     - PUT /api/absences/{id}/approve
     - PUT /api/absences/{id}
     - DELETE /api/absences/{id}
   - Build succeeded
13. **fix-smkversion-mapping** - Fixed SmkVersion value object mapping in Entity Framework configurations
   - Updated SpecializationConfiguration to use proper value conversion
   - Updated UserConfiguration to use proper value conversion
   - Applied successfully, API now runs without errors
14. **create-user-profile-endpoints** - Created user profile management endpoints
   - Created UserProfileController with all profile endpoints
   - Created queries: GetUserProfile
   - Created commands: UpdateUserProfile, ChangePassword, UpdateUserPreferences
   - Created handlers for all commands and queries
   - Added new properties to User entity: PhoneNumber, DateOfBirth, Bio, ProfilePicturePath, preferences fields
   - Created UserProfileDto and UserPreferencesDto
   - Added migration AddUserProfileFields and applied to database
   - Updated CLAUDE.md with new endpoints and examples
   - All endpoints:
     - GET /api/users/profile
     - PUT /api/users/profile
     - PUT /api/users/change-password
     - PUT /api/users/preferences
15. **educational-activities-endpoints** - Created educational activities management
   - Created EducationalActivity entity with value objects
   - Created repository interface and implementation
   - Created CRUD commands and queries with Result pattern
   - Created EducationalActivitiesController with all endpoints
   - Added entity configuration and ran migration
   - All endpoints:
     - GET /api/educationalactivities/specialization/{id}
     - GET /api/educationalactivities/{id}
     - GET /api/educationalactivities/specialization/{id}/type/{type}
     - POST /api/educationalactivities
     - PUT /api/educationalactivities/{id}
     - DELETE /api/educationalactivities/{id}
16. **implement-file-upload** - Created file upload/download functionality
   - Created FileMetadata entity with proper value objects
   - Created IFileStorageService for file operations
   - Created IFileMetadataRepository for metadata persistence
   - Implemented UploadFile command with Result pattern
   - Implemented DownloadFile query with proper authorization
   - Created FilesController with all endpoints
   - Added file validation (size, type, security)
   - Created FileCleanupService for orphaned files
   - All endpoints:
     - POST /api/files/upload
     - GET /api/files/{fileId}/download
     - GET /api/files/entity/{entityType}/{entityId}
     - DELETE /api/files/{fileId}

### 🔄 In Progress Tasks
None - all migration tasks completed!

### 📋 Pending Tasks (Priority Order)

#### IMMEDIATE - Quality Refactoring (MUST DO BEFORE NEW FEATURES)
1. **analyze-myspot-patterns** - Deep dive into MySpot architecture patterns
   - Study CQRS implementation
   - Understand Value Object usage
   - Review error handling patterns
   - Examine testing strategies
2. **refactor-command-handlers** - Apply MySpot patterns to all command handlers
   - Implement Result pattern consistently
   - Add proper validation
   - Ensure single responsibility
3. **refactor-query-handlers** - Apply MySpot patterns to all query handlers
   - Remove business logic from queries
   - Implement proper DTOs
   - Add specification pattern where needed
4. **implement-value-objects** - Create and use proper value objects
   - Review all primitive obsession cases
   - Add validation in value objects
   - Implement equality and comparison
5. **add-integration-tests** - Create comprehensive integration tests
   - Test all API endpoints
   - Verify authorization
   - Check validation rules
6. **implement-error-handling** - Consistent error handling across API
   - Custom exceptions with meaningful messages
   - Proper HTTP status codes
   - Error response DTOs

#### HIGH - Complete Migration (ONLY AFTER QUALITY REFACTORING)
1. **implement-missing-endpoints** - Create missing API endpoints for all MAUI app features
   - File upload/download endpoints
   - Export/reporting endpoints
   - Sync status management
   - Educational activities endpoints
   - Search and filtering endpoints
   - Notifications/reminders
   - Batch operations
   - Year transition management

#### MEDIUM - Polish and Optimization
1. **implement-caching** - Add caching for frequently accessed data
2. **add-logging** - Comprehensive logging with correlation IDs
3. **performance-optimization** - Query optimization and N+1 prevention
4. **api-documentation** - Complete Swagger documentation

#### LOW - Final Steps
1. **cleanup-obsolete-code** - Remove any obsolete code and ensure YAGNI principle
2. **final-verification** - Final verification that all MAUI functionality is available in Web API
3. **deployment-preparation** - Prepare for production deployment

## Key Features from MAUI App to Migrate

### Already Implemented
- ✅ Authentication (SignIn/SignUp)
- ✅ Complete Internships CRUD (including DELETE and GET by ID)
- ✅ Complete Medical Shifts CRUD
- ✅ Complete Procedures CRUD
- ✅ Specialization templates
- ✅ Complete Courses CRUD (all endpoints implemented)
- ✅ Complete Absences CRUD (all endpoints implemented)
- ✅ Complete Publications CRUD (all endpoints implemented)
- ✅ Complete Recognitions CRUD (all endpoints implemented)
- ✅ Complete SelfEducation CRUD (all endpoints implemented)
- ✅ Complete Educational Activities CRUD (with type filtering)
- ✅ Dashboard with progress tracking (overview, progress, statistics)
- ✅ Progress calculation service (35% internships, 25% courses, 30% procedures, 10% shifts)
- ✅ Module switching functionality
- ✅ Calculation services (internship days, time normalization, required shift hours)
- ✅ User profile management (profile, password, preferences)

### Needs Implementation (AFTER QUALITY REFACTORING)
- ✅ File upload/download for certificates
- ✅ Educational activities endpoints
- ❌ Export/reporting functionality
- ❌ Sync status management endpoints
- ❌ Search and filtering endpoints
- ❌ Notifications/reminders
- ❌ Batch operations
- ❌ Year transition management

## Architecture Patterns from MySpot to Apply

### Must Apply Immediately
- ⚠️ Result pattern for all command handlers
- ⚠️ Proper value object implementation with validation
- ⚠️ Consistent error handling with custom exceptions
- ⚠️ Specification pattern for complex queries
- ❌ Unit tests for all handlers
- ❌ Integration tests for all endpoints
- ❌ Proper DTO mapping without AutoMapper
- ❌ Request validation with FluentValidation or similar

### Already Applied
- ✅ CQRS pattern with separate commands and queries
- ✅ Value Objects for domain primitives (basic implementation)
- ✅ Repository pattern
- ✅ Clean architecture layers

### Nice to Have
- ❌ Decorator pattern for cross-cutting concerns
- ❌ Assembly scanning for auto-registration
- ❌ Domain events for decoupling

## Quality Checklist (MUST COMPLETE BEFORE NEW FEATURES)
- [ ] All command handlers use Result pattern
- [ ] All queries return DTOs, not entities
- [ ] Value objects have proper validation
- [ ] Custom exceptions with meaningful messages
- [ ] Integration tests for all endpoints
- [ ] Unit tests for all business logic
- [ ] Proper separation of concerns
- [ ] No business logic in controllers
- [ ] No primitive obsession
- [ ] SOLID principles applied throughout

## Testing Requirements
- [ ] Integration tests cover all HTTP status codes
- [ ] Authorization tests for data isolation
- [ ] Validation tests with edge cases
- [ ] Transaction rollback tests
- [ ] Performance tests for N+1 queries
- [ ] Error handling tests
- [ ] Concurrent access tests

## Migration Principles
1. **Quality First**: No new features until existing code meets MySpot standards
2. **Test Everything**: Every refactoring must maintain or improve test coverage
3. **Incremental Changes**: Small, testable changes with API verification
4. **Pattern Consistency**: Apply patterns uniformly across the codebase
5. **Documentation**: Update CLAUDE.md with each architectural decision

## Notes
- Government specialization JSON files are immutable
- API must maintain backward compatibility during migration
- Focus on code quality metrics: maintainability, testability, readability
- Use MySpot as the gold standard for implementation patterns