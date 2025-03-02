// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelfEducationDateRangeConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter zakresu dat samokształcenia na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter zakresu dat samokształcenia na tekst.
    /// </summary>
    public class SelfEducationDateRangeConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje daty samokształcenia na tekst z informacją o zakresie dat i liczbie dni.
        /// </summary>
        /// <param name="value">Wartość reprezentująca samokształcenie.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst zawierający zakres dat i liczbę dni.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SelfEducation education)
            {
                return $"{education.StartDate:dd.MM.yyyy} - {education.EndDate:dd.MM.yyyy} ({education.DurationDays} dni)";
            }

            return string.Empty;
        }
    }
}