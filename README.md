# SledzSpecke - Medical Specialization Tracking System

![Build Status](https://github.com/konrad-n/mock/actions/workflows/sledzspecke-cicd.yml/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=konrad-n_mock&metric=alert_status)](https://sonarcloud.io/dashboard?id=konrad-n_mock)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=konrad-n_mock&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=konrad-n_mock)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=konrad-n_mock&metric=security_rating)](https://sonarcloud.io/dashboard?id=konrad-n_mock)

## Project Description

SledzSpecke is a comprehensive web application that allows resident physicians in Poland to track their progress in medical specialization programs. The application supports both the old and new SMK (System Monitorowania Kształcenia) formats, offering an intuitive interface to manage all aspects of medical specialty training.

## Architecture

- **Backend**: .NET 9 Web API with Clean Architecture
- **Frontend**: React 18 with TypeScript and Material-UI
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **API Documentation**: Swagger/OpenAPI

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/) and npm
- [PostgreSQL 14+](https://www.postgresql.org/download/)
- Git

## Getting Started

### Quick Start (Recommended)

For a quick start, use the provided scripts:

**Linux/macOS:**
```bash
./start.sh
```

**Windows:**
```batch
start.bat
```

These scripts will automatically start both the backend and frontend with all necessary checks.

### Manual Setup

If you prefer manual setup or the quick start scripts don't work:

#### 1. Clone the Repository

```bash
git clone https://github.com/konrad-n/mock.git
cd mock
```

#### 2. Database Setup

Create a PostgreSQL database and user:

```sql
CREATE USER sledzspecke_user WITH PASSWORD 'SledzSpecke123!';
CREATE DATABASE sledzspecke_db OWNER sledzspecke_user;
GRANT ALL PRIVILEGES ON DATABASE sledzspecke_db TO sledzspecke_user;
```

#### 3. Backend Setup

Navigate to the backend directory:
```bash
cd SledzSpecke.WebApi
```

Update the connection string in `src/SledzSpecke.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=sledzspecke_db;Username=sledzspecke_user;Password=SledzSpecke123!"
  }
}
```

Run database migrations:
```bash
dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

Start the API:
```bash
dotnet run --project src/SledzSpecke.Api
```

The API will be available at `http://localhost:5000` with Swagger documentation at `http://localhost:5000/swagger`.

#### 4. Frontend Setup

In a new terminal, navigate to the frontend directory:
```bash
cd SledzSpecke-Frontend/packages/web
```

Install dependencies:
```bash
npm install
```

Start the development server:
```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`.

## Test Credentials

Use these credentials to log in:
- **Username**: `testuser`
- **Password**: `Test123!`

## Project Structure

```
mock/
├── SledzSpecke.WebApi/          # .NET Backend
│   ├── src/
│   │   ├── SledzSpecke.Api/     # Web API controllers and configuration
│   │   ├── SledzSpecke.Application/  # Business logic and CQRS handlers
│   │   ├── SledzSpecke.Core/    # Domain entities and value objects
│   │   └── SledzSpecke.Infrastructure/  # Data access and external services
│   └── tests/                   # Unit and integration tests
│
├── SledzSpecke-Frontend/        # React Frontend (Monorepo)
│   └── packages/
│       ├── shared/              # Shared types and utilities
│       └── web/                 # React web application
│
└── CLAUDE.md                    # Detailed technical documentation
```

## Key Features

- **Medical Shifts Management** - Track duty hours and on-call shifts
- **Procedures Tracking** - Record medical procedures as operator or assistant
- **Internships Management** - Monitor progress in clinical rotations
- **Courses & Training** - Document mandatory and additional courses
- **Self-Education** - Track conferences, workshops, and self-study
- **Publications** - Record scientific papers and research
- **Export Functionality** - Generate reports for SMK system
- **Multi-Module Support** - Basic and specialist modules
- **Approval Workflow** - Supervisor approval for activities

## API Endpoints

Main endpoint groups:
- `/api/auth` - Authentication (sign-in, sign-up)
- `/api/medical-shifts` - Medical shifts CRUD operations
- `/api/procedures` - Medical procedures management
- `/api/internships` - Clinical rotations tracking
- `/api/courses` - Courses and training records
- `/api/self-education` - Self-education activities
- `/api/publications` - Scientific publications
- `/api/export` - Data export (PDF, Excel, JSON)

## Development

### Running Tests

Backend tests:
```bash
cd SledzSpecke.WebApi
dotnet test
```

Frontend tests:
```bash
cd SledzSpecke-Frontend/packages/web
npm test
```

### Code Quality

Backend linting and analysis:
```bash
dotnet format
dotnet build /p:TreatWarningsAsErrors=true
```

Frontend linting:
```bash
npm run lint
npm run type-check
```

### Building for Production

Backend:
```bash
cd SledzSpecke.WebApi
dotnet publish -c Release -o ./publish
```

Frontend:
```bash
cd SledzSpecke-Frontend/packages/web
npm run build
```

## Troubleshooting

### Common Issues

1. **Database connection failed**
   - Ensure PostgreSQL is running
   - Verify connection string in appsettings.json
   - Check firewall settings

2. **Port already in use**
   - API: Change port in launchSettings.json
   - Frontend: Use `npm run dev -- --port 3000`

3. **Authentication errors**
   - Ensure JWT secret key is configured
   - Check token expiration settings
   - Verify CORS configuration

### Detailed Documentation

For comprehensive technical documentation, including:
- Architecture decisions
- Business rules
- API specifications
- Deployment guides

See [CLAUDE.md](./CLAUDE.md) and [FRONTEND-INSTRUCTION.md](./FRONTEND-INSTRUCTION.md).

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

© 2025 Konrad N. All rights reserved.

## Support

For bug reports and feature requests, please use the [GitHub Issues](https://github.com/konrad-n/mock/issues) page.

For development questions, refer to the technical documentation or contact the development team.