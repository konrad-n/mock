namespace SledzSpecke.Tests.TestUtilities
{
    /// <summary>
    /// Mock implementation of settings helper for testing.
    /// </summary>
    public static class TestSettings
    {
        private static readonly Dictionary<string, string> SettingsStorage = new Dictionary<string, string>();

        /// <summary>
        /// Clears all stored settings.
        /// </summary>
        public static void ClearSettings()
        {
            SettingsStorage.Clear();
        }

        /// <summary>
        /// Gets a setting value by key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value if found, or null if not found.</returns>
        public static Task<string> GetAsync(string key)
        {
            if (SettingsStorage.TryGetValue(key, out string value))
            {
                return Task.FromResult(value);
            }
            return Task.FromResult<string>(null);
        }

        /// <summary>
        /// Sets a setting value.
        /// </summary>
        /// <param name="key">The key to store under.</param>
        /// <param name="value">The value to store.</param>
        public static Task SetAsync(string key, string value)
        {
            SettingsStorage[key] = value;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a specific setting.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public static void Remove(string key)
        {
            if (SettingsStorage.ContainsKey(key))
            {
                SettingsStorage.Remove(key);
            }
        }

        /// <summary>
        /// Removes all settings.
        /// </summary>
        public static void RemoveAll()
        {
            SettingsStorage.Clear();
        }
    }
}