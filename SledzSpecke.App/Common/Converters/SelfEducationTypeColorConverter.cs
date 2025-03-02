using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    public class SelfEducationTypeColorConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SelfEducationType type)
            {
                return type switch
                {
                    SelfEducationType.Conference => new Color(13, 117, 156),
                    SelfEducationType.Workshop => new Color(76, 175, 80),
                    SelfEducationType.Course => new Color(255, 152, 0),
                    SelfEducationType.ScientificMeeting => new Color(156, 39, 176),
                    SelfEducationType.Publication => new Color(3, 169, 244),
                    SelfEducationType.Other => new Color(158, 158, 158), 
                    _ => Colors.Black
                };
            }

            return Colors.Black;
        }
    }
}
