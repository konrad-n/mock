using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Services
{
    public interface IExportService
    {
        Task<string> ExportToSMKAsync(SMKExportOptions options);
    }
}