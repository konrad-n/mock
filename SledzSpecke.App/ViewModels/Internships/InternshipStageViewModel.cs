using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Internships;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipStageViewModel : ObservableObject
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IInternshipService internshipService;

        private Internship requirement;
        private InternshipSummary summary;
        private ObservableCollection<RealizedInternshipNewSMK> realizationsNewSMK;
        private ObservableCollection<RealizedInternshipOldSMK> realizationsOldSMK;
        private bool isExpanded;
        private bool isNewSMK;
        private int? currentModuleId;

        public InternshipStageViewModel(
            Internship requirement,
            InternshipSummary summary,
            ISpecializationService specializationService,
            IDialogService dialogService,
            IInternshipService internshipService,
            bool isNewSMK,
            int? currentModuleId)
        {
            this.requirement = requirement;
            this.summary = summary;
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.internshipService = internshipService;
            this.isNewSMK = isNewSMK;
            this.currentModuleId = currentModuleId;

            this.realizationsNewSMK = new ObservableCollection<RealizedInternshipNewSMK>();
            this.realizationsOldSMK = new ObservableCollection<RealizedInternshipOldSMK>();

            this.ToggleExpandCommand = new RelayCommand(this.ToggleExpand);
            this.AddRealizationCommand = new AsyncRelayCommand(this.AddRealizationAsync);
            this.DeleteRealizationCommand = new AsyncRelayCommand<object>(this.DeleteRealizationAsync);
            this.EditRealizationCommand = new AsyncRelayCommand<object>(this.EditRealizationAsync);

            this.LoadRealizationsAsync().ConfigureAwait(false);
        }

        public string Name => this.requirement.InternshipName;
        public int Id => this.requirement.InternshipId;
        public string FormattedStatistics =>
            $"Zrealizowano {this.summary.CompletedDays} z {this.summary.RequiredDays} dni";
        public int RequiredDays => this.summary.RequiredDays;
        public int IntroducedDays => this.summary.CompletedDays;
        public int RecognizedDays => this.summary.RecognizedDays;
        public int SelfEducationDays => this.summary.SelfEducationDays;
        public int RemainingDays => this.summary.RemainingDays;
        public bool IsComplete => this.summary.IsCompleted;
        public double CompletionPercentage => this.summary.CompletionPercentage;

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        public ObservableCollection<RealizedInternshipNewSMK> RealizationsNewSMK
        {
            get => this.realizationsNewSMK;
            set => this.SetProperty(ref this.realizationsNewSMK, value);
        }

        public ObservableCollection<RealizedInternshipOldSMK> RealizationsOldSMK
        {
            get => this.realizationsOldSMK;
            set => this.SetProperty(ref this.realizationsOldSMK, value);
        }

        public bool IsNewSMK => this.isNewSMK;

        public ICommand ToggleExpandCommand { get; }
        public ICommand AddRealizationCommand { get; }
        public ICommand DeleteRealizationCommand { get; }
        public ICommand EditRealizationCommand { get; }

        private void ToggleExpand()
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private async Task LoadRealizationsAsync()
        {
            if (this.isNewSMK)
            {
                var realizations = await this.internshipService.GetRealizedInternshipsNewSMKAsync(
                    this.currentModuleId,
                    this.requirement.InternshipId);

                this.RealizationsNewSMK.Clear();
                foreach (var realization in realizations)
                {
                    this.RealizationsNewSMK.Add(realization);
                }
            }
            else
            {
                var realizations = await this.internshipService.GetRealizedInternshipsOldSMKAsync(
                    0, // wszystkie lata
                    this.requirement.InternshipName);

                this.RealizationsOldSMK.Clear();
                foreach (var realization in realizations)
                {
                    this.RealizationsOldSMK.Add(realization);
                }
            }
        }

        private async Task AddRealizationAsync()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "internshipRequirementId", this.requirement.InternshipId },
                { "moduleId", this.currentModuleId ?? 0 },
                { "isNewSMK", this.isNewSMK }
            };

            if (this.isNewSMK)
            {
                await Shell.Current.GoToAsync("//AddEditNewSMKInternship", navigationParameter);
            }
            else
            {
                await Shell.Current.GoToAsync("//AddEditOldSMKInternship", navigationParameter);
            }
        }

        private async Task DeleteRealizationAsync(object parameter)
        {
            bool confirm = await this.dialogService.DisplayConfirmationAsync(
                "Potwierdzenie",
                "Czy na pewno chcesz usunąć tę realizację stażu?",
                "Tak",
                "Nie");

            if (!confirm)
            {
                return;
            }

            bool success = false;

            if (this.isNewSMK && parameter is RealizedInternshipNewSMK newSMKRealization)
            {
                success = await this.internshipService.DeleteRealizedInternshipNewSMKAsync(newSMKRealization.RealizedInternshipId);
            }
            else if (!this.isNewSMK && parameter is RealizedInternshipOldSMK oldSMKRealization)
            {
                success = await this.internshipService.DeleteRealizedInternshipOldSMKAsync(oldSMKRealization.RealizedInternshipId);
            }

            if (success)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    "Realizacja stażu została usunięta.",
                    "OK");

                // Odświeżamy listę realizacji i podsumowanie
                await this.RefreshDataAsync();
            }
            else
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się usunąć realizacji stażu.",
                    "OK");
            }
        }

        private async Task EditRealizationAsync(object parameter)
        {
            if (this.isNewSMK && parameter is RealizedInternshipNewSMK newSMKRealization)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "realizedInternshipNewSMKId", newSMKRealization.RealizedInternshipId.ToString() },
                    { "internshipRequirementId", this.requirement.InternshipId },
                    { "moduleId", this.currentModuleId ?? 0 }
                };

                await Shell.Current.GoToAsync("//AddEditNewSMKInternship", navigationParameter);
            }
            else if (!this.isNewSMK && parameter is RealizedInternshipOldSMK oldSMKRealization)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "realizedInternshipOldSMKId", oldSMKRealization.RealizedInternshipId.ToString() },
                    { "internshipName", this.requirement.InternshipName }
                };

                await Shell.Current.GoToAsync("//AddEditOldSMKInternship", navigationParameter);
            }
        }

        public async Task RefreshDataAsync()
        {
            // Odświeżamy podsumowanie
            this.summary = await this.internshipService.GetInternshipSummaryAsync(
                this.requirement.InternshipId,
                this.currentModuleId);

            // Aktualizujemy właściwości
            this.OnPropertyChanged(nameof(this.FormattedStatistics));
            this.OnPropertyChanged(nameof(this.IntroducedDays));
            this.OnPropertyChanged(nameof(this.RecognizedDays));
            this.OnPropertyChanged(nameof(this.SelfEducationDays));
            this.OnPropertyChanged(nameof(this.RemainingDays));
            this.OnPropertyChanged(nameof(this.IsComplete));
            this.OnPropertyChanged(nameof(this.CompletionPercentage));

            // Odświeżamy listę realizacji
            await this.LoadRealizationsAsync();
        }
    }
}