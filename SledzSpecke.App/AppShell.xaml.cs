
namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        private readonly IServiceProvider serviceProvider;

        public AppShell(
            IServiceProvider serviceProvider)
        {
            this.InitializeComponent();

            this.serviceProvider = serviceProvider;
        }
    }
}
