using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class InternshipDefinition : BaseEntity
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationInWeeks { get; set; }
        public bool IsRequired { get; set; }
        public int RecommendedYear { get; set; }
        public string Requirements { get; set; }
        
        // Serializowana lista wymagań do zaliczenia
        private string _completionRequirementsJson;
        public string CompletionRequirementsJson
        {
            get => _completionRequirementsJson;
            set => _completionRequirementsJson = value;
        }
        
        // Właściwość nawigacyjna (nieserializowana)
        [SQLite.Ignore]
        public List<string> CompletionRequirements
        {
            get => string.IsNullOrEmpty(_completionRequirementsJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(_completionRequirementsJson);
            set => _completionRequirementsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
        
        // Nawigacja
        public Specialization Specialization { get; set; }
        public ICollection<InternshipModule> DetailedStructure { get; set; }
        public ICollection<Internship> Internships { get; set; }
    }
}