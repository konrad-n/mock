namespace SledzSpecke.App.Services.Export
{
    /// <summary>
    /// Klasa przechowująca wynik walidacji
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Czy walidacja przebiegła pomyślnie.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Komunikat z błędami walidacji
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}