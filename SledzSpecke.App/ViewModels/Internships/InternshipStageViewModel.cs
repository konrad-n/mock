using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipStageViewModel : ObservableObject
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;

        private Internship requirement;
        private Internship userInternship;
        private bool isExpanded;
        private bool isEditing;
        private Internship currentInternship;
        private int? currentModuleId;

        public InternshipStageViewModel(
            Internship requirement,
            Internship userInternship,
            ISpecializationService specializationService,
            IDialogService dialogService,
            int? currentModuleId)
        {
            this.requirement = requirement;
            this.userInternship = userInternship;
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.currentModuleId = currentModuleId;
            this.currentInternship = userInternship != null ?
                new Internship
                {
                    InternshipId = userInternship.InternshipId,
                    SpecializationId = userInternship.SpecializationId,
                    ModuleId = userInternship.ModuleId,
                    InstitutionName = userInternship.InstitutionName,
                    DepartmentName = userInternship.DepartmentName,
                    InternshipName = userInternship.InternshipName,
                    Year = userInternship.Year,
                    StartDate = userInternship.StartDate,
                    EndDate = userInternship.EndDate,
                    DaysCount = userInternship.DaysCount,
                    IsCompleted = userInternship.IsCompleted,
                    IsApproved = userInternship.IsApproved,
                    IsRecognition = userInternship.IsRecognition,
                    RecognitionReason = userInternship.RecognitionReason,
                    RecognitionDaysReduction = userInternship.RecognitionDaysReduction,
                    IsPartialRealization = userInternship.IsPartialRealization,
                    SupervisorName = userInternship.SupervisorName,
                    SyncStatus = userInternship.SyncStatus,
                    AdditionalFields = userInternship.AdditionalFields
                } :
                new Internship
                {
                    InternshipId = 0,
                    SpecializationId = requirement.SpecializationId,
                    ModuleId = currentModuleId,
                    InternshipName = requirement.InternshipName,
                    InstitutionName = string.Empty,
                    DepartmentName = string.Empty,
                    Year = 1,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(requirement.DaysCount),
                    DaysCount = requirement.DaysCount,
                    IsCompleted = false,
                    IsApproved = false,
                    IsRecognition = false,
                    RecognitionDaysReduction = 0,
                    IsPartialRealization = false,
                    SyncStatus = SyncStatus.NotSynced
                };

            this.ToggleExpandCommand = new RelayCommand(this.ToggleExpand);
            this.AddRealizationCommand = new AsyncRelayCommand(this.AddRealizationAsync);
            this.SaveCommand = new AsyncRelayCommand(this.SaveInternshipAsync);
            this.CancelCommand = new RelayCommand(this.CancelEdit);
        }

        public string Name => requirement.InternshipName;
        public int Id => requirement.InternshipId;
        public int DaysCount => requirement.DaysCount;
        public string FormattedStatistics => GetFormattedStatistics();
        public string Title => $"Staż: {Name}";
        public int RequiredDays => requirement.DaysCount;
        public int IntroducedDays => userInternship?.DaysCount ?? 0;
        public int RecognizedDays => (userInternship?.IsRecognition ?? false) ? userInternship.RecognitionDaysReduction : 0;
        public int SelfEducationDays => 0; // Placeholder, to be implemented if needed
        public int RemainingDays => RequiredDays - IntroducedDays - RecognizedDays;

        private string GetFormattedStatistics()
        {
            if (userInternship != null)
            {
                return $"Zrealizowano {userInternship.DaysCount} z {requirement.DaysCount} dni";
            }
            else
            {
                return $"Dni do zrealizowania: {requirement.DaysCount}";
            }
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public bool IsEditing
        {
            get => isEditing;
            set => SetProperty(ref isEditing, value);
        }

        public Internship CurrentInternship
        {
            get => currentInternship;
            set => SetProperty(ref currentInternship, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand AddRealizationCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }

        private async Task AddRealizationAsync()
        {
            IsEditing = true;

            var navigationParameter = new Dictionary<string, object>
            {
                { "internshipRequirementId", requirement.InternshipId },
                { "moduleId", currentModuleId ?? 0 }
            };

            await Shell.Current.GoToAsync("AddEditInternship", navigationParameter);
        }

        private async Task SaveInternshipAsync()
        {
            bool success;

            if (CurrentInternship.InternshipId == 0)
            {
                success = await specializationService.AddInternshipAsync(CurrentInternship);
            }
            else
            {
                success = await specializationService.UpdateInternshipAsync(CurrentInternship);
            }

            if (success)
            {
                userInternship = CurrentInternship;
                IsEditing = false;
                OnPropertyChanged(nameof(FormattedStatistics));
                OnPropertyChanged(nameof(IntroducedDays));
                OnPropertyChanged(nameof(RecognizedDays));
                OnPropertyChanged(nameof(RemainingDays));
            }
            else
            {
                await dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać stażu.",
                    "OK");
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
        }
    }
}