// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceCardColorConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter określający kolor karty nieobecności w zależności od jej typu.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter określający kolor karty nieobecności w zależności od jej typu.
    /// </summary>
    public class AbsenceCardColorConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje typ nieobecności na kolor tła.
        /// </summary>
        /// <param name="value">Wartość reprezentująca typ nieobecności.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Kolor tła dla karty nieobecności.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is AbsenceType type)
            {
                return type switch
                {
                    AbsenceType.SickLeave => Color.FromArgb("#FFE0E0"),
                    AbsenceType.VacationLeave => Color.FromArgb("#E0F7FA"),
                    AbsenceType.SelfEducationLeave => Color.FromArgb("#E8F5E9"),
                    AbsenceType.MaternityLeave => Color.FromArgb("#FFF8E1"),
                    AbsenceType.ParentalLeave => Color.FromArgb("#FFF8E1"),
                    _ => Color.FromArgb("#F5F5F5")
                };
            }

            return Color.FromArgb("#F5F5F5");
        }
    }
}