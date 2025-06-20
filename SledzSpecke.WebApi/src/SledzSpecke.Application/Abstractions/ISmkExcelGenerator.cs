using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

/// <summary>
/// Interface for SMK Excel generation
/// </summary>
public interface ISmkExcelGenerator
{
    Task<byte[]> GenerateSmkExportAsync(SmkExportData exportData);
}

/// <summary>
/// Data transfer object containing all data needed for SMK export
/// </summary>
public class SmkExportData
{
    public User User { get; set; } = null!;
    public Specialization Specialization { get; set; } = null!;
    public SmkVersion SmkVersion { get; set; }
    public IEnumerable<Module> Modules { get; set; } = new List<Module>();
    public IEnumerable<Internship> Internships { get; set; } = new List<Internship>();
    public IEnumerable<MedicalShift> MedicalShifts { get; set; } = new List<MedicalShift>();
    public IEnumerable<ProcedureRealization> ProcedureRealizations { get; set; } = new List<ProcedureRealization>();
    public IEnumerable<ProcedureRequirement> ProcedureRequirements { get; set; } = new List<ProcedureRequirement>();
    public IEnumerable<Course> Courses { get; set; } = new List<Course>();
    public IEnumerable<SelfEducation> SelfEducation { get; set; } = new List<SelfEducation>();
    public IEnumerable<AdditionalSelfEducationDays> AdditionalDays { get; set; } = new List<AdditionalSelfEducationDays>();
    public SmkValidationResult? ValidationResult { get; set; }
}