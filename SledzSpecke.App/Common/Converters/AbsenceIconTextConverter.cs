using SledzSpecke.Core.Models;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class AbsenceIconTextConverter : IValueConverter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AbsenceType type)
            {
                return type switch
                {
                    AbsenceType.SickLeave => "🤒",
                    AbsenceType.VacationLeave => "🏖️",
                    AbsenceType.SelfEducationLeave => "📚",
                    AbsenceType.MaternityLeave => "👶",
                    AbsenceType.ParentalLeave => "👶",
                    _ => "📅"
                };
            }
            return "📅";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}