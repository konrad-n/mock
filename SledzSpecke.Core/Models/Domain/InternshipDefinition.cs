// Zaktualizować SledzSpecke.Core/Models/Domain/InternshipDefinition.cs
using SledzSpecke.Core.Models.Enums;
using SQLite;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("InternshipDefinition")]
    public class InternshipDefinition : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("DurationInWeeks")]
        public int DurationInWeeks { get; set; }

        [Column("WorkingDays")]
        public int WorkingDays { get; set; }

        [Column("IsRequired")]
        public bool IsRequired { get; set; }

        [Column("RecommendedYear")]
        public int RecommendedYear { get; set; }

        [Column("Requirements")]
        public string Requirements { get; set; }

        [Column("Module")]
        public ModuleType Module { get; set; }

        // Serializowana lista wymagań do zaliczenia
        private string _completionRequirementsJson;
        [Column("CompletionRequirementsJson")]
        public string CompletionRequirementsJson
        {
            get => _completionRequirementsJson;
            set => _completionRequirementsJson = value;
        }

        // Właściwość nawigacyjna (nieserializowana)
        [Ignore]
        public List<string> CompletionRequirements
        {
            get => string.IsNullOrEmpty(_completionRequirementsJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(_completionRequirementsJson);
            set => _completionRequirementsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        // Nawigacja
        [Ignore]
        public Specialization Specialization { get; set; }

        [Ignore]
        public ICollection<InternshipModule> DetailedStructure { get; set; }

        [Ignore]
        public ICollection<Internship> Internships { get; set; }
    }
}
