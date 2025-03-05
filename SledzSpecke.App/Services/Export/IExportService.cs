using SledzSpecke.App.Models;

public interface IExportService
{
    /// <summary>
    /// Eksportuje dane do pliku Excel na podstawie podanych opcji.
    /// </summary>
    /// <param name="options">Opcje eksportu.</param>
    /// <returns>Ścieżka do utworzonego pliku Excel.</returns>
    Task<string> ExportToExcelAsync(ExportOptions options);

    /// <summary>
    /// Weryfikuje dane przed eksportem.
    /// </summary>
    /// <param name="options">Opcje eksportu.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ValidationException">Zgłaszany, gdy dane nie przejdą walidacji.</exception>
    Task ValidateExportDataAsync(ExportOptions options);

    /// <summary>
    /// Zwraca datę ostatniego eksportu.
    /// </summary>
    /// <returns>Data ostatniego eksportu lub null.</returns>
    Task<DateTime?> GetLastExportDateAsync();

    /// <summary>
    /// Zwraca ścieżkę do ostatniego wyeksportowanego pliku.
    /// </summary>
    /// <returns>Ścieżka do pliku.</returns>
    Task<string> GetLastExportFilePathAsync();

    /// <summary>
    /// Zapisuje datę eksportu.
    /// </summary>
    /// <param name="date">Data eksportu.</param>
    /// <returns>Task.</returns>
    Task SaveLastExportDateAsync(DateTime date);

    /// <summary>
    /// Udostępnia plik eksportu.
    /// </summary>
    /// <param name="filePath">Ścieżka do pliku.</param>
    /// <returns>True, jeśli udostępnianie się powiodło; w przeciwnym razie false.</returns>
    Task<bool> ShareExportFileAsync(string filePath);
}