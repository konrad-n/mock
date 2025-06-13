using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new CourseId(x));

        builder.Property(x => x.SpecializationId)
            .HasConversion(x => x.Value, x => new SpecializationId(x))
            .IsRequired();

        builder.Property(x => x.ModuleId)
            .HasConversion(x => x != null ? x.Value : (int?)null, 
                           x => x.HasValue ? new ModuleId(x.Value) : null);

        builder.Property(x => x.CourseType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CourseName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.CourseNumber)
            .HasMaxLength(100);

        builder.Property(x => x.InstitutionName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CompletionDate)
            .IsRequired();

        builder.Property(x => x.HasCertificate)
            .IsRequired();

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(100);

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

        builder.HasIndex(x => x.SpecializationId);
        builder.HasIndex(x => x.ModuleId);
        builder.HasIndex(x => x.CourseType);
        builder.HasIndex(x => x.CompletionDate);
    }
}