using System;

namespace SledzSpecke.Core.Models
{
    public class SmkDutyExport
    {
        public int Hours { get; set; }

        public int Minutes { get; set; }

        public DateTime StartDate { get; set; }

        public string DepartmentName { get; set; }
    }
}
