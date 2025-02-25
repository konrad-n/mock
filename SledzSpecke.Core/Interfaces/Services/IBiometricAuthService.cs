using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IBiometricAuthService
    {
        Task<bool> IsBiometricAvailableAsync();
        Task<bool> AuthenticateAsync(string reason);
        Task<BiometricType> GetBiometricTypeAsync();
    }

    public enum BiometricType
    {
        None,
        Fingerprint,
        FaceId,
        TouchId,
        Iris
    }
}