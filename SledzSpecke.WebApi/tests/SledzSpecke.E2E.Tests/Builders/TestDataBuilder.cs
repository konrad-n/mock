using Bogus;
using Bogus.Extensions;

namespace SledzSpecke.E2E.Tests.Builders;

/// <summary>
/// Base class for test data builders following Builder pattern and Open/Closed Principle
/// </summary>
public abstract class TestDataBuilder<T> where T : class
{
    protected readonly Faker Faker;
    
    protected TestDataBuilder()
    {
        // Use Polish locale for realistic test data
        Faker = new Faker("pl");
    }
    
    public abstract T Build();
    
    public List<T> BuildMany(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => Build())
            .ToList();
    }
}

/// <summary>
/// Builder for creating test users with Polish medical context
/// </summary>
public class TestUserBuilder : TestDataBuilder<TestUser>
{
    private string? _username;
    private string? _password;
    private string? _email;
    private string? _firstName;
    private string? _lastName;
    private string? _specialization;
    private int? _year;
    private string? _pwz; // Medical license number

    public TestUserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public TestUserBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public TestUserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public TestUserBuilder WithFullName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }

    public TestUserBuilder WithSpecialization(string specialization, int year)
    {
        _specialization = specialization;
        _year = year;
        return this;
    }

    public TestUserBuilder WithMedicalLicense(string pwz)
    {
        _pwz = pwz;
        return this;
    }

    public override TestUser Build()
    {
        return new TestUser
        {
            Username = _username ?? Faker.Internet.UserName(),
            Password = _password ?? "Test123!",
            Email = _email ?? Faker.Internet.Email(),
            FirstName = _firstName ?? Faker.Name.FirstName(),
            LastName = _lastName ?? Faker.Name.LastName(),
            Specialization = _specialization ?? GetRandomSpecialization(),
            Year = _year ?? Faker.Random.Int(1, 6),
            PWZ = _pwz ?? GeneratePWZ()
        };
    }

    private string GetRandomSpecialization()
    {
        var specializations = new[]
        {
            "Anestezjologia i intensywna terapia",
            "Chirurgia ogólna",
            "Choroby wewnętrzne",
            "Medycyna rodzinna",
            "Pediatria",
            "Psychiatria",
            "Radiologia i diagnostyka obrazowa"
        };
        
        return Faker.PickRandom(specializations);
    }

    private string GeneratePWZ()
    {
        // PWZ format: 7 digits
        return Faker.Random.Replace("#######");
    }
}

/// <summary>
/// Builder for medical shift test data following SMK requirements
/// </summary>
public class MedicalShiftBuilder : TestDataBuilder<MedicalShiftTestData>
{
    private DateTime? _date;
    private TimeSpan? _startTime;
    private TimeSpan? _endTime;
    private string? _type;
    private string? _place;
    private string? _description;
    private bool _isNightShift;
    private bool _isWeekend;

    public MedicalShiftBuilder OnDate(DateTime date)
    {
        _date = date;
        return this;
    }

    public MedicalShiftBuilder WithTimes(string startTime, string endTime)
    {
        _startTime = TimeSpan.Parse(startTime);
        _endTime = TimeSpan.Parse(endTime);
        return this;
    }

    public MedicalShiftBuilder AsNightShift()
    {
        _isNightShift = true;
        return this;
    }

    public MedicalShiftBuilder AsWeekendShift()
    {
        _isWeekend = true;
        return this;
    }

    public MedicalShiftBuilder AtPlace(string place)
    {
        _place = place;
        return this;
    }

    public MedicalShiftBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public MedicalShiftBuilder OfType(string type)
    {
        _type = type;
        return this;
    }

    public override MedicalShiftTestData Build()
    {
        var date = _date ?? GenerateShiftDate();
        
        if (_isNightShift && !_startTime.HasValue)
        {
            _startTime = new TimeSpan(19, 0, 0);
            _endTime = new TimeSpan(7, 0, 0);
        }
        else if (!_startTime.HasValue)
        {
            var shiftStart = Faker.Random.Int(6, 8);
            _startTime = new TimeSpan(shiftStart, 0, 0);
            _endTime = new TimeSpan(shiftStart + 8, 0, 0);
        }
        
        return new MedicalShiftTestData
        {
            Date = date,
            StartTime = _startTime.Value.ToString(@"hh\:mm"),
            EndTime = _endTime.Value.ToString(@"hh\:mm"),
            Type = _type ?? DetermineShiftType(),
            Place = _place ?? GenerateHospitalDepartment(),
            Description = _description ?? GenerateShiftDescription(),
            IsWeekend = _isWeekend || (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday),
            IsNightShift = _isNightShift
        };
    }

    private DateTime GenerateShiftDate()
    {
        if (_isWeekend)
        {
            // Find next weekend
            var date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }
            return date;
        }
        
        return Faker.Date.Between(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(30));
    }

    private string DetermineShiftType()
    {
        if (_isNightShift) return "on-call";
        if (_isWeekend) return "weekend";
        return "regular";
    }

    private string GenerateHospitalDepartment()
    {
        var departments = new[]
        {
            "Szpital Wojewódzki - Oddział Anestezjologii",
            "Szpital Kliniczny - Blok Operacyjny",
            "Centrum Medyczne - SOR",
            "Szpital Miejski - OIOM",
            "Klinika Uniwersytecka - Oddział Kardioanestezji",
            "Szpital Specjalistyczny - Neuroanestezja"
        };
        
        return Faker.PickRandom(departments);
    }

    private string GenerateShiftDescription()
    {
        var descriptions = new[]
        {
            "Dyżur w ramach specjalizacji",
            "Dyżur medyczny - anestezjologia",
            "Praca na bloku operacyjnym",
            "Dyżur na oddziale intensywnej terapii",
            "Zabezpieczenie anestezjologiczne"
        };
        
        return Faker.PickRandom(descriptions);
    }
}

// Test data models
public class TestUser
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Specialization { get; set; } = "";
    public int Year { get; set; }
    public string PWZ { get; set; } = "";
}

public class MedicalShiftTestData
{
    public DateTime Date { get; set; }
    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";
    public string Type { get; set; } = "";
    public string Place { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsWeekend { get; set; }
    public bool IsNightShift { get; set; }
    
    public int CalculatedHours
    {
        get
        {
            if (TimeSpan.TryParse(StartTime, out var start) && TimeSpan.TryParse(EndTime, out var end))
            {
                var duration = end - start;
                if (duration.TotalHours < 0) // Overnight shift
                {
                    duration = duration.Add(TimeSpan.FromHours(24));
                }
                return (int)duration.TotalHours;
            }
            return 0;
        }
    }
}