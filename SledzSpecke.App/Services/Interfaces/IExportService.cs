using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Services.Interfaces
{
    public interface IExportService
    {
        Task<string> ExportToSMKAsync(SmkExportOptions options);
    }
}
