using System;
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    /// <summary>
    /// Konwerter sprawdzający, czy ciąg znaków nie jest pusty ani null i zwracający odpowiednią wartość logiczną.
    /// </summary>
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return !string.IsNullOrEmpty(str);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}