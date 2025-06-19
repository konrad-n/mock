using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System;

namespace SledzSpecke.Tests.Common.Builders.Domain;

public class UserBuilder : TestDataBuilder<User>
{
    private int _id = 1;
    private string _email;
    private string _firstName;
    private string _lastName;
    private string _street;
    private string _houseNumber;
    private string _apartmentNumber;
    private string _postalCode;
    private string _city;
    private string _phoneNumber;
    private string _university;
    private int _graduationYear;
    private string _passwordHash = "$2a$10$xQxWKnQZ1cNQvMkNjXmhCOZdXKPM/0xO7qCnLV9yoFT1cKcUPl6Pu"; // Test123!
    
    // Polish medical universities
    private readonly string[] _medicalUniversities = new[]
    {
        "Uniwersytet Medyczny w Warszawie",
        "Collegium Medicum Uniwersytetu Jagiellońskiego",
        "Uniwersytet Medyczny w Poznaniu",
        "Gdański Uniwersytet Medyczny",
        "Śląski Uniwersytet Medyczny w Katowicach",
        "Uniwersytet Medyczny we Wrocławiu",
        "Uniwersytet Medyczny w Lublinie",
        "Uniwersytet Medyczny w Łodzi",
        "Collegium Medicum UMK w Bydgoszczy",
        "Pomorski Uniwersytet Medyczny w Szczecinie"
    };
    
    public UserBuilder()
    {
        _email = Faker.Internet.Email();
        _firstName = Faker.Name.FirstName();
        _lastName = Faker.Name.LastName();
        
        // Polish address
        _street = Faker.PickRandom(new[] 
        { 
            "ul. Marszałkowska", "ul. Nowy Świat", "ul. Krakowskie Przedmieście",
            "ul. Grunwaldzka", "ul. Piotrkowska", "ul. Świętojańska",
            "al. Niepodległości", "ul. 3 Maja", "ul. Długa"
        });
        _houseNumber = Faker.Random.Int(1, 200).ToString();
        _apartmentNumber = Faker.Random.Bool() ? Faker.Random.Int(1, 50).ToString() : null;
        _postalCode = Faker.Random.Replace("##-###");
        _city = Faker.PickRandom(new[] 
        { 
            "Warszawa", "Kraków", "Łódź", "Wrocław", "Poznań", 
            "Gdańsk", "Szczecin", "Bydgoszcz", "Lublin", "Białystok"
        });
        _phoneNumber = Faker.Phone.PhoneNumber("+48 ### ### ###");
        _university = Faker.PickRandom(_medicalUniversities);
        _graduationYear = DateTime.Now.Year - Faker.Random.Int(1, 5);
    }
    
    public UserBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }
    
    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }
    
    public UserBuilder AsCardiologyResident()
    {
        _email = $"{_firstName.ToLower()}.{_lastName.ToLower()}@kardiologia.pl";
        return this;
    }
    
    public UserBuilder AsAnesthesiologyResident()
    {
        _email = $"{_firstName.ToLower()}.{_lastName.ToLower()}@anestezjologia.pl";
        return this;
    }
    
    public UserBuilder FromSpecificUniversity(string universityName)
    {
        _university = universityName;
        return this;
    }
    
    public UserBuilder WithSpecificAddress(string city, string postalCode)
    {
        _city = city;
        _postalCode = postalCode;
        return this;
    }
    
    
    public override User Build()
    {
        // Generate a date of birth based on graduation year (assuming graduation at age 25)
        var dateOfBirth = new DateTime(_graduationYear - 25, Faker.Random.Int(1, 12), Faker.Random.Int(1, 28));
        
        var user = _id > 0
            ? User.CreateWithId(
                id: new UserId(_id),
                email: new Email(_email),
                password: new HashedPassword(_passwordHash),
                firstName: new FirstName(_firstName),
                secondName: null,
                lastName: new LastName(_lastName),
                phoneNumber: new PhoneNumber(_phoneNumber),
                dateOfBirth: dateOfBirth,
                correspondenceAddress: new Address(
                    street: _street,
                    houseNumber: _houseNumber,
                    apartmentNumber: _apartmentNumber,
                    postalCode: _postalCode,
                    city: _city,
                    province: "Mazowieckie",
                    country: "Polska"
                ),
                registrationDate: DateTime.UtcNow
            )
            : User.Create(
                email: new Email(_email),
                password: new HashedPassword(_passwordHash),
                firstName: new FirstName(_firstName),
                secondName: null,
                lastName: new LastName(_lastName),
                phoneNumber: new PhoneNumber(_phoneNumber),
                dateOfBirth: dateOfBirth,
                correspondenceAddress: new Address(
                    street: _street,
                    houseNumber: _houseNumber,
                    apartmentNumber: _apartmentNumber,
                    postalCode: _postalCode,
                    city: _city,
                    province: "Mazowieckie",
                    country: "Polska"
                )
            );

        // ID will be set by repository when saving if it's 0
        
        return user;
    }
    
    // Create a complete test scenario user
    public static User CreateCompleteTestUser(string specialization = "Kardiologia")
    {
        return new UserBuilder()
            .WithEmail($"jan.kowalski@{specialization.ToLower()}.test.pl")
            .Build();
    }
}