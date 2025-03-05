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
                    "A" => Color.FromArgb("#24C1DE"),  // Primary color for operator
                    "B" => Color.FromArgb("#F59E0B"),  // Warning color for assistant
                    _ => Color.FromArgb("#547E9E"),    // Default text-muted color
                };
            }

            return Color.FromArgb("#547E9E");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
