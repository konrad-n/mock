// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelfEducationRequiredColorConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter informacji o wymagalności samokształcenia na kolor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter informacji o wymagalności samokształcenia na kolor.
    /// </summary>
    public class SelfEducationRequiredColorConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje informację o wymagalności samokształcenia na odpowiedni kolor.
        /// </summary>
        /// <param name="value">Wartość reprezentująca wymagalność samokształcenia.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Kolor odpowiadający statusowi wymagalności samokształcenia.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isRequired)
            {
                return isRequired ? new Color(8, 32, 68) : Colors.DarkGreen;
            }

            return new Color(84, 126, 158);
        }
    }
}