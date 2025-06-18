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
  -d '{"email": "user@example.com", "password": "password"}'
```

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
| POST | `/api/auth/refresh` | Refresh JWT token |
| POST | `/api/auth/change-password` | Change user password |

### User Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/me` | Get current user profile |
| GET | `/api/users/profile` | Get current user profile (alternative) |
| PUT | `/api/users/me` | Update user profile |
| PUT | `/api/users/profile` | Update user profile (alternative) |
| PUT | `/api/users/change-password` | Change password |
| GET | `/api/users/me/preferences` | Get user preferences |
| GET | `/api/users/preferences` | Get user preferences (alternative) |
| PUT | `/api/users/me/preferences` | Update user preferences |
| PUT | `/api/users/preferences` | Update user preferences (alternative) |
| GET | `/api/Users` | List all users |
| GET | `/api/Users/{userId}` | Get specific user |
| PUT | `/api/Users/{userId}` | Update user |
| DELETE | `/api/Users/{userId}` | Delete user |

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
| GET | `/api/Internships` | List all internships |
| GET | `/api/Internships/{internshipId}` | Get specific internship |
| POST | `/api/Internships` | Create new internship |
| PUT | `/api/Internships/{internshipId}` | Update internship |
| DELETE | `/api/Internships/{internshipId}` | Delete internship |
| POST | `/api/Internships/{internshipId}/approve` | Approve internship |
| POST | `/api/Internships/{internshipId}/complete` | Complete internship |

### Medical Shifts
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/MedicalShifts` | List all shifts |
| GET | `/api/MedicalShifts/{shiftId}` | Get specific shift |
| POST | `/api/MedicalShifts` | Add new shift |
| PUT | `/api/MedicalShifts/{shiftId}` | Update shift |
| DELETE | `/api/MedicalShifts/{shiftId}` | Delete shift |
| GET | `/api/MedicalShifts/statistics` | Get shift statistics |
| POST | `/api/medical-shifts/bulk` | Add multiple shifts |

### Procedures
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Procedures` | List all procedures |
| GET | `/api/Procedures/{procedureId}` | Get specific procedure |
| POST | `/api/Procedures` | Add new procedure |
| PUT | `/api/Procedures/{procedureId}` | Update procedure |
| DELETE | `/api/Procedures/{procedureId}` | Delete procedure |
| GET | `/api/Procedures/statistics` | Get procedure statistics |
| GET | `/api/procedures/search` | Search procedures by code/name |

### Courses
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Courses` | List all courses |
| GET | `/api/Courses/{courseId}` | Get specific course |
| POST | `/api/Courses` | Create new course |
| PUT | `/api/Courses/{courseId}` | Update course |
| DELETE | `/api/Courses/{courseId}` | Delete course |
| POST | `/api/Courses/{courseId}/complete` | Mark course as completed |
| POST | `/api/Courses/{courseId}/approve` | Approve course |

### Educational Activities
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/EducationalActivities` | List all activities |
| GET | `/api/EducationalActivities/specialization/{specializationId}` | Get by specialization |
| GET | `/api/EducationalActivities/specialization/{specializationId}/type/{type}` | Get by type |
| GET | `/api/EducationalActivities/{id}` | Get specific activity |
| POST | `/api/EducationalActivities` | Create new activity |
| PUT | `/api/EducationalActivities/{id}` | Update activity |
| DELETE | `/api/EducationalActivities/{id}` | Delete activity |

### Self Education
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/SelfEducation` | List all self education |
| GET | `/api/SelfEducation/{id}` | Get specific self education |
| POST | `/api/SelfEducation` | Create new self education |
| PUT | `/api/SelfEducation/{id}` | Update self education |
| DELETE | `/api/SelfEducation/{id}` | Delete self education |
| POST | `/api/SelfEducation/{id}/complete` | Complete self education |
| GET | `/api/SelfEducation/user/{userId}` | Get user's self education |
| GET | `/api/SelfEducation/user/{userId}/year/{year}` | Get by year |
| GET | `/api/SelfEducation/user/{userId}/specialization/{specializationId}/completed` | Get completed |
| GET | `/api/SelfEducation/user/{userId}/specialization/{specializationId}/credit-hours` | Get credit hours |
| GET | `/api/SelfEducation/user/{userId}/specialization/{specializationId}/quality-score` | Get quality score |

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
| GET | `/api/Absences` | List all absences |
| GET | `/api/Absences/user/{userId}` | Get user's absences |
| GET | `/api/Absences/{id}` | Get specific absence |
| POST | `/api/Absences` | Create new absence |
| PUT | `/api/Absences/{id}` | Update absence |
| DELETE | `/api/Absences/{id}` | Delete absence |
| POST | `/api/Absences/{id}/approve` | Approve absence |

### Dashboard
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Dashboard/overview` | Get dashboard overview |
| GET | `/api/Dashboard/progress/{specializationId}` | Get progress by specialization |
| GET | `/api/Dashboard/statistics/{specializationId}` | Get statistics |

### Calculations
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Calculations/internship-days` | Calculate internship days |
| POST | `/api/Calculations/normalize-time` | Normalize time format |
| POST | `/api/Calculations/required-shift-hours` | Calculate required hours |

### Publications
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Publications` | List all publications |
| GET | `/api/Publications/{id}` | Get specific publication |
| POST | `/api/Publications` | Create new publication |
| PUT | `/api/Publications/{id}` | Update publication |
| DELETE | `/api/Publications/{id}` | Delete publication |
| GET | `/api/Publications/user/{userId}` | Get user's publications |
| GET | `/api/Publications/user/{userId}/first-author` | Get first-author publications |
| GET | `/api/Publications/user/{userId}/peer-reviewed` | Get peer-reviewed publications |
| GET | `/api/Publications/user/{userId}/specialization/{specializationId}/impact-score` | Get impact score |

### Recognitions
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Recognitions` | List all recognitions |
| GET | `/api/Recognitions/{id}` | Get specific recognition |
| POST | `/api/Recognitions` | Create new recognition |
| PUT | `/api/Recognitions/{id}` | Update recognition |
| DELETE | `/api/Recognitions/{id}` | Delete recognition |
| POST | `/api/Recognitions/{id}/approve` | Approve recognition |
| GET | `/api/Recognitions/user/{userId}` | Get user's recognitions |
| GET | `/api/Recognitions/user/{userId}/specialization/{specializationId}/total-reduction` | Get total reduction |

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

### SMK Export Format
```bash
GET /api/specializations/1/export
Accept: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
```

Returns XLSX file with sheets:
1. Summary - Overview statistics
2. Medical Shifts - All shifts with hours
3. Procedures - All procedures with counts
4. Courses - Completed courses
5. Internships - All internships

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

Last Updated: 2025-06-18
Status: Development/Testing Phase