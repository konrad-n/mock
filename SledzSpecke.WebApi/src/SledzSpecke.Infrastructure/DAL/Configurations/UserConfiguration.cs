using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new UserId(x))
            .ValueGeneratedNever();

        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, x => new Email(x))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasConversion(x => x.Value, x => new Username(x))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Password)
            .HasConversion(x => x.Value, x => new Password(x))
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasConversion(x => x.Value, x => new FullName(x))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SmkVersion)
            .HasConversion(x => x.Value, x => new SmkVersion(x));

        builder.Property(x => x.SpecializationId)
            .HasConversion(x => x.Value, x => new SpecializationId(x));

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Username).IsUnique();
    }
}