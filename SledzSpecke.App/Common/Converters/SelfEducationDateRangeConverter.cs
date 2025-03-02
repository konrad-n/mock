using SledzSpecke.Core.Models;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class SelfEducationDateRangeConverter : IValueConverter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SelfEducation education)
            {
                return $"{education.StartDate:dd.MM.yyyy} - {education.EndDate:dd.MM.yyyy} ({education.DurationDays} dni)";
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