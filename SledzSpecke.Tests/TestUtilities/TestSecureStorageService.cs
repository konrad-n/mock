using SledzSpecke.App.Services.Storage;

namespace SledzSpecke.Tests.TestUtilities
{
    /// <summary>
    /// Test implementation of ISecureStorageService for unit testing.
    /// </summary>
    public class TestSecureStorageService : ISecureStorageService
    {
        private readonly Dictionary<string, string> storageDict = new Dictionary<string, string>();

        public async Task<string> GetAsync(string key)
        {
            if (this.storageDict.TryGetValue(key, out string value))
            {
                return value;
            }
            return null;
        }

        public async Task SetAsync(string key, string value)
        {
            this.storageDict[key] = value;
        }

        public void Remove(string key)
        {
            if (this.storageDict.ContainsKey(key))
            {
                this.storageDict.Remove(key);
            }
        }

        public void RemoveAll()
        {
            this.storageDict.Clear();
        }
    }
}
