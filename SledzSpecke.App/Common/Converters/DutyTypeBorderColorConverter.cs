using SledzSpecke.Core.Models.Enums;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class DutyTypeBorderColorConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Independent ?
                    new Color(8, 32, 68) : // Ciemny niebieski
                    Colors.DarkGreen;
            }
            return Colors.LightGray;
        }
    }
}