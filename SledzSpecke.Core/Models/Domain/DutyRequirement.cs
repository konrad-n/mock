using SledzSpecke.Core.Models.Enums;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class DutyRequirement : BaseEntity
    {
        public int SpecializationId { get; set; }
        public string Type { get; set; }
        public decimal RequiredHours { get; set; }
        public string Description { get; set; }
        public int RequiredYear { get; set; } // Rok specjalizacji
        public bool RequiresSupervision { get; set; }
        public int MinimumHoursPerMonth { get; set; }
        public int MinimumDutiesPerMonth { get; set; }
        
        // Serializowana lista wymaganych kompetencji
        private string _requiredCompetenciesJson;
        public string RequiredCompetenciesJson
        {
            get => _requiredCompetenciesJson;
            set => _requiredCompetenciesJson = value;
        }
        
        // Właściwość nawigacyjna (nieserializowana)
        [SQLite.Ignore]
        public List<string> RequiredCompetencies
        {
            get => string.IsNullOrEmpty(_requiredCompetenciesJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(_requiredCompetenciesJson);
            set => _requiredCompetenciesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
        
        // Nawigacja
        public Specialization Specialization { get; set; }
    }
}