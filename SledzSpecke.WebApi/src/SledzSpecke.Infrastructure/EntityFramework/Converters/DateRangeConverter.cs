using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SledzSpecke.Core.ValueObjects;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.EntityFramework.Converters;

public class DateRangeConverter : ValueConverter<DateRange, string>
{
    public DateRangeConverter() : base(
        dateRange => JsonSerializer.Serialize(new { Start = dateRange.StartDate, End = dateRange.EndDate }, (JsonSerializerOptions?)null),
        json => DeserializeDateRange(json))
    { }
    
    private static DateRange DeserializeDateRange(string json)
    {
        var data = JsonSerializer.Deserialize<DateRangeData>(json);
        return new DateRange(data!.Start, data.End);
    }
    
    private class DateRangeData
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}