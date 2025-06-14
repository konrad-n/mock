using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class EducationalActivityConfiguration : IEntityTypeConfiguration<EducationalActivity>
{
    public void Configure(EntityTypeBuilder<EducationalActivity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new EducationalActivityId(x))
            .ValueGeneratedOnAdd();

        builder.Property(x => x.SpecializationId)
            .HasConversion(x => x.Value, x => new SpecializationId(x))
            .IsRequired();

        builder.Property(x => x.ModuleId)
            .HasConversion(x => x!.Value, x => new ModuleId(x));

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Title)
            .HasConversion(x => x.Value, x => new ActivityTitle(x))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(x => x!.Value, x => x != null ? new Description(x) : null)
            .HasMaxLength(2000);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.SyncStatus)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasIndex(x => x.SpecializationId);
        builder.HasIndex(x => x.ModuleId);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => new { x.StartDate, x.EndDate });
    }
}