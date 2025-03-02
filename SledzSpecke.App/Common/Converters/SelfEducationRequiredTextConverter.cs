using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class SelfEducationRequiredTextConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isRequired)
            {
                return isRequired ? "Wymagane w programie specjalizacji" : "Dodatkowe";
            }

            return "Nieznane";
        }
    }
}