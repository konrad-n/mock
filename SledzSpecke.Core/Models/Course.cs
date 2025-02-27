using System;

namespace SledzSpecke.Core.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationDays { get; set; }
        public bool IsRequired { get; set; }
        public ModuleType Module { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsAttended { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasPassedExam { get; set; }
        public string CertificateFilePath { get; set; }
        public string Notes { get; set; }
    }



    public enum ModuleType
    {
        Basic,
        Specialistic
    }
}