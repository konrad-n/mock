using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Auditing;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // Since we generate it in the Create method
            
        builder.Property(e => e.EntityType)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.EntityId)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Action)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Timestamp)
            .IsRequired();
            
        builder.Property(e => e.PropertyName)
            .HasMaxLength(100);
            
        // JSONB columns for PostgreSQL
        builder.Property(e => e.OldValues)
            .HasColumnType("jsonb");
            
        builder.Property(e => e.NewValues)
            .HasColumnType("jsonb");
            
        // Indexes for performance
        builder.HasIndex(e => new { e.EntityType, e.EntityId })
            .HasDatabaseName("IX_AuditLogs_Entity");
            
        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");
            
        builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_AuditLogs_Timestamp");
            
        builder.HasIndex(e => e.Action)
            .HasDatabaseName("IX_AuditLogs_Action");
    }
}