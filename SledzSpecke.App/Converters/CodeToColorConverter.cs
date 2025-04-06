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
                    "A" => Color.FromArgb("#24C1DE"),
                    "B" => Color.FromArgb("#F59E0B"),
                    _ => Color.FromArgb("#547E9E"),
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
