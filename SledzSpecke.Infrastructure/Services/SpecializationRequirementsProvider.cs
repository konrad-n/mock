using System.Collections.Generic;
using System.Linq;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Requirements;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.Infrastructure.Services
{
    public class SpecializationRequirementsProvider : ISpecializationRequirementsProvider
    {
        public Dictionary<string, List<RequiredProcedure>> GetRequiredProceduresBySpecialization(int specializationId)
        {
            var result = new Dictionary<string, List<RequiredProcedure>>();
            var allProcedures = DataSeeder.GetRequiredProcedures();

            foreach (var category in allProcedures.Keys)
            {
                var proceduresInCategory = allProcedures[category]
                    .Where(p => p.SpecializationId == specializationId)
                    .ToList();

                if (proceduresInCategory.Any())
                {
                    result[category] = proceduresInCategory;
                }
            }

            return result;
        }

        public List<DutyRequirements.DutySpecification> GetDutyRequirementsBySpecialization(int specializationId)
        {
            return DutyRequirements.GetDutyRequirements()
                .Where(dr => dr.SpecializationId == specializationId)
                .ToList();
        }
    }
}
