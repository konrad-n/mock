using SledzSpecke.Core.Models.Enums;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class DutyTypeSupervisionVisibilityConverter : IValueConverter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Accompanied && !string.IsNullOrEmpty(parameter as string);
            }
            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}