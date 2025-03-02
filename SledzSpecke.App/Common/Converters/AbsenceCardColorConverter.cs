using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    public class AbsenceCardColorConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is AbsenceType type)
            {
                return type switch
                {
                    AbsenceType.SickLeave => Color.FromArgb("#FFE0E0"),
                    AbsenceType.VacationLeave => Color.FromArgb("#E0F7FA"),
                    AbsenceType.SelfEducationLeave => Color.FromArgb("#E8F5E9"),
                    AbsenceType.MaternityLeave => Color.FromArgb("#FFF8E1"),
                    AbsenceType.ParentalLeave => Color.FromArgb("#FFF8E1"),
                    _ => Color.FromArgb("#F5F5F5")
                };
            }
            return Color.FromArgb("#F5F5F5");
        }
    }
}