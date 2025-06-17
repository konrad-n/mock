using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public sealed class SelfEducationByIdSpecification : Specification<SelfEducation>
{
    private readonly SelfEducationId _id;

    public SelfEducationByIdSpecification(SelfEducationId id)
    {
        _id = id;
    }

    public override Expression<Func<SelfEducation, bool>> ToExpression()
    {
        return selfEducation => selfEducation.Id == _id;
    }
}

public sealed class SelfEducationByModuleSpecification : Specification<SelfEducation>
{
    private readonly ModuleId _moduleId;

    public SelfEducationByModuleSpecification(ModuleId moduleId)
    {
        _moduleId = moduleId;
    }

    public override Expression<Func<SelfEducation, bool>> ToExpression()
    {
        return selfEducation => selfEducation.ModuleId == _moduleId;
    }
}

public sealed class SelfEducationByTypeSpecification : Specification<SelfEducation>
{
    private readonly SelfEducationType _type;

    public SelfEducationByTypeSpecification(SelfEducationType type)
    {
        _type = type;
    }

    public override Expression<Func<SelfEducation, bool>> ToExpression()
    {
        return selfEducation => selfEducation.Type == _type;
    }
}

public sealed class SelfEducationInDateRangeSpecification : Specification<SelfEducation>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public SelfEducationInDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<SelfEducation, bool>> ToExpression()
    {
        return selfEducation => selfEducation.Date >= _startDate && selfEducation.Date <= _endDate;
    }
}

public sealed class PublicationSelfEducationSpecification : Specification<SelfEducation>
{
    public override Expression<Func<SelfEducation, bool>> ToExpression()
    {
        return selfEducation => selfEducation.Type == SelfEducationType.Publication;
    }
}

public sealed class PeerReviewedSelfEducationPublicationSpecification : Specification<SelfEducation>
{
    public override Expression<Func<SelfEducation, bool>> ToExpression()
    {
        return selfEducation => selfEducation.Type == SelfEducationType.Publication && selfEducation.IsPeerReviewed;
    }
}

public sealed class SelfEducationBySyncStatusSpecification : Specification<SelfEducation>
{
    private readonly SyncStatus _syncStatus;

    public SelfEducationBySyncStatusSpecification(SyncStatus syncStatus)
    {
        _syncStatus = syncStatus;
    }

    public override Expression<Func<SelfEducation, bool>> ToExpression()
    {
        return selfEducation => selfEducation.SyncStatus == _syncStatus;
    }
}

public sealed class SelfEducationWithMinHoursSpecification : Specification<SelfEducation>
{
    private readonly int _minHours;

    public SelfEducationWithMinHoursSpecification(int minHours)
    {
        _minHours = minHours;
    }

    public override Expression<Func<SelfEducation, bool>> ToExpression()
    {
        return selfEducation => selfEducation.Hours >= _minHours;
    }
}

public static class SelfEducationSpecificationExtensions
{
    public static ISpecification<SelfEducation> GetSelfEducationForModule(ModuleId moduleId)
    {
        return new SelfEducationByModuleSpecification(moduleId);
    }

    public static ISpecification<SelfEducation> GetSelfEducationByTypeForModule(ModuleId moduleId, SelfEducationType type)
    {
        return new SelfEducationByModuleSpecification(moduleId)
            .And(new SelfEducationByTypeSpecification(type));
    }

    public static ISpecification<SelfEducation> GetPublicationsForModule(ModuleId moduleId)
    {
        return new SelfEducationByModuleSpecification(moduleId)
            .And(new PublicationSelfEducationSpecification());
    }

    public static ISpecification<SelfEducation> GetPeerReviewedPublicationsForModule(ModuleId moduleId)
    {
        return new SelfEducationByModuleSpecification(moduleId)
            .And(new PeerReviewedSelfEducationPublicationSpecification());
    }

    public static ISpecification<SelfEducation> GetSelfEducationInDateRange(ModuleId moduleId, DateTime startDate, DateTime endDate)
    {
        return new SelfEducationByModuleSpecification(moduleId)
            .And(new SelfEducationInDateRangeSpecification(startDate, endDate));
    }

    public static ISpecification<SelfEducation> GetUnsyncedSelfEducation(ModuleId moduleId)
    {
        return new SelfEducationByModuleSpecification(moduleId)
            .And(new SelfEducationBySyncStatusSpecification(SyncStatus.NotSynced));
    }

    public static ISpecification<SelfEducation> GetSelfEducationWithMinHours(ModuleId moduleId, int minHours)
    {
        return new SelfEducationByModuleSpecification(moduleId)
            .And(new SelfEducationWithMinHoursSpecification(minHours));
    }

    public static ISpecification<SelfEducation> GetModifiableSelfEducation(ModuleId moduleId)
    {
        return new SelfEducationByModuleSpecification(moduleId)
            .And(new SelfEducationBySyncStatusSpecification(SyncStatus.Synced).Not());
    }
}