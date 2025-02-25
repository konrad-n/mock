using System;

namespace SledzSpecke.Core.Models.Domain
{
    public class SMKSpecialization
    {
        public string SMKId { get; set; }
        public string Name { get; set; }
        public string ProgramVersion { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int DurationInMonths { get; set; }
        public bool IsActive { get; set; }
        public string Requirements { get; set; }
        
        // Dodatkowe pola specyficzne dla SMK
        public string SMKSpecialtyCode { get; set; }
        public string LegalBasis { get; set; }
        public string MinisterOfHealthRegulation { get; set; }
        public DateTime? RegulationDate { get; set; }
        
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
