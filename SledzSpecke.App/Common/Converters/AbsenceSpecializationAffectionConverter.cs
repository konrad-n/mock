using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class AbsenceSpecializationAffectionConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool affectsSpecialization)
            {
                return affectsSpecialization ? "Wydluza specjalizacje" : string.Empty;
            }

            return string.Empty;
        }
    }
}
