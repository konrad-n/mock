using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class MedicalShiftConfiguration : IEntityTypeConfiguration<MedicalShift>
{
    public void Configure(EntityTypeBuilder<MedicalShift> builder)
    {
        builder.HasKey(x => x.ShiftId);

        builder.Property(x => x.ShiftId)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.InternshipId)
            .IsRequired();

        builder.Property(x => x.ModuleId)
            .IsRequired(false);

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

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
    }
}