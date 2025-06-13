using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

public interface ISpecializationTemplateService
{
    Task<SpecializationTemplate?> GetTemplateAsync(string specializationCode, SmkVersion smkVersion);
    Task<IEnumerable<SpecializationTemplate>> GetAllTemplatesAsync();
    Task<ModuleTemplate?> GetModuleTemplateAsync(string specializationCode, SmkVersion smkVersion, int moduleId);
    Task<ProcedureTemplate?> GetProcedureTemplateAsync(string specializationCode, SmkVersion smkVersion, int procedureId);
    Task<bool> ValidateProcedureRequirementsAsync(string specializationCode, SmkVersion smkVersion, int procedureId, string procedureCode);
    Task<InternshipTemplate?> GetInternshipTemplateAsync(string specializationCode, SmkVersion smkVersion, int internshipId);
    Task<CourseTemplate?> GetCourseTemplateAsync(string specializationCode, SmkVersion smkVersion, int courseId);
}