using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class MedicalShiftsListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISmkVersionStrategy smkStrategy;

        private ObservableCollection<MedicalShift> shifts;
        private MedicalShift selectedShift;
        private bool isRefreshing;
        private MedicalShiftsSummary summary;
        private List<Internship> internships;
        private bool isNewSmk;
        private Internship selectedInternship;
        private string shiftsDescription;
        private int totalShiftHours;
        private int requiredShiftHours;
        private double shiftProgress;
        private string moduleTitle;

        public MedicalShiftsListViewModel(
            ISpecializationService specializationService,
            IAuthService authService,
            IDialogService dialogService,
            ISmkVersionStrategy smkStrategy)
        {
            this.specializationService = specializationService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.smkStrategy = smkStrategy;

            this.Title = "Dyżury medyczne";
            this.Shifts = new ObservableCollection<MedicalShift>();
            this.Internships = new List<Internship>();
            this.Summary = new MedicalShiftsSummary();

            // Ustalenie czy jest to nowy SMK
            this.IsNewSmk = smkStrategy is NewSmkStrategy;

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.AddShiftCommand = new AsyncRelayCommand(this.AddShiftAsync);
            this.EditShiftCommand = new AsyncRelayCommand<MedicalShift>(this.EditShiftAsync);
            this.DeleteShiftCommand = new AsyncRelayCommand<MedicalShift>(this.DeleteShiftAsync);
            this.SelectInternshipCommand = new AsyncRelayCommand<Internship>(this.SelectInternshipAsync);

            // Wczytanie danych
            this.LoadDataAsync().ConfigureAwait(false);
        }

        #region Properties

        public ObservableCollection<MedicalShift> Shifts
        {
            get => this.shifts;
            set => this.SetProperty(ref this.shifts, value);
        }

        public MedicalShift SelectedShift
        {
            get => this.selectedShift;
            set => this.SetProperty(ref this.selectedShift, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public MedicalShiftsSummary Summary
        {
            get => this.summary;
            set => this.SetProperty(ref this.summary, value);
        }

        public List<Internship> Internships
        {
            get => this.internships;
            set => this.SetProperty(ref this.internships, value);
        }

        public bool IsNewSmk
        {
            get => this.isNewSmk;
            set => this.SetProperty(ref this.isNewSmk, value);
        }

        public Internship SelectedInternship
        {
            get => this.selectedInternship;
            set => this.SetProperty(ref this.selectedInternship, value);
        }

        public string ShiftsDescription
        {
            get => this.shiftsDescription;
            set => this.SetProperty(ref this.shiftsDescription, value);
        }

        public int TotalShiftHours
        {
            get => this.totalShiftHours;
            set => this.SetProperty(ref this.totalShiftHours, value);
        }

        public int RequiredShiftHours
        {
            get => this.requiredShiftHours;
            set => this.SetProperty(ref this.requiredShiftHours, value);
        }

        public double ShiftProgress
        {
            get => this.shiftProgress;
            set => this.SetProperty(ref this.shiftProgress, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand AddShiftCommand { get; }
        public ICommand EditShiftCommand { get; }
        public ICommand DeleteShiftCommand { get; }
        public ICommand SelectInternshipCommand { get; }

        #endregion

        #region Methods

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
                // Pobierz bieżący moduł
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    this.ModuleTitle = currentModule.Name;
                }

                // Pobierz statystyki wymaganych dyżurów
                var stats = await this.specializationService.GetSpecializationStatisticsAsync(
                    currentModule?.ModuleId);

                this.TotalShiftHours = stats.CompletedShiftHours;
                this.RequiredShiftHours = stats.RequiredShiftHours;

                // Oblicz postęp
                if (this.RequiredShiftHours > 0)
                {
                    this.ShiftProgress = Math.Min(1.0, (double)this.TotalShiftHours / this.RequiredShiftHours);
                }
                else
                {
                    this.ShiftProgress = 0;
                }

                // Pobierz wszystkie staże (tylko dla modułu jeśli nowy SMK)
                if (this.IsNewSmk && currentModule != null)
                {
                    this.Internships = await this.specializationService.GetInternshipsAsync(currentModule.ModuleId);
                }
                else
                {
                    this.Internships = await this.specializationService.GetInternshipsAsync();
                }

                // Jeśli jeszcze nie mamy wybranego stażu, a są dostępne staże, wybierz pierwszy
                if (this.SelectedInternship == null && this.Internships.Count > 0)
                {
                    await this.SelectInternshipAsync(this.Internships[0]);
                }
                else if (this.SelectedInternship != null)
                {
                    // Odśwież dane dla bieżącego stażu
                    await this.LoadShiftsForInternshipAsync(this.SelectedInternship.InternshipId);
                }
                else
                {
                    // Nie ma żadnych staży, pobierz wszystkie dyżury
                    var shifts = await this.specializationService.GetMedicalShiftsAsync();
                    this.Shifts.Clear();
                    foreach (var shift in shifts)
                    {
                        this.Shifts.Add(shift);
                    }

                    // Oblicz sumę dyżurów
                    this.Summary = MedicalShiftsSummary.CalculateFromShifts(shifts);
                    this.ShiftsDescription = "Wszystkie dyżury";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania dyżurów: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania dyżurów. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
            }
        }

        private async Task LoadShiftsForInternshipAsync(int internshipId)
        {
            try
            {
                // Pobierz dyżury dla wybranego stażu
                var shifts = await this.specializationService.GetMedicalShiftsAsync(internshipId);
                this.Shifts.Clear();
                foreach (var shift in shifts)
                {
                    this.Shifts.Add(shift);
                }

                // Oblicz sumę dyżurów
                this.Summary = MedicalShiftsSummary.CalculateFromShifts(shifts);

                // Zaktualizuj opis
                if (this.SelectedInternship != null)
                {
                    if (this.IsNewSmk)
                    {
                        this.ShiftsDescription = $"Dyżury do stażu {this.SelectedInternship.InternshipName}";
                    }
                    else
                    {
                        this.ShiftsDescription = "Lista zrealizowanych dyżurów medycznych";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania dyżurów dla stażu: {ex.Message}");
            }
        }

        private async Task SelectInternshipAsync(Internship internship)
        {
            if (internship == null)
            {
                return;
            }

            this.SelectedInternship = internship;
            await this.LoadShiftsForInternshipAsync(internship.InternshipId);
        }

        private async Task AddShiftAsync()
        {
            if (this.SelectedInternship == null && this.Internships.Count > 0)
            {
                // Jeśli nie wybrano stażu, a są dostępne, pytamy użytkownika o wybór
                var result = await this.dialogService.DisplayActionSheetAsync(
                    "Wybierz staż",
                    "Anuluj",
                    null,
                    this.Internships.Select(i => i.InternshipName).ToArray());

                if (result == "Anuluj" || result == null)
                {
                    return;
                }

                this.SelectedInternship = this.Internships.FirstOrDefault(i => i.InternshipName == result);
            }
            else if (this.Internships.Count == 0)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Informacja",
                    "Nie można dodać dyżuru, ponieważ nie ma zdefiniowanych staży. Najpierw dodaj staż.",
                    "OK");
                return;
            }

            if (this.SelectedInternship != null)
            {
                await Shell.Current.GoToAsync($"AddEditMedicalShift?internshipId={this.SelectedInternship.InternshipId}");
            }
        }

        private async Task EditShiftAsync(MedicalShift shift)
        {
            if (shift == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"AddEditMedicalShift?shiftId={shift.ShiftId}");
        }

        private async Task DeleteShiftAsync(MedicalShift shift)
        {
            if (shift == null)
            {
                return;
            }

            // Sprawdź, czy można usunąć dyżur (tylko niezatwierdzone)
            if (!shift.CanBeDeleted)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Informacja",
                    "Nie można usunąć zatwierdzonego dyżuru.",
                    "OK");
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdzenie",
                "Czy na pewno chcesz usunąć ten dyżur?",
                "Tak",
                "Nie");

            if (confirm)
            {
                try
                {
                    bool success = await this.specializationService.DeleteMedicalShiftAsync(shift.ShiftId);
                    if (success)
                    {
                        this.Shifts.Remove(shift);

                        // Aktualizacja podsumowania
                        this.Summary = MedicalShiftsSummary.CalculateFromShifts(this.Shifts.ToList());

                        // Aktualizacja postępu modułu
                        var currentModule = await this.specializationService.GetCurrentModuleAsync();
                        if (currentModule != null)
                        {
                            await this.specializationService.UpdateModuleProgressAsync(currentModule.ModuleId);
                        }

                        // Odśwież statystyki
                        await this.LoadDataAsync();
                    }
                    else
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się usunąć dyżuru. Spróbuj ponownie.",
                            "OK");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania dyżuru: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Wystąpił problem podczas usuwania dyżuru. Spróbuj ponownie.",
                        "OK");
                }
            }
        }

        #endregion
    }
}