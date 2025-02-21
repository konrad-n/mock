using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Duties;
using SledzSpecke.App.Views.Procedures;

namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rejestracja tras
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("procedures", typeof(ProceduresPage));
            Routing.RegisterRoute("duties", typeof(DutiesPage));
        }
    }
}