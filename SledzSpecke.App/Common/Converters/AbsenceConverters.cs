using SledzSpecke.Core.Models;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class AbsenceTypeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AbsenceCardColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AbsenceIconTextConverter : IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AbsenceDateRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Absence absence)
            {
                return $"{absence.StartDate:dd.MM.yyyy} - {absence.EndDate:dd.MM.yyyy} ({absence.DurationDays} dni)";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AbsenceSpecializationAffectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool affectsSpecialization)
            {
                return affectsSpecialization ? "Wydłuża specjalizację" : "";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}