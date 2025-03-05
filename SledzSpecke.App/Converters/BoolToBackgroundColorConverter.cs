using System.Globalization;

namespace SledzSpecke.App.Converters
{
    /// <summary>
    /// Konwerter do ustawiania koloru tła wiersza w zależności od wartości logicznej.
    /// </summary>
    public class BoolToBackgroundColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Colors.WhiteSmoke : Colors.White;
            }

            return Colors.White;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}