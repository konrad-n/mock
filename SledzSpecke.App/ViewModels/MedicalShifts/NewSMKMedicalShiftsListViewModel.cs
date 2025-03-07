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

    // ViewModel dla pojedynczego wymagania stażowego
    public class InternshipRequirementViewModel : ObservableObject
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IDialogService dialogService;

        private InternshipRequirement requirement;
        private MedicalShiftsSummary summary;
        private ObservableCollection<RealizedMedicalShiftNewSMK> shifts;
        private bool isExpanded;
        private RealizedMedicalShiftNewSMK currentShift;
        private bool isEditing;

        public InternshipRequirementViewModel(
            InternshipRequirement requirement,
            MedicalShiftsSummary summary,
            List<RealizedMedicalShiftNewSMK> shifts,
            IMedicalShiftsService medicalShiftsService,
            IDialogService dialogService)
        {
            this.requirement = requirement;
            this.summary = summary;
            this.medicalShiftsService = medicalShiftsService;
            this.dialogService = dialogService;

            this.Shifts = new ObservableCollection<RealizedMedicalShiftNewSMK>(shifts);
            this.currentShift = new RealizedMedicalShiftNewSMK
            {
                InternshipRequirementId = requirement.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Hours = summary.TotalHours,
                Minutes = summary.TotalMinutes
            };

            // Inicjalizacja komend
            this.ToggleExpandCommand = new RelayCommand(this.ToggleExpand);
            this.SaveCommand = new AsyncRelayCommand(this.SaveShiftAsync);
            this.CancelCommand = new RelayCommand(this.CancelEdit);
        }

        public string Name => this.requirement.Name;
        public int Id => this.requirement.Id;
        public string FormattedTime => $"{this.summary.TotalHours} godz. {this.summary.TotalMinutes} min.";
        public string Title => $"Dyżury do stażu\n{this.Name}";
        public string Summary => $"Czas wprowadzony:\n{this.FormattedTime}";

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        public RealizedMedicalShiftNewSMK CurrentShift
        {
            get => this.currentShift;
            set => this.SetProperty(ref this.currentShift, value);
        }

        public ObservableCollection<RealizedMedicalShiftNewSMK> Shifts
        {
            get => this.shifts;
            set => this.SetProperty(ref this.shifts, value);
        }

        public bool IsEditing
        {
            get => this.isEditing;
            set => this.SetProperty(ref this.isEditing, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void ToggleExpand()
        {
            this.IsExpanded = !this.IsExpanded;
            this.IsEditing = this.IsExpanded;
        }

        private async Task SaveShiftAsync()
        {
            try
            {
                // Zapisz dyżur
                bool success = await this.medicalShiftsService.SaveNewSMKShiftAsync(this.CurrentShift);

                if (success)
                {
                    // Odśwież dane
                    var shifts = await this.medicalShiftsService.GetNewSMKShiftsAsync(this.Id);
                    this.Shifts.Clear();
                    foreach (var shift in shifts)
                    {
                        this.Shifts.Add(shift);
                    }

                    // Zaktualizuj podsumowanie
                    this.summary = await this.medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: this.Id);

                    // Powiadom o zmianie właściwości
                    this.OnPropertyChanged(nameof(this.FormattedTime));
                    this.OnPropertyChanged(nameof(this.Summary));

                    // Zamknij edycję
                    this.IsEditing = false;
                    this.IsExpanded = false;
                }
                else
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zapisać dyżuru.",
                        "OK");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania dyżuru: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas zapisywania dyżuru.",
                    "OK");
            }
        }

        private void CancelEdit()
        {
            this.IsEditing = false;
            this.IsExpanded = false;
        }
    }
}