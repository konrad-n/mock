using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.Duties;
using SledzSpecke.App.Views.Profile;
using SledzSpecke.App.Views.Specialization;

namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Main routes
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("procedures", typeof(ProceduresPage));
            Routing.RegisterRoute("duties", typeof(DutiesPage));

            // Procedures routes
            Routing.RegisterRoute("procedure/add", typeof(ProcedureAddPage));
            Routing.RegisterRoute("procedure/edit", typeof(ProcedureEditPage));

            // Duties routes
            Routing.RegisterRoute("duty/add", typeof(DutyAddPage));
            Routing.RegisterRoute("duty/edit", typeof(DutyEditPage));

            // Profile routes
            Routing.RegisterRoute("profile", typeof(ProfilePage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("profile/edit", typeof(ProfileEditPage));

            // Specialization progress
            Routing.RegisterRoute("progress", typeof(SpecializationProgressPage));
        }
    }
}