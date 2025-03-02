using SledzSpecke.Core.Models;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class InternshipSupervisorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                bool isCurrentOrCompleted = internship.IsCompleted ||
                                          (internship.StartDate.HasValue && !internship.EndDate.HasValue);

                return !string.IsNullOrEmpty(internship.SupervisorName) || isCurrentOrCompleted;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}