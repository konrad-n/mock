namespace SledzSpecke.App.Services.Storage
{
    /// <summary>
    /// Interface for secure storage operations, allowing for better testability.
    /// </summary>
    public interface ISecureStorageService
    {
        /// <summary>
        /// Gets a value from secure storage.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The stored value, or null if not found.</returns>
        Task<string> GetAsync(string key);

        /// <summary>
        /// Sets a value in secure storage.
        /// </summary>
        /// <param name="key">The key to store under.</param>
        /// <param name="value">The value to store.</param>
        Task SetAsync(string key, string value);

        /// <summary>
        /// Removes a value from secure storage.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        void Remove(string key);

        /// <summary>
        /// Removes all values from secure storage.
        /// </summary>
        void RemoveAll();
    }
}
