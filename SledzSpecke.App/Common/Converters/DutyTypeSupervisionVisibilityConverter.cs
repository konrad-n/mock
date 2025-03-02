using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    public class DutyTypeSupervisionVisibilityConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Accompanied && !string.IsNullOrEmpty(parameter as string);
            }

            return false;
        }
    }
}
