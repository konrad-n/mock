using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public sealed class PublicationByIdSpecification : Specification<Publication>
{
    private readonly PublicationId _id;

    public PublicationByIdSpecification(PublicationId id)
    {
        _id = id;
    }

    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.Id == _id;
    }
}

public sealed class PublicationByUserSpecification : Specification<Publication>
{
    private readonly UserId _userId;

    public PublicationByUserSpecification(UserId userId)
    {
        _userId = userId;
    }

    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.UserId == _userId;
    }
}

public sealed class PublicationBySpecializationSpecification : Specification<Publication>
{
    private readonly SpecializationId _specializationId;

    public PublicationBySpecializationSpecification(SpecializationId specializationId)
    {
        _specializationId = specializationId;
    }

    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.SpecializationId == _specializationId;
    }
}

public sealed class PublicationByUserAndSpecializationSpecification : Specification<Publication>
{
    private readonly UserId _userId;
    private readonly SpecializationId _specializationId;

    public PublicationByUserAndSpecializationSpecification(UserId userId, SpecializationId specializationId)
    {
        _userId = userId;
        _specializationId = specializationId;
    }

    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.UserId == _userId && publication.SpecializationId == _specializationId;
    }
}

public sealed class PublicationByTypeSpecification : Specification<Publication>
{
    private readonly PublicationType _type;

    public PublicationByTypeSpecification(PublicationType type)
    {
        _type = type;
    }

    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.Type == _type;
    }
}

public sealed class PeerReviewedPublicationSpecification : Specification<Publication>
{
    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.IsPeerReviewed;
    }
}

public sealed class FirstAuthorPublicationSpecification : Specification<Publication>
{
    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.IsFirstAuthor;
    }
}

public sealed class PublicationAfterDateSpecification : Specification<Publication>
{
    private readonly DateTime _afterDate;

    public PublicationAfterDateSpecification(DateTime afterDate)
    {
        _afterDate = afterDate;
    }

    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.PublicationDate >= _afterDate;
    }
}

public sealed class PublicationBeforeDateSpecification : Specification<Publication>
{
    private readonly DateTime _beforeDate;

    public PublicationBeforeDateSpecification(DateTime beforeDate)
    {
        _beforeDate = beforeDate;
    }

    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.PublicationDate <= _beforeDate;
    }
}

public sealed class PublicationByJournalSpecification : Specification<Publication>
{
    private readonly string _journalName;

    public PublicationByJournalSpecification(string journalName)
    {
        _journalName = journalName;
    }

    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => publication.Journal == _journalName;
    }
}

public sealed class PublicationWithCoAuthorsSpecification : Specification<Publication>
{
    public override Expression<Func<Publication, bool>> ToExpression()
    {
        return publication => !string.IsNullOrWhiteSpace(publication.Authors) && publication.Authors.Contains(",");
    }
}

public static class PublicationSpecificationExtensions
{
    public static ISpecification<Publication> GetPeerReviewedPublicationsForUser(UserId userId)
    {
        return new PublicationByUserSpecification(userId)
            .And(new PeerReviewedPublicationSpecification());
    }

    public static ISpecification<Publication> GetFirstAuthorPublicationsForUser(UserId userId)
    {
        return new PublicationByUserSpecification(userId)
            .And(new FirstAuthorPublicationSpecification());
    }

    public static ISpecification<Publication> GetRecentPublicationsForUser(UserId userId, int years = 5)
    {
        var cutoffDate = DateTime.UtcNow.AddYears(-years);
        return new PublicationByUserSpecification(userId)
            .And(new PublicationAfterDateSpecification(cutoffDate));
    }

    public static ISpecification<Publication> GetPublicationsByTypeForUser(UserId userId, PublicationType type)
    {
        return new PublicationByUserSpecification(userId)
            .And(new PublicationByTypeSpecification(type));
    }

    public static ISpecification<Publication> GetPublicationsForUserAndSpecialization(UserId userId, SpecializationId specializationId)
    {
        return new PublicationByUserAndSpecializationSpecification(userId, specializationId);
    }

    public static ISpecification<Publication> GetPublicationsInDateRange(DateTime startDate, DateTime endDate)
    {
        return new PublicationAfterDateSpecification(startDate)
            .And(new PublicationBeforeDateSpecification(endDate));
    }

    public static ISpecification<Publication> GetHighImpactPublications(UserId userId)
    {
        // High impact publications are typically peer-reviewed journal articles where the user is first author
        return new PublicationByUserSpecification(userId)
            .And(new PublicationByTypeSpecification(PublicationType.Journal))
            .And(new PeerReviewedPublicationSpecification())
            .And(new FirstAuthorPublicationSpecification());
    }
}