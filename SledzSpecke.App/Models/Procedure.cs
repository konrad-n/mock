﻿using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Procedure
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ProcedureId { get; set; }

        [Indexed]
        public int InternshipId { get; set; }

        public DateTime Date { get; set; }

        public int Year { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        [MaxLength(10)]
        public string OperatorCode { get; set; }

        [MaxLength(100)]
        public string PerformingPerson { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [MaxLength(10)]
        public string PatientInitials { get; set; }

        [MaxLength(1)]
        public string PatientGender { get; set; }

        public string AssistantData { get; set; }

        public string ProcedureGroup { get; set; }

        [MaxLength(20)]
        public string Status { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }
    }
}
