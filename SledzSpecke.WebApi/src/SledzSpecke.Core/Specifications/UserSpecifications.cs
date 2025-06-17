using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

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

public class UserByPeselSpecification : Specification<User>
{
    private readonly Pesel _pesel;

    public UserByPeselSpecification(Pesel pesel)
    {
        _pesel = pesel;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Pesel == _pesel;
    }
}

public class UserByPwzNumberSpecification : Specification<User>
{
    private readonly PwzNumber _pwzNumber;

    public UserByPwzNumberSpecification(PwzNumber pwzNumber)
    {
        _pwzNumber = pwzNumber;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.PwzNumber == _pwzNumber;
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
        // Search by first name, last name, or full name
        return user => user.FirstName.Value.ToLower().Contains(_searchTerm) ||
                      user.LastName.Value.ToLower().Contains(_searchTerm) ||
                      (user.FirstName.Value + " " + user.LastName.Value).ToLower().Contains(_searchTerm);
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
        return user => user.CorrespondenceAddress.Province.ToLower() == _province;
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
        return user => user.CorrespondenceAddress.City.ToLower() == _city;
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
        return user => user.Email.Value.ToLower().Contains(_emailPart);
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