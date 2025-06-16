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

        builder.Property(x => x.Password)
            .HasConversion(x => x.Value, x => new HashedPassword(x))
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasConversion(x => x.Value, x => new FirstName(x))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasConversion(x => x.Value, x => new LastName(x))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Pesel)
            .HasConversion(x => x.Value, x => new Pesel(x))
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(x => x.PwzNumber)
            .HasConversion(x => x.Value, x => new PwzNumber(x))
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasConversion(x => x.Value, x => new PhoneNumber(x))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .IsRequired();

        builder.OwnsOne(x => x.CorrespondenceAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).IsRequired();
            address.Property(a => a.HouseNumber).HasMaxLength(50).IsRequired();
            address.Property(a => a.ApartmentNumber).HasMaxLength(50);
            address.Property(a => a.PostalCode).HasMaxLength(10).IsRequired();
            address.Property(a => a.City).HasMaxLength(100).IsRequired();
            address.Property(a => a.Province).HasMaxLength(100).IsRequired();
        });

        builder.Property(x => x.RegistrationDate)
            .IsRequired();

        builder.Property(x => x.NotificationsEnabled)
            .IsRequired();

        builder.Property(x => x.EmailNotificationsEnabled)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastLoginAt);

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Pesel).IsUnique();
        builder.HasIndex(x => x.PwzNumber).IsUnique();
    }
}