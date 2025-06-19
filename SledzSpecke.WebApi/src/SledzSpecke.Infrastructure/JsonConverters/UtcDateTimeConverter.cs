using System.Text.Json;
using System.Text.Json.Serialization;

namespace SledzSpecke.Infrastructure.JsonConverters;

/// <summary>
/// Custom JsonConverter to handle DateTime values as UTC
/// Ensures all incoming DateTime values are converted to UTC for PostgreSQL compatibility
/// </summary>
public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTime = reader.GetDateTime();
        
        // If DateTime Kind is Unspecified or Local, convert to UTC
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            // Treat unspecified dates as UTC
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        else if (dateTime.Kind == DateTimeKind.Local)
        {
            // Convert local time to UTC
            return dateTime.ToUniversalTime();
        }
        
        // Already UTC
        return dateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Always write as UTC
        writer.WriteStringValue(value.ToUniversalTime());
    }
}