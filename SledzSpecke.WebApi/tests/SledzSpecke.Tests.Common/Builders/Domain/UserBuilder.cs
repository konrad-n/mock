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
        var user = new User
        {
            Id = _id,
            Email = new Email(_email),
            FirstName = _firstName,
            LastName = _lastName,
            Pesel = new Pesel(_pesel),
            Pwz = new Pwz(_pwz),
            PasswordHash = _passwordHash,
            Address = new Address(
                _street,
                _houseNumber,
                _apartmentNumber,
                _postalCode,
                _city
            ),
            PhoneNumber = _phoneNumber,
            University = _university,
            GraduationYear = _graduationYear,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        
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