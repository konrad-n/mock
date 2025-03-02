using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    public class InternshipStatusColorConverter : BaseConverter
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                if (internship.IsCompleted)
                {
                    return Colors.Green;
                }

                bool isCurrentInternship = internship.StartDate.HasValue && !internship.EndDate.HasValue;

                if (isCurrentInternship)
                {
                    return Colors.Blue;
                }

                if (internship.StartDate.HasValue)
                {
                    return Colors.Orange;
                }

                return new Color(84, 126, 158);
            }

            return Colors.Gray;
        }
    }
}
