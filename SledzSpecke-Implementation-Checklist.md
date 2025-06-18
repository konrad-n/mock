# SledzSpecke Advanced Patterns Implementation Checklist

## Overview
This checklist provides a step-by-step implementation guide with build checkpoints to ensure continuous integration success.

## Pre-Implementation Setup

- [ ] Create feature branch (optional - can work directly on master as instructed)
- [ ] Ensure all existing tests pass: `sudo dotnet test`
- [ ] Check current build status: `sudo dotnet build`
- [ ] Backup database: `sudo -u postgres pg_dump sledzspecke_db > backup_$(date +%Y%m%d).sql`

---

## Phase 1: Saga Pattern Implementation (Priority: HIGH)

### 1.1 Core Infrastructure
- [ ] Create directory structure:
  ```bash
  sudo mkdir -p src/SledzSpecke.Core/Sagas
  sudo mkdir -p src/SledzSpecke.Infrastructure/Sagas
  sudo mkdir -p src/SledzSpecke.Application/Sagas
  ```

- [ ] Implement base abstractions:
  - [ ] `ISaga.cs`
  - [ ] `ISagaStep.cs`
  - [ ] `SagaBase.cs`
  - [ ] **BUILD CHECK**: `sudo dotnet build`

### 1.2 SMK Monthly Report Saga
- [ ] Create `SMKMonthlyReportSaga.cs`
- [ ] Implement saga steps:
  - [ ] `ValidateMonthlyHoursStep.cs`
  - [ ] `ValidateProceduresStep.cs`
  - [ ] `GenerateMonthlyReportStep.cs`
  - [ ] `NotifySupervisorStep.cs`
  - [ ] `ArchiveMonthlyDataStep.cs`
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 1.3 Saga Persistence
- [ ] Create `SagaStateEntity.cs`
- [ ] Create `SagaStepEntity.cs`
- [ ] Add EF Core configuration
- [ ] Create migration: `sudo dotnet ef migrations add AddSagaPattern -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api`
- [ ] **BUILD CHECK**: `sudo dotnet build`
- [ ] Apply migration: `sudo dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api`

### 1.4 Saga Orchestrator
- [ ] Implement `ISagaOrchestrator.cs`
- [ ] Implement `SagaOrchestrator.cs`
- [ ] Add dependency injection registration
- [ ] **BUILD CHECK**: `sudo dotnet build`
- [ ] **TEST CHECK**: `sudo dotnet test --filter "Saga"`

### 1.5 Integration Test
- [ ] Create `SMKMonthlyReportSagaTests.cs`
- [ ] Test happy path
- [ ] Test compensation logic
- [ ] **FULL TEST**: `sudo dotnet test`

---

## Phase 2: Enhanced Message Pipeline (Priority: MEDIUM)

### 2.1 Pipeline Core
- [ ] Create `IPipelineBuilder.cs`
- [ ] Create `PipelineBuilder.cs`
- [ ] Create `MessageContext.cs`
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 2.2 Pipeline Steps
- [ ] Implement `ValidationStep.cs`
- [ ] Implement `RetryStep.cs`
- [ ] Implement `DeadLetterStep.cs`
- [ ] Create retry policies
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 2.3 Pipeline Factory
- [ ] Create `IPipelineFactory.cs`
- [ ] Implement `PipelineFactory.cs`
- [ ] Register medical shift pipeline
- [ ] Register procedure pipeline
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 2.4 Dead Letter Queue
- [ ] Create `DeadLetterEntity.cs`
- [ ] Create `IDeadLetterService.cs`
- [ ] Implement `DeadLetterService.cs`
- [ ] Add migration: `sudo dotnet ef migrations add AddDeadLetterQueue -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api`
- [ ] **BUILD & MIGRATE**: 
  ```bash
  sudo dotnet build
  sudo dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
  ```

### 2.5 Pipeline Tests
- [ ] Create `PipelineTests.cs`
- [ ] Test step execution order
- [ ] Test error handling
- [ ] Test retry logic
- [ ] **TEST CHECK**: `sudo dotnet test --filter "Pipeline"`

---

## Phase 3: Comprehensive Audit Trail (Priority: MEDIUM)

### 3.1 Audit Infrastructure
- [ ] Create `IAuditable.cs`
- [ ] Create `AuditLog.cs`
- [ ] Update existing entities to implement `IAuditable`
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 3.2 Audit Interceptor
- [ ] Create `AuditInterceptor.cs`
- [ ] Register interceptor in `DbContext`
- [ ] Configure JSON serialization for audit data
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 3.3 Audit Storage
- [ ] Add `AuditLogs` DbSet
- [ ] Configure entity mapping
- [ ] Create migration: `sudo dotnet ef migrations add AddAuditLogging -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api`
- [ ] **BUILD & MIGRATE**:
  ```bash
  sudo dotnet build
  sudo dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
  ```

### 3.4 Audit Query API
- [ ] Create `AuditController.cs`
- [ ] Add search endpoints
- [ ] Add export functionality
- [ ] Secure with authorization
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 3.5 Audit Tests
- [ ] Test entity change tracking
- [ ] Test audit log generation
- [ ] Test query functionality
- [ ] **TEST CHECK**: `sudo dotnet test --filter "Audit"`

---

## Phase 4: Event Sourcing (Priority: LOW - Optional)

### 4.1 Event Store Core
- [ ] Create `IEventSourced.cs`
- [ ] Create `EventSourcedAggregate.cs`
- [ ] Create `IEventStore.cs`
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 4.2 Event Storage
- [ ] Create `EventData.cs` entity
- [ ] Create `PostgresEventStore.cs`
- [ ] Add EF configuration
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 4.3 Event-Sourced Entities
- [ ] Create `EventSourcedMedicalShift.cs`
- [ ] Create `EventSourcedInternship.cs`
- [ ] Implement event handlers
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 4.4 Snapshots
- [ ] Create `Snapshot.cs` entity
- [ ] Implement `ISnapshotStore.cs`
- [ ] Implement `PostgresSnapshotStore.cs`
- [ ] Configure snapshot frequency
- [ ] **BUILD CHECK**: `sudo dotnet build`

### 4.5 Projections
- [ ] Create read models
- [ ] Implement projection handlers
- [ ] Configure MediatR for projections
- [ ] Create migration: `sudo dotnet ef migrations add AddEventSourcing -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api`
- [ ] **BUILD & MIGRATE**:
  ```bash
  sudo dotnet build
  sudo dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
  ```

### 4.6 Event Sourcing Tests
- [ ] Test event storage
- [ ] Test aggregate reconstruction
- [ ] Test snapshots
- [ ] Test projections
- [ ] **FULL TEST**: `sudo dotnet test`

---

## Phase 5: Integration & E2E Testing

### 5.1 Update E2E Tests
- [ ] Add saga workflow tests
- [ ] Add audit trail verification
- [ ] Test pipeline execution in E2E scenarios
- [ ] **E2E TEST**: `./run-e2e-tests-isolated.sh`

### 5.2 Performance Testing
- [ ] Load test saga execution
- [ ] Test pipeline throughput
- [ ] Measure audit log impact
- [ ] Optimize if needed

---

## Phase 6: Production Deployment

### 6.1 Pre-Deployment
- [ ] All tests passing
- [ ] Code review completed
- [ ] Documentation updated
- [ ] **FINAL BUILD**: `sudo dotnet build -c Release`

### 6.2 Deployment Steps
```bash
# 1. Commit changes
sudo git add .
sudo git commit -m "feat: Implement Saga, Enhanced Pipeline, Audit Trail, and Event Sourcing patterns"

# 2. Push to master (triggers GitHub Actions)
sudo git push origin master

# 3. Monitor deployment
./check-builds.sh latest

# 4. Verify API status
sudo systemctl status sledzspecke-api

# 5. Check logs
sudo journalctl -u sledzspecke-api -f
```

### 6.3 Post-Deployment Verification
- [ ] Check monitoring dashboard: https://api.sledzspecke.pl/monitoring/dashboard
- [ ] Verify saga endpoints work
- [ ] Test audit log generation
- [ ] Run smoke tests
- [ ] Monitor error rates

---

## Build Frequency Reminders

**IMPORTANT**: Run `sudo dotnet build` after each major step to catch errors early!

### Quick Build Commands
```bash
# Build only
sudo dotnet build

# Build and run tests
sudo dotnet test

# Build specific project
sudo dotnet build src/SledzSpecke.Api

# Clean and rebuild
sudo dotnet clean && sudo dotnet build
```

### Common Build Fixes
1. **Missing namespace**: Add appropriate `using` statements
2. **Type conflicts**: Use fully qualified names
3. **Missing dependencies**: Add NuGet packages via `dotnet add package`
4. **Migration issues**: Ensure DbContext is properly configured

---

## Rollback Plan

If issues occur in production:

```bash
# 1. Quick rollback
sudo git -C /home/ubuntu/sledzspecke reset --hard HEAD~1
sudo git -C /home/ubuntu/sledzspecke pull

# 2. Rebuild and restart
sudo dotnet publish /home/ubuntu/sledzspecke/SledzSpecke.WebApi/src/SledzSpecke.Api -c Release -o /var/www/sledzspecke-api
sudo systemctl restart sledzspecke-api

# 3. Database rollback (if needed)
sudo dotnet ef database update <PreviousMigrationName> -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
```

---

## Success Criteria

- [ ] All patterns implemented and tested
- [ ] Zero build errors
- [ ] All existing tests still pass (97/106 baseline)
- [ ] E2E tests pass with new features
- [ ] Production deployment successful
- [ ] No performance degradation
- [ ] Monitoring shows healthy status

---

## Notes

- Work directly on master branch as instructed
- Use `sudo` for all commands (admin access available)
- GitHub admin access available via `gh` command
- Build logs saved on VPS - check with `./check-builds.sh`
- Don't add time estimates - focus on implementation