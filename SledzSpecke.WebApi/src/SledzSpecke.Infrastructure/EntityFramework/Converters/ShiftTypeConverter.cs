using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.EntityFramework.Converters;

public class ShiftTypeConverter : ValueConverter<ShiftType, string>
{
    public ShiftTypeConverter() : base(
        shiftType => shiftType.ToString(),
        value => Enum.Parse<ShiftType>(value))
    { }
}