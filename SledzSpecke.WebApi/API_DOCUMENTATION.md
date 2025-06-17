# SledzSpecke API Documentation

## Overview
SledzSpecke API provides comprehensive endpoints for managing medical specialization tracking in compliance with Polish SMK (System Monitorowania Kształcenia) requirements.

## Base URL
- Production: `https://api.sledzspecke.pl`
- Development: `http://localhost:5000`

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

## Core Endpoints

### Authentication
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
| PUT | `/api/users/me` | Update user profile |
| GET | `/api/users/me/preferences` | Get user preferences |
| PUT | `/api/users/me/preferences` | Update user preferences |

### Specializations
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/specializations` | List all specializations |
| GET | `/api/specializations/{id}` | Get specialization details |
| POST | `/api/specializations` | Create new specialization |
| GET | `/api/specializations/{id}/statistics` | Get specialization statistics |
| GET | `/api/specializations/{id}/export` | Export specialization data (XLSX) |

### Modules
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/modules/specialization/{specializationId}` | Get modules for specialization |
| GET | `/api/modules/{moduleId}` | Get module details |
| GET | `/api/modules/{moduleId}/progress` | Get module progress |
| PUT | `/api/modules/switch` | Switch active module |
| POST | `/api/modules/{moduleId}/complete` | Mark module as complete |
| POST | `/api/modules/{moduleId}/internships` | Create internship in module |
| POST | `/api/modules/{moduleId}/medical-shifts` | Add medical shift |
| POST | `/api/modules/{moduleId}/procedures` | Add procedure |
| POST | `/api/modules/{moduleId}/courses` | Create course |
| POST | `/api/modules/{moduleId}/self-education` | Add self-education activity |
| POST | `/api/modules/{moduleId}/additional-days` | Add additional self-education days |

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

### Medical Shifts
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/medical-shifts` | List medical shifts |
| GET | `/api/medical-shifts/{id}` | Get shift details |
| POST | `/api/medical-shifts` | Add medical shift |
| PUT | `/api/medical-shifts/{id}` | Update shift |
| DELETE | `/api/medical-shifts/{id}` | Delete shift |
| POST | `/api/medical-shifts/bulk` | Add multiple shifts |

### Procedures
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/procedures` | List procedures |
| GET | `/api/procedures/{id}` | Get procedure details |
| POST | `/api/procedures` | Add procedure |
| PUT | `/api/procedures/{id}` | Update procedure |
| DELETE | `/api/procedures/{id}` | Delete procedure |
| GET | `/api/procedures/search` | Search procedures by code/name |

### Courses
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/courses` | List courses |
| GET | `/api/courses/{id}` | Get course details |
| POST | `/api/courses` | Create course |
| PUT | `/api/courses/{id}` | Update course |
| DELETE | `/api/courses/{id}` | Delete course |
| POST | `/api/courses/{id}/complete` | Mark course as complete |

### Internships
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/internships` | List internships |
| GET | `/api/internships/{id}` | Get internship details |
| POST | `/api/internships` | Create internship |
| PUT | `/api/internships/{id}` | Update internship |
| DELETE | `/api/internships/{id}` | Delete internship |
| POST | `/api/internships/{id}/complete` | Mark as complete |

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

## Rate Limiting
- 100 requests per minute per IP
- 1000 requests per hour per authenticated user
- Export endpoints: 10 requests per hour

## Webhooks (Coming Soon)
Subscribe to events:
- Specialization completed
- Module completed
- Internship status changed
- Course completed

## Versioning
API version is included in the URL: `/api/v1/...`
Current version: v1

## SDKs
- JavaScript/TypeScript: `npm install @sledzspecke/api-client`
- Python: `pip install sledzspecke-client`
- .NET: `dotnet add package SledzSpecke.ApiClient`

## Support
- Documentation: https://docs.sledzspecke.pl
- Issues: https://github.com/sledzspecke/api/issues
- Email: support@sledzspecke.pl