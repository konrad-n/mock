# SledzSpecke API Documentation

## Overview
SledzSpecke API provides comprehensive endpoints for managing medical specialization tracking in compliance with Polish SMK (System Monitorowania Kształcenia) requirements.

## Production URLs
- **Main API**: https://api.sledzspecke.pl
- **Frontend**: https://sledzspecke.pl
- **VPS IP**: 51.77.59.184

## Base URLs
- **Production**: `https://api.sledzspecke.pl`
- **Development**: `http://localhost:5000`

## Authentication
All endpoints require JWT Bearer token authentication except for:
- `POST /api/auth/sign-up`
- `POST /api/auth/sign-in`

### Getting a Token
```bash
curl -X POST https://api.sledzspecke.pl/api/auth/sign-in \
  -H "Content-Type: application/json" \
  -d '{"Username": "user@example.com", "Password": "YourPassword"}'
```

**Important Notes:**
- The sign-in endpoint uses `Username` field (not `email`) which accepts email addresses
- Password field is case-sensitive (`Password` not `password`)
- Passwords are currently stored as SHA256 hashes in Base64 format
- JWT tokens may have configuration issues in production - check signature key

### Using the Token
```bash
curl -X GET https://api.sledzspecke.pl/api/modules/1 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Key Features
- **SMK Compliance**: Full compatibility with both old (2014-2018) and new (2023+) SMK versions
- **Module-Based Structure**: Support for Basic and Specialized modules
- **Comprehensive Tracking**: Medical shifts, procedures, courses, internships, and self-education
- **Data Export**: XLSX export for SMK import
- **Polish Medical System Integration**: PESEL, PWZ validation

## API Documentation & Monitoring

### Swagger UI
- **URL**: https://api.sledzspecke.pl/swagger/index.html
- **Status**: ✅ Available in Production
- **Description**: Interactive API documentation

### Monitoring Dashboard
- **URL**: https://api.sledzspecke.pl/monitoring/dashboard
- **Status**: ✅ Available in Production (temporarily for testing)
- **Description**: Real-time API metrics and health monitoring

### Health Check
- **URL**: https://api.sledzspecke.pl/monitoring/health
- **Status**: ✅ Available
- **Response**: JSON with health status

### Logs Endpoints
- **Recent Logs**: https://api.sledzspecke.pl/api/Logs/recent
- **Errors Only**: https://api.sledzspecke.pl/api/Logs/errors
- **Stats**: https://api.sledzspecke.pl/api/Logs/stats
- **Status**: ✅ Available in Production (temporarily for testing)

## Core API Endpoints

### Authentication ✅
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/sign-up` | Register new user |
| POST | `/api/auth/sign-in` | Login user |
| ~~POST~~ | ~~/api/auth/refresh~~ | ~~Refresh JWT token~~ (Not implemented) |
| ~~POST~~ | ~~/api/auth/change-password~~ | ~~Change user password~~ (Use `/api/users/change-password`)

### User Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/profile` | Get current user profile |
| PUT | `/api/users/profile` | Update user profile |
| PUT | `/api/users/change-password` | Change password |
| PUT | `/api/users/preferences` | Update user preferences |
| GET | `/api/users` | List users (UsersController - details TBD) |
| GET | `/api/users/{userId}` | Get specific user (UsersController - details TBD) |

### Specializations
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/specializations` | List all specializations |
| GET | `/api/specializations/{id}` | Get specialization details |
| POST | `/api/specializations` | Create new specialization |
| GET | `/api/specializations/{id}/statistics` | Get specialization statistics |
| GET | `/api/specializations/{id}/export` | Export specialization data (XLSX) |

### Specialization Templates
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/specialization-templates` | List all templates |
| GET | `/api/specialization-templates/{specializationCode}/{smkVersion}` | Get specific template |
| GET | `/api/specialization-templates/{specializationCode}/{smkVersion}/modules/{moduleId}` | Get module |
| GET | `/api/specialization-templates/{specializationCode}/{smkVersion}/courses/{courseId}` | Get course |
| GET | `/api/specialization-templates/{specializationCode}/{smkVersion}/internships/{internshipId}` | Get internship |
| GET | `/api/specialization-templates/{specializationCode}/{smkVersion}/procedures/{procedureId}` | Get procedure |
| POST | `/api/specialization-templates/{specializationCode}/{smkVersion}/procedures/{procedureId}/validate` | Validate procedure |

### Modules
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/modules/specialization/{specializationId}` | Get modules for specialization |
| GET | `/api/modules/{moduleId}` | Get module details |
| GET | `/api/modules/{moduleId}/progress` | Get module progress |
| PUT | `/api/modules/switch` | Switch active module |
| POST | `/api/modules/switch` | Switch module (alternative) |
| POST | `/api/modules/{moduleId}/complete` | Mark module as complete |
| POST | `/api/modules/{moduleId}/internships` | Create internship in module |
| POST | `/api/modules/{moduleId}/medical-shifts` | Add medical shift |
| POST | `/api/modules/{moduleId}/procedures` | Add procedure |
| POST | `/api/modules/{moduleId}/courses` | Create course |
| POST | `/api/modules/{moduleId}/self-education` | Add self-education activity |
| POST | `/api/modules/{moduleId}/additional-days` | Add additional self-education days |

### Internships
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/internships` | List internships (with filters: specializationId, moduleId) |
| GET | `/api/internships/{internshipId}` | Get specific internship |
| POST | `/api/internships` | Create new internship |
| PUT | `/api/internships/{internshipId}` | Update internship |
| DELETE | `/api/internships/{internshipId}` | Delete internship |
| POST | `/api/internships/{internshipId}/approve` | Approve internship |
| POST | `/api/internships/{internshipId}/complete` | Complete internship |

### Medical Shifts
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/medical-shifts` | List user's shifts (with filters: specializationId, moduleId, internshipId, startDate, endDate) |
| GET | `/api/medical-shifts/{shiftId}` | Get specific shift |
| POST | `/api/medical-shifts` | Add new shift |
| PUT | `/api/medical-shifts/{shiftId}` | Update shift |
| DELETE | `/api/medical-shifts/{shiftId}` | Delete shift |
| GET | `/api/medical-shifts/statistics` | Get shift statistics |

### Procedures
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/procedures` | List user's procedures (with filters: specializationId, moduleId, internshipId, startDate, endDate) |
| GET | `/api/procedures/{procedureId}` | Get specific procedure |
| POST | `/api/procedures` | Add new procedure |
| PUT | `/api/procedures/{procedureId}` | Update procedure |
| DELETE | `/api/procedures/{procedureId}` | Delete procedure |
| GET | `/api/procedures/statistics` | Get procedure statistics |

### Courses
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/courses` | List courses (with filters: specializationId, moduleId, courseType) |
| GET | `/api/courses/{courseId}` | Get specific course |
| POST | `/api/courses` | Create new course |
| PUT | `/api/courses/{courseId}` | Update course |
| DELETE | `/api/courses/{courseId}` | Delete course |
| POST | `/api/courses/{courseId}/complete` | Mark course as completed |
| POST | `/api/courses/{courseId}/approve` | Approve course |

### Educational Activities
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/educational-activities` | List all activities |
| GET | `/api/educational-activities/specialization/{specializationId}` | Get by specialization |
| GET | `/api/educational-activities/specialization/{specializationId}/type/{type}` | Get by type |
| GET | `/api/educational-activities/{id}` | Get specific activity |
| POST | `/api/educational-activities` | Create new activity |
| PUT | `/api/educational-activities/{id}` | Update activity |
| DELETE | `/api/educational-activities/{id}` | Delete activity |

### Self Education
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/self-education` | List all self education |
| GET | `/api/self-education/{id}` | Get specific self education |
| POST | `/api/self-education` | Create new self education |
| PUT | `/api/self-education/{id}` | Update self education |
| DELETE | `/api/self-education/{id}` | Delete self education |
| POST | `/api/self-education/{id}/complete` | Complete self education |
| GET | `/api/self-education/user/{userId}` | Get user's self education |
| GET | `/api/self-education/user/{userId}/year/{year}` | Get by year |
| GET | `/api/self-education/user/{userId}/specialization/{specializationId}/completed` | Get completed |
| GET | `/api/self-education/user/{userId}/specialization/{specializationId}/credit-hours` | Get credit hours |
| GET | `/api/self-education/user/{userId}/specialization/{specializationId}/quality-score` | Get quality score |

### Additional Self-Education Days
Manage additional educational days (max 6 per year per SMK regulations)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/additional-self-education-days` | Add additional days |
| GET | `/api/additional-self-education-days/specialization/{id}` | Get days by specialization |
| GET | `/api/additional-self-education-days/module/{moduleId}` | Get days by module |
| GET | `/api/additional-self-education-days/{id}` | Get specific record |
| PUT | `/api/additional-self-education-days/{id}` | Update days |
| DELETE | `/api/additional-self-education-days/{id}` | Delete days |

### Absences
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/absences` | List all absences |
| GET | `/api/absences/user/{userId}` | Get user's absences |
| GET | `/api/absences/{id}` | Get specific absence |
| POST | `/api/absences` | Create new absence |
| PUT | `/api/absences/{id}` | Update absence |
| DELETE | `/api/absences/{id}` | Delete absence |
| POST | `/api/absences/{id}/approve` | Approve absence |

### Dashboard
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/dashboard/overview` | Get dashboard overview |
| GET | `/api/dashboard/progress/{specializationId}` | Get progress by specialization |
| GET | `/api/dashboard/statistics/{specializationId}` | Get statistics |

### Calculations
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/calculations/internship-days` | Calculate internship days |
| POST | `/api/calculations/normalize-time` | Normalize time format |
| POST | `/api/calculations/required-shift-hours` | Calculate required hours |

### Publications
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/publications` | List all publications |
| GET | `/api/publications/{id}` | Get specific publication |
| POST | `/api/publications` | Create new publication |
| PUT | `/api/publications/{id}` | Update publication |
| DELETE | `/api/publications/{id}` | Delete publication |
| GET | `/api/publications/user/{userId}` | Get user's publications |
| GET | `/api/publications/user/{userId}/first-author` | Get first-author publications |
| GET | `/api/publications/user/{userId}/peer-reviewed` | Get peer-reviewed publications |
| GET | `/api/publications/user/{userId}/specialization/{specializationId}/impact-score` | Get impact score |

### Recognitions
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/recognitions` | List all recognitions |
| GET | `/api/recognitions/{id}` | Get specific recognition |
| POST | `/api/recognitions` | Create new recognition |
| PUT | `/api/recognitions/{id}` | Update recognition |
| DELETE | `/api/recognitions/{id}` | Delete recognition |
| POST | `/api/recognitions/{id}/approve` | Approve recognition |
| GET | `/api/recognitions/user/{userId}` | Get user's recognitions |
| GET | `/api/recognitions/user/{userId}/specialization/{specializationId}/total-reduction` | Get total reduction |

### File Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/files/upload` | Upload file |
| GET | `/api/files/{fileId}` | Get file info |
| GET | `/api/files/{fileId}/download` | Download file |
| DELETE | `/api/files/{fileId}` | Delete file |
| GET | `/api/files/entity/{entityType}/{entityId}` | Get files by entity |

## Request/Response Examples

### Add Additional Self-Education Days
```json
POST /api/additional-self-education-days
{
  "specializationId": 1,
  "year": 2024,
  "daysUsed": 3,
  "comment": "Annual Cardiology Conference"
}

Response: 201 Created
{
  "id": 123
}
```

### Create Module Internship
```json
POST /api/modules/1/internships
{
  "specializationId": 1,
  "name": "Cardiology Ward Internship",
  "institutionName": "City Hospital",
  "departmentName": "Cardiology",
  "startDate": "2024-01-15",
  "endDate": "2024-04-15",
  "plannedWeeks": 12,
  "plannedDays": 60
}

Response: 200 OK
{
  "internshipId": 456,
  "message": "Internship created successfully"
}
```

### Add Medical Shift
```json
POST /api/modules/1/medical-shifts
{
  "internshipId": 123,
  "date": "2024-01-15",
  "hours": 8,
  "minutes": 30,
  "location": "Emergency Department"
}

Response: 200 OK
{
  "shiftId": 789,
  "message": "Medical shift added successfully"
}
```

### Add Procedure
```json
POST /api/modules/1/procedures
{
  "internshipId": 123,
  "date": "2024-01-15",
  "code": "89.52",
  "name": "Electrocardiogram",
  "location": "Cardiology Department",
  "executionType": "CodeA",
  "supervisorName": "Dr. Jan Kowalski"
}

Response: 200 OK
{
  "procedureId": 321,
  "message": "Procedure added successfully"
}
```

## Error Responses
All errors follow RFC 7807 Problem Details format:

```json
{
  "type": "https://sledzspecke.pl/errors/validation",
  "title": "Validation Error",
  "status": 400,
  "detail": "Cannot exceed 6 additional self-education days per year",
  "instance": "/api/additional-self-education-days"
}
```

### Common Error Codes
- `400 Bad Request` - Invalid input data
- `401 Unauthorized` - Missing or invalid authentication
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Conflict with existing data
- `422 Unprocessable Entity` - Business rule violation
- `500 Internal Server Error` - Server error

## Validation Rules

### Polish Medical System
- **PESEL**: 11-digit Polish national ID with checksum validation
- **PWZ**: 7-digit medical license number, cannot start with 0
- **Medical Year**: Must be between 1-6
- **Shift Duration**: Minutes can exceed 59 (e.g., 90 minutes = 1h 30m)

### SMK Rules
- **Additional Self-Education Days**: Maximum 6 days per year
- **Module Types**: basic, specialistic, complementary
- **SMK Versions**: "old" or "new"
- **Procedure Execution Types**: CodeA or CodeB

### Date Validation
- Medical shifts and procedures cannot be in the future
- Internship dates must be within module dates
- Course dates must be logical (end after start)

## Data Export

### Export Controller
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/export/specialization/{id}/xlsx` | Export specialization to Excel (SMK format) |
| GET | `/api/export/specialization/{id}/preview` | Preview export data before download |
| POST | `/api/export/specialization/{id}/validate` | Validate data for SMK compliance |

### Test Export Controller
| Method | Endpoint | Description |
|--------|----------|-------------|
| Various | `/api/test-export/*` | Test export endpoints (development) |

### SMK Export Format
The XLSX export includes sheets:
1. Summary - Overview statistics
2. Medical Shifts - All shifts with hours
3. Procedures - All procedures with counts
4. Courses - Completed courses
5. Internships - All internships

## CMKP Specialization Templates

### Overview
The CMKP template system manages all 77 medical specializations defined by Centrum Medyczne Kształcenia Podyplomowego.

### Admin Endpoints (Requires AdminOnly Policy)

#### List All Templates
```bash
GET /api/admin/specialization-templates
Authorization: Bearer YOUR_ADMIN_TOKEN

Response: 200 OK
[
  {
    "id": 1,
    "code": "cardiology",
    "name": "Kardiologia",
    "version": "CMKP 2023",
    "isActive": true,
    "createdAt": "2024-12-19T00:00:00Z",
    "modules": [...]
  }
]
```

#### Get Specific Template
```bash
GET /api/admin/specialization-templates/cardiology/CMKP%202023
Authorization: Bearer YOUR_ADMIN_TOKEN

Response: 200 OK
{
  "id": 1,
  "code": "cardiology",
  "name": "Kardiologia",
  "version": "CMKP 2023",
  "totalDuration": {
    "years": 5,
    "months": 0,
    "days": 0
  },
  "modules": [...]
}
```

#### Import Single Template
```bash
POST /api/admin/specialization-templates/import
Authorization: Bearer YOUR_ADMIN_TOKEN
Content-Type: application/json

{
  "code": "anestezjologia",
  "name": "Anestezjologia i intensywna terapia",
  "version": "CMKP 2023",
  "totalDuration": {
    "years": 5,
    "months": 0,
    "days": 0
  },
  "modules": [...]
}

Response: 200 OK
{
  "templateId": 2,
  "message": "Template imported successfully"
}
```

#### Bulk Import Templates
```bash
POST /api/admin/specialization-templates/import-bulk
Authorization: Bearer YOUR_ADMIN_TOKEN
Content-Type: application/json

{
  "directoryPath": "/path/to/templates"
}

Response: 200 OK
{
  "importedCount": 77,
  "errors": [],
  "message": "Successfully imported 77 templates"
}
```

#### Import from CMKP Website
```bash
POST /api/admin/specialization-templates/import-cmkp/new
Authorization: Bearer YOUR_ADMIN_TOKEN

Response: 200 OK
{
  "importedCount": 77,
  "message": "Successfully imported 77 templates from CMKP website"
}
```

#### Update Template
```bash
PUT /api/admin/specialization-templates/cardiology/CMKP%202023
Authorization: Bearer YOUR_ADMIN_TOKEN
Content-Type: application/json

{
  "name": "Kardiologia - Updated",
  "modules": [...]
}

Response: 200 OK
{
  "message": "Template updated successfully"
}
```

#### Delete Template
```bash
DELETE /api/admin/specialization-templates/cardiology/CMKP%202023
Authorization: Bearer YOUR_ADMIN_TOKEN

Response: 200 OK
{
  "message": "Template deactivated successfully"
}
```

### Regular User Endpoints

#### Get Available Templates
```bash
GET /api/specialization-templates
Authorization: Bearer YOUR_TOKEN

Response: 200 OK
[
  {
    "code": "cardiology",
    "name": "Kardiologia",
    "versions": ["CMKP 2014", "CMKP 2023"]
  }
]
```

#### Get Template Details
```bash
GET /api/specialization-templates/cardiology/new
Authorization: Bearer YOUR_TOKEN

Response: 200 OK
{
  "code": "cardiology",
  "name": "Kardiologia",
  "modules": [...],
  "totalDuration": {...}
}
```

## System & Monitoring Endpoints

### Health Check
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | System health check (returns status) |

### Monitoring Dashboard
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/monitoring/health` | Monitoring health check |
| GET | `/monitoring/dashboard` | HTML monitoring dashboard (Development only) |

### Logs API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/logs/recent` | Get recent logs |
| GET | `/api/logs/errors` | Get error logs only |
| GET | `/api/logs/stats` | Get log statistics |

### Performance Metrics
| Method | Endpoint | Description |
|--------|----------|-------------|
| Various | `/api/performance-metrics/*` | Performance monitoring endpoints |

### E2E Test Results
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/e2e/results` | E2E test results (Development only) |

## Known Issues & Validation Requirements

### Registration Validation

#### PESEL (Polish National ID)
- Must be exactly 11 digits
- Must have valid checksum
- Date of birth in request must match PESEL
- Example valid PESEL: `90010110019` (Male, born 1990-01-01)

#### PWZ (Medical License Number)
- Must be exactly 7 digits
- First digit cannot be 0
- Last digit must equal: (sum of first 6 digits × position) % 11
- Valid examples: `1000001`, `1000002`, `1000003`

#### Password Requirements
- Minimum 8 characters
- At least one uppercase letter (A-Z)
- At least one lowercase letter (a-z)
- At least one digit (0-9)
- At least one special character (@$!%*?&)
- Current storage: SHA256 hash in Base64 format

#### Address Validation
- All fields required except ApartmentNumber
- Province must be valid Polish province (lowercase)
- Postal code format: XX-XXX (e.g., `00-001`)
- Province must match postal code prefix

### Common Issues

1. **JWT Token Errors**
   - Error: "The signature key was not found"
   - Solution: Ensure JWT signing key is configured in production

2. **Registration Failures**
   - Most common: Invalid PESEL or PWZ checksum
   - Solution: Use provided valid examples or calculate correct checksums

3. **Password Special Characters**
   - Some special characters may cause JSON parsing errors
   - Use `@` or other simple special characters

4. **Empty API Responses**
   - Many endpoints return empty when no data exists
   - Check if user has associated specialization data

### SMK Integration
| Method | Endpoint | Description |
|--------|----------|-------------|
| Various | `/api/smk/*` | SMK system integration endpoints (SmkController) |

## External Monitoring Services

### Seq (Log Aggregation)
- **URL**: http://51.77.59.184:5341
- **Status**: ✅ Available
- **Description**: Centralized structured logging
- **Access**: Direct IP (public access enabled for testing)
- **Default Password**: No password set (ACCEPT_EULA=Y)

### Grafana (Metrics Dashboard)
- **URL**: http://51.77.59.184:3000
- **Status**: ✅ Available
- **Description**: System and application metrics visualization
- **Default Credentials**: admin / SledzSpecke2024!

### Prometheus (Metrics Collection)
- **URL**: http://51.77.59.184:9090
- **Status**: ✅ Available
- **Description**: Time-series metrics database
- **Targets**: API, Node Exporter, cAdvisor

### Node Exporter (System Metrics)
- **URL**: http://51.77.59.184:9100/metrics
- **Status**: ✅ Available
- **Description**: System-level metrics (CPU, memory, disk, network)

### cAdvisor (Container Metrics)
- **URL**: http://51.77.59.184:8080
- **Status**: ✅ Available
- **Description**: Docker container resource usage and performance

## Testing the APIs

```bash
# Health check
curl https://api.sledzspecke.pl/monitoring/health

# View recent logs
curl https://api.sledzspecke.pl/api/Logs/recent

# View error logs
curl https://api.sledzspecke.pl/api/Logs/errors

# View log statistics
curl https://api.sledzspecke.pl/api/Logs/stats

# Test authentication (example)
curl -X POST https://api.sledzspecke.pl/api/auth/sign-in \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"test123"}'

# Get Swagger documentation
curl https://api.sledzspecke.pl/swagger/v1/swagger.json | jq '.paths | keys'

# Access monitoring services
# Seq: http://51.77.59.184:5341
# Grafana: http://51.77.59.184:3000 (admin/SledzSpecke2024!)
# Prometheus: http://51.77.59.184:9090
# cAdvisor: http://51.77.59.184:8080
```

## Rate Limiting
- 100 requests per minute per IP
- 1000 requests per hour per authenticated user
- Export endpoints: 10 requests per hour

## Versioning
API version is included in the URL: `/api/v1/...`
Current version: v1

## SDKs
- JavaScript/TypeScript: `npm install @sledzspecke/api-client`
- Python: `pip install sledzspecke-client`
- .NET: `dotnet add package SledzSpecke.ApiClient`

## DevOps Components Implemented
1. **Docker & Docker Compose**: Full container orchestration
2. **Monitoring Stack**: Seq, Grafana, Prometheus, Node Exporter, cAdvisor
3. **Security**: UFW firewall, Fail2ban intrusion prevention
4. **SSL/TLS**: Let's Encrypt certificates with auto-renewal
5. **Backup System**: Daily automated backups with 7-day retention
6. **Auto-updates**: Watchtower for container updates

## Backup Management

```bash
# Run manual backup
sudo /usr/local/bin/backup-sledzspecke.sh

# View backup schedule
sudo crontab -l

# Check backup directory
ls -lh /var/backups/sledzspecke/

# Restore database from backup
gunzip -c /var/backups/sledzspecke/db_backup_TIMESTAMP.sql.gz | sudo -u postgres psql sledzspecke_db
```

## Security Notes

⚠️ **IMPORTANT**: The following are temporarily enabled for testing:
- Monitoring dashboard in production
- Detailed error messages
- All log endpoints

These MUST be disabled before customer release by updating:
- `MonitoringDashboardController.cs`
- `LogsController.cs`
- `EnhancedExceptionHandlingMiddleware.cs`

## Access Control

Currently in production:
- ✅ HTTPS enforced for all api.sledzspecke.pl endpoints
- ✅ JWT authentication required for protected endpoints
- ⚠️ Monitoring endpoints temporarily public (for testing)
- ⚠️ Seq and Grafana accessible via direct IP (no authentication)

## Current Service Availability Status

### ✅ Working Services
- **API Service**: Running on port 5000
- **PostgreSQL**: Active and accepting connections
- **Nginx**: Serving HTTPS traffic
- **Swagger UI**: Full API documentation available
- **Monitoring Dashboard**: Real-time metrics
- **Health Endpoint**: Returning healthy status
- **Log Endpoints**: All log APIs accessible
- **Seq**: Running on port 5341
- **Grafana**: Running on port 3000
- **Docker**: Installed and running
- **UFW Firewall**: Active with all necessary rules
- **Fail2ban**: Active with SSH and nginx protection
- **SSL Certificate**: Let's Encrypt configured and auto-renewing
- **Backup System**: Automated daily backups at 2 AM

## Webhooks (Coming Soon)
Subscribe to events:
- Specialization completed
- Module completed
- Internship status changed
- Course completed

## Local Development

When running locally:
- API: http://localhost:5000
- Use `ASPNETCORE_ENVIRONMENT=Development`
- Database: Local PostgreSQL or in-memory

## Support
- Documentation: https://docs.sledzspecke.pl
- Issues: https://github.com/sledzspecke/api/issues
- Email: support@sledzspecke.pl

---

Last Updated: 2025-06-19
Status: Development/Testing Phase

## Summary of Changes (December 2024)

### API Structure Updates
- All endpoints now use **lowercase kebab-case** URLs (e.g., `/api/medical-shifts` instead of `/api/MedicalShifts`)
- **No Minimal API endpoints** - all endpoints use traditional controllers
- **29 controllers** total, all inheriting from BaseController or BaseResultController
- Most endpoints require **JWT authentication**
- Admin endpoints require **AdminOnly** policy

### Key Differences from Original Documentation
1. **Authentication**: No refresh token endpoint implemented (use sign-in to get new tokens)
2. **User Management**: Simplified endpoints through UserProfileController
3. **URL Convention**: All URLs now follow kebab-case convention
4. **Export**: Dedicated ExportController for SMK Excel exports
5. **Monitoring**: Comprehensive monitoring endpoints added
6. **E2E Testing**: Dedicated endpoints for test results

### Missing Features (Not Yet Implemented)
- JWT token refresh endpoint
- Some user management endpoints in UsersController
- Webhook system
- Some module-specific endpoints
- Bulk medical shift import