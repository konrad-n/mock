using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Specifications.EducationalActivity;

public class EducationalActivityByModuleSpecification : Specification<Core.Entities.EducationalActivity>
{
    private readonly ModuleId _moduleId;

    public EducationalActivityByModuleSpecification(ModuleId moduleId)
    {
        _moduleId = moduleId;
    }

    public override Expression<Func<Core.Entities.EducationalActivity, bool>> ToExpression()
        => activity => activity.ModuleId == _moduleId;
}