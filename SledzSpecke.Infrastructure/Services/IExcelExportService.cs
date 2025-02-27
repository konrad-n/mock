using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IExcelExportService
    {
        Task<string> ExportProceduresToExcelAsync(List<ProcedureExecution> procedures);
        Task<string> ExportDutiesToExcelAsync(List<Duty> duties);
        Task<string> ExportAllDataToExcelAsync(
            List<ProcedureExecution> procedures,
            List<Duty> duties,
            List<Course> courses = null,
            List<Internship> internships = null);
    }
}
