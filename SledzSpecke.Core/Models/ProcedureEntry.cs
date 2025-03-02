using System;
using SQLite;

namespace SledzSpecke.Core.Models
{
    [Table("ProcedureEntries")]
    public class ProcedureEntry
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string PatientId { get; set; }

        public string Location { get; set; }

        public string SupervisorName { get; set; }

        public string Notes { get; set; }

        [Indexed]
        public int ProcedureId { get; set; }

        public string PatientGender { get; set; }

        public string FirstAssistantData { get; set; }

        public string SecondAssistantData { get; set; }

        public string ProcedureGroup { get; set; }

        public string InternshipName { get; set; }
    }
}
