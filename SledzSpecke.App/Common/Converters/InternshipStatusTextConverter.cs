using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    public class InternshipStatusTextConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                if (internship.IsCompleted)
                {
                    return "Ukończony";
                }

                bool isCurrentInternship = internship.StartDate.HasValue && !internship.EndDate.HasValue;

                if (isCurrentInternship)
                {
                    return $"W trakcie od: {internship.StartDate?.ToString("dd.MM.yyyy")}";
                }

                if (internship.StartDate.HasValue)
                {
                    return $"Zaplanowany na: {internship.StartDate?.ToString("dd.MM.yyyy")}";
                }

                return "Oczekujący";
            }

            return string.Empty;
        }
    }
}