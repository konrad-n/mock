using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.Entities;

public sealed class User
{
    public UserId? Id { get; private set; }
    public Email Email { get; private set; }
    public HashedPassword Password { get; private set; }
    public FirstName FirstName { get; private set; }
    public SecondName? SecondName { get; private set; }
    public LastName LastName { get; private set; }
    public Pesel Pesel { get; private set; }
    public PwzNumber PwzNumber { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Address CorrespondenceAddress { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public bool NotificationsEnabled { get; private set; }
    public bool EmailNotificationsEnabled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Private constructor for EF Core
    private User() { }

    // Factory method for creating a new user
    public static User Create(
        Email email, 
        HashedPassword password, 
        FirstName firstName,
        SecondName? secondName,
        LastName lastName,
        Pesel pesel,
        PwzNumber pwzNumber,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth,
        Address correspondenceAddress)
    {
        // Validate that date of birth matches PESEL
        var peselDateOfBirth = pesel.GetDateOfBirth();
        if (peselDateOfBirth.Date != dateOfBirth.Date)
            throw new DomainException("Date of birth does not match PESEL.");

        return new User
        {
            // Id will be set by repository using SetId method
            Email = email,
            Password = password,
            FirstName = firstName,
            SecondName = secondName,
            LastName = lastName,
            Pesel = pesel,
            PwzNumber = pwzNumber,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth,
            CorrespondenceAddress = correspondenceAddress,
            RegistrationDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            NotificationsEnabled = true,
            EmailNotificationsEnabled = true
        };
    }

    // Factory method for creating a user with known ID (e.g., from database)
    public static User CreateWithId(
        UserId id,
        Email email, 
        HashedPassword password, 
        FirstName firstName,
        SecondName? secondName,
        LastName lastName,
        Pesel pesel,
        PwzNumber pwzNumber,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth,
        Address correspondenceAddress,
        DateTime registrationDate)
    {
        return new User
        {
            Id = id,
            Email = email,
            Password = password,
            FirstName = firstName,
            SecondName = secondName,
            LastName = lastName,
            Pesel = pesel,
            PwzNumber = pwzNumber,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth,
            CorrespondenceAddress = correspondenceAddress,
            RegistrationDate = registrationDate,
            CreatedAt = registrationDate,
            NotificationsEnabled = true,
            EmailNotificationsEnabled = true
        };
    }
    
    // Internal method for repository to set ID after creation
    internal void SetId(UserId id)
    {
        if (Id != null)
            throw new InvalidOperationException("Cannot change ID once it's set");
        Id = id;
    }

    public void ChangePassword(HashedPassword newPassword)
    {
        if (newPassword == null)
            throw new ArgumentNullException(nameof(newPassword));
            
        Password = newPassword;
    }

    public void UpdateProfile(
        Email email, 
        FirstName firstName, 
        LastName lastName,
        PhoneNumber phoneNumber,
        Address correspondenceAddress)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));
        if (firstName == null)
            throw new ArgumentNullException(nameof(firstName));
        if (lastName == null)
            throw new ArgumentNullException(nameof(lastName));
        if (phoneNumber == null)
            throw new ArgumentNullException(nameof(phoneNumber));
        if (correspondenceAddress == null)
            throw new ArgumentNullException(nameof(correspondenceAddress));
            
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CorrespondenceAddress = correspondenceAddress;
    }

    public void UpdateNotificationPreferences(bool notificationsEnabled, bool emailNotificationsEnabled)
    {
        NotificationsEnabled = notificationsEnabled;
        EmailNotificationsEnabled = emailNotificationsEnabled;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
    
    // Business logic methods
    public string GetFullName()
    {
        if (SecondName != null && SecondName.HasValue)
            return $"{FirstName} {SecondName} {LastName}";
        return $"{FirstName} {LastName}";
    }
    
    public bool IsProfileComplete()
    {
        return true; // All required fields are mandatory during registration
    }
    
    public bool HasRecentActivity(int daysThreshold = 30)
    {
        return LastLoginAt.HasValue && 
               (DateTime.UtcNow - LastLoginAt.Value).TotalDays <= daysThreshold;
    }
}