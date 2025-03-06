using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class CodeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string code)
            {
                return code switch
                {
                    "A" => Color.FromHex("#24C1DE"),  // Primary color for operator
                    "B" => Color.FromHex("#F59E0B"),  // Warning color for assistant
                    _ => Color.FromHex("#547E9E"),    // Default text-muted color
                };
            }

            return Color.FromHex("#547E9E");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
