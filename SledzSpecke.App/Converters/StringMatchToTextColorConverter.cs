using System.Globalization;

namespace SledzSpecke.App.Converters
{
    /// <summary>
    /// Konwerter do ustawiania koloru tekstu przycisku zakładki w zależności od tego czy jest wybrana.
    /// </summary>
    public class StringMatchToTextColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string parameterValue)
            {
                var resources = Application.Current?.Resources;
                return stringValue.Equals(parameterValue, StringComparison.OrdinalIgnoreCase) ?
                    Colors.White : resources?["PrimaryColor"] ?? Colors.Black;
            }

            return Application.Current?.Resources["PrimaryColor"] ?? Colors.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}