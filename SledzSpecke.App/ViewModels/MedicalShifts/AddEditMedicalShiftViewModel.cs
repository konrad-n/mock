using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    [QueryProperty(nameof(InternshipId), "internshipId")]
    public class AddEditMedicalShiftViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISmkVersionStrategy smkStrategy;

        private int shiftId;
        private int internshipId;
        private MedicalShift shift;
        private Internship internship;

        // Pola formularza
        private DateTime date;
        private int hours;
        private int minutes;
        private string location;
        private int year;
        private string oldSmkField1; // Osoba nadzorująca (stary SMK)
        private string oldSmkField2; // Oddział (stary SMK)

        // Flagi widoczności pól
        private bool isDateVisible;
        private bool isHoursVisible;
        private bool isMinutesVisible;
        private bool isLocationVisible;
        private bool isYearVisible;
        private bool isOldSmkField1Visible;
        private bool isOldSmkField2Visible;

        // Etykiety pól
        private string dateLabel;
        private string hoursLabel;
        private string minutesLabel;
        private string locationLabel;
        private string yearLabel;
        private string oldSmkField1Label;
        private string oldSmkField2Label;

        // Opcje wyboru roku
        private Dictionary<string, string> yearOptions;

        private bool isNewMode;
        private bool isSmkNew;

        public AddEditMedicalShiftViewModel(
            ISpecializationService specializationService,
            IAuthService authService,
            IDialogService dialogService,
            ISmkVersionStrategy smkStrategy)
        {
            this.specializationService = specializationService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.smkStrategy = smkStrategy;

            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync, this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);

            this.YearOptions = this.smkStrategy.GetPickerOptions("AddEditMedicalShift", "Year");

            // Określenie czy jest to nowy SMK
            this.IsSmkNew = this.smkStrategy is NewSmkStrategy;

            // Ustawienie widoczności pól na podstawie strategii SMK
            var visibleFields = this.smkStrategy.GetVisibleFields("AddEditMedicalShift");
            this.IsDateVisible = visibleFields.TryGetValue("Date", out bool dateVisible) && dateVisible;
            this.IsHoursVisible = visibleFields.TryGetValue("Hours", out bool hoursVisible) && hoursVisible;
            this.IsMinutesVisible = visibleFields.TryGetValue("Minutes", out bool minutesVisible) && minutesVisible;
            this.IsLocationVisible = visibleFields.TryGetValue("Location", out bool locationVisible) && locationVisible;
            this.IsYearVisible = visibleFields.TryGetValue("Year", out bool yearVisible) && yearVisible;
            this.IsOldSmkField1Visible = visibleFields.TryGetValue("OldSMKField1", out bool field1Visible) && field1Visible;
            this.IsOldSmkField2Visible = visibleFields.TryGetValue("OldSMKField2", out bool field2Visible) && field2Visible;

            // Ustawienie etykiet pól
            var fieldLabels = this.smkStrategy.GetFieldLabels("AddEditMedicalShift");
            this.DateLabel = fieldLabels.TryGetValue("Date", out string dateLabel) ? dateLabel : "Data dyżuru";
            this.HoursLabel = fieldLabels.TryGetValue("Hours", out string hoursLabel) ? hoursLabel : "Godziny";
            this.MinutesLabel = fieldLabels.TryGetValue("Minutes", out string minutesLabel) ? minutesLabel : "Minuty";
            this.LocationLabel = fieldLabels.TryGetValue("Location", out string locationLabel) ? locationLabel : "Miejsce dyżuru";
            this.YearLabel = fieldLabels.TryGetValue("Year", out string yearLabel) ? yearLabel : "Rok szkolenia";
            this.OldSmkField1Label = fieldLabels.TryGetValue("OldSMKField1", out string field1Label) ? field1Label : "Osoba nadzorująca";
            this.OldSmkField2Label = fieldLabels.TryGetValue("OldSMKField2", out string field2Label) ? field2Label : "Oddział";

            // Ustawienie domyślnych wartości
            var defaultValues = this.smkStrategy.GetDefaultValues("AddEditMedicalShift");
            this.Date = defaultValues.TryGetValue("Date", out object dateValue) && dateValue is DateTime dateTime
                ? dateTime
                : DateTime.Today;

            this.Hours = defaultValues.TryGetValue("Hours", out object hoursValue) && hoursValue is int hoursInt
                ? hoursInt
                : 10;

            this.Minutes = defaultValues.TryGetValue("Minutes", out object minutesValue) && minutesValue is int minutesInt
                ? minutesInt
                : 0;

            this.Year = defaultValues.TryGetValue("Year", out object yearValue) && yearValue is int yearInt
                ? yearInt
                : 1;

            this.OldSmkField1 = defaultValues.TryGetValue("OldSMKField1", out object field1Value) && field1Value is string field1String
                ? field1String
                : string.Empty;

            this.OldSmkField2 = defaultValues.TryGetValue("OldSMKField2", out object field2Value) && field2Value is string field2String
                ? field2String
                : string.Empty;

            this.Title = this.smkStrategy.GetViewTitle("AddEditMedicalShift");
        }

        public int ShiftId
        {
            get => this.shiftId;
            set
            {
                this.SetProperty(ref this.shiftId, value);
                this.IsNewMode = value <= 0;
                this.LoadShiftAsync().ConfigureAwait(false);
            }
        }

        public int InternshipId
        {
            get => this.internshipId;
            set
            {
                this.SetProperty(ref this.internshipId, value);
                this.LoadInternshipAsync().ConfigureAwait(false);
            }
        }

        public bool IsNewMode
        {
            get => this.isNewMode;
            set => this.SetProperty(ref this.isNewMode, value);
        }

        public bool IsSmkNew
        {
            get => this.isSmkNew;
            set => this.SetProperty(ref this.isSmkNew, value);
        }

        public MedicalShift Shift
        {
            get => this.shift;
            set => this.SetProperty(ref this.shift, value);
        }

        public Internship Internship
        {
            get => this.internship;
            set => this.SetProperty(ref this.internship, value);
        }

        // Pola formularza
        public DateTime Date
        {
            get => this.date;
            set => this.SetProperty(ref this.date, value);
        }

        public int Hours
        {
            get => this.hours;
            set => this.SetProperty(ref this.hours, value);
        }

        public int Minutes
        {
            get => this.minutes;
            set => this.SetProperty(ref this.minutes, value);
        }

        public string Location
        {
            get => this.location;
            set => this.SetProperty(ref this.location, value);
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public string OldSmkField1
        {
            get => this.oldSmkField1;
            set => this.SetProperty(ref this.oldSmkField1, value);
        }

        public string OldSmkField2
        {
            get => this.oldSmkField2;
            set => this.SetProperty(ref this.oldSmkField2, value);
        }

        // Flagi widoczności pól
        public bool IsDateVisible
        {
            get => this.isDateVisible;
            set => this.SetProperty(ref this.isDateVisible, value);
        }

        public bool IsHoursVisible
        {
            get => this.isHoursVisible;
            set => this.SetProperty(ref this.isHoursVisible, value);
        }

        public bool IsMinutesVisible
        {
            get => this.isMinutesVisible;
            set => this.SetProperty(ref this.isMinutesVisible, value);
        }

        public bool IsLocationVisible
        {
            get => this.isLocationVisible;
            set => this.SetProperty(ref this.isLocationVisible, value);
        }

        public bool IsYearVisible
        {
            get => this.isYearVisible;
            set => this.SetProperty(ref this.isYearVisible, value);
        }

        public bool IsOldSmkField1Visible
        {
            get => this.isOldSmkField1Visible;
            set => this.SetProperty(ref this.isOldSmkField1Visible, value);
        }

        public bool IsOldSmkField2Visible
        {
            get => this.isOldSmkField2Visible;
            set => this.SetProperty(ref this.isOldSmkField2Visible, value);
        }

        // Etykiety pól
        public string DateLabel
        {
            get => this.dateLabel;
            set => this.SetProperty(ref this.dateLabel, value);
        }

        public string HoursLabel
        {
            get => this.hoursLabel;
            set => this.SetProperty(ref this.hoursLabel, value);
        }

        public string MinutesLabel
        {
            get => this.minutesLabel;
            set => this.SetProperty(ref this.minutesLabel, value);
        }

        public string LocationLabel
        {
            get => this.locationLabel;
            set => this.SetProperty(ref this.locationLabel, value);
        }

        public string YearLabel
        {
            get => this.yearLabel;
            set => this.SetProperty(ref this.yearLabel, value);
        }

        public string OldSmkField1Label
        {
            get => this.oldSmkField1Label;
            set => this.SetProperty(ref this.oldSmkField1Label, value);
        }

        public string OldSmkField2Label
        {
            get => this.oldSmkField2Label;
            set => this.SetProperty(ref this.oldSmkField2Label, value);
        }

        // Opcje wyboru roku
        public Dictionary<string, string> YearOptions
        {
            get => this.yearOptions;
            set => this.SetProperty(ref this.yearOptions, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadShiftAsync()
        {
            if (this.ShiftId <= 0)
            {
                return;
            }

            try
            {
                this.IsBusy = true;

                var shift = await this.specializationService.GetMedicalShiftAsync(this.ShiftId);
                if (shift == null)
                {
                    return;
                }

                this.Shift = shift;

                // Jeśli mamy dyżur, ale nie mamy stażu, pobierz staż
                if (this.Internship == null)
                {
                    await this.LoadInternshipAsync(shift.InternshipId);
                }

                // Wypełnij pola formularza
                this.Date = shift.Date;
                this.Hours = shift.Hours;
                this.Minutes = shift.Minutes;
                this.Location = shift.Location;
                this.Year = shift.Year;

                // Odczytaj pola dodatkowe z JSON
                var additionalFields = this.smkStrategy.ParseAdditionalFields(shift.AdditionalFields);

                if (additionalFields.TryGetValue("OldSMKField1", out object field1) && field1 is string field1Str)
                {
                    this.OldSmkField1 = field1Str;
                }

                if (additionalFields.TryGetValue("OldSMKField2", out object field2) && field2 is string field2Str)
                {
                    this.OldSmkField2 = field2Str;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania dyżuru: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania danych dyżuru. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadInternshipAsync()
        {
            if (this.internshipId <= 0)
            {
                return;
            }

            await this.LoadInternshipAsync(this.internshipId);
        }

        private async Task LoadInternshipAsync(int id)
        {
            try
            {
                this.IsBusy = true;

                var internship = await this.specializationService.GetInternshipAsync(id);
                if (internship == null)
                {
                    return;
                }

                this.Internship = internship;

                // Ustaw rok szkolenia na podstawie stażu
                this.Year = internship.Year;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania stażu: {ex.Message}");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private bool CanSave()
        {
            // Walidacja pól
            var requiredFields = this.smkStrategy.GetRequiredFields("AddEditMedicalShift");

            if (requiredFields.Contains("Date") && !this.IsDateVisible)
            {
                return false;
            }

            if (requiredFields.Contains("Hours") && this.Hours <= 0)
            {
                return false;
            }

            if (requiredFields.Contains("Location") && string.IsNullOrEmpty(this.Location))
            {
                return false;
            }

            if (requiredFields.Contains("OldSMKField1") && string.IsNullOrEmpty(this.OldSmkField1) && this.IsOldSmkField1Visible)
            {
                return false;
            }

            if (requiredFields.Contains("OldSMKField2") && string.IsNullOrEmpty(this.OldSmkField2) && this.IsOldSmkField2Visible)
            {
                return false;
            }

            if (this.InternshipId <= 0)
            {
                return false;
            }

            return true;
        }

        private async Task SaveAsync()
        {
            if (!this.CanSave())
            {
                await this.dialogService.DisplayAlertAsync(
                    "Walidacja",
                    "Proszę wypełnić wszystkie wymagane pola.",
                    "OK");
                return;
            }

            if (this.InternshipId <= 0)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie wybrano stażu dla dyżuru.",
                    "OK");
                return;
            }

            try
            {
                this.IsBusy = true;

                // Przygotuj pola dodatkowe jako słownik
                var additionalFields = new Dictionary<string, object>();
                if (this.IsOldSmkField1Visible)
                {
                    additionalFields["OldSMKField1"] = this.OldSmkField1;
                }

                if (this.IsOldSmkField2Visible)
                {
                    additionalFields["OldSMKField2"] = this.OldSmkField2;
                }

                // Stwórz lub uaktualnij obiekt dyżuru
                if (this.Shift == null || this.IsNewMode)
                {
                    this.Shift = new MedicalShift
                    {
                        InternshipId = internshipId,
                        Date = this.Date,
                        Hours = this.Hours,
                        Minutes = this.Minutes,
                        Location = this.Location,
                        Year = this.Year,
                        SyncStatus = SyncStatus.NotSynced,
                        AdditionalFields = this.smkStrategy.FormatAdditionalFields(additionalFields)
                    };

                    bool success = await this.specializationService.AddMedicalShiftAsync(this.Shift);
                    if (success)
                    {
                        // Aktualizuj postęp modułu
                        var currentModule = await this.specializationService.GetCurrentModuleAsync();
                        if (currentModule != null)
                        {
                            await this.specializationService.UpdateModuleProgressAsync(currentModule.ModuleId);
                        }

                        // Wróć do listy
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się dodać dyżuru. Spróbuj ponownie.",
                            "OK");
                    }
                }
                else
                {
                    // Aktualizuj istniejący dyżur
                    this.Shift.Date = this.Date;
                    this.Shift.Hours = this.Hours;
                    this.Shift.Minutes = this.Minutes;
                    this.Shift.Location = this.Location;
                    this.Shift.Year = this.Year;
                    this.Shift.SyncStatus = SyncStatus.Modified;
                    this.Shift.AdditionalFields = this.smkStrategy.FormatAdditionalFields(additionalFields);

                    bool success = await this.specializationService.UpdateMedicalShiftAsync(this.Shift);
                    if (success)
                    {
                        // Aktualizuj postęp modułu
                        var currentModule = await this.specializationService.GetCurrentModuleAsync();
                        if (currentModule != null)
                        {
                            await this.specializationService.UpdateModuleProgressAsync(currentModule.ModuleId);
                        }

                        // Wróć do listy
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się zaktualizować dyżuru. Spróbuj ponownie.",
                            "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania dyżuru: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas zapisywania dyżuru. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}