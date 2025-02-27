// Zaktualizować SledzSpecke.Core/Models/Domain/CourseDefinition.cs
using SledzSpecke.Core.Models.Enums;
using SQLite;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("CourseDefinition")]
    public class CourseDefinition : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("DurationInHours")]
        public int DurationInHours { get; set; }

        [Column("DurationInDays")]
        public int DurationInDays { get; set; }

        [Column("IsRequired")]
        public bool IsRequired { get; set; }

        [Column("CanBeRemote")]
        public bool CanBeRemote { get; set; }

        [Column("RecommendedYear")]
        public int RecommendedYear { get; set; }

        [Column("Requirements")]
        public string Requirements { get; set; }

        [Column("CompletionRequirements")]
        public string CompletionRequirements { get; set; }

        [Column("Module")]
        public ModuleType Module { get; set; }

        // Serializowana lista tematów kursu
        private string _courseTopicsJson;
        [Column("CourseTopicsJson")]
        public string CourseTopicsJson
        {
            get => _courseTopicsJson;
            set => _courseTopicsJson = value;
        }

        // Właściwość nawigacyjna (nieserializowana)
        [Ignore]
        public List<string> CourseTopics
        {
            get => string.IsNullOrEmpty(_courseTopicsJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(_courseTopicsJson);
            set => _courseTopicsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        // Nawigacja
        [Ignore]
        public Specialization Specialization { get; set; }

        [Ignore]
        public ICollection<Course> Courses { get; set; }
    }
}
