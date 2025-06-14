# SledzSpecke.WebApi - AI Assistant Guidelines

## Project Overview
SledzSpecke.WebApi is a medical internship tracking API that serves as the backend for the SledzSpecke.App MAUI application. It follows Clean Architecture principles with inspiration from the MySpot project patterns.

## Architecture Patterns

### Clean Architecture Layers
- **Core**: Domain entities, value objects, repository interfaces, exceptions
- **Application**: Command/Query handlers, DTOs, application services
- **Infrastructure**: Data access, external services, authentication
- **Api**: Controllers, middleware, API configuration

### Key Patterns Used
1. **CQRS-lite**: Separate command and query handlers
2. **Value Objects**: Strong typing with validation (Email, UserId, Duration, etc.)
3. **Result Pattern**: Enhanced handlers use Result<T> for error handling
4. **Unit of Work**: Transaction management
5. **Repository Pattern**: Data access abstraction
6. **Assembly Scanning**: Auto-registration of handlers using Scrutor

## Development Commands

### Build and Test
```bash
# Build the solution
dotnet build

# Run the API
dotnet run --project src/SledzSpecke.Api

# Run tests (when available)
dotnet test
```

### Database Commands
```bash
# Add migration
dotnet ef migrations add MigrationName -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api

# Update database
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

### Code Quality
```bash
# Run linter (if configured)
dotnet format

# Check for code issues
dotnet build -warnaserror
```

## Important Conventions

### Value Objects
- Use Value Objects for all domain primitives (Email, SmkVersion, Duration, etc.)
- Include validation in constructors
- Provide implicit conversions for ease of use
- Override ToString() for display

### Command Handlers
- Enhanced handlers use Result pattern
- Include proper validation steps
- Use Unit of Work for transactions
- Follow single responsibility principle

### Entity Rules
- Keep entities simple (no AggregateRoot pattern)
- Business logic belongs in entities
- Use Value Objects for properties
- No anemic domain model

### API Responses
- Use consistent error handling via middleware
- Return proper HTTP status codes
- Use DTOs for responses, not entities

## Special Considerations

### SMK Versions
- Two versions: "old" and "new"
- Different validation rules apply
- Government requirements - cannot be changed

### Medical Shifts
- Allow minutes > 59 (normalization at display level)
- Duration must be > 0
- Location is required

### Year Calculation
- Medical specializations have different year structures
- Use IYearCalculationService for year validation
- Years range from 1-10 maximum

## Testing the API

### Swagger UI
Navigate to: http://localhost:5000/swagger

### Example Requests
```bash
# SignUp
POST /api/auth/sign-up
{
  "email": "test@example.com",
  "username": "testuser",
  "password": "Test123!",
  "fullName": "Test User",
  "smkVersion": "new",
  "specializationId": 1
}

# SignIn
POST /api/auth/sign-in
{
  "email": "test@example.com",
  "password": "Test123!"
}
```

## Common Issues

### Database Initialization
If database fails to initialize, check:
1. PostgreSQL connection string in appsettings.json
2. Migrations are up to date
3. User has proper database permissions

### Value Object Errors
Common validation errors:
- Email: Must be valid format
- SmkVersion: Must be "old" or "new"
- Duration: Hours/minutes cannot be negative
- Year: Must be between 1-10

## Future Improvements
- Add comprehensive integration tests
- Implement caching for frequently accessed data
- Add API versioning
- Enhance logging and monitoring
- Add health checks endpoint