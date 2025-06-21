using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.HasKey(x => x.SpecializationId);
        builder.Property(x => x.SpecializationId)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ProgramCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.SmkVersion)
            .HasConversion<string>();

        builder.Property(x => x.CurrentModuleId)
            .IsRequired(false);

        builder.Property(x => x.ProgramStructure)
            .IsRequired();

        builder.HasMany(x => x.Modules)
            .WithOne()
            .HasForeignKey(x => x.SpecializationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}