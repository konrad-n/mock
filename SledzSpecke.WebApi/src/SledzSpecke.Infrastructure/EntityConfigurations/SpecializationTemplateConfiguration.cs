using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.SpecializationTemplates;

namespace SledzSpecke.Infrastructure.EntityConfigurations;

public class SpecializationTemplateConfiguration : IEntityTypeConfiguration<SpecializationTemplateDefinition>
{
    public void Configure(EntityTypeBuilder<SpecializationTemplateDefinition> builder)
    {
        builder.ToTable("SpecializationTemplates");

        builder.HasKey(st => st.Id);

        builder.Property(st => st.Id)
            .ValueGeneratedOnAdd();

        builder.Property(st => st.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(st => st.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(st => st.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(st => st.JsonContent)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(st => st.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(st => st.CreatedAt)
            .IsRequired();

        builder.Property(st => st.UpdatedAt)
            .IsRequired(false);

        // Unique constraint on Code + Version
        builder.HasIndex(st => new { st.Code, st.Version })
            .IsUnique()
            .HasDatabaseName("UQ_SpecializationTemplate_Code_Version");

        // Index for active templates
        builder.HasIndex(st => st.IsActive)
            .HasDatabaseName("IX_SpecializationTemplate_IsActive");
    }
}