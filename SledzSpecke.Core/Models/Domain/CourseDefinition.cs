using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class CourseDefinition : BaseEntity
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationInHours { get; set; }
        public int DurationInDays { get; set; }
        public bool IsRequired { get; set; }
        public bool CanBeRemote { get; set; }
        public int RecommendedYear { get; set; }
        public string Requirements { get; set; }
        public string CompletionRequirements { get; set; }
        
        // Serializowana lista tematów kursu
        private string _courseTopicsJson;
        public string CourseTopicsJson
        {
            get => _courseTopicsJson;
            set => _courseTopicsJson = value;
        }
        
        // Właściwość nawigacyjna (nieserializowana)
        [SQLite.Ignore]
        public List<string> CourseTopics
        {
            get => string.IsNullOrEmpty(_courseTopicsJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(_courseTopicsJson);
            set => _courseTopicsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
        
        // Nawigacja
        public Specialization Specialization { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}