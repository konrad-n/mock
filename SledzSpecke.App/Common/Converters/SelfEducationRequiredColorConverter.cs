using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class SelfEducationRequiredColorConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isRequired)
            {
                return isRequired ? new Color(8, 32, 68) : Colors.DarkGreen;
            }

            return new Color(84, 126, 158);
        }
    }
}