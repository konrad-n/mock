using SledzSpecke.Core.Models.Enums;
using SQLite;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("CourseDocument")]
    public class CourseDocument : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("CourseId")]
        public int CourseId { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("FilePath")]
        public string FilePath { get; set; }

        [Column("Type")]
        public CourseDocumentType Type { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public Course Course { get; set; }
    }
}
