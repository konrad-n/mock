using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Tak" : "Nie";
            }

            return "Nie";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.Equals("Tak", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}