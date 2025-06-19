# SledzSpecke - Medical Specialization Tracking System

SledzSpecke is a comprehensive web application for tracking medical specialization progress in Poland, compliant with the SMK (System Monitorowania Kształcenia) requirements.

## 🏥 Overview

SledzSpecke helps medical residents track their:
- Medical shifts (dyżury medyczne)
- Procedures performed
- Courses and training
- Module progression
- SMK compliance reports

## 🏗️ Architecture

The system follows Clean Architecture principles with CQRS pattern:

```
SledzSpecke.WebApi/
├── src/
│   ├── SledzSpecke.Api/          # REST API Controllers, Minimal APIs
│   ├── SledzSpecke.Application/  # CQRS Handlers, DTOs, Business Logic
│   ├── SledzSpecke.Core/         # Domain Entities, Value Objects, Specifications
│   └── SledzSpecke.Infrastructure/ # EF Core, Repositories, External Services
└── tests/
    ├── SledzSpecke.Core.Tests/
    ├── SledzSpecke.Application.Tests/
    └── SledzSpecke.E2E.Tests/
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 14+
- Node.js 18+ (for E2E tests)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/SledzSpecke.git
   cd SledzSpecke.WebApi
   ```

2. **Set up the database**
   ```bash
   # Update connection string in appsettings.Development.json
   # Run migrations
   dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
   ```

3. **Run the application**
   ```bash
   dotnet run --project src/SledzSpecke.Api
   ```

4. **Access the API**
   - Swagger UI: https://localhost:5001/swagger
   - Health check: https://localhost:5001/health

## 🧪 Testing

### Unit Tests
```bash
dotnet test tests/SledzSpecke.Core.Tests
dotnet test tests/SledzSpecke.Application.Tests
```

### Integration Tests
```bash
dotnet test tests/SledzSpecke.Tests.Integration
```

### E2E Tests
```bash
cd tests/SledzSpecke.E2E.Tests
npm install
dotnet test
```

## 📡 API Endpoints

### Authentication
- `POST /api/auth/sign-up` - User registration
- `POST /api/auth/sign-in` - User login
- `POST /api/auth/refresh` - Refresh JWT token

### Medical Shifts
- `GET /api/MedicalShifts` - List user's medical shifts
- `POST /api/MedicalShifts` - Add new medical shift
- `PUT /api/MedicalShifts/{id}` - Update medical shift
- `DELETE /api/MedicalShifts/{id}` - Delete medical shift

### Procedures
- `GET /api/Procedures` - List user's procedures
- `POST /api/Procedures` - Add new procedure
- `PUT /api/Procedures/{id}` - Update procedure
- `DELETE /api/Procedures/{id}` - Delete procedure

### Modules & Internships
- `GET /api/modules` - List specialization modules
- `GET /api/internships` - List internships
- `POST /api/modules/{id}/complete` - Mark module as complete

## ⚠️ Known Issues & Validation

### PWZ Number Validation
The system validates PWZ (medical license) numbers using the algorithm:
- 7 digits total
- First digit cannot be 0
- Last digit = (sum of first 6 digits × position) % 11

Valid examples: `1000001`, `2000023`, `3000003`

### PESEL Validation
Polish national ID validation includes:
- 11 digits
- Valid checksum
- Date of birth must match PESEL

Valid example: `90010110019` (Male, born 1990-01-01)

### Password Requirements
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

Note: Current system uses SHA256 hashing (legacy), migration to BCrypt is planned.

### JWT Configuration
For production deployment, ensure JWT signing key is properly configured in appsettings.Production.json:
```json
{
  "Auth": {
    "JwtKey": "your-secure-jwt-signing-key-here"
  }
}
```

## 🌐 Production URLs

- API: https://api.sledzspecke.pl
- Web App: https://sledzspecke.pl
- Monitoring: https://api.sledzspecke.pl/monitoring/dashboard (dev only)

## 📋 SMK Compliance

The system is designed to be compliant with Polish SMK requirements:
- Tracks all required medical activities
- Generates monthly reports
- Supports both "old" and "new" SMK versions
- Excel export for SMK Chrome extension compatibility

## 🛠️ Technology Stack

- **Backend**: .NET 9, C# 13
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Testing**: xUnit, Playwright
- **Documentation**: Swagger/OpenAPI
- **Patterns**: Clean Architecture, CQRS, Domain-Driven Design

## 🔒 Security Considerations

1. All passwords are hashed (currently SHA256, BCrypt migration planned)
2. JWT tokens expire after 1 hour
3. Input validation on all endpoints
4. SQL injection prevention via Entity Framework Core
5. CORS configured for production domains only

## 📝 Development Guidelines

1. **Always** use Value Objects instead of primitive types
2. **Always** validate in domain constructors
3. **Always** use Result pattern for error handling
4. **Never** throw exceptions in handlers
5. **Follow** SOLID principles and Clean Architecture

## 🐛 Troubleshooting

### Registration fails with validation errors
- Ensure PESEL checksum is valid
- Ensure PWZ checksum is valid (use sequential numbers like 1000001)
- Password must meet complexity requirements
- All address fields are required

### JWT token errors
- Check if JWT signing key is configured
- Ensure token hasn't expired
- Verify Authorization header format: `Bearer {token}`

### Empty API responses
- Check authentication is working
- Verify user has associated specialization data
- Check database migrations are up to date

## 📚 Additional Documentation

- [API Documentation](docs/api/API_DOCUMENTATION.md)
- [Architecture Guide](docs/architecture/COMPLETE_ARCHITECTURE.md)
- [Testing Strategy](docs/architecture/TESTING_STRATEGY.md)
- [Deployment Guide](docs/deployment/DEPLOYMENT-OPTIONS.md)

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is proprietary software. All rights reserved.

## 👥 Contact

For questions or support, please contact the development team.