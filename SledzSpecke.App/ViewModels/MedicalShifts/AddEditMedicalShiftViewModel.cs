using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class AddEditMedicalShiftViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private bool isEdit;
        private MedicalShift shift;
        private Internship selectedInternship;
        private ObservableCollection<Internship> availableInternships;
        private ObservableCollection<KeyValuePair<string, string>> yearOptions;
        private string selectedYear;

        public AddEditMedicalShiftViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            this.AvailableInternships = new ObservableCollection<Internship>();
            this.YearOptions = new ObservableCollection<KeyValuePair<string, string>>();

            // Inicjalizacja komend
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            // Utworzenie nowego dyżuru
            this.shift = new MedicalShift
            {
                Date = DateTime.Now,
                Hours = 10,
                Minutes = 5,
                Year = 1,
                SyncStatus = SyncStatus.NotSynced
            };
        }

        // Właściwości
        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public MedicalShift Shift
        {
            get => this.shift;
            set => this.SetProperty(ref this.shift, value);
        }

        public ObservableCollection<Internship> AvailableInternships
        {
            get => this.availableInternships;
            set => this.SetProperty(ref this.availableInternships, value);
        }

        public Internship SelectedInternship
        {
            get => this.selectedInternship;
            set
            {
                if (this.SetProperty(ref this.selectedInternship, value) && value != null)
                {
                    this.Shift.InternshipId = value.InternshipId;
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> YearOptions
        {
            get => this.yearOptions;
            set => this.SetProperty(ref this.yearOptions, value);
        }

        public string SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value) && !string.IsNullOrEmpty(value))
                {
                    this.Shift.Year = int.Parse(value);
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        // Komendy
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public async Task InitializeAsync(int? shiftId = null)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Określ czy to edycja czy nowy
                this.IsEdit = shiftId.HasValue && shiftId.Value > 0;

                // Ustawienie tytułu strony
                this.Title = this.IsEdit ? "Edytuj dyżur" : "Dodaj nowy dyżur";

                // Ładowanie opcji roku z odpowiedniej strategii SMK
                await this.LoadYearOptionsAsync();

                // Ładowanie dostępnych staży
                await this.LoadInternshipsAsync();

                // Jeśli edycja, załaduj istniejący dyżur
                if (this.IsEdit && shiftId.HasValue)
                {
                    var existingShift = await this.databaseService.GetMedicalShiftAsync(shiftId.Value);
                    if (existingShift != null)
                    {
                        this.Shift = existingShift;

                        // Ustaw wybrany staż
                        this.SelectedInternship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == existingShift.InternshipId);

                        // Ustaw wybrany rok
                        this.SelectedYear = existingShift.Year.ToString();
                    }
                }
                else
                {
                    // Dla nowego dyżuru, ustaw domyślny staż (jeśli istnieje)
                    if (this.AvailableInternships.Count > 0)
                    {
                        this.SelectedInternship = this.AvailableInternships[0];
                    }

                    // Ustaw domyślny rok
                    this.SelectedYear = "1";
                }
            }
            catch (Exception ex)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Wystąpił problem podczas ładowania danych: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadYearOptionsAsync()
        {
            try
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();

                for (int i = 1; i <= specialization.DurationYears; i++)
                {
                    this.YearOptions.Add(new KeyValuePair<string, string>(i.ToString(), $"Rok {i}"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania opcji roku: {ex.Message}");

                // Awaryjnie dodaj domyślne wartości
                this.YearOptions.Clear();
                for (int i = 1; i <= 5; i++)
                {
                    this.YearOptions.Add(new KeyValuePair<string, string>(i.ToString(), $"Rok {i}"));
                }
            }
        }

        private async Task LoadInternshipsAsync()
        {
            try
            {
                // Pobierz aktualny moduł
                var currentModule = await this.specializationService.GetCurrentModuleAsync();

                // Używamy dostępnej metody GetInternshipsAsync zamiast GetUserInternshipsAsync
                var internships = await this.specializationService.GetInternshipsAsync(moduleId: currentModule?.ModuleId);

                // Filtrujemy tylko rzeczywiste staże (z ID > 0, nie szablony)
                var userInternships = internships.Where(i => i.InternshipId > 0).ToList();

                this.AvailableInternships.Clear();
                foreach (var internship in userInternships)
                {
                    this.AvailableInternships.Add(internship);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania staży: {ex.Message}");
            }
        }

        private bool CanSave()
        {
            // Sprawdzenie czy wszystkie wymagane pola są wypełnione
            return this.Shift != null
                && this.Shift.Hours > 0
                && this.SelectedInternship != null;
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Upewnij się, że ID stażu jest ustawione
                this.Shift.InternshipId = this.SelectedInternship.InternshipId;

                // Zapisz dyżur
                bool success;
                if (this.IsEdit)
                {
                    success = await this.specializationService.UpdateMedicalShiftAsync(this.Shift);
                }
                else
                {
                    success = await this.specializationService.AddMedicalShiftAsync(this.Shift);
                }

                if (success)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        this.IsEdit ? "Dyżur został zaktualizowany." : "Dyżur został dodany.",
                        "OK");

                    // Wróć do poprzedniej strony
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zapisać dyżuru. Sprawdź poprawność danych.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Wystąpił problem podczas zapisywania dyżuru: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}