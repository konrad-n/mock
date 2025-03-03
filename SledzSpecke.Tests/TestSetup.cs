using NUnit.Framework;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Storage;
using SledzSpecke.Tests.TestUtilities;

namespace SledzSpecke.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            // Initialize test implementations of services
            var fileSystemService = new TestFileSystemService(useInMemoryStorage: true);
            var secureStorageService = new TestSecureStorageService();

            // Set these implementations as the ones to use
            Constants.SetFileSystemService(fileSystemService);
            Settings.SetSecureStorageService(secureStorageService);
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            // Clean up any resources if needed
        }
    }
}