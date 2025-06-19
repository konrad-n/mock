using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class ProcedureRequirementConfiguration : IEntityTypeConfiguration<ProcedureRequirement>
{
    public void Configure(EntityTypeBuilder<ProcedureRequirement> builder)
    {
        builder.ToTable("ProcedureRequirements");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ProcedureRequirementId(value))
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
            
        builder.Property(x => x.ModuleId)
            .HasConversion(
                id => id.Value,
                value => new ModuleId(value))
            .HasColumnName("ModuleId")
            .IsRequired();
            
        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(x => x.Name)
            .HasMaxLength(500)
            .IsRequired();
            
        builder.Property(x => x.RequiredAsOperator)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(x => x.RequiredAsAssistant)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);
            
        // Relationships
        builder.HasOne(x => x.Module)
            .WithMany()
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Realizations)
            .WithOne(x => x.Requirement)
            .HasForeignKey(x => x.RequirementId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(x => x.ModuleId);
        builder.HasIndex(x => new { x.ModuleId, x.Code }).IsUnique();
    }
}