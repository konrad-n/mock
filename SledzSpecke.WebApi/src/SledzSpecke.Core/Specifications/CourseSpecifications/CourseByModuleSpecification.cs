using System.Linq.Expressions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Specifications.CourseSpecifications;

public sealed class CourseByModuleSpecification : Specification<Course>
{
    private readonly ModuleId _moduleId;

    public CourseByModuleSpecification(ModuleId moduleId)
    {
        _moduleId = moduleId;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.ModuleId == _moduleId;
    }
}