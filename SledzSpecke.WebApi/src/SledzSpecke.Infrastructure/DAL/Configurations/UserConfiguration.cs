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
            .HasConversion(x => x.Value, x => new HashedPassword(x))
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasConversion(x => x.Value, x => new FullName(x))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SmkVersion)
            .HasConversion(x => x.Value, x => new SmkVersion(x));

        builder.Property(x => x.SpecializationId)
            .HasConversion(x => x.Value, x => new SpecializationId(x));

        builder.Property(x => x.PhoneNumber)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? new PhoneNumber(x) : null)
            .HasMaxLength(20);

        builder.Property(x => x.Bio)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? new UserBio(x) : null)
            .HasMaxLength(1000);

        builder.Property(x => x.ProfilePicturePath)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? new FilePath(x) : null)
            .HasMaxLength(500);

        builder.Property(x => x.PreferredLanguage)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? new Language(x) : null)
            .HasMaxLength(2);

        builder.Property(x => x.PreferredTheme)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? new Theme(x) : null)
            .HasMaxLength(10);

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Username).IsUnique();
    }
}