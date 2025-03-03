using Microsoft.Extensions.Logging;

namespace SledzSpecke.App
{
    public partial class App : Application
    {
        private readonly ILogger<App> logger;
        private readonly IServiceProvider serviceProvider;

        public App(
            IServiceProvider serviceProvider,
            ILogger<App> logger)
        {
            this.InitializeComponent();

            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
    }
}
