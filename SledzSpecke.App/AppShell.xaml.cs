using SledzSpecke.App.Views;

namespace SledzSpecke;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rejestracja tras dla nawigacji
        Routing.RegisterRoute(nameof(CourseDetailsPage), typeof(CourseDetailsPage));
        Routing.RegisterRoute(nameof(InternshipDetailsPage), typeof(InternshipDetailsPage));
        Routing.RegisterRoute(nameof(ProcedureDetailsPage), typeof(ProcedureDetailsPage));
        Routing.RegisterRoute(nameof(ProcedureEntryPage), typeof(ProcedureEntryPage));
        Routing.RegisterRoute(nameof(DutyShiftDetailsPage), typeof(DutyShiftDetailsPage));
        Routing.RegisterRoute(nameof(SelfEducationDetailsPage), typeof(SelfEducationDetailsPage));
        Routing.RegisterRoute(nameof(SelfEducationPage), typeof(SelfEducationPage));
        Routing.RegisterRoute(nameof(SMKExportPage), typeof(SMKExportPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
    }
}