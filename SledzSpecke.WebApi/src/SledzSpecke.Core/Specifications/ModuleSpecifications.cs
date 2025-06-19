using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public sealed class ModuleByIdSpecification : Specification<Module>
{
    private readonly ModuleId _id;

    public ModuleByIdSpecification(ModuleId id)
    {
        _id = id;
    }

    public override Expression<Func<Module, bool>> ToExpression()
    {
        return module => module.Id == _id;
    }
}

public sealed class ModuleBySpecializationSpecification : Specification<Module>
{
    private readonly SpecializationId _specializationId;

    public ModuleBySpecializationSpecification(SpecializationId specializationId)
    {
        _specializationId = specializationId;
    }

    public override Expression<Func<Module, bool>> ToExpression()
    {
        return module => module.SpecializationId == _specializationId;
    }
}

public sealed class ModuleByTypeSpecification : Specification<Module>
{
    private readonly ModuleType _moduleType;

    public ModuleByTypeSpecification(ModuleType moduleType)
    {
        _moduleType = moduleType;
    }

    public override Expression<Func<Module, bool>> ToExpression()
    {
        return module => module.Type == _moduleType;
    }
}

public sealed class ModuleByDateRangeSpecification : Specification<Module>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public ModuleByDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<Module, bool>> ToExpression()
    {
        return module => module.StartDate >= _startDate && module.EndDate <= _endDate;
    }
}

public static class ModuleSpecificationExtensions
{
    public static ISpecification<Module> GetBasicModulesForSpecialization(SpecializationId specializationId)
    {
        return new ModuleBySpecializationSpecification(specializationId)
            .And(new ModuleByTypeSpecification(ModuleType.Basic));
    }

    public static ISpecification<Module> GetSpecialistModulesForSpecialization(SpecializationId specializationId)
    {
        return new ModuleBySpecializationSpecification(specializationId)
            .And(new ModuleByTypeSpecification(ModuleType.Specialist));
    }

    public static ISpecification<Module> GetModulesInDateRange(SpecializationId specializationId, DateTime startDate, DateTime endDate)
    {
        return new ModuleBySpecializationSpecification(specializationId)
            .And(new ModuleByDateRangeSpecification(startDate, endDate));
    }
}