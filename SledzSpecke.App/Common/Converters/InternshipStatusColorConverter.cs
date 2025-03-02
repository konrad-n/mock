// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternshipStatusColorConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter statusu stażu na kolor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter statusu stażu na kolor.
    /// </summary>
    public class InternshipStatusColorConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje informacje o stażu na kolor odpowiadający jego statusowi.
        /// </summary>
        /// <param name="value">Wartość reprezentująca staż.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Kolor odpowiadający statusowi stażu.</returns>
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