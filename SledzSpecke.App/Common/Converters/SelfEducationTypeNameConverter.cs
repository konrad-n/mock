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
}