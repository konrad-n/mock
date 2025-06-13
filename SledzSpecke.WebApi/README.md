# SledzSpecke Web API

A clean architecture .NET 9 Web API based on the SledzSpecke MAUI application, following SOLID principles and design patterns from the MySpot reference project.

## Architecture

The solution follows Clean Architecture principles with the following layers:

### Core Layer (`SledzSpecke.Core`)
- **Entities**: Domain entities like `User`, `Specialization`, `Module`, `MedicalShift`, `Procedure`
- **Value Objects**: Strongly-typed primitives like `UserId`, `Email`, `Username`, `Password`, etc.
- **Repositories**: Repository interfaces
- **Exceptions**: Domain-specific exceptions
- **Abstractions**: Core abstractions like `IClock`

### Application Layer (`SledzSpecke.Application`)
- **Commands & Queries**: CQRS pattern implementation
- **Handlers**: Command and query handlers
- **DTOs**: Data transfer objects
- **Security**: Authentication and authorization abstractions
- **Exceptions**: Application-specific exceptions

### Infrastructure Layer (`SledzSpecke.Infrastructure`)
- **Data Access**: Entity Framework Core implementation
- **Authentication**: JWT token generation and validation
- **Security**: Password hashing and verification
- **Time**: System clock implementation
- **Repositories**: Repository implementations

### API Layer (`SledzSpecke.Api`)
- **Controllers**: REST API endpoints
- **Configuration**: Application startup and dependency injection

## Key Features

- **Clean Architecture**: Separation of concerns with clear dependency rules
- **CQRS Pattern**: Command Query Responsibility Segregation
- **Repository Pattern**: Data access abstraction
- **Value Objects**: Domain-driven design with strongly-typed primitives
- **JWT Authentication**: Secure token-based authentication
- **Entity Framework Core**: Modern ORM with SQL Server support
- **Swagger/OpenAPI**: API documentation
- **Serilog**: Structured logging

## Domain Model

The API manages medical specialization training with:

- **Users**: Medical professionals with specialization assignments
- **Specializations**: Medical training programs (cardiology, psychiatry, etc.)
- **Modules**: Training modules within specializations
- **Medical Shifts**: Recorded work shifts with hours and locations
- **Procedures**: Medical procedures performed during training

## Getting Started

1. Update the connection string in `appsettings.json`
2. Run Entity Framework migrations to create the database
3. Start the application: `dotnet run --project src/SledzSpecke.Api`
4. Visit `https://localhost:5001/swagger` for API documentation

## Authentication

The API uses JWT bearer tokens for authentication. Sign up for a new account or sign in with existing credentials to receive a token, then include it in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## Design Patterns Used

- **Clean Architecture**: Dependency inversion and separation of concerns
- **CQRS**: Separate command and query handlers
- **Repository Pattern**: Data access abstraction
- **Factory Pattern**: Value object creation
- **Decorator Pattern**: Cross-cutting concerns (logging, transactions)
- **Strategy Pattern**: Different policies for business rules

This implementation demonstrates enterprise-grade .NET development practices following the excellent architectural patterns established in the MySpot reference project.