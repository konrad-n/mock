using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLowerInvariant() switch
                {
                    var s when s.Contains("zaliczon") => Colors.Green,
                    var s when s.Contains("uznan") => Colors.Purple,
                    var s when s.Contains("wymaga") => Colors.Orange,
                    var s when s.Contains("certyfikat") => Colors.Green,
                    var s when s.Contains("ukończon") => Colors.Green,
                    _ => Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black,
                };
            }

            return Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
