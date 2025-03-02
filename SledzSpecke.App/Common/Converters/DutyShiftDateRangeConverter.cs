using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class DutyShiftDateRangeConverter : IValueConverter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime startDate && parameter is DateTime endDate)
            {
                return $"{startDate:dd.MM.yyyy HH:mm} - {endDate:dd.MM.yyyy HH:mm}";
            }
            return string.Empty;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}