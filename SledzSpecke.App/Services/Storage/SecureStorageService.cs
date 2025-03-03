namespace SledzSpecke.App.Services.Storage
{

    /// <summary>
    /// Implementation of ISecureStorageService that uses the MAUI SecureStorage.
    /// </summary>
    public class SecureStorageService : ISecureStorageService
    {
        public async Task<string> GetAsync(string key)
        {
            try
            {
                return await SecureStorage.GetAsync(key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading from secure storage: {ex.Message}");
                return null;
            }
        }

        public async Task SetAsync(string key, string value)
        {
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