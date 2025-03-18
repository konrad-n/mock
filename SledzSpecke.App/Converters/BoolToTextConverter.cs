using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
            {
                return string.Empty;
            }

            if (parameter is not string textParameter)
            {
                return boolValue ? "Tak" : "Nie";
            }

            var parts = textParameter.Split(',');
            if (parts.Length < 2)
            {
                return boolValue ? "Tak" : "Nie";
            }

            return boolValue ? parts[0] : parts[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}