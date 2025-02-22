using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.Core.Models.Domain
{
    public class InternshipDocument : BaseEntity
    {
        public int InternshipId { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public DocumentType Type { get; set; }
        public string Notes { get; set; }

        // Właściwości nawigacyjne
        public Internship Internship { get; set; }
    }
}
