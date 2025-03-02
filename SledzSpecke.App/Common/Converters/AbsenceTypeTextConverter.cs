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
                    AbsenceType.SelfEducationLeave => "Urlop szkoleniowy (samoksztalcenie)",
                    AbsenceType.MaternityLeave => "Urlop macierzynski",
                    AbsenceType.ParentalLeave => "Urlop rodzicielski",
                    AbsenceType.SpecialLeave => "Urlop okolicznosciowy",
                    AbsenceType.UnpaidLeave => "Urlop bezplatny",
                    AbsenceType.Other => "Inna nieobecnosc",
                    _ => "Nieobecnosc"
                };
            }

            return "Nieobecnosc";
        }
    }
}
