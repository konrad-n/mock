using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke;
using SledzSpecke.App;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.App.ViewModels.Internships;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipRequirementViewModel : ObservableObject
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IDialogService dialogService;
        private readonly IExceptionHandlerService exceptionHandler;

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
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler,
            int? currentModuleId)
        {
            this.requirement = requirement;
            this.summary = summary;
            this.medicalShiftsService = medicalShiftsService;
            this.dialogService = dialogService;
            this.exceptionHandler = exceptionHandler;

            Shifts = new ObservableCollection<RealizedMedicalShiftNewSMK>(shifts);
            currentShift = new RealizedMedicalShiftNewSMK
            {
                InternshipRequirementId = requirement.Id,
                ModuleId = currentModuleId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Hours = summary.TotalHours,
                Minutes = summary.TotalMinutes
            };
            ToggleExpandCommand = new RelayCommand(ToggleExpand);
            SaveCommand = new AsyncRelayCommand(SaveShiftAsync);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        public string Name => requirement.Name;
        public int Id => requirement.Id;
        public string FormattedTime => $"{summary.TotalHours} godz. {summary.TotalMinutes} min.";
        public string Title => $"Dyżury do stażu\n{Name}";
        public string Summary => $"Czas wprowadzony:\n{FormattedTime}";

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public RealizedMedicalShiftNewSMK CurrentShift
        {
            get => currentShift;
            set => SetProperty(ref currentShift, value);
        }

        public ObservableCollection<RealizedMedicalShiftNewSMK> Shifts
        {
            get => shifts;
            set => SetProperty(ref shifts, value);
        }

        public bool IsEditing
        {
            get => isEditing;
            set => SetProperty(ref isEditing, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
            IsEditing = IsExpanded;
        }

        private async Task SaveShiftAsync()
        {
            try
            {
                // Validate shift data
                if (CurrentShift.Hours < 0 || CurrentShift.Minutes < 0 || CurrentShift.Minutes >= 60)
                {
                    throw new InvalidInputException(
                        "Invalid shift time",
                        "Niepoprawny czas dyżuru. Godziny muszą być nieujemne, a minuty w zakresie 0-59.");
                }

                if (CurrentShift.EndDate < CurrentShift.StartDate)
                {
                    throw new InvalidInputException(
                        "End date before start date",
                        "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
                }

                // Use exception handler to safely execute the save operation
                bool success = exceptionHandler != null
                    ? await exceptionHandler.ExecuteAsync(
                        async () => await medicalShiftsService.SaveNewSMKShiftAsync(CurrentShift),
                        new Dictionary<string, object> { { "InternshipRequirementId", Id } },
                        "Nie udało się zapisać dyżuru. Sprawdź poprawność danych.")
                    : await medicalShiftsService.SaveNewSMKShiftAsync(CurrentShift);

                if (success)
                {
                    // Safely retrieve updated shifts using the exception handler
                    var updatedShifts = exceptionHandler != null
                        ? await exceptionHandler.ExecuteAsync(
                            async () => await medicalShiftsService.GetNewSMKShiftsAsync(Id),
                            null,
                            "Nie udało się pobrać zaktualizowanych danych.")
                        : await medicalShiftsService.GetNewSMKShiftsAsync(Id);

                    Shifts.Clear();
                    foreach (var shift in updatedShifts)
                    {
                        Shifts.Add(shift);
                    }

                    // Safely update summary
                    var updatedSummary = exceptionHandler != null
                        ? await exceptionHandler.ExecuteAsync(
                            async () => await medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: Id),
                            null,
                            "Nie udało się zaktualizować podsumowania.")
                        : await medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: Id);

                    if (updatedSummary != null)
                    {
                        summary = updatedSummary;
                        OnPropertyChanged(nameof(FormattedTime));
                        OnPropertyChanged(nameof(Summary));
                    }

                    IsEditing = false;
                    IsExpanded = false;
                }
                else
                {
                    await dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zapisać dyżuru.",
                        "OK");
                }
            }
            catch (InvalidInputException ex)
            {
                // These will be caught and handled by the exception handler if available
                if (exceptionHandler != null)
                {
                    throw;
                }
                else
                {
                    await dialogService.DisplayAlertAsync("Błąd", ex.UserFriendlyMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                // Fallback error handling if exceptionHandler not available
                if (exceptionHandler == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving shift: {ex.Message}");
                    await dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Wystąpił nieoczekiwany błąd podczas zapisywania dyżuru.",
                        "OK");
                }
                else
                {
                    throw; // Let the exceptionHandler handle it
                }
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            IsExpanded = false;
        }
    }
}
