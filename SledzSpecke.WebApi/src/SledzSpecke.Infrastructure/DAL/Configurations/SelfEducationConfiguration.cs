using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class SelfEducationConfiguration : IEntityTypeConfiguration<SelfEducation>
{
    public void Configure(EntityTypeBuilder<SelfEducation> builder)
    {
        builder.HasKey(se => se.Id);

        builder.Property(se => se.Id)
            .HasConversion(id => id.Value, value => new SelfEducationId(value));

        builder.Property(se => se.ModuleId)
            .HasConversion(id => id.Value, value => new ModuleId(value));

        builder.Property(se => se.Type)
            .HasConversion<string>();

        builder.Property(se => se.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(se => se.Date)
            .IsRequired();

        builder.Property(se => se.Hours)
            .IsRequired();

        builder.Property(se => se.PublicationTitle)
            .HasMaxLength(500);

        builder.Property(se => se.JournalName)
            .HasMaxLength(200);

        builder.Property(se => se.IsPeerReviewed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(se => se.Role)
            .HasConversion<string>();

        builder.Property(se => se.SyncStatus)
            .HasConversion<string>()
            .HasDefaultValue(SyncStatus.NotSynced);

        builder.Property(se => se.CreatedAt)
            .IsRequired();

        builder.Property(se => se.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(se => se.ModuleId);
        builder.HasIndex(se => se.Type);
        builder.HasIndex(se => se.Date);
        
        // Note: The navigation property is configured from the Module side in ModuleConfiguration
    }
}