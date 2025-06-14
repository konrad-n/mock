using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class RecognitionConfiguration : IEntityTypeConfiguration<Recognition>
{
    public void Configure(EntityTypeBuilder<Recognition> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => new RecognitionId(value));

        builder.Property(r => r.SpecializationId)
            .HasConversion(id => id.Value, value => new SpecializationId(value));

        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.Property(r => r.Type)
            .HasConversion<string>();

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.Property(r => r.Institution)
            .HasMaxLength(500);

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.EndDate)
            .IsRequired();

        builder.Property(r => r.DaysReduction)
            .IsRequired();

        builder.Property(r => r.IsApproved)
            .HasDefaultValue(false);

        builder.Property(r => r.ApprovedAt);

        builder.Property(r => r.ApprovedBy)
            .HasConversion(id => id != null ? (int?)id.Value : null,
                          value => value.HasValue ? new UserId(value.Value) : null);

        builder.Property(r => r.DocumentPath)
            .HasMaxLength(500);

        builder.Property(r => r.SyncStatus)
            .HasConversion<string>()
            .HasDefaultValue(SyncStatus.NotSynced);

        builder.Property(r => r.AdditionalFields)
            .HasMaxLength(2000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.SpecializationId);
        builder.HasIndex(r => new { r.UserId, r.SpecializationId });
        builder.HasIndex(r => r.Type);
        builder.HasIndex(r => r.IsApproved);
    }
}