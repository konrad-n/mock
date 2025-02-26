using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive && isActive)
            {
                // Zwróć kolor podświetlenia dla aktywnego przycisku
                return Color.FromArgb("#1E88E5"); // Primary color
            }

            // Domyślny kolor dla nieaktywnego przycisku
            return Color.FromArgb("#808080"); // Gray color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
