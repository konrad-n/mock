// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceIconTextConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter typu nieobecności na ikony tekstowe.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter typu nieobecności na ikony tekstowe.
    /// </summary>
    public class AbsenceIconTextConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje typ nieobecności na odpowiednią ikonę tekstową (emoji).
        /// </summary>
        /// <param name="value">Wartość reprezentująca typ nieobecności.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Ikona tekstowa (emoji) odpowiadająca typowi nieobecności.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is AbsenceType type)
            {
                return type switch
                {
                    AbsenceType.SickLeave => "🤒",
                    AbsenceType.VacationLeave => "🏖️",
                    AbsenceType.SelfEducationLeave => "📚",
                    AbsenceType.MaternityLeave => "👶",
                    AbsenceType.ParentalLeave => "👶",
                    _ => "📅"
                };
            }

            return "📅";
        }
    }
}