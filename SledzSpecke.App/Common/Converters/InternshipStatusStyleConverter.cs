using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    public class InternshipStatusStyleConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                if (internship.IsCompleted)
                {
                    return "CompletedInternshipStyle";
                }

                bool isCurrentInternship = internship.StartDate.HasValue && !internship.EndDate.HasValue;

                if (isCurrentInternship)
                {
                    return "CurrentInternshipStyle";
                }

                if (internship.StartDate.HasValue)
                {
                    return "PlannedInternshipStyle";
                }

                return "PendingInternshipStyle";
            }

            return "PendingInternshipStyle";
        }
    }
}