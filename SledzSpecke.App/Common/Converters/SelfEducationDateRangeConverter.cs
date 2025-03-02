using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    public class SelfEducationDateRangeConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SelfEducation education)
            {
                return $"{education.StartDate:dd.MM.yyyy} - {education.EndDate:dd.MM.yyyy} ({education.DurationDays} dni)";
            }

            return string.Empty;
        }
    }
}