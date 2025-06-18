using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SledzSpecke.Core.Auditing;
using SledzSpecke.Core.Services;

namespace SledzSpecke.Infrastructure.Auditing;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly List<AuditLog> _auditLogs = new();

    public AuditInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ProcessAuditableEntities(eventData.Context);
            GenerateAuditLogs(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ProcessAuditableEntities(eventData.Context);
            GenerateAuditLogs(eventData.Context);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context is not null && _auditLogs.Any())
        {
            // Save audit logs after the main transaction
            using var auditContext = eventData.Context;
            auditContext.Set<AuditLog>().AddRange(_auditLogs);
            _auditLogs.Clear();
            auditContext.SaveChanges();
        }

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null && _auditLogs.Any())
        {
            // Save audit logs after the main transaction
            using var auditContext = eventData.Context;
            auditContext.Set<AuditLog>().AddRange(_auditLogs);
            _auditLogs.Clear();
            await auditContext.SaveChangesAsync(cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void ProcessAuditableEntities(DbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditable &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var auditable = (IAuditable)entry.Entity;
            var now = DateTime.UtcNow;
            var user = _currentUserService.UserId ?? "System";

            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = user;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditable.ModifiedAt)).CurrentValue = now;
                entry.Property(nameof(IAuditable.ModifiedBy)).CurrentValue = user;
                
                // Ensure CreatedAt and CreatedBy are not modified
                entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
            }
        }
    }

    private void GenerateAuditLogs(DbContext context)
    {
        // Define which entities should be audited
        var auditableEntityTypes = new[] { "MedicalShift", "Procedure", "Internship", "User", "Module", "ModuleCompletion" };

        var entries = context.ChangeTracker.Entries()
            .Where(e => auditableEntityTypes.Contains(e.Entity.GetType().Name) &&
                       (e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted));

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType().Name;
            var entityId = GetEntityId(entry);
            var action = entry.State.ToString();
            var userId = _currentUserService.UserId ?? "System";

            if (entry.State == EntityState.Modified)
            {
                // Log each modified property separately
                foreach (var property in entry.Properties.Where(p => p.IsModified))
                {
                    // Skip audit fields themselves
                    if (property.Metadata.Name is "CreatedAt" or "CreatedBy" or "ModifiedAt" or "ModifiedBy")
                        continue;

                    var auditLog = AuditLog.Create(
                        entityType,
                        entityId,
                        "Modified",
                        userId,
                        property.OriginalValue,
                        property.CurrentValue,
                        property.Metadata.Name
                    );

                    _auditLogs.Add(auditLog);
                }
            }
            else
            {
                // For Added/Deleted, log the entire entity
                var auditLog = AuditLog.Create(
                    entityType,
                    entityId,
                    action,
                    userId,
                    entry.State == EntityState.Deleted ? GetEntityValues(entry) : null,
                    entry.State == EntityState.Added ? GetEntityValues(entry) : null
                );

                _auditLogs.Add(auditLog);
            }
        }
    }

    private string GetEntityId(EntityEntry entry)
    {
        var keyProperties = entry.Metadata.FindPrimaryKey()?.Properties;
        if (keyProperties == null || !keyProperties.Any())
            return "Unknown";

        var keyValues = keyProperties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? "null")
            .ToList();

        return string.Join(",", keyValues);
    }

    private Dictionary<string, object?> GetEntityValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            values[property.Metadata.Name] = property.CurrentValue;
        }

        return values;
    }
}