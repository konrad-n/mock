using SledzSpecke.Core.Models.Enums;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class DutyTypeColorConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Independent ?
                    new Color(8, 32, 68) : // Ciemny niebieski
                    new Color(0, 100, 0);  // Ciemny zielony
            }
            return Colors.Black;
        }
    }
}