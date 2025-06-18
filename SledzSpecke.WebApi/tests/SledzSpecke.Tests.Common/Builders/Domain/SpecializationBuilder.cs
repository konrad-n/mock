using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System;

namespace SledzSpecke.Tests.Common.Builders.Domain;

public class SpecializationBuilder : TestDataBuilder<Specialization>
{
    private int _id = 1;
    private int _userId = 1;
    private string _name = "Kardiologia";
    private string _smkVersion = "new";
    private DateTime _startDate = DateTime.Today.AddMonths(-6);
    private DateTime _plannedEndDate = DateTime.Today.AddYears(5);
    private string _supervisorName;
    
    public SpecializationBuilder()
    {
        _supervisorName = GeneratePolishDoctorName();
    }
    
    public SpecializationBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public SpecializationBuilder ForUser(int userId)
    {
        _userId = userId;
        return this;
    }
    
    public SpecializationBuilder AsCardiology()
    {
        _name = "Kardiologia";
        return this;
    }
    
    public SpecializationBuilder AsAnesthesiology()
    {
        _name = "Anestezjologia i intensywna terapia";
        return this;
    }
    
    public SpecializationBuilder AsInternalMedicine()
    {
        _name = "Choroby wewnętrzne";
        return this;
    }
    
    public SpecializationBuilder WithSmkVersion(string version)
    {
        _smkVersion = version;
        return this;
    }
    
    public SpecializationBuilder StartedOn(DateTime date)
    {
        _startDate = date;
        _plannedEndDate = date.AddYears(5);
        return this;
    }
    
    private string GeneratePolishDoctorName()
    {
        var titles = new[] { "dr hab.", "prof. dr hab." };
        var firstNames = new[] 
        { 
            "Anna", "Maria", "Katarzyna", "Małgorzata", "Agnieszka",
            "Piotr", "Krzysztof", "Andrzej", "Tomasz", "Paweł"
        };
        var lastNames = new[] 
        { 
            "Nowak", "Kowalski", "Wiśniewski", "Wójcik", "Kowalczyk",
            "Kamiński", "Lewandowski", "Zieliński", "Szymański", "Woźniak"
        };
        
        return $"{Faker.PickRandom(titles)} {Faker.PickRandom(firstNames)} {Faker.PickRandom(lastNames)}";
    }
    
    public override Specialization Build()
    {
        var specialization = _id > 0
            ? new Specialization(
                id: new SpecializationId(_id),
                userId: new UserId(_userId),
                name: _name,
                programCode: $"CODE-{_name.ToUpper().Replace(" ", "")}",
                smkVersion: SmkVersion.New,
                programVariant: "standard",
                startDate: _startDate,
                plannedEndDate: _plannedEndDate,
                plannedPesYear: _startDate.Year,
                programStructure: "5-letnia specjalizacja",
                durationYears: 5
            )
            : new Specialization(
                userId: new UserId(_userId),
                name: _name,
                programCode: $"CODE-{_name.ToUpper().Replace(" ", "")}",
                smkVersion: SmkVersion.New,
                programVariant: "standard",
                startDate: _startDate,
                plannedEndDate: _plannedEndDate,
                plannedPesYear: _startDate.Year,
                programStructure: "5-letnia specjalizacja",
                durationYears: 5
            );
            
        return specialization;
    }
}