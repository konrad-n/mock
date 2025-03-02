// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternshipSupervisorVisibilityConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter widoczności informacji o opiekunie stażu.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter widoczności informacji o opiekunie stażu.
    /// </summary>
    public class InternshipSupervisorVisibilityConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje informacje o stażu na wartość określającą widoczność informacji o opiekunie.
        /// </summary>
        /// <param name="value">Wartość reprezentująca staż.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Wartość true, jeśli informacja o opiekunie powinna być widoczna, w przeciwnym razie false.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Internship internship)
            {
                bool isCurrentOrCompleted = internship.IsCompleted ||
                                          (internship.StartDate.HasValue && !internship.EndDate.HasValue);

                return !string.IsNullOrEmpty(internship.SupervisorName) || isCurrentOrCompleted;
            }

            return false;
        }
    }
}