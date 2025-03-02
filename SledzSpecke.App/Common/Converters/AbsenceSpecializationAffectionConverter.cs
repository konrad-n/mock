using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
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