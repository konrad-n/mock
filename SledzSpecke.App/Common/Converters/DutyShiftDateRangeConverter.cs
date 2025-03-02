// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DutyShiftDateRangeConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter zakresu dat dyżuru na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter zakresu dat dyżuru na tekst.
    /// </summary>
    public class DutyShiftDateRangeConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje daty dyżuru na tekst z informacją o zakresie dat i godzinach.
        /// </summary>
        /// <param name="value">Data rozpoczęcia dyżuru.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Data zakończenia dyżuru.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst zawierający zakres dat i godzin dyżuru.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime startDate && parameter is DateTime endDate)
            {
                return $"{startDate:dd.MM.yyyy HH:mm} - {endDate:dd.MM.yyyy HH:mm}";
            }

            return string.Empty;
        }
    }
}