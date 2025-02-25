using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Authentication;
using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.Services.Platform
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
                return await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var availability = await BiometricAuthentication.GetAvailabilityAsync();
                    return availability == BiometricAvailability.Available;
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
                return await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var result = await BiometricAuthentication.AuthenticateAsync(reason);
                    return result == BiometricAuthenticationResult.Succeeded;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating with biometrics");
                return false;
            }
        }

        public async Task<Core.Interfaces.Services.BiometricType> GetBiometricTypeAsync()
        {
            try
            {
                return await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // In a real implementation, you would detect the platform and return the appropriate type
                    if (DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        // On iOS check if Face ID is available
                        var isFaceIdAvailable = await BiometricAuthentication.IsSupportedAsync(
                            BiometricAuthenticationType.FaceId);

                        if (isFaceIdAvailable)
                        {
                            return Core.Interfaces.Services.BiometricType.FaceId;
                        }
                        else
                        {
                            return Core.Interfaces.Services.BiometricType.TouchId;
                        }
                    }
                    else if (DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        // Android may use fingerprint or face recognition
                        return Core.Interfaces.Services.BiometricType.Fingerprint;
                    }

                    return Core.Interfaces.Services.BiometricType.None;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting biometric type");
                return Core.Interfaces.Services.BiometricType.None;
            }
        }
    }
}
