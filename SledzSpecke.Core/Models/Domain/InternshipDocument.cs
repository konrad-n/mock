using SledzSpecke.Core.Models.Enums;
using SQLite;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("InternshipDocument")]
    public class InternshipDocument : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("InternshipId")]
        public int InternshipId { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("FilePath")]
        public string FilePath { get; set; }

        [Column("Type")]
        public DocumentType Type { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public Internship Internship { get; set; }
    }
}
