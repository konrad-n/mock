using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
{
    public void Configure(EntityTypeBuilder<Procedure> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ProcedureId(x));

        builder.Property(x => x.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.OperatorCode)
            .HasMaxLength(10);

        builder.Property(x => x.PerformingPerson)
            .HasMaxLength(100);

        builder.Property(x => x.Location)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PatientInitials)
            .HasMaxLength(10);

        builder.Property(x => x.PatientGender)
            .HasMaxLength(1);

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.SyncStatus)
            .HasConversion<int>();

        builder.HasIndex(x => x.InternshipId);
        builder.HasIndex(x => x.Code);
        builder.HasIndex(x => x.Date);

        builder.Ignore(x => x.IsCompleted);
        builder.Ignore(x => x.CanBeModified);
    }
}