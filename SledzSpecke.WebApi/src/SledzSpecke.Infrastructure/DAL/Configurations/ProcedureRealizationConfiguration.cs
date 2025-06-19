using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class ProcedureRealizationConfiguration : IEntityTypeConfiguration<ProcedureRealization>
{
    public void Configure(EntityTypeBuilder<ProcedureRealization> builder)
    {
        builder.ToTable("ProcedureRealizations");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ProcedureRealizationId(value))
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
            
        builder.Property(x => x.RequirementId)
            .HasConversion(
                id => id.Value,
                value => new ProcedureRequirementId(value))
            .HasColumnName("RequirementId")
            .IsRequired();
            
        builder.Property(x => x.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .HasColumnName("UserId")
            .IsRequired();
            
        builder.Property(x => x.Date)
            .HasColumnType("date")
            .IsRequired();
            
        builder.Property(x => x.Location)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(x => x.Role)
            .HasConversion<int>()
            .IsRequired();
            
        builder.Property(x => x.Year)
            .IsRequired(false);
            
        builder.Property(x => x.CreatedAt)
            .IsRequired();
            
        // Relationships
        builder.HasOne(x => x.Requirement)
            .WithMany(x => x.Realizations)
            .HasForeignKey(x => x.RequirementId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(x => x.RequirementId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Date);
        builder.HasIndex(x => new { x.UserId, x.RequirementId, x.Date });
        
        // Check constraints
        builder.ToTable(tb => tb.HasCheckConstraint("CK_ProcedureRealizations_Year", 
            "\"Year\" IS NULL OR (\"Year\" >= 1 AND \"Year\" <= 6)"));
    }
}