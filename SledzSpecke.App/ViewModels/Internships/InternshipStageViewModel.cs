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

            this.ToggleExpandCommand = new RelayCommand(this.ToggleExpand);
            this.AddRealizationCommand = new AsyncRelayCommand(this.AddRealizationAsync);

            // Sprawdzenie wersji SMK
            this.CheckSMKVersionAsync().ConfigureAwait(false);
        }

        private async Task CheckSMKVersionAsync()
        {
            var user = await this.authService.GetCurrentUserAsync();
            this.isNewSMK = user?.SmkVersion == SmkVersion.New;
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
            return $"Zrealizowano {introduced} z {RequiredDays} dni";
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand AddRealizationCommand { get; }

        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }

        private async Task AddRealizationAsync()
        {
            // Zapisz ID wymagania do parametrów nawigacji
            var navigationParameter = new Dictionary<string, object>
            {
                { "InternshipRequirementId", requirement.InternshipId.ToString() } // Tu musi być poprawne ID
            };

            // Wypisz dla debugowania
            System.Diagnostics.Debug.WriteLine($"Przekazuję ID wymagania stażu: {requirement.InternshipId}");

            if (this.isNewSMK && this.currentModuleId.HasValue)
            {
                navigationParameter.Add("ModuleId", this.currentModuleId.Value.ToString());
            }
            else if (!this.isNewSMK)
            {
                // Dla starego SMK potrzebujemy roku zamiast ID modułu
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