// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternshipStatusStyleConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter statusu stażu na nazwę stylu.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter statusu stażu na nazwę stylu.
    /// </summary>
    public class InternshipStatusStyleConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje informacje o stażu na nazwę stylu odpowiadającego jego statusowi.
        /// </summary>
        /// <param name="value">Wartość reprezentująca staż.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Nazwa stylu odpowiadająca statusowi stażu.</returns>
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