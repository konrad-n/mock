using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class InternshipModule : BaseEntity
    {
        public int InternshipDefinitionId { get; set; }
        public string Name { get; set; }
        public int DurationInWeeks { get; set; }
        public string Location { get; set; }
        public string Requirements { get; set; }
        
        // Serializowane listy
        private string _requiredSkillsJson;
        public string RequiredSkillsJson
        {
            get => _requiredSkillsJson;
            set => _requiredSkillsJson = value;
        }
        
        private string _requiredProceduresJson;
        public string RequiredProceduresJson
        {
            get => _requiredProceduresJson;
            set => _requiredProceduresJson = value;
        }
        
        // Właściwości nawigacyjne (nieserializowane)
        [SQLite.Ignore]
        public List<string> RequiredSkills
        {
            get => string.IsNullOrEmpty(_requiredSkillsJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(_requiredSkillsJson);
            set => _requiredSkillsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
        
        [SQLite.Ignore]
        public Dictionary<string, int> RequiredProcedures
        {
            get => string.IsNullOrEmpty(_requiredProceduresJson)
                ? new Dictionary<string, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(_requiredProceduresJson);
            set => _requiredProceduresJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
        
        // Nawigacja
        public InternshipDefinition InternshipDefinition { get; set; }
    }
}