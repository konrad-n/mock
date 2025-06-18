using SledzSpecke.Core.Entities;
using System;

namespace SledzSpecke.Tests.Common.Builders.Domain;

public class ModuleBuilder : TestDataBuilder<Module>
{
    private int _id = 1;
    private int _specializationId = 1;
    private string _name = "Moduł podstawowy - choroby wewnętrzne";
    private string _type = "basic";
    private DateTime _startDate = DateTime.Today.AddMonths(-3);
    private DateTime? _plannedEndDate = DateTime.Today.AddMonths(21);
    
    public ModuleBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public ModuleBuilder ForSpecialization(int specializationId)
    {
        _specializationId = specializationId;
        return this;
    }
    
    public ModuleBuilder AsBasicModule()
    {
        _name = "Moduł podstawowy - choroby wewnętrzne";
        _type = "basic";
        return this;
    }
    
    public ModuleBuilder AsSpecialisticModule(string name)
    {
        _name = name;
        _type = "specialistic";
        return this;
    }
    
    public ModuleBuilder StartedOn(DateTime date)
    {
        _startDate = date;
        _plannedEndDate = date.AddMonths(24);
        return this;
    }
    
    public ModuleBuilder WithDuration(int months)
    {
        _plannedEndDate = _startDate.AddMonths(months);
        return this;
    }
    
    public override Module Build()
    {
        return new Module
        {
            Id = _id,
            SpecializationId = _specializationId,
            Name = _name,
            Type = _type,
            StartDate = _startDate,
            PlannedEndDate = _plannedEndDate,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}