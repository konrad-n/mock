using SledzSpecke.Core.Models.Enums;
using SQLite;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("DutyRequirement")]
    public class DutyRequirement : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        [Column("Type")]
        public string Type { get; set; }

        [Column("RequiredHours")]
        public decimal RequiredHours { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("RequiredYear")]
        public int RequiredYear { get; set; } // Rok specjalizacji

        [Column("RequiresSupervision")]
        public bool RequiresSupervision { get; set; }

        [Column("MinimumHoursPerMonth")]
        public int MinimumHoursPerMonth { get; set; }

        [Column("MinimumDutiesPerMonth")]
        public int MinimumDutiesPerMonth { get; set; }

        // Serializowana lista wymaganych kompetencji
        private string _requiredCompetenciesJson;
        [Column("RequiredCompetenciesJson")]
        public string RequiredCompetenciesJson
        {
            get => _requiredCompetenciesJson;
            set => _requiredCompetenciesJson = value;
        }

        // Właściwość nawigacyjna (nieserializowana)
        [Ignore]
        public List<string> RequiredCompetencies
        {
            get => string.IsNullOrEmpty(_requiredCompetenciesJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(_requiredCompetenciesJson);
            set => _requiredCompetenciesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        // Nawigacja
        [Ignore]
        public Specialization Specialization { get; set; }
    }
}