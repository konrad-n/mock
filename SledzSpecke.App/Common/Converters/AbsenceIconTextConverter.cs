using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    public class AbsenceIconTextConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
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
    }
}