using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Enums;
using System;

namespace SledzSpecke.Tests.Common.Builders.Domain;

public class InternshipBuilder : TestDataBuilder<Internship>
{
    private int _id = 1;
    private int _specializationId = 1;
    private int _moduleId = 1;
    private string _name = "Oddział Chorób Wewnętrznych";
    private string _hospital;
    private string _department = "Oddział Kardiologii";
    private DateTime _startDate = DateTime.Today.AddMonths(-1);
    private DateTime? _plannedEndDate = DateTime.Today.AddMonths(2);
    private int _plannedWeeks = 12;
    
    public InternshipBuilder()
    {
        _hospital = GeneratePolishHospital();
    }
    
    public InternshipBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public InternshipBuilder ForModule(int moduleId)
    {
        _moduleId = moduleId;
        return this;
    }
    
    public InternshipBuilder ForSpecialization(int specializationId)
    {
        _specializationId = specializationId;
        return this;
    }
    
    public InternshipBuilder AtHospital(string hospital)
    {
        _hospital = hospital;
        return this;
    }
    
    public InternshipBuilder InDepartment(string department)
    {
        _department = department;
        return this;
    }
    
    public InternshipBuilder StartingOn(DateTime date)
    {
        _startDate = date;
        _plannedEndDate = date.AddDays(_plannedWeeks * 7);
        return this;
    }
    
    public InternshipBuilder WithDurationWeeks(int weeks)
    {
        _plannedWeeks = weeks;
        _plannedEndDate = _startDate.AddDays(weeks * 7);
        return this;
    }
    
    private string GeneratePolishHospital()
    {
        var hospitals = new[]
        {
            "Szpital Uniwersytecki w Krakowie",
            "Centrum Zdrowia Dziecka w Warszawie",
            "Szpital Kliniczny im. Heliodora Święcickiego w Poznaniu",
            "Uniwersytecki Szpital Kliniczny we Wrocławiu",
            "Szpital Specjalistyczny im. S. Żeromskiego w Krakowie",
            "Samodzielny Publiczny Szpital Kliniczny Nr 1 w Lublinie",
            "Wojewódzki Szpital Specjalistyczny im. M. Kopernika w Łodzi",
            "Mazowiecki Szpital Specjalistyczny w Radomiu",
            "Szpital Kliniczny Przemienienia Pańskiego w Poznaniu",
            "Centralny Szpital Kliniczny MSWiA w Warszawie"
        };
        
        return Faker.PickRandom(hospitals);
    }
    
    public override Internship Build()
    {
        var internship = Internship.Create(
            specializationId: _specializationId,
            name: _name,
            institutionName: _hospital,
            departmentName: _department,
            startDate: _startDate,
            endDate: _plannedEndDate ?? _startDate.AddMonths(3),
            plannedWeeks: _plannedWeeks,
            plannedDays: _plannedWeeks * 5 // Assuming 5 days per week
        );

        // Set the ID if needed (using reflection since EF Core needs it)
        if (_id != 0)
        {
            var idProperty = internship.GetType().GetProperty("InternshipId");
            idProperty?.SetValue(internship, _id);
        }

        // Assign to module if provided
        if (_moduleId > 0)
        {
            internship.AssignToModule(_moduleId);
        }

        return internship;
    }
}