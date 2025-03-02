// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternshipStatusTextConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter statusu stażu na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter statusu stażu na tekst.
    /// </summary>
    public class InternshipStatusTextConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje informacje o stażu na tekst opisujący jego status.
        /// </summary>
        /// <param name="value">Wartość reprezentująca staż.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst opisujący status stażu.</returns>
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