using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAlternate && isAlternate)
            {
                return Color.FromArgb("#F5F5F5"); // Lekko szary kolor dla alternatywnych wierszy
            }

            return Colors.White; // Kolor dla standardowych wierszy
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
