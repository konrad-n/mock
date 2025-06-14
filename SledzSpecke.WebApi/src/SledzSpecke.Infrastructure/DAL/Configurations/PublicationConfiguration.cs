using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class PublicationConfiguration : IEntityTypeConfiguration<Publication>
{
    public void Configure(EntityTypeBuilder<Publication> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new PublicationId(value));

        builder.Property(p => p.SpecializationId)
            .HasConversion(id => id.Value, value => new SpecializationId(value));

        builder.Property(p => p.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.Property(p => p.Type)
            .HasConversion<string>();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Authors)
            .HasMaxLength(2000);

        builder.Property(p => p.Journal)
            .HasMaxLength(500);

        builder.Property(p => p.Publisher)
            .HasMaxLength(500);

        builder.Property(p => p.PublicationDate)
            .IsRequired();

        builder.Property(p => p.Volume)
            .HasMaxLength(50);

        builder.Property(p => p.Issue)
            .HasMaxLength(50);

        builder.Property(p => p.Pages)
            .HasMaxLength(50);

        builder.Property(p => p.DOI)
            .HasMaxLength(255);

        builder.Property(p => p.PMID)
            .HasMaxLength(50);

        builder.Property(p => p.ISBN)
            .HasMaxLength(50);

        builder.Property(p => p.URL)
            .HasMaxLength(1000);

        builder.Property(p => p.Abstract)
            .HasMaxLength(5000);

        builder.Property(p => p.Keywords)
            .HasMaxLength(1000);

        builder.Property(p => p.FilePath)
            .HasMaxLength(500);

        builder.Property(p => p.IsFirstAuthor)
            .HasDefaultValue(false);

        builder.Property(p => p.IsCorrespondingAuthor)
            .HasDefaultValue(false);

        builder.Property(p => p.IsPeerReviewed)
            .HasDefaultValue(false);

        builder.Property(p => p.SyncStatus)
            .HasConversion<string>()
            .HasDefaultValue(SyncStatus.NotSynced);

        builder.Property(p => p.AdditionalFields)
            .HasMaxLength(2000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.SpecializationId);
        builder.HasIndex(p => new { p.UserId, p.SpecializationId });
        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.PublicationDate);
        builder.HasIndex(p => p.IsFirstAuthor);
        builder.HasIndex(p => p.IsPeerReviewed);
    }
}