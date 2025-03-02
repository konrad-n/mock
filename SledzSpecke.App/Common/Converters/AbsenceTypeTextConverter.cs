using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    public class AbsenceTypeTextConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is AbsenceType type)
            {
                return type switch
                {
                    AbsenceType.SickLeave => "Zwolnienie lekarskie (L4)",
                    AbsenceType.VacationLeave => "Urlop wypoczynkowy",
                    AbsenceType.SelfEducationLeave => "Urlop szkoleniowy (samokształcenie)",
                    AbsenceType.MaternityLeave => "Urlop macierzyński",
                    AbsenceType.ParentalLeave => "Urlop rodzicielski",
                    AbsenceType.SpecialLeave => "Urlop okolicznościowy",
                    AbsenceType.UnpaidLeave => "Urlop bezpłatny",
                    AbsenceType.Other => "Inna nieobecność",
                    _ => "Nieobecność"
                };
            }

            return "Nieobecność";
        }
    }
}