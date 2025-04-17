using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipStageViewModel : ObservableObject
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IAuthService authService;

        private Internship requirement;
        private List<RealizedInternshipNewSMK> newSMKRealizations;
        private List<RealizedInternshipOldSMK> oldSMKRealizations;
        private bool isExpanded;
        private bool isNewSMK;
        private int? currentModuleId;
        private bool isLoading;
        private bool hasLoadedData;

        public ICommand EditRealizationCommand { get; }
        public ICommand DeleteRealizationCommand { get; }

        public InternshipStageViewModel(
            Internship requirement,
            List<RealizedInternshipNewSMK> newSMKRealizations,
            List<RealizedInternshipOldSMK> oldSMKRealizations,
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService,
            int? currentModuleId)
        {
            this.requirement = requirement;
            this.newSMKRealizations = newSMKRealizations ?? new List<RealizedInternshipNewSMK>();
            this.oldSMKRealizations = oldSMKRealizations ?? new List<RealizedInternshipOldSMK>();
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;
            this.currentModuleId = currentModuleId;
            this.isLoading = false;
            this.hasLoadedData = false;

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.AddRealizationCommand = new AsyncRelayCommand(this.AddRealizationAsync);

            this.EditRealizationCommand = new AsyncRelayCommand<int>(this.EditRealizationAsync);
            this.DeleteRealizationCommand = new AsyncRelayCommand<int>(this.DeleteRealizationAsync);

            this.CheckSMKVersionAsync().ConfigureAwait(false);
        }

        private async Task CheckSMKVersionAsync()
        {
            var user = await this.authService.GetCurrentUserAsync();
            this.isNewSMK = user?.SmkVersion == SmkVersion.New;
        }

        private async Task EditRealizationAsync(int realizationId)
        {
            if (realizationId <= 0)
            {
                return;
            }

            // Logika nawigacji do strony edycji
            var navigationParameter = new Dictionary<string, object>
            {
                { "RealizedInternshipId", realizationId.ToString() },
                { "InternshipRequirementId", this.requirement.InternshipId.ToString() }
            };

            if (!this.isNewSMK)
            {
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    int year = 1;
                    if (currentModule.Type == ModuleType.Basic)
                    {
                        year = 1; // Pierwszy rok dla modułu podstawowego
                    }
                    else
                    {
                        year = 3; // Trzeci rok dla modułu specjalistycznego 
                    }
                    navigationParameter.Add("Year", year.ToString());
                }
            }
            else if (this.currentModuleId.HasValue)
            {
                navigationParameter.Add("ModuleId", this.currentModuleId.Value.ToString());
            }

            await Shell.Current.GoToAsync("//AddEditRealizedInternship", navigationParameter);
        }

        private async Task DeleteRealizationAsync(int realizationId)
        {
            if (realizationId <= 0)
            {
                return;
            }

            bool confirmed = await this.dialogService.DisplayAlertAsync(
                "Potwierdzenie",
                "Czy na pewno chcesz usunąć tę realizację stażu?",
                "Tak",
                "Nie");

            if (!confirmed)
            {
                return;
            }

            bool success = false;

            if (this.isNewSMK)
            {
                success = await this.specializationService.DeleteRealizedInternshipNewSMKAsync(realizationId);
            }
            else
            {
                success = await this.specializationService.DeleteRealizedInternshipOldSMKAsync(realizationId);
            }

            if (success)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    "Realizacja stażu została usunięta.",
                    "OK");

                if (this.isNewSMK)
                {
                    this.newSMKRealizations = await this.specializationService.GetRealizedInternshipsNewSMKAsync(
                        moduleId: this.currentModuleId,
                        internshipRequirementId: this.requirement.InternshipId);
                }
                else
                {
                    var currentModule = await this.specializationService.GetCurrentModuleAsync();
                    int startYear = 1;
                    int endYear = 2;

                    if (currentModule != null && currentModule.Type == ModuleType.Specialistic)
                    {
                        startYear = 3;
                        endYear = 6;
                    }

                    List<RealizedInternshipOldSMK> allRealizations = new List<RealizedInternshipOldSMK>();
                    for (int year = startYear; year <= endYear; year++)
                    {
                        var yearRealizations = await this.specializationService.GetRealizedInternshipsOldSMKAsync(year);
                        allRealizations.AddRange(yearRealizations);
                    }

                    this.oldSMKRealizations = allRealizations
                        .Where(r => r.InternshipName != null &&
                               r.InternshipName.Equals(this.requirement.InternshipName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                this.OnPropertyChanged(nameof(NewSMKRealizationsCollection));
                this.OnPropertyChanged(nameof(OldSMKRealizationsCollection));
                this.OnPropertyChanged(nameof(IntroducedDays));
                this.OnPropertyChanged(nameof(RemainingDays));
                this.OnPropertyChanged(nameof(FormattedStatistics));
            }
            else
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się usunąć realizacji stażu.",
                    "OK");
            }
        }

        public string Name => requirement.InternshipName;
        public int Id => requirement.InternshipId;
        public int RequiredDays => requirement.DaysCount;
        public string FormattedStatistics => GetFormattedStatistics();
        public string Title => $"Staż: {Name}";

        public int IntroducedDays => isNewSMK
            ? newSMKRealizations?.Sum(i => i.DaysCount) ?? 0
            : oldSMKRealizations?.Sum(i => i.DaysCount) ?? 0;

        public int RecognizedDays => isNewSMK
            ? newSMKRealizations?.Where(i => i.IsRecognition).Sum(i => i.RecognitionDaysReduction) ?? 0
            : 0;

        public int SelfEducationDays => 0; // Placeholder, do zaimplementowania jeśli potrzebne

        public int RemainingDays => RequiredDays - IntroducedDays - RecognizedDays;

        // Właściwości dla listy realizacji (można dodać później jeśli potrzebne)
        public ObservableCollection<RealizedInternshipNewSMK> NewSMKRealizationsCollection =>
            new ObservableCollection<RealizedInternshipNewSMK>(this.newSMKRealizations);

        public ObservableCollection<RealizedInternshipOldSMK> OldSMKRealizationsCollection =>
            new ObservableCollection<RealizedInternshipOldSMK>(this.oldSMKRealizations);

        private string GetFormattedStatistics()
        {
            int introduced = IntroducedDays;
            string competionMark = introduced >= RequiredDays ? "✔️" : "";
            return $"Zrealizowano {introduced} z {RequiredDays} dni {competionMark}";
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand AddRealizationCommand { get; }

        // Nowa implementacja metody OnToggleExpandAsync, identyczna jak w ProcedureRequirementViewModel
        private async Task OnToggleExpandAsync()
        {
            if (this.isExpanded)
            {
                this.IsExpanded = false;
                return;
            }

            if (!this.hasLoadedData && !this.isLoading)
            {
                this.isLoading = true;
                this.IsExpanded = true;

                try
                {
                    await Task.Run(async () =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            this.hasLoadedData = true;
                            this.isLoading = false;
                            this.OnPropertyChanged(nameof(this.IsLoading));
                        });
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania danych stażu: {ex.Message}");
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Wystąpił problem podczas ładowania danych stażu.",
                            "OK");

                        this.isLoading = false;
                        this.OnPropertyChanged(nameof(this.IsLoading));
                    });
                }
            }
            else
            {
                this.IsExpanded = !this.isExpanded;
            }
        }

        private async Task AddRealizationAsync()
        {
            // Pobieramy nazwę stażu, zamiast polegać na ID
            var navigationParameter = new Dictionary<string, object>
            {
                { "InternshipName", this.requirement.InternshipName },
                { "DaysCount", this.requirement.DaysCount.ToString() }
            };

            // Wypisz dla debugowania
            System.Diagnostics.Debug.WriteLine($"Przekazuję dane stażu: Nazwa: {this.requirement.InternshipName}, Dni: {this.requirement.DaysCount}");

            if (this.currentModuleId.HasValue)
            {
                navigationParameter.Add("ModuleId", this.currentModuleId.Value.ToString());
            }

            await Shell.Current.GoToAsync("//AddEditRealizedInternship", navigationParameter);
        }

        public void Refresh(List<RealizedInternshipNewSMK> newSMKRealizations, List<RealizedInternshipOldSMK> oldSMKRealizations)
        {
            if (isNewSMK && newSMKRealizations != null)
            {
                this.newSMKRealizations = newSMKRealizations.Where(r =>
                    r.InternshipName.Equals(this.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else if (!isNewSMK && oldSMKRealizations != null)
            {
                this.oldSMKRealizations = oldSMKRealizations.Where(r =>
                    r.InternshipName.Equals(this.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Odświeżenie właściwości
            OnPropertyChanged(nameof(IntroducedDays));
            OnPropertyChanged(nameof(FormattedStatistics));
            OnPropertyChanged(nameof(RemainingDays));
            OnPropertyChanged(nameof(NewSMKRealizationsCollection));
            OnPropertyChanged(nameof(OldSMKRealizationsCollection));
        }
    }
}