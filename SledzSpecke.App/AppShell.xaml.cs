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

            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("procedures", typeof(ProceduresPage));
            Routing.RegisterRoute("duties", typeof(DutiesPage));
            Routing.RegisterRoute("procedure/add", typeof(ProcedureAddPage));
            Routing.RegisterRoute("procedure/edit", typeof(ProcedureEditPage));
            Routing.RegisterRoute("duty/add", typeof(DutyAddPage));
            Routing.RegisterRoute("duty/edit", typeof(DutyEditPage));
            Routing.RegisterRoute("profile", typeof(ProfilePage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("profile/edit", typeof(ProfileEditPage));
            Routing.RegisterRoute("progress", typeof(SpecializationProgressPage));
            Routing.RegisterRoute("report", typeof(SpecializationProgressPage)); // TO DO: implement somewhere report button
        }
    }
}