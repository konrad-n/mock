using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IDocumentScanService
    {
        Task<string> ScanDocumentAsync();
        Task<string> RecognizeTextAsync(Stream imageStream);
        Task<DocumentInfo> ProcessDocumentAsync(Stream imageStream);
    }

    public class DocumentInfo
    {
        public string RecognizedText { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public Dictionary<string, string> ExtractedFields { get; set; } = new Dictionary<string, string>();
    }
}