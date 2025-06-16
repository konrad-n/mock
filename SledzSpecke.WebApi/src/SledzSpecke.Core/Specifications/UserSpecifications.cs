using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public class UserByUsernameSpecification : Specification<User>
{
    private readonly Username _username;

    public UserByUsernameSpecification(Username username)
    {
        _username = username;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Username == _username;
    }
}

public class UserByEmailSpecification : Specification<User>
{
    private readonly Email _email;

    public UserByEmailSpecification(Email email)
    {
        _email = email;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Email == _email;
    }
}

public class UserBySpecializationSpecification : Specification<User>
{
    private readonly SpecializationId _specializationId;

    public UserBySpecializationSpecification(SpecializationId specializationId)
    {
        _specializationId = specializationId;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.SpecializationId == _specializationId;
    }
}

public class UserByRecentActivitySpecification : Specification<User>
{
    private readonly int _daysThreshold;

    public UserByRecentActivitySpecification(int daysThreshold = 30)
    {
        _daysThreshold = daysThreshold;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-_daysThreshold);
        return user => user.LastLoginAt.HasValue && user.LastLoginAt.Value >= cutoffDate;
    }
}

public class UserByProfileCompleteSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.PhoneNumber != null && 
                      user.DateOfBirth.HasValue && 
                      user.Bio != null;
    }
}

public class UserByFullNameSpecification : Specification<User>
{
    private readonly string _searchTerm;

    public UserByFullNameSpecification(string searchTerm)
    {
        _searchTerm = searchTerm.ToLower();
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.FullName.Value.ToLower().Contains(_searchTerm);
    }
}

// Composite specifications for common scenarios
public static class UserSpecificationExtensions
{
    public static ISpecification<User> GetActiveUsers(int daysThreshold = 30)
    {
        return new UserByRecentActivitySpecification(daysThreshold);
    }

    public static ISpecification<User> GetUsersWithCompleteProfiles()
    {
        return new UserByProfileCompleteSpecification();
    }

    public static ISpecification<User> GetUsersBySpecialization(SpecializationId specializationId)
    {
        return new UserBySpecializationSpecification(specializationId);
    }

    public static ISpecification<User> SearchUsers(string searchTerm)
    {
        // Always search by full name
        Specification<User> fullNameSpec = new UserByFullNameSpecification(searchTerm);
        
        // Try to search by username if valid
        try
        {
            var usernameSpec = new UserByUsernameSpecification(new Username(searchTerm));
            return fullNameSpec.Or(usernameSpec);
        }
        catch
        {
            // If not a valid username, just use full name search
            return fullNameSpec;
        }
    }
}