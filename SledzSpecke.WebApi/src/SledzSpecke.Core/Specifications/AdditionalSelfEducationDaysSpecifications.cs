using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public sealed class AdditionalSelfEducationDaysByIdSpecification : Specification<AdditionalSelfEducationDays>
{
    private readonly int _id;

    public AdditionalSelfEducationDaysByIdSpecification(int id)
    {
        _id = id;
    }

    public override Expression<Func<AdditionalSelfEducationDays, bool>> ToExpression()
    {
        return days => days.Id == _id;
    }
}

public sealed class AdditionalSelfEducationDaysByModuleSpecification : Specification<AdditionalSelfEducationDays>
{
    private readonly ModuleId _moduleId;

    public AdditionalSelfEducationDaysByModuleSpecification(ModuleId moduleId)
    {
        _moduleId = moduleId;
    }

    public override Expression<Func<AdditionalSelfEducationDays, bool>> ToExpression()
    {
        return days => days.ModuleId == _moduleId;
    }
}

public sealed class AdditionalSelfEducationDaysByInternshipSpecification : Specification<AdditionalSelfEducationDays>
{
    private readonly InternshipId _internshipId;

    public AdditionalSelfEducationDaysByInternshipSpecification(InternshipId internshipId)
    {
        _internshipId = internshipId;
    }

    public override Expression<Func<AdditionalSelfEducationDays, bool>> ToExpression()
    {
        return days => days.InternshipId == _internshipId;
    }
}

public sealed class ApprovedAdditionalSelfEducationDaysSpecification : Specification<AdditionalSelfEducationDays>
{
    public override Expression<Func<AdditionalSelfEducationDays, bool>> ToExpression()
    {
        return days => days.IsApproved;
    }
}

public sealed class PendingAdditionalSelfEducationDaysSpecification : Specification<AdditionalSelfEducationDays>
{
    public override Expression<Func<AdditionalSelfEducationDays, bool>> ToExpression()
    {
        return days => !days.IsApproved;
    }
}

public sealed class AdditionalSelfEducationDaysInDateRangeSpecification : Specification<AdditionalSelfEducationDays>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public AdditionalSelfEducationDaysInDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<AdditionalSelfEducationDays, bool>> ToExpression()
    {
        return days => days.StartDate <= _endDate && days.EndDate >= _startDate;
    }
}

public sealed class AdditionalSelfEducationDaysInYearSpecification : Specification<AdditionalSelfEducationDays>
{
    private readonly int _year;

    public AdditionalSelfEducationDaysInYearSpecification(int year)
    {
        _year = year;
    }

    public override Expression<Func<AdditionalSelfEducationDays, bool>> ToExpression()
    {
        return days => days.StartDate.Year == _year || days.EndDate.Year == _year;
    }
}

public static class AdditionalSelfEducationDaysSpecificationExtensions
{
    public static ISpecification<AdditionalSelfEducationDays> GetDaysForModule(ModuleId moduleId)
    {
        return new AdditionalSelfEducationDaysByModuleSpecification(moduleId);
    }

    public static ISpecification<AdditionalSelfEducationDays> GetApprovedDaysForModule(ModuleId moduleId)
    {
        return new AdditionalSelfEducationDaysByModuleSpecification(moduleId)
            .And(new ApprovedAdditionalSelfEducationDaysSpecification());
    }

    public static ISpecification<AdditionalSelfEducationDays> GetPendingDaysForModule(ModuleId moduleId)
    {
        return new AdditionalSelfEducationDaysByModuleSpecification(moduleId)
            .And(new PendingAdditionalSelfEducationDaysSpecification());
    }

    public static ISpecification<AdditionalSelfEducationDays> GetDaysForInternship(InternshipId internshipId)
    {
        return new AdditionalSelfEducationDaysByInternshipSpecification(internshipId);
    }

    public static ISpecification<AdditionalSelfEducationDays> GetDaysInYear(ModuleId moduleId, int year)
    {
        return new AdditionalSelfEducationDaysByModuleSpecification(moduleId)
            .And(new AdditionalSelfEducationDaysInYearSpecification(year));
    }

    public static ISpecification<AdditionalSelfEducationDays> GetDaysInDateRange(ModuleId moduleId, DateTime startDate, DateTime endDate)
    {
        return new AdditionalSelfEducationDaysByModuleSpecification(moduleId)
            .And(new AdditionalSelfEducationDaysInDateRangeSpecification(startDate, endDate));
    }

    public static ISpecification<AdditionalSelfEducationDays> GetOverlappingDays(ModuleId moduleId, DateTime startDate, DateTime endDate, int? excludeId = null)
    {
        var spec = new AdditionalSelfEducationDaysByModuleSpecification(moduleId)
            .And(new AdditionalSelfEducationDaysInDateRangeSpecification(startDate, endDate));

        if (excludeId.HasValue)
        {
            spec = spec.And(new AdditionalSelfEducationDaysByIdSpecification(excludeId.Value).Not());
        }

        return spec;
    }
}