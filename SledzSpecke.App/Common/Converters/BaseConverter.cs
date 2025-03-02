// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseConverter.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Bazowy konwerter dla wszystkich konwerterów wartości w aplikacji.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace SledzSpecke.App.Common.Converters
{
    /// <summary>
    /// Bazowa klasa dla konwerterów wartości z obsługą nullowalności zgodną z IValueConverter.
    /// </summary>
    public abstract class BaseConverter : IValueConverter
    {
        /// <summary>
        /// Konwertuje wartość na inny typ dla celów wiązania danych.
        /// </summary>
        /// <param name="value">Wartość produkowana przez źródło wiązania.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Przekonwertowana wartość.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public virtual object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }

        /// <summary>
        /// Konwertuje wartość z powrotem do typu źródłowego.
        /// </summary>
        /// <param name="value">Wartość przekonwertowana uprzednio przez konwerter.</param>
        /// <param name="targetType">Typ docelowy.</param>
        /// <param name="parameter">Parametr konwersji.</param>
        /// <param name="culture">Kultura używana w konwersji.</param>
        /// <returns>Wartość przekonwertowana z powrotem do typu źródłowego.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325", Justification = "Required for IValueConverter interface")]
        public virtual object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}