using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class SelfEducationTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SelfEducationType type)
            {
                return type switch
                {
                    SelfEducationType.Conference => "Konferencja",
                    SelfEducationType.Workshop => "Warsztaty",
                    SelfEducationType.Course => "Kurs",
                    SelfEducationType.ScientificMeeting => "Spotkanie naukowe",
                    SelfEducationType.Publication => "Publikacja",
                    SelfEducationType.Other => "Inne",
                    _ => "Nieznany"
                };
            }
            return "Nieznany";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelfEducationTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SelfEducationType type)
            {
                return type switch
                {
                    SelfEducationType.Conference => new Color(13, 117, 156), // Niebieska
                    SelfEducationType.Workshop => new Color(76, 175, 80),    // Zielona
                    SelfEducationType.Course => new Color(255, 152, 0),      // Pomarańczowa
                    SelfEducationType.ScientificMeeting => new Color(156, 39, 176), // Fioletowa
                    SelfEducationType.Publication => new Color(3, 169, 244),  // Jasnoniebieska
                    SelfEducationType.Other => new Color(158, 158, 158),      // Szara
                    _ => Colors.Black
                };
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelfEducationRequiredColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRequired)
            {
                return isRequired ? new Color(8, 32, 68) : Colors.DarkGreen;
            }
            return new Color(84, 126, 158);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelfEducationRequiredTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRequired)
            {
                return isRequired ? "Wymagane w programie specjalizacji" : "Dodatkowe";
            }
            return "Nieznane";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelfEducationDateRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SelfEducation education)
            {
                return $"{education.StartDate:dd.MM.yyyy} - {education.EndDate:dd.MM.yyyy} ({education.DurationDays} dni)";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}