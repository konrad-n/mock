using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Specifications.EducationalActivity;

public class EducationalActivityByDateRangeSpecification : Specification<Core.Entities.EducationalActivity>
{
    private readonly SpecializationId _specializationId;
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public EducationalActivityByDateRangeSpecification(
        SpecializationId specializationId, 
        DateTime startDate, 
        DateTime endDate)
    {
        _specializationId = specializationId;
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<Core.Entities.EducationalActivity, bool>> ToExpression()
        => activity => activity.SpecializationId == _specializationId 
            && activity.StartDate >= _startDate 
            && activity.EndDate <= _endDate;
}