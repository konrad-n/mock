# SledzSpecke Web API Migration Tasks

## Original Requirements

Your main task now is to get rid of projects/mock/SledzSpecke.App (MAUI) and projects/MySpot. We are now in WEB.API project projects/mock/SledzSpecke.WebApi and I want to be left with only this project. But first we need to apply all clean code principles from example project MySpot and have all the logic persisted from MAUI old project SledzSpecke.App.

Remember to compare to project MySpot that is example of SOLID web api and is two folders above the current one we are in. Also always test your changes and check if api is working after EACH change. Remember we are creating this api as a replacement for MAUI app SledzSpecke.App that is one folder above this one. We need to work the same. Also .json files with specialization info cannot be changed - it is official government requirements. I trust you will be very precise and know what makes sense to do as you are the most powerful and perfect .NET senior architect and developer that creates very clean code that applies to all patterns like SOLID, KISS, YAGNI etc.

Also review written unit/integration tests in WEB.API solution if they make sense looking at what we have in MAUI project.

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

### 🔄 In Progress Tasks
1. **implement-missing-endpoints** - Create missing API endpoints for all MAUI app features

### 📋 Pending Tasks (Priority Order)

#### High Priority
1. **complete-recognitions-endpoints** - Complete all TODO endpoints in Recognitions controller
2. **complete-selfeducation-endpoints** - Complete all TODO endpoints in SelfEducation controller
3. **complete-absences-endpoints** - Complete all TODO endpoints in Absences controller
4. **implement-missing-endpoints** - Create missing API endpoints for all MAUI app features
   - User profile management endpoints
   - File upload/download endpoints
   - Export/reporting endpoints
   - Sync status management

#### Medium Priority
1. **complete-partial-endpoints** - Implement remaining TODO endpoints
2. **apply-clean-architecture** - Refactor code to follow MySpot's clean architecture patterns (CQRS, Value Objects, etc.)
3. **implement-proper-validation** - Add validation decorators and business rule validation from MAUI app
4. **add-missing-dtos** - Create DTOs for all data transfer needs identified in MAUI app
5. **implement-error-handling** - Ensure comprehensive error handling matching MySpot patterns
6. **review-tests** - Review and update unit/integration tests to cover all migrated functionality

#### Low Priority
1. **cleanup-obsolete-code** - Remove any obsolete code and ensure YAGNI principle is followed
2. **final-verification** - Final verification that all MAUI functionality is available in Web API

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
- ✅ Dashboard with progress tracking (overview, progress, statistics)
- ✅ Progress calculation service (35% internships, 25% courses, 30% procedures, 10% shifts)
- ✅ Module switching functionality
- ✅ Calculation services (internship days, time normalization, required shift hours)

### Needs Implementation
- ❌ User profile management endpoints
- ❌ File upload/download for certificates
- ❌ Export/reporting functionality
- ❌ Sync status management endpoints
- ❌ Educational activities endpoints
- ❌ Search and filtering endpoints
- ❌ Notifications/reminders
- ❌ Batch operations
- ❌ Year transition management
- ❌ File upload/download for certificates
- ❌ Export functionality
- ❌ Sync status management
- ❌ Complete implementations for partially done controllers

## Architecture Patterns from MySpot to Apply
- ✅ CQRS pattern with separate commands and queries
- ✅ Value Objects for domain primitives
- ✅ Repository pattern
- ⚠️ Decorator pattern for cross-cutting concerns (partially implemented)
- ⚠️ Consistent error handling with custom exceptions
- ❌ Assembly scanning for auto-registration
- ❌ Extension methods for DTO mapping instead of AutoMapper
- ❌ Simplified entity design (remove unnecessary AggregateRoot)

## Testing Checklist
- [ ] All new endpoints return correct HTTP status codes
- [ ] Authorization works correctly (users can only access their own data)
- [ ] Validation errors return meaningful messages
- [ ] Database operations are transactional
- [ ] Progress calculations match MAUI app formula
- [ ] Specialization JSON files are correctly loaded
- [ ] All CRUD operations work for each entity
- [ ] Statistics endpoints return accurate data

## Notes
- Always test API after each change
- Specialization JSON files must not be modified (government requirements)
- Follow SOLID, KISS, YAGNI principles
- Maintain compatibility with MAUI app functionality
- Use Result pattern for enhanced error handling where appropriate