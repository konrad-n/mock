using SledzSpecke.Core.Models.Enums;
using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    public class DutyTypeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Independent ? "Samodzielny" : "Towarzyszący";
            }
            return "Nieznany";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DutyTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Independent ?
                    new Color(8, 32, 68) : // Ciemny niebieski
                    new Color(0, 100, 0);  // Ciemny zielony
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DutyTypeBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Independent ?
                    new Color(8, 32, 68) : // Ciemny niebieski
                    Colors.DarkGreen;
            }
            return Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DutyTypeSupervisionVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Accompanied && !string.IsNullOrEmpty(parameter as string);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DutyShiftDateRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime startDate && parameter is DateTime endDate)
            {
                return $"{startDate:dd.MM.yyyy HH:mm} - {endDate:dd.MM.yyyy HH:mm}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}