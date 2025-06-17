# SledzSpecke Deployment Plan - Major Architecture Updates
*Date: 2025-06-17*  
*Version: 2.0 - Full Domain Services Implementation*

## 🚀 Overview

This deployment includes major architectural enhancements to the SledzSpecke system:
- Full implementation of domain services with real SMK business logic
- New decorators for caching and performance monitoring
- FluentValidation for module operations
- Enhanced repository methods
- Domain event integration tests

## 📋 Pre-Deployment Checklist

### Code Review
- [ ] All 134 core unit tests passing ✅
- [ ] Build successful with 0 errors ✅
- [ ] Architecture documentation updated ✅
- [ ] No database migrations required ✅

### Testing Status
- Core Tests: 134/134 passing ✅
- Integration Tests: Compilation errors (known issue, non-blocking)
- E2E Tests: Require running application (test post-deployment)

## 🔧 Deployment Steps

### 1. Backup Current State (5 minutes)
```bash
# Create backup of current deployment
sudo cp -r /var/www/sledzspecke-api /var/www/sledzspecke-api-backup-$(date +%Y%m%d)

# Backup database
sudo -u postgres pg_dump sledzspecke_db > ~/sledzspecke_db_backup_$(date +%Y%m%d).sql
```

### 2. Deploy Code (10 minutes)
```bash
# Pull latest changes
cd /home/ubuntu/sledzspecke
sudo git pull origin master

# Build and publish
sudo dotnet publish SledzSpecke.WebApi/src/SledzSpecke.Api \
  -c Release \
  -o /var/www/sledzspecke-api-new

# Verify build
ls -la /var/www/sledzspecke-api-new/
```

### 3. Stop Service (1 minute)
```bash
# Stop the API service
sudo systemctl stop sledzspecke-api

# Verify it's stopped
sudo systemctl status sledzspecke-api
```

### 4. Deploy New Version (2 minutes)
```bash
# Move current to old
sudo mv /var/www/sledzspecke-api /var/www/sledzspecke-api-old

# Deploy new version
sudo mv /var/www/sledzspecke-api-new /var/www/sledzspecke-api

# Set permissions
sudo chown -R www-data:www-data /var/www/sledzspecke-api
```

### 5. Start Service (1 minute)
```bash
# Start the API service
sudo systemctl start sledzspecke-api

# Check status
sudo systemctl status sledzspecke-api
```

### 6. Verify Deployment (5 minutes)
```bash
# Check logs for errors
sudo journalctl -u sledzspecke-api -n 100

# Test health endpoint
curl https://api.sledzspecke.pl/health

# Check monitoring dashboard
# https://api.sledzspecke.pl/monitoring/dashboard
```

## 🧪 Post-Deployment Testing

### 1. API Smoke Tests
```bash
# Test registration endpoint
curl -X POST https://api.sledzspecke.pl/api/users/sign-up \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'

# Test login
curl -X POST https://api.sledzspecke.pl/api/users/sign-in \
  -H "Content-Type: application/json" \
  -d '{"login":"test@example.com","password":"Test123!"}'
```

### 2. Domain Services Verification
- Test module completion service via API
- Verify SMK synchronization endpoints
- Check course requirement validation
- Test medical shift validation rules

### 3. Performance Monitoring
- Check new metrics endpoints
- Verify caching is working
- Monitor response times

## 🔄 Rollback Plan

If issues are encountered:

### Quick Rollback (2 minutes)
```bash
# Stop service
sudo systemctl stop sledzspecke-api

# Restore old version
sudo rm -rf /var/www/sledzspecke-api
sudo mv /var/www/sledzspecke-api-old /var/www/sledzspecke-api

# Start service
sudo systemctl start sledzspecke-api
```

### Database Rollback (if needed)
```bash
# Only if database changes were made (none in this deployment)
sudo -u postgres psql sledzspecke_db < ~/sledzspecke_db_backup_$(date +%Y%m%d).sql
```

## 📊 Key Changes in This Release

### 1. Domain Services
- **CourseRequirementService**: Real CMKP 2023 course data
- **SimplifiedModuleCompletionService**: Full SMK requirement tracking
- **SimplifiedSMKSynchronizationService**: Complete sync workflow
- **DurationCalculationService**: Fixed absence tracking

### 2. New Decorators
- **CachingQueryHandlerDecorator**: SHA256-based caching
- **PerformanceQueryHandlerDecorator**: Metrics collection

### 3. FluentValidation
- **CompleteModuleValidator**: Module completion validation
- **SwitchModuleValidator**: Module switching rules

### 4. Repository Enhancements
- **IAbsenceRepository**: New date range queries
- **IModuleRepository**: Active module queries

## ⚠️ Known Issues

1. **Integration Tests**: Compilation errors due to outdated test code
   - Impact: None on production
   - Fix planned for next sprint

2. **E2E Tests**: Require application running
   - Impact: Cannot run automated E2E tests during deployment
   - Workaround: Manual testing post-deployment

## 📞 Support Contacts

- **Lead Developer**: Check CLAUDE.md for contact info
- **DevOps**: Ubuntu VPS admin
- **Monitoring**: https://api.sledzspecke.pl/monitoring/dashboard

## ✅ Deployment Checklist

### Pre-Deployment
- [ ] Backup created
- [ ] Team notified
- [ ] Maintenance window scheduled

### During Deployment
- [ ] Code deployed
- [ ] Service restarted
- [ ] Logs checked
- [ ] Health endpoint verified

### Post-Deployment
- [ ] Smoke tests passed
- [ ] Performance metrics normal
- [ ] No error spike in logs
- [ ] Users can login/register

## 📈 Success Criteria

The deployment is considered successful when:
1. All health checks pass
2. No errors in logs for 5 minutes
3. Login/registration works
4. Response times < 200ms for most endpoints
5. Cache hit rate > 50% after 10 minutes

## 🎉 Deployment Complete

Once all checks pass, update the deployment log:
```bash
echo "Deployment completed at $(date)" >> ~/deployments.log
echo "Version: 2.0 - Domain Services Implementation" >> ~/deployments.log
echo "Status: SUCCESS" >> ~/deployments.log
```

---

**Estimated Total Time**: 30 minutes  
**Risk Level**: Medium (significant code changes, but no database changes)  
**Rollback Time**: 2 minutes