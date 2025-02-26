using System.Collections.Generic;
using SledzSpecke.Core.Models.Requirements;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface ISpecializationRequirementsProvider
    {
        Dictionary<string, List<RequiredProcedure>> GetRequiredProceduresBySpecialization(int specializationId);
        List<DutyRequirements.DutySpecification> GetDutyRequirementsBySpecialization(int specializationId);
    }
}
