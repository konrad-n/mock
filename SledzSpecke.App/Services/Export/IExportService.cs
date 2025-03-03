using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Export
{
    public interface IExportService
    {
        Task<string> ExportToExcelAsync(ExportOptions options);

        Task<DateTime?> GetLastExportDateAsync();

        Task<string> GetLastExportFilePathAsync();

        Task SaveLastExportDateAsync(DateTime date);

        Task<bool> ShareExportFileAsync(string filePath);
    }
}
