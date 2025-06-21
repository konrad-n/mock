using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public class UserByEmailSpecification : Specification<User>
{
    private readonly string _email;

    public UserByEmailSpecification(string email)
    {
        _email = email;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Email == _email;
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

public class UserByFullNameSpecification : Specification<User>
{
    private readonly string _searchTerm;

    public UserByFullNameSpecification(string searchTerm)
    {
        _searchTerm = searchTerm.ToLower();
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        // Search by name
        return user => user.Name.ToLower().Contains(_searchTerm);
    }
}

public class UserByProvinceSpecification : Specification<User>
{
    private readonly string _province;

    public UserByProvinceSpecification(string province)
    {
        _province = province.ToLower();
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        // Since CorrespondenceAddress is now a simple string, we can't filter by province
        // This specification would need to be removed or reimplemented differently
        return user => user.CorrespondenceAddress.ToLower().Contains(_province);
    }
}

public class UserByCitySpecification : Specification<User>
{
    private readonly string _city;

    public UserByCitySpecification(string city)
    {
        _city = city.ToLower();
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        // Since CorrespondenceAddress is now a simple string, we can't filter by city
        // This specification would need to be removed or reimplemented differently
        return user => user.CorrespondenceAddress.ToLower().Contains(_city);
    }
}

public class UserByPartialEmailSpecification : Specification<User>
{
    private readonly string _emailPart;

    public UserByPartialEmailSpecification(string emailPart)
    {
        _emailPart = emailPart.ToLower();
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Email.ToLower().Contains(_emailPart);
    }
}

// Composite specifications for common scenarios
public static class UserSpecificationExtensions
{
    public static ISpecification<User> GetActiveUsers(int daysThreshold = 30)
    {
        return new UserByRecentActivitySpecification(daysThreshold);
    }

    public static ISpecification<User> GetUsersByProvince(string province)
    {
        return new UserByProvinceSpecification(province);
    }

    public static ISpecification<User> GetUsersByCity(string city)
    {
        return new UserByCitySpecification(city);
    }

    public static ISpecification<User> SearchUsers(string searchTerm)
    {
        // Search by full name
        Specification<User> fullNameSpec = new UserByFullNameSpecification(searchTerm);
        
        // Search by partial email
        Specification<User> emailSpec = new UserByPartialEmailSpecification(searchTerm);
        
        // Combine both specifications
        return fullNameSpec.Or(emailSpec);
    }
}