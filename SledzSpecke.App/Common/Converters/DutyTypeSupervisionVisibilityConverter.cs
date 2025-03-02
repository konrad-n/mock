// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DutyTypeSupervisionVisibilityConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter widoczności nadzoru w zależności od typu dyżuru.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter widoczności nadzoru w zależności od typu dyżuru.
    /// </summary>
    public class DutyTypeSupervisionVisibilityConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje typ dyżuru na wartość określającą widoczność informacji o nadzorze.
        /// </summary>
        /// <param name="value">Wartość reprezentująca typ dyżuru.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji (nazwa lekarza nadzorującego).</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Wartość true, jeśli informacja o nadzorze powinna być widoczna, w przeciwnym razie false.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Accompanied && !string.IsNullOrEmpty(parameter as string);
            }

            return false;
        }
    }
}