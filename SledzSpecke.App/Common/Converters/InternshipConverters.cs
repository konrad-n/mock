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

    public class InternshipStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                if (internship.IsCompleted)
                    return Colors.Green;

                bool isCurrentInternship = internship.StartDate.HasValue && !internship.EndDate.HasValue;

                if (isCurrentInternship)
                    return Colors.Blue;

                if (internship.StartDate.HasValue)
                    return Colors.Orange;

                return new Color(84, 126, 158);
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InternshipStatusStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                if (internship.IsCompleted)
                    return "CompletedInternshipStyle";

                bool isCurrentInternship = internship.StartDate.HasValue && !internship.EndDate.HasValue;

                if (isCurrentInternship)
                    return "CurrentInternshipStyle";

                if (internship.StartDate.HasValue)
                    return "PlannedInternshipStyle";

                return "PendingInternshipStyle";
            }
            return "PendingInternshipStyle";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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