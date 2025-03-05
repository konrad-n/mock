using System.Globalization;

namespace SledzSpecke.App.Converters
{
    /// <summary>
    /// Konwerter do ustawiania koloru tła przycisku zakładki w zależności od tego czy jest wybrana.
    /// </summary>
    public class StringMatchToBgColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string parameterValue)
            {
                var resources = Application.Current?.Resources;
                return stringValue.Equals(parameterValue, StringComparison.OrdinalIgnoreCase) ?
                    resources?["PrimaryColor"] ?? Colors.Transparent : Colors.Transparent;
            }

            return Colors.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}