using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new SpecializationId(x));

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ProgramCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.SmkVersion)
            .HasConversion(x => x.Value, x => new SmkVersion(x));

        builder.Property(x => x.CurrentModuleId)
            .HasConversion(x => x!.Value, x => new ModuleId(x))
            .IsRequired(false);

        builder.Property(x => x.ProgramStructure)
            .IsRequired();

        builder.HasMany(x => x.Modules)
            .WithOne()
            .HasForeignKey(x => x.SpecializationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}