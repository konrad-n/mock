using System.Collections.Generic;
using SQLite;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("InternshipModule")]
    public class InternshipModule : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("InternshipDefinitionId")]
        public int InternshipDefinitionId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("DurationInWeeks")]
        public int DurationInWeeks { get; set; }

        [Column("Location")]
        public string Location { get; set; }

        [Column("Requirements")]
        public string Requirements { get; set; }

        // Serializowane listy
        private string _requiredSkillsJson;
        [Column("RequiredSkillsJson")]
        public string RequiredSkillsJson
        {
            get => _requiredSkillsJson;
            set => _requiredSkillsJson = value;
        }

        private string _requiredProceduresJson;
        [Column("RequiredProceduresJson")]
        public string RequiredProceduresJson
        {
            get => _requiredProceduresJson;
            set => _requiredProceduresJson = value;
        }

        private string _assistantProceduresJson;
        [Column("AssistantProceduresJson")]
        public string AssistantProceduresJson
        {
            get => _assistantProceduresJson;
            set => _assistantProceduresJson = value;
        }

        // Właściwości nawigacyjne (nieserializowane)
        [Ignore]
        public List<string> RequiredSkills
        {
            get => string.IsNullOrEmpty(_requiredSkillsJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(_requiredSkillsJson);
            set => _requiredSkillsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [Ignore]
        public Dictionary<string, int> RequiredProcedures
        {
            get => string.IsNullOrEmpty(_requiredProceduresJson)
                ? new Dictionary<string, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(_requiredProceduresJson);
            set => _requiredProceduresJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [Ignore]
        public Dictionary<string, int> AssistantProcedures
        {
            get => string.IsNullOrEmpty(_assistantProceduresJson)
                ? new Dictionary<string, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(_assistantProceduresJson);
            set => _assistantProceduresJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        // Nawigacja
        [Ignore]
        public InternshipDefinition InternshipDefinition { get; set; }
    }
}
