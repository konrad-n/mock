using System;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.Core.Models
{
    public class SmkExportOptions
    {
        public bool IncludeCourses { get; set; } = true;

        public bool IncludeInternships { get; set; } = true;

        public bool IncludeProcedures { get; set; } = true;

        public bool IncludeDutyShifts { get; set; } = false;

        public bool IncludeSelfEducation { get; set; } = false;

        public ExportFormat Format { get; set; } = ExportFormat.Excel;

        public SmkExportModuleFilter ModuleFilter { get; set; } = SmkExportModuleFilter.All;

        public SmkExportType ExportType { get; set; } = SmkExportType.General;

        public bool UseCustomDateRange { get; set; } = false;

        public DateTime? StartDate { get; set; } = DateTime.Now.AddMonths(-3);

        public DateTime? EndDate { get; set; } = DateTime.Now;

        // New fields for SMK format according to manual
        public bool UseSmkExactFormat { get; set; } = true;

        public bool IncludePatientIds { get; set; } = true;

        public bool SplitDutyHoursAndMinutes { get; set; } = true;

        public string ExportFileName => $"SMK_Export_{DateTime.Now:yyyyMMdd}";

        public string FullExportFileName => this.ExportFileName + (this.Format == ExportFormat.Excel ? ".xlsx" : ".csv");
    }
}
