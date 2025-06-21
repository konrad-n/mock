using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Password)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .IsRequired();

        builder.Property(x => x.CorrespondenceAddress)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.RegistrationDate)
            .IsRequired();

        builder.Property(x => x.NotificationsEnabled)
            .IsRequired();

        builder.Property(x => x.EmailNotificationsEnabled)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastLoginAt);

        builder.Property(x => x.SmkVersion)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.SyncStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();
    }
}