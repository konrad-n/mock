using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class MedicalShiftConfiguration : IEntityTypeConfiguration<MedicalShift>
{
    public void Configure(EntityTypeBuilder<MedicalShift> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new Core.ValueObjects.MedicalShiftId(x));

        builder.Property(x => x.InternshipId)
            .HasConversion(x => x.Value, x => new Core.ValueObjects.InternshipId(x));

        builder.Property(x => x.Location)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SyncStatus)
            .HasConversion<int>();

        builder.Property(x => x.ApproverName)
            .HasMaxLength(100);

        builder.Property(x => x.ApproverRole)
            .HasMaxLength(100);

        builder.HasIndex(x => x.InternshipId);
        builder.HasIndex(x => x.Date);

        builder.Ignore(x => x.IsApproved);
        builder.Ignore(x => x.CanBeDeleted);
        
        // Configure Duration as an owned entity
        builder.OwnsOne(x => x.Duration, duration =>
        {
            duration.Property(d => d.Hours)
                .HasColumnName("Hours")
                .IsRequired();
            duration.Property(d => d.Minutes)
                .HasColumnName("Minutes")
                .IsRequired();
            duration.Ignore(d => d.TotalMinutes);
        });
    }
}