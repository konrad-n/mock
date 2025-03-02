// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelfEducationTypeNameConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter typu samokształcenia na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter typu samokształcenia na tekst.
    /// </summary>
    public class SelfEducationTypeNameConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje typ samokształcenia na odpowiadający mu tekst.
        /// </summary>
        /// <param name="value">Wartość reprezentująca typ samokształcenia.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst opisujący typ samokształcenia.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SelfEducationType type)
            {
                return type switch
                {
                    SelfEducationType.Conference => "Konferencja",
                    SelfEducationType.Workshop => "Warsztaty",
                    SelfEducationType.Course => "Kurs",
                    SelfEducationType.ScientificMeeting => "Spotkanie naukowe",
                    SelfEducationType.Publication => "Publikacja",
                    SelfEducationType.Other => "Inne",
                    _ => "Nieznany"
                };
            }

            return "Nieznany";
        }
    }
}