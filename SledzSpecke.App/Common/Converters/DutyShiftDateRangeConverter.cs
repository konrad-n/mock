using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class DutyShiftDateRangeConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime startDate && parameter is DateTime endDate)
            {
                return $"{startDate:dd.MM.yyyy HH:mm} - {endDate:dd.MM.yyyy HH:mm}";
            }

            return string.Empty;
        }
    }
}
