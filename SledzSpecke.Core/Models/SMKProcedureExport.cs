using System;

namespace SledzSpecke.Core.Models
{
    public class SmkProcedureExport
    {
        public string PatientName { get; set; }
        public DateTime Date { get; set; }
        public string PerformingDoctor { get; set; }
        public string AssistingDoctors { get; set; }
        public string ProcedureGroup { get; set; }
    }
}
