using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class ItemCountToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count && int.TryParse(parameter?.ToString(), out int heightPerItem))
            {
                return count * heightPerItem;
            }

            // Domyślna wysokość
            return 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}