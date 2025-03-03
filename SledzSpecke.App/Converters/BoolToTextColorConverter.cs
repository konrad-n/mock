using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected ? Colors.White : Colors.Parse("#082044");
            }

            return Colors.Parse("#082044");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
