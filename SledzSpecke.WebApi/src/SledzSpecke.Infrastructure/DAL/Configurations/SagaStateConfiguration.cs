using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Infrastructure.Sagas;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

public class SagaStateConfiguration : IEntityTypeConfiguration<SagaStateEntity>
{
    public void Configure(EntityTypeBuilder<SagaStateEntity> builder)
    {
        builder.ToTable("SagaStates");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Type)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.Property(s => s.State)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(2000);
            
        builder.Property(s => s.Data)
            .HasColumnType("jsonb");
            
        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.State);
        builder.HasIndex(s => s.CreatedAt);
        
        builder.HasMany(s => s.Steps)
            .WithOne(step => step.Saga)
            .HasForeignKey(step => step.SagaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SagaStepConfiguration : IEntityTypeConfiguration<SagaStepEntity>
{
    public void Configure(EntityTypeBuilder<SagaStepEntity> builder)
    {
        builder.ToTable("SagaSteps");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(2000);
            
        builder.HasIndex(s => new { s.SagaId, s.Name });
        builder.HasIndex(s => s.Status);
    }
}