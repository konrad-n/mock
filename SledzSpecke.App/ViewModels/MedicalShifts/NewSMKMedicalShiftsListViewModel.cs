using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class NewSMKMedicalShiftsListViewModel : BaseViewModel
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<InternshipRequirementViewModel> internshipRequirements;
        private bool isRefreshing;
        private string moduleTitle;

        public NewSMKMedicalShiftsListViewModel(
            IMedicalShiftsService medicalShiftsService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Dyżury medyczne (Nowy SMK)";
            this.InternshipRequirements = new ObservableCollection<InternshipRequirementViewModel>();

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);

            // Załaduj dane
            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<InternshipRequirementViewModel> InternshipRequirements
        {
            get => this.internshipRequirements;
            set => this.SetProperty(ref this.internshipRequirements, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public ICommand RefreshCommand { get; }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                // Pobierz tytuł modułu
                var module = await this.specializationService.GetCurrentModuleAsync();
                if (module != null)
                {
                    this.ModuleTitle = module.Name;
                }

                // Pobierz dostępne wymagania stażowe
                var requirements = await this.medicalShiftsService.GetAvailableInternshipRequirementsAsync();

                // Utworzenie ViewModeli dla każdego wymagania stażowego
                var viewModels = new List<InternshipRequirementViewModel>();

                foreach (var requirement in requirements)
                {
                    // Pobierz podsumowanie dyżurów dla tego wymagania
                    var summary = await this.medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: requirement.Id);

                    // Pobierz wszystkie dyżury dla tego wymagania
                    var shifts = await this.medicalShiftsService.GetNewSMKShiftsAsync(requirement.Id);

                    // Utwórz ViewModel
                    var viewModel = new InternshipRequirementViewModel(
                        requirement,
                        summary,
                        shifts,
                        this.medicalShiftsService,
                        this.dialogService);

                    viewModels.Add(viewModel);
                }

                // Zaktualizuj kolekcję
                this.InternshipRequirements.Clear();
                foreach (var viewModel in viewModels)
                {
                    this.InternshipRequirements.Add(viewModel);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania wymagań stażowych: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania danych dyżurów.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
            }
        }
    }
}