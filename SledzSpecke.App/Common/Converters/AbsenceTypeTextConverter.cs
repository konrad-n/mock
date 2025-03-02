// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceTypeTextConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter typu nieobecności na tekst.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter typu nieobecności na tekst.
    /// </summary>
    public class AbsenceTypeTextConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje typ nieobecności na odpowiadający mu tekst.
        /// </summary>
        /// <param name="value">Wartość reprezentująca typ nieobecności.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Tekst opisujący typ nieobecności.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is AbsenceType type)
            {
                return type switch
                {
                    AbsenceType.SickLeave => "Zwolnienie lekarskie (L4)",
                    AbsenceType.VacationLeave => "Urlop wypoczynkowy",
                    AbsenceType.SelfEducationLeave => "Urlop szkoleniowy (samokształcenie)",
                    AbsenceType.MaternityLeave => "Urlop macierzyński",
                    AbsenceType.ParentalLeave => "Urlop rodzicielski",
                    AbsenceType.SpecialLeave => "Urlop okolicznościowy",
                    AbsenceType.UnpaidLeave => "Urlop bezpłatny",
                    AbsenceType.Other => "Inna nieobecność",
                    _ => "Nieobecność"
                };
            }

            return "Nieobecność";
        }
    }
}