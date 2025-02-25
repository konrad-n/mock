using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.Core.Services.Authentication
{
    public class BiometricAuthService : IBiometricAuthService
    {
        private readonly ILogger<BiometricAuthService> _logger;
        
        public BiometricAuthService(ILogger<BiometricAuthService> logger)
        {
            _logger = logger;
        }
        
        public async Task<bool> IsBiometricAvailableAsync()
        {
            try
            {
                return await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var availability = await Microsoft.Maui.Authentication.BiometricAuthentication.GetAvailabilityAsync();
                    return availability == Microsoft.Maui.Authentication.BiometricAvailability.Available;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking biometric availability");
                return false;
            }
        }
        
        public async Task<bool> AuthenticateAsync(string reason)
        {
            try
            {
                return await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var result = await Microsoft.Maui.Authentication.BiometricAuthentication.AuthenticateAsync(reason);
                    return result == Microsoft.Maui.Authentication.BiometricAuthenticationResult.Succeeded;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating with biometrics");
                return false;
            }
        }
        
        public async Task<BiometricType> GetBiometricTypeAsync()
        {
            try
            {
                return await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // W zależności od platformy zwróć odpowiedni typ biometryczny
                    if (Microsoft.Maui.Devices.DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.iOS)
                    {
                        // Na iOS sprawdź, czy jest dostępny Face ID
                        var isFaceIdAvailable = await Microsoft.Maui.Authentication.BiometricAuthentication.IsSupportedAsync(
                            Microsoft.Maui.Authentication.BiometricAuthenticationType.FaceId);
                        
                        if (isFaceIdAvailable)
                        {
                            return BiometricType.FaceId;
                        }
                        else
                        {
                            return BiometricType.TouchId;
                        }
                    }
                    else if (Microsoft.Maui.Devices.DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.Android)
                    {
                        // Na Androidzie może być odcisk palca lub rozpoznawanie twarzy
                        return BiometricType.Fingerprint;
                    }
                    
                    return BiometricType.None;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting biometric type");
                return BiometricType.None;
            }
        }
    }
    
    public enum BiometricType
    {
        None,
        Fingerprint,
        FaceId,
        TouchId,
        Iris
    }
    
    public interface IBiometricAuthService
    {
        Task<bool> IsBiometricAvailableAsync();
        Task<bool> AuthenticateAsync(string reason);
        Task<BiometricType> GetBiometricTypeAsync();
    }
}
