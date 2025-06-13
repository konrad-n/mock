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

        builder.Property(se => se.SpecializationId)
            .HasConversion(id => id.Value, value => new SpecializationId(value));

        builder.Property(se => se.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.Property(se => se.Type)
            .HasConversion<string>();

        builder.Property(se => se.Year)
            .IsRequired();

        builder.Property(se => se.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(se => se.Description)
            .HasMaxLength(2000);

        builder.Property(se => se.Provider)
            .HasMaxLength(500);

        builder.Property(se => se.Publisher)
            .HasMaxLength(500);

        builder.Property(se => se.StartDate);

        builder.Property(se => se.EndDate);

        builder.Property(se => se.DurationHours);

        builder.Property(se => se.IsCompleted)
            .HasDefaultValue(false);

        builder.Property(se => se.CompletedAt);

        builder.Property(se => se.CertificatePath)
            .HasMaxLength(500);

        builder.Property(se => se.URL)
            .HasMaxLength(1000);

        builder.Property(se => se.ISBN)
            .HasMaxLength(50);

        builder.Property(se => se.DOI)
            .HasMaxLength(255);

        builder.Property(se => se.CreditHours)
            .IsRequired();

        builder.Property(se => se.SyncStatus)
            .HasConversion<string>()
            .HasDefaultValue(SyncStatus.NotSynced);

        builder.Property(se => se.AdditionalFields)
            .HasMaxLength(2000);

        builder.Property(se => se.CreatedAt)
            .IsRequired();

        builder.Property(se => se.UpdatedAt)
            .IsRequired();

        builder.HasIndex(se => se.UserId);
        builder.HasIndex(se => se.SpecializationId);
        builder.HasIndex(se => new { se.UserId, se.SpecializationId });
        builder.HasIndex(se => se.Type);
        builder.HasIndex(se => se.Year);
        builder.HasIndex(se => se.IsCompleted);
    }
}