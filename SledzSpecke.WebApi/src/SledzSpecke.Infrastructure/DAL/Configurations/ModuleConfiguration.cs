using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.HasKey(x => x.ModuleId);
        builder.Property(x => x.ModuleId)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.SpecializationId)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>();

        builder.Property(x => x.SmkVersion)
            .HasConversion<string>();

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Structure)
            .IsRequired();

        builder.HasIndex(x => x.SpecializationId);
        
        // Navigation properties
        builder.HasMany(x => x.Procedures)
            .WithOne(x => x.Module)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(x => x.SelfEducations)
            .WithOne(x => x.Module)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(x => x.Courses)
            .WithOne(x => x.Module)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}