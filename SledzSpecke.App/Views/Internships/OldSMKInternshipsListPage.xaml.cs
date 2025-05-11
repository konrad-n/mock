using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class OldSMKInternshipsListPage : ContentPage
    {
        private readonly OldSMKInternshipsListViewModel viewModel;

        public OldSMKInternshipsListPage(OldSMKInternshipsListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedInternshipOldSMK realization)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RealizedInternshipId", realization.RealizedInternshipId.ToString() },
                    { "Year", realization.Year.ToString() }
                };

                // Znajdź wymaganie stażowe o tej samej nazwie
                var specializationService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Specialization.ISpecializationService>();
                var internships = await specializationService.GetInternshipsAsync(null);
                var requirement = internships.FirstOrDefault(i => i.InternshipName == realization.InternshipName);

                if (requirement != null)
                {
                    navigationParameter.Add("InternshipRequirementId", requirement.InternshipId.ToString());
                }
                else
                {
                    // Jeśli nie znaleziono, wyświetl komunikat
                    await this.DisplayAlert("Uwaga",
                        "Nie znaleziono wymagania stażowego dla tej realizacji. Edycja może być niekompletna.",
                        "OK");

                    // Użyj pierwszego dostępnego wymagania
                    if (internships.Count > 0)
                    {
                        navigationParameter.Add("InternshipRequirementId", internships[0].InternshipId.ToString());
                    }
                    else
                    {
                        await this.DisplayAlert("Błąd",
                            "Brak wymagań stażowych. Nie można edytować realizacji.",
                            "OK");
                        return;
                    }
                }

                await Shell.Current.GoToAsync("//AddEditRealizedInternship", navigationParameter);
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedInternshipOldSMK realization)
            {
                bool confirm = await this.DisplayAlert(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę realizację stażu?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    var specializationService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Specialization.ISpecializationService>();
                    bool success = await specializationService.DeleteRealizedInternshipOldSMKAsync(realization.RealizedInternshipId);

                    if (success)
                    {
                        await this.DisplayAlert("Sukces", "Realizacja stażu została usunięta.", "OK");
                        this.viewModel.RefreshCommand.Execute(null);
                    }
                    else
                    {
                        await this.DisplayAlert("Błąd", "Nie udało się usunąć realizacji stażu.", "OK");
                    }
                }
            }
        }
    }
}