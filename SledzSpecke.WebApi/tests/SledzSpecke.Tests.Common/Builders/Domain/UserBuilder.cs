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
    private string _pesel;
    private string _pwz;
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
        _pesel = GenerateValidPesel();
        _pwz = GenerateValidPwz();
        
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
    
    private string GenerateValidPesel()
    {
        // Generate valid PESEL (Polish national ID)
        var year = Faker.Random.Int(85, 99);
        var month = Faker.Random.Int(1, 12);
        var day = Faker.Random.Int(1, 28);
        var serial = Faker.Random.Int(100, 999);
        var sex = Faker.Random.Int(1, 9);
        
        var peselWithoutChecksum = $"{year:D2}{month:D2}{day:D2}{serial:D3}{sex}";
        var weights = new[] { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        var sum = 0;
        
        for (int i = 0; i < 10; i++)
        {
            sum += int.Parse(peselWithoutChecksum[i].ToString()) * weights[i];
        }
        
        var checksum = (10 - (sum % 10)) % 10;
        return peselWithoutChecksum + checksum;
    }
    
    private string GenerateValidPwz()
    {
        // Generate valid PWZ (Polish medical license)
        return Faker.Random.Replace("#######");
    }
    
    public override User Build()
    {
        // Ensure we have a valid date of birth that matches the PESEL
        var pesel = new Pesel(_pesel);
        var dateOfBirth = pesel.GetDateOfBirth();
        
        var user = _id > 0
            ? User.CreateWithId(
                id: new UserId(_id),
                email: new Email(_email),
                password: new HashedPassword(_passwordHash),
                firstName: new FirstName(_firstName),
                secondName: null,
                lastName: new LastName(_lastName),
                pesel: pesel,
                pwzNumber: new PwzNumber(_pwz),
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
                pesel: pesel,
                pwzNumber: new PwzNumber(_pwz),
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