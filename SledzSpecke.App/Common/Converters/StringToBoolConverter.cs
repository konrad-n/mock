// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringToBoolConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter ciągu znaków na wartość logiczną.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter ciągu znaków na wartość logiczną.
    /// </summary>
    public class StringToBoolConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje ciąg znaków na wartość logiczną na podstawie jego niepustości.
        /// </summary>
        /// <param name="value">Ciąg znaków do przekonwertowania.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Wartość true, jeśli ciąg nie jest pusty, w przeciwnym razie false.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }
    }
}