using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Policies.MedicalShift;
using SledzSpecke.Core.Policies.Procedure;

namespace SledzSpecke.Core.Policies;

public interface ISmkPolicyFactory
{
    ISmkPolicy<T> GetPolicy<T>(SmkVersion version);
}

public class SmkPolicyFactory : ISmkPolicyFactory
{
    private readonly Dictionary<(Type, SmkVersion), object> _policies;

    public SmkPolicyFactory()
    {
        _policies = new Dictionary<(Type, SmkVersion), object>
        {
            // Medical Shift Policies
            { (typeof(Entities.MedicalShift), SmkVersion.Old), new OldSmkMedicalShiftPolicy() },
            { (typeof(Entities.MedicalShift), SmkVersion.New), new NewSmkMedicalShiftPolicy() },
            
            // Procedure Policies
            { (typeof(Entities.ProcedureBase), SmkVersion.Old), new OldSmkProcedurePolicy() },
            { (typeof(Entities.ProcedureBase), SmkVersion.New), new NewSmkProcedurePolicy() },
        };
    }

    public ISmkPolicy<T> GetPolicy<T>(SmkVersion version)
    {
        var key = (typeof(T), version);
        
        if (_policies.TryGetValue(key, out var policy))
        {
            return (ISmkPolicy<T>)policy;
        }

        throw new NotSupportedException($"No policy found for type {typeof(T).Name} and SMK version {version}");
    }
}