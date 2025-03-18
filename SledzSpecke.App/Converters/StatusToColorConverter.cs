using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                if (status.Contains("Ukończon") || status.Contains("Zatwierdzon"))
                {
                    return Colors.Green;
                }
                else if (status.Contains("Oczekując") || status.Contains("Wymaga"))
                {
                    return Colors.Orange;
                }
            }

            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}