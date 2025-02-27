using System;

namespace SledzSpecke.Core.Models
{
    public class SelfEducation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public SelfEducationType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationDays { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public bool IsRequired { get; set; }
        public string CertificateFilePath { get; set; }
        public string Notes { get; set; }
    }

    public enum SelfEducationType
    {
        Conference,
        Workshop,
        Course,
        ScientificMeeting,
        Publication,
        Other
    }
}
