using SledzSpecke.Core.Models.Enums;
using System;

namespace SledzSpecke.Core.Models
{
    public class SMKExportOptions
    {
        public bool IncludeCourses { get; set; } = true;
        public bool IncludeInternships { get; set; } = true;
        public bool IncludeProcedures { get; set; } = true;
        public bool IncludeDutyShifts { get; set; } = false;
        public bool IncludeSelfEducation { get; set; } = false;
        public ExportFormat Format { get; set; } = ExportFormat.Excel;
        public SMKExportModuleFilter ModuleFilter { get; set; } = SMKExportModuleFilter.All;
        public SMKExportType ExportType { get; set; } = SMKExportType.General;

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
