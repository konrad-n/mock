using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PWZ { get; set; } // Numer prawa wykonywania zawodu
        public int? CurrentSpecializationId { get; set; }
        public DateTime SpecializationStartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int? SupervisorId { get; set; }

        // Właściwości nawigacyjne
        public Specialization CurrentSpecialization { get; set; }
        public User Supervisor { get; set; }
        public ICollection<User> Supervisees { get; set; }
    }
}
