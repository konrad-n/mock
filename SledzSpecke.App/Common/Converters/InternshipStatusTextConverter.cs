using SledzSpecke.Core.Models;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class InternshipStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                if (internship.IsCompleted)
                    return "Ukończony";

                bool isCurrentInternship = internship.StartDate.HasValue && !internship.EndDate.HasValue;

                if (isCurrentInternship)
                    return $"W trakcie od: {internship.StartDate?.ToString("dd.MM.yyyy")}";

                if (internship.StartDate.HasValue)
                    return $"Zaplanowany na: {internship.StartDate?.ToString("dd.MM.yyyy")}";

                return "Oczekujący";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}