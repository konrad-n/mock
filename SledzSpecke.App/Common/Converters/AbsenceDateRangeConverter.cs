// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceDateRangeConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter zakresu dat nieobecności na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter zakresu dat nieobecności na tekst.
    /// </summary>
    public class AbsenceDateRangeConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje daty nieobecności na tekst z informacją o zakresie dat i liczbie dni.
        /// </summary>
        /// <param name="value">Wartość reprezentująca nieobecność.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst zawierający zakres dat i liczbę dni.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Absence absence)
            {
                return $"{absence.StartDate:dd.MM.yyyy} - {absence.EndDate:dd.MM.yyyy} ({absence.DurationDays} dni)";
            }

            return string.Empty;
        }
    }
}