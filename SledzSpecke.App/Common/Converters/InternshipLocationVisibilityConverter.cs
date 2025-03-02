using SledzSpecke.Core.Models;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class InternshipLocationVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                bool isCurrentOrCompleted = internship.IsCompleted ||
                                          (internship.StartDate.HasValue && !internship.EndDate.HasValue);

                return !string.IsNullOrEmpty(internship.Location) || isCurrentOrCompleted;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}