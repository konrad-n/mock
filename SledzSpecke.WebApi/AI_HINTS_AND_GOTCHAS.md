# AI Hints and Gotchas for SledzSpecke.WebApi

This file documents tricky bugs and solutions encountered during development.

## 1. Database Issues

### PostgreSQL vs SQLite Confusion
- **Problem**: Initial test scripts assumed SQLite database
- **Reality**: Project uses PostgreSQL
- **Solution**: Check `appsettings.json` for connection string
- **Location**: `test_api.py` was updated to reflect this

### Specialization Seeding
- **Problem**: "Specialization with ID 1 not found" errors
- **Issue**: User entity references SpecializationId(1) but specializations aren't seeded
- **Files**: `DataSeeder.cs`, migration files
- **Solution**: Ensure specializations are seeded before users

## 2. Entity Framework Gotchas

### Include(s => s.Modules) Error
- **Problem**: "The expression 's.Modules' is invalid inside an 'Include' operation"
- **Cause**: `Modules` property is explicitly ignored in `SpecializationConfiguration.cs`
- **File**: `src/SledzSpecke.Infrastructure/DAL/Configurations/SpecializationConfiguration.cs:38`
- **Solution**: Remove all `.Include(s => s.Modules)` statements

## 3. Authentication Issues

### Login Field Names
- **Problem**: Login fails with "The Username field is required"
- **Cause**: API expects `username`, not `email` in login request
- **File**: `src/SledzSpecke.Application/Commands/SignIn.cs`
- **Solution**: Use `{"username": "testuser", "password": "Test123!"}`

### Password Hashing
- **Problem**: "Invalid credentials" even with correct password
- **Method**: SHA256 hashing, NOT bcrypt
- **Correct hash for "Test123!"**: `VN5/YG8lI8uo76wXP6tC+39Z1Wzv+XTI/bc0LPLP40U=`
- **Update DB**: `UPDATE "Users" SET "Password" = 'VN5/YG8lI8uo76wXP6tC+39Z1Wzv+XTI/bc0LPLP40U=' WHERE "Username" = 'testuser';`

## 4. Business Logic Changes

### Sync Status Management
- **Original MAUI**: Synced items were read-only
- **New Behavior**: Synced items CAN be modified
- **Auto-transition**: Synced → Modified when edited
- **Files affected**:
  - `MedicalShift.cs`
  - `ProcedureBase.cs`
  - `Internship.cs`
- **Key point**: Only APPROVED items are truly read-only

### Medical Shift Duration Validation
- **MAUI behavior**: Allows hours > 24 and minutes > 59
- **Example**: 8 hours 90 minutes is VALID
- **Normalization**: Only happens at display level via `TimeNormalizationHelper`
- **Don't add**: Validation like `if (minutes > 59)` - this is intentional!

### Year vs Calendar Year
- **Confusion**: `Year` field is NOT calendar year (2024/2025)
- **Reality**: `Year` is medical education year (1-6 for Old SMK)
- **Special case**: Year 0 means "unassigned"
- **File**: `AddProcedureHandler.cs` has validation logic

## 5. Missing Endpoints

### Commented Out Endpoints
Some endpoints are commented out due to missing implementations:
- `GET /api/internships/{id}` - GetInternshipById query not implemented
- `DELETE /api/internships/{id}` - DeleteInternship command not implemented

## 6. Common Compilation Errors

### Missing Logger
- **Error**: "The type or namespace name 'ILogger' could not be found"
- **Solution**: Add `<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />`
- **File**: Add to `.csproj` file in Application project

## 7. Testing Tips

### API Won't Start
1. Kill existing process: `pkill -f "dotnet.*SledzSpecke.Api"`
2. Check port: `lsof -i :5000`
3. Rebuild: `dotnet build`
4. Start: `dotnet run --project src/SledzSpecke.Api`

### Database Reset
```bash
sudo -u postgres psql -c "DROP DATABASE IF EXISTS sledzspecke_db; CREATE DATABASE sledzspecke_db;"
dotnet ef database update --project src/SledzSpecke.Infrastructure --startup-project src/SledzSpecke.Api
```

## 8. Project Structure Notes

- **Clean Architecture**: Core → Application → Infrastructure → Api
- **CQRS Pattern**: Commands and Queries separated
- **DDD**: Value Objects, Entities, Aggregates
- **Repository Pattern**: Interfaces in Core, implementations in Infrastructure