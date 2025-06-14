using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class ProcedureBaseConfiguration : IEntityTypeConfiguration<ProcedureBase>
{
    public void Configure(EntityTypeBuilder<ProcedureBase> builder)
    {
        builder.ToTable("Procedures");
        
        // Configure inheritance using discriminator
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<Procedure>("Procedure")
            .HasValue<ProcedureOldSmk>("ProcedureOldSmk")
            .HasValue<ProcedureNewSmk>("ProcedureNewSmk");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ProcedureId(x));

        builder.Property(x => x.InternshipId)
            .HasConversion(x => x.Value, x => new InternshipId(x));

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.OperatorCode)
            .HasMaxLength(20);

        builder.Property(x => x.PerformingPerson)
            .HasMaxLength(100);

        builder.Property(x => x.Location)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PatientInitials)
            .HasMaxLength(10);

        builder.Property(x => x.PatientGender)
            .HasMaxLength(1);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.SyncStatus)
            .HasConversion<int>();

        builder.Property(x => x.SmkVersion)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(x => x.AssistantData)
            .HasMaxLength(500);

        builder.Property(x => x.ProcedureGroup)
            .HasMaxLength(100);

        builder.Property(x => x.AdditionalFields);

        builder.HasIndex(x => x.InternshipId);
        builder.HasIndex(x => x.Code);
        builder.HasIndex(x => x.Date);

        builder.Ignore(x => x.IsCompleted);
        builder.Ignore(x => x.CanBeModified);
        builder.Ignore(x => x.IsApproved);
        builder.Ignore(x => x.IsTypeA);
        builder.Ignore(x => x.IsTypeB);
    }
}

internal sealed class ProcedureOldSmkConfiguration : IEntityTypeConfiguration<ProcedureOldSmk>
{
    public void Configure(EntityTypeBuilder<ProcedureOldSmk> builder)
    {
        builder.Property(x => x.ProcedureRequirementId);
        builder.Property(x => x.InternshipName)
            .HasMaxLength(200);
    }
}

internal sealed class ProcedureNewSmkConfiguration : IEntityTypeConfiguration<ProcedureNewSmk>
{
    public void Configure(EntityTypeBuilder<ProcedureNewSmk> builder)
    {
        builder.Property(x => x.ModuleId)
            .HasConversion(x => x!.Value, x => new ModuleId(x));
        
        builder.Property(x => x.ProcedureRequirementId)
            .IsRequired();
        
        builder.Property(x => x.ProcedureName)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(x => x.CountA);
        builder.Property(x => x.CountB);
        
        builder.Property(x => x.Supervisor)
            .HasMaxLength(100);
        
        builder.Property(x => x.Institution)
            .HasMaxLength(200);
        
        builder.Property(x => x.Comments)
            .HasMaxLength(1000);
    }
}