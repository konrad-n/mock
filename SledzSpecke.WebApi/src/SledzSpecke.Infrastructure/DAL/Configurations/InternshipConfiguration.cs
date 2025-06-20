using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class InternshipConfiguration : IEntityTypeConfiguration<Internship>
{
    public void Configure(EntityTypeBuilder<Internship> builder)
    {
        builder.HasKey(x => x.InternshipId);
        builder.Property(x => x.InternshipId)
            .HasColumnName("Id");

        builder.Property(x => x.SpecializationId)
            .IsRequired();

        builder.Property(x => x.ModuleId);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.InstitutionName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DepartmentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SupervisorName)
            .HasMaxLength(200);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.DaysCount)
            .IsRequired();

        builder.Property(x => x.PlannedWeeks)
            .IsRequired();

        builder.Property(x => x.PlannedDays)
            .IsRequired();

        builder.Property(x => x.CompletedDays)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsCompleted)
            .IsRequired();

        builder.Property(x => x.IsApproved)
            .IsRequired();

        builder.Property(x => x.ApproverName)
            .HasMaxLength(200);

        builder.Property(x => x.SyncStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Configure navigation properties
        builder.HasMany(x => x.MedicalShifts)
            .WithOne()
            .HasForeignKey("InternshipId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Procedures)
            .WithOne()
            .HasForeignKey(p => p.InternshipId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SpecializationId);
        builder.HasIndex(x => x.ModuleId);
        builder.HasIndex(x => new { x.StartDate, x.EndDate });
    }
}