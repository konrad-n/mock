using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Tests.Common.Builders.Domain;

public class MedicalShiftBuilder : TestDataBuilder<MedicalShift>
{
    private int _id = 1;
    private int _internshipId = 1;
    private DateTime _date = DateTime.Today;
    private int _hours = 8;
    private int _minutes = 0;
    private string _type = "Oddział";
    private string _location = "Szpital Uniwersytecki w Krakowie";
    private bool _isNightShift = false;
    
    public MedicalShiftBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public MedicalShiftBuilder WithInternship(int internshipId)
    {
        _internshipId = internshipId;
        return this;
    }
    
    public MedicalShiftBuilder WithDate(DateTime date)
    {
        _date = date;
        return this;
    }
    
    public MedicalShiftBuilder WithDuration(int hours, int minutes)
    {
        _hours = hours;
        _minutes = minutes;
        return this;
    }
    
    public MedicalShiftBuilder AsNightShift()
    {
        _isNightShift = true;
        _hours = 12;
        _minutes = 0;
        _type = "Dyżur nocny";
        return this;
    }
    
    public MedicalShiftBuilder AsICUShift()
    {
        _type = "Intensywna terapia";
        _location = "Oddział Intensywnej Terapii";
        return this;
    }
    
    public MedicalShiftBuilder AsERShift()
    {
        _type = "SOR";
        _location = "Szpitalny Oddział Ratunkowy";
        _hours = 10;
        _minutes = 0;
        return this;
    }
    
    public MedicalShiftBuilder WithRandomPolishHospital()
    {
        var hospitals = new[]
        {
            "Szpital Uniwersytecki w Krakowie",
            "Centrum Zdrowia Dziecka w Warszawie",
            "Szpital Kliniczny im. Heliodora Święcickiego w Poznaniu",
            "Uniwersytecki Szpital Kliniczny we Wrocławiu",
            "Szpital Specjalistyczny im. S. Żeromskiego w Krakowie",
            "Samodzielny Publiczny Szpital Kliniczny Nr 1 w Lublinie",
            "Wojewódzki Szpital Specjalistyczny im. M. Kopernika w Łodzi"
        };
        
        _location = Faker.PickRandom(hospitals);
        return this;
    }
    
    public override MedicalShift Build()
    {
        var shift = new MedicalShift
        {
            Id = _id,
            InternshipId = _internshipId,
            Date = _date,
            Hours = _hours,
            Minutes = _minutes,
            Type = _type,
            Location = _location,
            IsNightShift = _isNightShift,
            CreatedAt = DateTime.UtcNow
        };
        
        return shift;
    }
    
    // Builder for complete month of shifts
    public List<MedicalShift> BuildMonthlyRotation(int year, int month, int internshipId)
    {
        var shifts = new List<MedicalShift>();
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var currentId = _id;
        
        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);
            
            // Skip weekends unless it's a night shift
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                if (day % 7 == 0) // Every 7th day add weekend duty
                {
                    shifts.Add(new MedicalShiftBuilder()
                        .WithId(currentId++)
                        .WithInternship(internshipId)
                        .WithDate(date)
                        .AsNightShift()
                        .Build());
                }
                continue;
            }
            
            // Regular weekday shifts
            shifts.Add(new MedicalShiftBuilder()
                .WithId(currentId++)
                .WithInternship(internshipId)
                .WithDate(date)
                .WithDuration(8, 0)
                .WithRandomPolishHospital()
                .Build());
        }
        
        return shifts;
    }
}