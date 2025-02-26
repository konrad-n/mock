using SQLite;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("SMKSpecialization")]
    public class SMKSpecialization
    {
        [PrimaryKey]
        [Column("SMKId")]
        public string SMKId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("ProgramVersion")]
        public string ProgramVersion { get; set; }

        [Column("ApprovalDate")]
        public DateTime? ApprovalDate { get; set; }

        [Column("DurationInMonths")]
        public int DurationInMonths { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("Requirements")]
        public string Requirements { get; set; }

        // Dodatkowe pola specyficzne dla SMK
        [Column("SMKSpecialtyCode")]
        public string SMKSpecialtyCode { get; set; }

        [Column("LegalBasis")]
        public string LegalBasis { get; set; }

        [Column("MinisterOfHealthRegulation")]
        public string MinisterOfHealthRegulation { get; set; }

        [Column("RegulationDate")]
        public DateTime? RegulationDate { get; set; }

        [Ignore]
        public Specialization Specialization => ToSpecialization();

        // Metoda konwersji do modelu wewnętrznego aplikacji
        public Specialization ToSpecialization()
        {
            return new Specialization
            {
                Name = this.Name,
                ProgramVersion = this.ProgramVersion,
                ApprovalDate = this.ApprovalDate,
                DurationInWeeks = this.DurationInMonths * 4, // Przybliżona konwersja
                Requirements = this.Requirements,
                Description = $"Specjalizacja na podstawie {this.MinisterOfHealthRegulation} z dnia {this.RegulationDate?.ToShortDateString() ?? "N/A"}"
            };
        }
    }
}
