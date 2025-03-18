using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Colors.Gray;
            }

            try
            {
                int selectedValue = (int)parameter;
                int currentValue = (int)value;

                return selectedValue == currentValue ? Colors.DarkBlue : Colors.LightSlateGray;
            }
            catch
            {
                return Colors.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}