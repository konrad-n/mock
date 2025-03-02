using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class StringToBoolConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }
    }
}
