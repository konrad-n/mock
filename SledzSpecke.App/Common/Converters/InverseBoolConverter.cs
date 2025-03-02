// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InverseBoolConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Konwerter odwracający wartość logiczną.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Konwerter odwracający wartość logiczną.
    /// </summary>
    public class InverseBoolConverter : BaseConverter
    {
        /// <summary>
        /// Konwertuje wartość logiczną na jej przeciwieństwo.
        /// </summary>
        /// <param name="value">Wartość logiczna do przekonwertowania.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Zanegowana wartość logiczna lub oryginalna wartość, jeśli nie jest wartością logiczną.</returns>
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }

            return value;
        }

        /// <summary>
        /// Konwertuje wartość z powrotem, co w tym przypadku również oznacza negację.
        /// </summary>
        /// <param name="value">Wartość do przekonwertowania z powrotem.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Zanegowana wartość logiczna lub oryginalna wartość, jeśli nie jest wartością logiczną.</returns>
        public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }
}