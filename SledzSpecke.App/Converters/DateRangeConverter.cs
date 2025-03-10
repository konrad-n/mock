using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class DateRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Models.RealizedProcedureNewSMK procedure)
            {
                return string.Empty;
            }

            return $"{procedure.StartDate:dd.MM.yyyy} - {procedure.EndDate:dd.MM.yyyy}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}