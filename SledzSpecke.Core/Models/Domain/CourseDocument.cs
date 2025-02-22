using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.Core.Models.Domain
{
    public class CourseDocument : BaseEntity
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public CourseDocumentType Type { get; set; }
        public string Notes { get; set; }

        // Właściwości nawigacyjne
        public Course Course { get; set; }
    }
}
