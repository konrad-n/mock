using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Storage;

namespace SledzSpecke.Tests.TestUtilities
{
    /// <summary>
    /// Service provider for tests that manages the test service instances.
    /// </summary>
    public static class TestServiceProvider
    {
        private static IFileSystemService fileSystemService;
        private static ISecureStorageService secureStorageService;

        static TestServiceProvider()
        {
            // Default implementations
            fileSystemService = new TestFileSystemService(useInMemoryStorage: true);
            secureStorageService = new TestSecureStorageService();
        }

        /// <summary>
        /// Sets the file system service to use in tests.
        /// </summary>
        /// <param name="service">The file system service implementation.</param>
        public static void SetFileSystemService(IFileSystemService service)
        {
            fileSystemService = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Sets the secure storage service to use in tests.
        /// </summary>
        /// <param name="service">The secure storage service implementation.</param>
        public static void SetSecureStorageService(ISecureStorageService service)
        {
            secureStorageService = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Gets the current file system service.
        /// </summary>
        /// <returns>The file system service implementation.</returns>
        public static IFileSystemService GetFileSystemService()
        {
            return fileSystemService;
        }

        /// <summary>
        /// Gets the current secure storage service.
        /// </summary>
        /// <returns>The secure storage service implementation.</returns>
        public static ISecureStorageService GetSecureStorageService()
        {
            return secureStorageService;
        }
    }
}