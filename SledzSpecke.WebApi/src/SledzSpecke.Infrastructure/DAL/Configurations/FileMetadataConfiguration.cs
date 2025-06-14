using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Configurations;

internal sealed class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
{
    public void Configure(EntityTypeBuilder<FileMetadata> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255)
            .HasConversion(
                fileName => fileName.Value,
                value => new FileName(value));
        
        builder.Property(f => f.FilePath)
            .IsRequired()
            .HasMaxLength(500)
            .HasConversion(
                filePath => filePath.Value,
                value => new FilePath(value));
        
        builder.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                contentType => contentType.Value,
                value => new ContentType(value));
        
        builder.Property(f => f.FileSize)
            .IsRequired()
            .HasConversion(
                fileSize => fileSize.Bytes,
                value => new FileSize(value));
        
        builder.Property(f => f.UploadedByUserId)
            .IsRequired()
            .HasConversion(
                userId => userId.Value,
                value => new UserId(value));
        
        builder.Property(f => f.UploadedAt)
            .IsRequired();
        
        builder.Property(f => f.Description)
            .HasMaxLength(500);
        
        builder.Property(f => f.EntityType)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(f => f.EntityId)
            .IsRequired();
        
        builder.Property(f => f.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(f => f.DeletedAt);
        
        // Indexes
        builder.HasIndex(f => new { f.EntityType, f.EntityId })
            .HasFilter("\"IsDeleted\" = false");
        
        builder.HasIndex(f => f.UploadedByUserId)
            .HasFilter("\"IsDeleted\" = false");
        
        builder.HasIndex(f => f.FilePath)
            .IsUnique();
    }
}