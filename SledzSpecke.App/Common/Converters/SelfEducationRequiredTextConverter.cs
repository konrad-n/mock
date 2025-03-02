// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelfEducationRequiredTextConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter informacji o wymagalności samokształcenia na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter informacji o wymagalności samokształcenia na tekst.
    /// </summary>
    public class SelfEducationRequiredTextConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje informację o wymagalności samokształcenia na odpowiedni tekst.
        /// </summary>
        /// <param name="value">Wartość reprezentująca wymagalność samokształcenia.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst opisujący status wymagalności samokształcenia.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isRequired)
            {
                return isRequired ? "Wymagane w programie specjalizacji" : "Dodatkowe";
            }

            return "Nieznane";
        }
    }
}