// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceSpecializationAffectionConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter wpływu nieobecności na specjalizację na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter wpływu nieobecności na specjalizację na tekst.
    /// </summary>
    public class AbsenceSpecializationAffectionConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje informację o wpływie nieobecności na specjalizację na odpowiedni tekst.
        /// </summary>
        /// <param name="value">Wartość reprezentująca wpływ na specjalizację.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst informujący o wpływie na specjalizację.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool affectsSpecialization)
            {
                return affectsSpecialization ? "Wydłuża specjalizację" : string.Empty;
            }

            return string.Empty;
        }
    }
}