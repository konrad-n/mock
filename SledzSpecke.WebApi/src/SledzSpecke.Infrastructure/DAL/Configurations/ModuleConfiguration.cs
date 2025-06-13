using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ModuleId(x));

        builder.Property(x => x.SpecializationId)
            .HasConversion(x => x.Value, x => new SpecializationId(x));

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.SmkVersion)
            .HasConversion<int>();

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Structure)
            .IsRequired();

        builder.HasIndex(x => x.SpecializationId);
    }
}