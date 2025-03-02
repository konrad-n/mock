using System;
using SledzSpecke.Core.Models.Enums;
using SQLite;

namespace SledzSpecke.Core.Models
{
    [Table("SelfEducation")]
    public class SelfEducation
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [MaxLength(255)]
        [Indexed]
        public string Title { get; set; }

        public SelfEducationType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DurationDays { get; set; }

        public string Location { get; set; }

        public string Organizer { get; set; }

        public bool IsRequired { get; set; }

        public string CertificateFilePath { get; set; }

        public string Notes { get; set; }

        public int SpecializationId { get; set; }
    }
}
