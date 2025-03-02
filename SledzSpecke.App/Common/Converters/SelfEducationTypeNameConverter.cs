using SledzSpecke.Core.Models.Enums;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class SelfEducationTypeNameConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
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
    }
}