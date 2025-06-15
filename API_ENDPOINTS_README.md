# SledzSpecke API Endpoints & Services

## Production URLs
- **Main API**: https://api.sledzspecke.pl
- **Frontend**: https://sledzspecke.pl
- **VPS IP**: 51.77.59.184

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

## Main API Endpoints

### Authentication ✅
- `POST /api/auth/sign-up` - User registration
- `POST /api/auth/sign-in` - User login

### User Management
- `GET /api/users/profile` - Get current user profile
- `PUT /api/users/profile` - Update user profile
- `PUT /api/users/change-password` - Change password
- `GET /api/users/preferences` - Get user preferences
- `PUT /api/users/preferences` - Update user preferences
- `GET /api/Users` - List all users
- `GET /api/Users/{userId}` - Get specific user
- `PUT /api/Users/{userId}` - Update user
- `DELETE /api/Users/{userId}` - Delete user

### Internships
- `GET /api/Internships` - List all internships
- `GET /api/Internships/{internshipId}` - Get specific internship
- `POST /api/Internships` - Create new internship
- `PUT /api/Internships/{internshipId}` - Update internship
- `DELETE /api/Internships/{internshipId}` - Delete internship
- `POST /api/Internships/{internshipId}/approve` - Approve internship
- `POST /api/Internships/{internshipId}/complete` - Complete internship

### Medical Shifts
- `GET /api/MedicalShifts` - List all shifts
- `GET /api/MedicalShifts/{shiftId}` - Get specific shift
- `POST /api/MedicalShifts` - Add new shift
- `PUT /api/MedicalShifts/{shiftId}` - Update shift
- `DELETE /api/MedicalShifts/{shiftId}` - Delete shift
- `GET /api/MedicalShifts/statistics` - Get shift statistics

### Procedures
- `GET /api/Procedures` - List all procedures
- `GET /api/Procedures/{procedureId}` - Get specific procedure
- `POST /api/Procedures` - Add new procedure
- `PUT /api/Procedures/{procedureId}` - Update procedure
- `DELETE /api/Procedures/{procedureId}` - Delete procedure
- `GET /api/Procedures/statistics` - Get procedure statistics

### Courses
- `GET /api/Courses` - List all courses
- `GET /api/Courses/{courseId}` - Get specific course
- `POST /api/Courses` - Create new course
- `PUT /api/Courses/{courseId}` - Update course
- `DELETE /api/Courses/{courseId}` - Delete course
- `POST /api/Courses/{courseId}/complete` - Mark course as completed
- `POST /api/Courses/{courseId}/approve` - Approve course

### Educational Activities
- `GET /api/EducationalActivities` - List all activities
- `GET /api/EducationalActivities/specialization/{specializationId}` - Get by specialization
- `GET /api/EducationalActivities/specialization/{specializationId}/type/{type}` - Get by type
- `GET /api/EducationalActivities/{id}` - Get specific activity
- `POST /api/EducationalActivities` - Create new activity
- `PUT /api/EducationalActivities/{id}` - Update activity
- `DELETE /api/EducationalActivities/{id}` - Delete activity

### Absences
- `GET /api/Absences` - List all absences
- `GET /api/Absences/user/{userId}` - Get user's absences
- `GET /api/Absences/{id}` - Get specific absence
- `POST /api/Absences` - Create new absence
- `PUT /api/Absences/{id}` - Update absence
- `DELETE /api/Absences/{id}` - Delete absence
- `POST /api/Absences/{id}/approve` - Approve absence

### Dashboard
- `GET /api/Dashboard/overview` - Get dashboard overview
- `GET /api/Dashboard/progress/{specializationId}` - Get progress by specialization
- `GET /api/Dashboard/statistics/{specializationId}` - Get statistics

### Calculations
- `POST /api/Calculations/internship-days` - Calculate internship days
- `POST /api/Calculations/normalize-time` - Normalize time format
- `POST /api/Calculations/required-shift-hours` - Calculate required hours

### Specializations
- `GET /api/Specializations/{specializationId}` - Get specific specialization
- `GET /api/Specializations/{specializationId}/statistics` - Get specialization statistics

### Modules
- `GET /api/Modules/specialization/{specializationId}` - Get modules by specialization
- `POST /api/Modules/switch` - Switch module

### Publications
- `GET /api/Publications` - List all publications
- `GET /api/Publications/{id}` - Get specific publication
- `POST /api/Publications` - Create new publication
- `PUT /api/Publications/{id}` - Update publication
- `DELETE /api/Publications/{id}` - Delete publication
- `GET /api/Publications/user/{userId}` - Get user's publications
- `GET /api/Publications/user/{userId}/first-author` - Get first-author publications
- `GET /api/Publications/user/{userId}/peer-reviewed` - Get peer-reviewed publications
- `GET /api/Publications/user/{userId}/specialization/{specializationId}/impact-score` - Get impact score

### Recognitions
- `GET /api/Recognitions` - List all recognitions
- `GET /api/Recognitions/{id}` - Get specific recognition
- `POST /api/Recognitions` - Create new recognition
- `PUT /api/Recognitions/{id}` - Update recognition
- `DELETE /api/Recognitions/{id}` - Delete recognition
- `POST /api/Recognitions/{id}/approve` - Approve recognition
- `GET /api/Recognitions/user/{userId}` - Get user's recognitions
- `GET /api/Recognitions/user/{userId}/specialization/{specializationId}/total-reduction` - Get total reduction

### Self Education
- `GET /api/SelfEducation` - List all self education
- `GET /api/SelfEducation/{id}` - Get specific self education
- `POST /api/SelfEducation` - Create new self education
- `PUT /api/SelfEducation/{id}` - Update self education
- `DELETE /api/SelfEducation/{id}` - Delete self education
- `POST /api/SelfEducation/{id}/complete` - Complete self education
- `GET /api/SelfEducation/user/{userId}` - Get user's self education
- `GET /api/SelfEducation/user/{userId}/year/{year}` - Get by year
- `GET /api/SelfEducation/user/{userId}/specialization/{specializationId}/completed` - Get completed
- `GET /api/SelfEducation/user/{userId}/specialization/{specializationId}/credit-hours` - Get credit hours
- `GET /api/SelfEducation/user/{userId}/specialization/{specializationId}/quality-score` - Get quality score

### File Management
- `POST /api/files/upload` - Upload file
- `GET /api/files/{fileId}` - Get file info
- `GET /api/files/{fileId}/download` - Download file
- `DELETE /api/files/{fileId}` - Delete file
- `GET /api/files/entity/{entityType}/{entityId}` - Get files by entity

### Specialization Templates
- `GET /api/specialization-templates` - List all templates
- `GET /api/specialization-templates/{specializationCode}/{smkVersion}` - Get specific template
- `GET /api/specialization-templates/{specializationCode}/{smkVersion}/modules/{moduleId}` - Get module
- `GET /api/specialization-templates/{specializationCode}/{smkVersion}/courses/{courseId}` - Get course
- `GET /api/specialization-templates/{specializationCode}/{smkVersion}/internships/{internshipId}` - Get internship
- `GET /api/specialization-templates/{specializationCode}/{smkVersion}/procedures/{procedureId}` - Get procedure
- `POST /api/specialization-templates/{specializationCode}/{smkVersion}/procedures/{procedureId}/validate` - Validate procedure

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

### ❌ Previously Not Available (Now Fixed ✅)
- **Seq**: ✅ Running on port 5341
- **Grafana**: ✅ Running on port 3000
- **Docker**: ✅ Installed and running
- **UFW Firewall**: ✅ Active with all necessary rules
- **Fail2ban**: ✅ Active with SSH and nginx protection
- **SSL Certificate**: ✅ Let's Encrypt configured and auto-renewing
- **Backup System**: ✅ Automated daily backups at 2 AM

### 🔧 DevOps Components Implemented
1. **Docker & Docker Compose**: Full container orchestration
2. **Monitoring Stack**: Seq, Grafana, Prometheus, Node Exporter, cAdvisor
3. **Security**: UFW firewall, Fail2ban intrusion prevention
4. **SSL/TLS**: Let's Encrypt certificates with auto-renewal
5. **Backup System**: Daily automated backups with 7-day retention
6. **Auto-updates**: Watchtower for container updates

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

## Local Development

When running locally:
- API: http://localhost:5000
- Use `ASPNETCORE_ENVIRONMENT=Development`
- Database: Local PostgreSQL or in-memory

---

Last Updated: 2025-06-15
Status: Development/Testing Phase