// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DutyTypeColorConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter typu dyżuru na kolor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter typu dyżuru na kolor.
    /// </summary>
    public class DutyTypeColorConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje typ dyżuru na odpowiedni kolor.
        /// </summary>
        /// <param name="value">Wartość reprezentująca typ dyżuru.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Kolor odpowiadający typowi dyżuru.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DutyType dutyType)
            {
                return dutyType == DutyType.Independent ?
                    new Color(8, 32, 68) : // Ciemny niebieski
                    new Color(0, 100, 0);  // Ciemny zielony
            }

            return Colors.Black;
        }
    }
}