using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.Persistence.EntityTypeConfigurations;

public class AdditionalSelfEducationDaysConfiguration : IEntityTypeConfiguration<AdditionalSelfEducationDays>
{
    public void Configure(EntityTypeBuilder<AdditionalSelfEducationDays> builder)
    {
        builder.ToTable("AdditionalSelfEducationDays");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.ModuleId)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new ModuleId(v));

        builder.Property(x => x.InternshipId)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new InternshipId(v));

        builder.Property(x => x.StartDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.EndDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.NumberOfDays)
            .IsRequired();

        builder.Property(x => x.Purpose)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.EventName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.Organizer)
            .HasMaxLength(200);

        builder.Property(x => x.IsApproved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.ModuleId)
            .HasDatabaseName("IX_AdditionalSelfEducationDays_ModuleId");

        builder.HasIndex(x => x.InternshipId)
            .HasDatabaseName("IX_AdditionalSelfEducationDays_InternshipId");

        builder.HasIndex(x => new { x.StartDate, x.EndDate })
            .HasDatabaseName("IX_AdditionalSelfEducationDays_Dates");
    }
}