using System.Globalization;

namespace SledzSpecke.App.Converters
{
    /// <summary>
    /// Konwerter zmieniający wartość logiczną na tekst.
    /// Pozwala wybrać różny tekst dla wartości true i false.
    /// Parametr konwersji powinien mieć format "TextForTrue,TextForFalse".
    /// </summary>
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