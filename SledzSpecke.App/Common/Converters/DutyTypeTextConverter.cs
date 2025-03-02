// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DutyTypeTextConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter typu dyżuru na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter typu dyżuru na tekst.
    /// </summary>
    public class DutyTypeTextConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje typ dyżuru na odpowiadający mu tekst.
        /// </summary>
        /// <param name="value">Wartość reprezentująca typ dyżuru.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst opisujący typ dyżuru.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Independent ? "Samodzielny" : "Towarzyszący";
            }

            return "Nieznany";
        }
    }
}