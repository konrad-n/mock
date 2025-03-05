namespace SledzSpecke.App.Services.Storage
{

    /// <summary>
    /// Implementation of ISecureStorageService that uses the MAUI SecureStorage.
    /// </summary>
    public class SecureStorageService : ISecureStorageService
    {
        public async Task<string> GetAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            try
            {
                return await SecureStorage.GetAsync(key) ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading from secure storage: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task SetAsync(string key, string value)
        {
            ArgumentNullException.ThrowIfNull(key);
            value ??= string.Empty;

            try
            {
                await SecureStorage.SetAsync(key, value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error writing to secure storage: {ex.Message}");
                throw;
            }
        }

        public void Remove(string key)
        {
            try
            {
                SecureStorage.Remove(key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing from secure storage: {ex.Message}");
            }
        }

        public void RemoveAll()
        {
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing all from secure storage: {ex.Message}");
            }
        }
    }
}