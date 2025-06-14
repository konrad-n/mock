using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class AbsenceConfiguration : IEntityTypeConfiguration<Absence>
{
    public void Configure(EntityTypeBuilder<Absence> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => new AbsenceId(value));

        builder.Property(a => a.SpecializationId)
            .HasConversion(id => id.Value, value => new SpecializationId(value));

        builder.Property(a => a.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.Property(a => a.Type)
            .HasConversion<string>();

        builder.Property(a => a.StartDate)
            .IsRequired();

        builder.Property(a => a.EndDate)
            .IsRequired();

        builder.Property(a => a.DurationInDays)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.DocumentPath)
            .HasMaxLength(500);

        builder.Property(a => a.IsApproved)
            .HasDefaultValue(false);

        builder.Property(a => a.ApprovedAt);

        builder.Property(a => a.ApprovedBy)
            .HasConversion(id => id != null ? (int?)id.Value : null,
                          value => value.HasValue ? new UserId(value.Value) : null);

        builder.Property(a => a.SyncStatus)
            .HasConversion<string>()
            .HasDefaultValue(SyncStatus.NotSynced);

        builder.Property(a => a.AdditionalFields)
            .HasMaxLength(2000);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.SpecializationId);
        builder.HasIndex(a => new { a.UserId, a.StartDate, a.EndDate });
        builder.HasIndex(a => a.Type);
    }
}