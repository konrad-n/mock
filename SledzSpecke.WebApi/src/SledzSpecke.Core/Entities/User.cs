using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class User
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public FullName FullName { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public DateTime RegistrationDate { get; private set; }

    private User() { }

    public User(UserId id, Email email, Username username, Password password, FullName fullName,
        SmkVersion smkVersion, SpecializationId specializationId, DateTime registrationDate)
    {
        Id = id;
        Email = email;
        Username = username;
        Password = password;
        FullName = fullName;
        SmkVersion = smkVersion;
        SpecializationId = specializationId;
        RegistrationDate = registrationDate;
    }

    public User(Email email, Username username, Password password, FullName fullName,
        SmkVersion smkVersion, SpecializationId specializationId, DateTime registrationDate)
    {
        Email = email;
        Username = username;
        Password = password;
        FullName = fullName;
        SmkVersion = smkVersion;
        SpecializationId = specializationId;
        RegistrationDate = registrationDate;
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
    }

    public void UpdateProfile(Email email, FullName fullName)
    {
        Email = email;
        FullName = fullName;
    }
}