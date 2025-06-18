using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SledzSpecke.Core.Outbox;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.Outbox.Data;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", "outbox");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Type)
            .HasMaxLength(256)
            .IsRequired();
            
        builder.Property(x => x.Data)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) 
                     ?? new Dictionary<string, object>()
            );
            
        builder.Property(x => x.OccurredAt)
            .IsRequired();
            
        builder.Property(x => x.ProcessedAt);
        
        builder.Property(x => x.Error)
            .HasMaxLength(2048);
            
        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);
            
        // Indexes for performance
        builder.HasIndex(x => x.ProcessedAt)
            .HasFilter("\"ProcessedAt\" IS NULL")
            .HasDatabaseName("IX_OutboxMessages_Unprocessed");
            
        builder.HasIndex(x => x.OccurredAt);
        builder.HasIndex(x => x.Type);
    }
}