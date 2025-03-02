// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelfEducationTypeColorConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter typu samokształcenia na kolor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter typu samokształcenia na kolor.
    /// </summary>
    public class SelfEducationTypeColorConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje typ samokształcenia na odpowiedni kolor.
        /// </summary>
        /// <param name="value">Wartość reprezentująca typ samokształcenia.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Kolor odpowiadający typowi samokształcenia.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SelfEducationType type)
            {
                return type switch
                {
                    SelfEducationType.Conference => new Color(13, 117, 156), // Niebieska
                    SelfEducationType.Workshop => new Color(76, 175, 80),    // Zielona
                    SelfEducationType.Course => new Color(255, 152, 0),      // Pomarańczowa
                    SelfEducationType.ScientificMeeting => new Color(156, 39, 176), // Fioletowa
                    SelfEducationType.Publication => new Color(3, 169, 244),  // Jasnoniebieska
                    SelfEducationType.Other => new Color(158, 158, 158),      // Szara
                    _ => Colors.Black
                };
            }

            return Colors.Black;
        }
    }
}