using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class AbsenceSpecializationAffectionConverter : IValueConverter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool affectsSpecialization)
            {
                return affectsSpecialization ? "Wydłuża specjalizację" : "";
            }
            return "";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}