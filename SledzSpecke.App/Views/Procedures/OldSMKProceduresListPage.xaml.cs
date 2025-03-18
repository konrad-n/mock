using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class OldSMKProceduresListPage : ContentPage
    {
        private readonly OldSMKProceduresListViewModel viewModel;
        private readonly IProcedureService procedureService;

        public OldSMKProceduresListPage(OldSMKProceduresListViewModel viewModel, IProcedureService procedureService)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.procedureService = procedureService;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (this.viewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedProcedureOldSMK procedure)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", procedure.ProcedureId.ToString() },
                    { "IsEdit", "true" },
                    { "Date", procedure.Date.ToString("o") },
                    { "Year", procedure.Year.ToString() },
                    { "Code", procedure.Code ?? string.Empty },
                    { "PerformingPerson", procedure.PerformingPerson ?? string.Empty },
                    { "Location", procedure.Location ?? string.Empty },
                    { "PatientInitials", procedure.PatientInitials ?? string.Empty },
                    { "PatientGender", procedure.PatientGender ?? string.Empty },
                    { "AssistantData", procedure.AssistantData ?? string.Empty },
                    { "ProcedureGroup", procedure.ProcedureGroup ?? string.Empty },
                    { "InternshipId", procedure.InternshipId.ToString() },
                    { "InternshipName", procedure.InternshipName ?? string.Empty }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedProcedureOldSMK procedure)
            {
                bool confirm = await DisplayAlert(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę procedurę?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    if (button.Parent?.Parent?.BindingContext is RealizedProcedureOldSMK &&
                        button.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.BindingContext is ProcedureGroupViewModel groupViewModel)
                    {
                        await groupViewModel.OnDeleteProcedure(procedure);
                    }
                    else
                    {
                        bool success = await this.procedureService.DeleteOldSMKProcedureAsync(procedure.ProcedureId);

                        if (success)
                        {
                            this.viewModel.RefreshCommand.Execute(null);
                            await DisplayAlert("Sukces", "Procedura została usunięta.", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Błąd", "Nie udało się usunąć procedury. Spróbuj ponownie.", "OK");
                        }
                    }
                }
            }
        }
    }
}