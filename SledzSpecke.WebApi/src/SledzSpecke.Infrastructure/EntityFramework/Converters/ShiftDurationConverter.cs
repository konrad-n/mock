using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.EntityFramework.Converters;

public class ShiftDurationConverter : ValueConverter<ShiftDuration, int>
{
    public ShiftDurationConverter() : base(
        duration => duration.TotalMinutes,
        minutes => ShiftDuration.FromMinutes(minutes))
    { }
}