using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    [QueryProperty(nameof(ShiftId), "shiftId")]
    public partial class AddEditMedicalShiftViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly ISmkVersionStrategy smkStrategy;

        private int shiftId;
        private DateTime date = DateTime.Today;
        private int hours = 10;
        private int minutes;
        private string location = string.Empty;
        private int year = 1;
        private string oldSMKField1 = string.Empty;
        private string oldSMKField2 = string.Empty;
        private bool isOldSmkVersion;
        private bool canSave;

        private ObservableCollection<InternshipListItem> availableInternships;
        private InternshipListItem selectedInternship;
        private List<int> yearOptions = new List<int> { 1, 2, 3, 4, 5, 6 };

        public AddEditMedicalShiftViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            ISmkVersionStrategy smkStrategy)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.smkStrategy = smkStrategy;

            // Set title based on whether we're adding or editing
            this.Title = "Nowy dyżur";

            // Initialize commands
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, () => this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            // Determine SMK version
            this.IsOldSmkVersion = this.smkStrategy.GetType().Name.Contains("Old");

            // Initialize collections
            this.AvailableInternships = new ObservableCollection<InternshipListItem>();

            // Load data
            this.LoadDataAsync();
        }

        // Properties
        public int ShiftId
        {
            get => this.shiftId;
            set
            {
                if (this.SetProperty(ref this.shiftId, value) && value > 0)
                {
                    // Update title for edit mode
                    this.Title = "Edytuj dyżur";

                    // Load shift data
                    this.LoadShiftAsync(value);
                }
            }
        }

        public DateTime Date
        {
            get => this.date;
            set
            {
                this.SetProperty(ref this.date, value);
                this.ValidateInput();
            }
        }

        public int Hours
        {
            get => this.hours;
            set
            {
                this.SetProperty(ref this.hours, value);
                this.ValidateInput();
            }
        }

        public int Minutes
        {
            get => this.minutes;
            set
            {
                this.SetProperty(ref this.minutes, value);
                this.ValidateInput();
            }
        }

        public string Location
        {
            get => this.location;
            set
            {
                this.SetProperty(ref this.location, value);
                this.ValidateInput();
            }
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public string OldSMKField1
        {
            get => this.oldSMKField1;
            set
            {
                this.SetProperty(ref this.oldSMKField1, value);
                this.ValidateInput();
            }
        }

        public string OldSMKField2
        {
            get => this.oldSMKField2;
            set
            {
                this.SetProperty(ref this.oldSMKField2, value);
                this.ValidateInput();
            }
        }

        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set => this.SetProperty(ref this.isOldSmkVersion, value);
        }

        public bool CanSave
        {
            get => this.canSave;
            set => this.SetProperty(ref this.canSave, value);
        }

        public ObservableCollection<InternshipListItem> AvailableInternships
        {
            get => this.availableInternships;
            set => this.SetProperty(ref this.availableInternships, value);
        }

        public InternshipListItem SelectedInternship
        {
            get => this.selectedInternship;
            set => this.SetProperty(ref this.selectedInternship, value);
        }

        public List<int> YearOptions
        {
            get => this.yearOptions;
            set => this.SetProperty(ref this.yearOptions, value);
        }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Methods
        private async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Get current specialization
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Błąd",
                        "Nie znaleziono aktywnej specjalizacji.",
                        "OK");
                    await this.OnCancelAsync();
                    return;
                }

                // Load internships for current specialization
                int? moduleId = null;
                if (specialization.HasModules && specialization.CurrentModuleId.HasValue)
                {
                    moduleId = specialization.CurrentModuleId.Value;
                }

                var internships = await this.databaseService.GetInternshipsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                // Populate internships collection
                this.AvailableInternships.Clear();
                foreach (var internship in internships)
                {
                    this.AvailableInternships.Add(new InternshipListItem
                    {
                        InternshipId = internship.InternshipId,
                        DisplayName = $"{internship.InternshipName} - {internship.InstitutionName}"
                    });
                }

                // Select first internship if available and not in edit mode
                if (this.AvailableInternships.Count > 0 && this.ShiftId == 0)
                {
                    this.SelectedInternship = this.AvailableInternships[0];
                }

                // Validate input
                this.ValidateInput();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Błąd",
                    "Nie udało się załadować danych. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadShiftAsync(int shiftId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Load shift
                var shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
                if (shift == null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Błąd",
                        "Nie znaleziono dyżuru.",
                        "OK");
                    await this.OnCancelAsync();
                    return;
                }

                // Set properties
                this.Date = shift.Date;
                this.Hours = shift.Hours;
                this.Minutes = shift.Minutes;
                this.Location = shift.Location;
                this.Year = shift.Year;

                // Select the correct internship
                if (shift.InternshipId > 0)
                {
                    this.SelectedInternship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == shift.InternshipId);
                }

                // Parse additional fields for old SMK version
                if (this.IsOldSmkVersion && !string.IsNullOrEmpty(shift.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = this.smkStrategy.ParseAdditionalFields(shift.AdditionalFields);

                        if (additionalFields.TryGetValue("OldSMKField1", out object field1))
                        {
                            this.OldSMKField1 = field1?.ToString() ?? string.Empty;
                        }

                        if (additionalFields.TryGetValue("OldSMKField2", out object field2))
                        {
                            this.OldSMKField2 = field2?.ToString() ?? string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error parsing additional fields: {ex.Message}");
                    }
                }

                // Validate input
                this.ValidateInput();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading shift: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Błąd",
                    "Nie udało się załadować danych dyżuru.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void ValidateInput()
        {
            // Check if all required fields are filled
            bool isValid = true;

            // General validation
            if (this.SelectedInternship == null)
            {
                isValid = false;
            }

            if (this.Hours <= 0)
            {
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.Location))
            {
                isValid = false;
            }

            // Old SMK specific validation
            if (this.IsOldSmkVersion)
            {
                if (string.IsNullOrWhiteSpace(this.OldSMKField1))
                {
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(this.OldSMKField2))
                {
                    isValid = false;
                }
            }

            this.CanSave = isValid;
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy || !this.CanSave || this.SelectedInternship == null)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Create or get shift object
                MedicalShift shift;
                if (this.ShiftId > 0)
                {
                    shift = await this.databaseService.GetMedicalShiftAsync(this.ShiftId);
                    if (shift == null)
                    {
                        throw new Exception("Nie znaleziono dyżuru do edycji.");
                    }

                    // Mark as modified if previously synced
                    if (shift.SyncStatus == SyncStatus.Synced)
                    {
                        shift.SyncStatus = SyncStatus.Modified;
                    }
                }
                else
                {
                    shift = new MedicalShift
                    {
                        SyncStatus = SyncStatus.NotSynced
                    };
                }

                // Update shift properties
                shift.Date = this.Date;
                shift.Hours = this.Hours;
                shift.Minutes = this.Minutes;
                shift.Location = this.Location;
                shift.Year = this.Year;
                shift.InternshipId = this.SelectedInternship.InternshipId;

                // Handle additional fields for old SMK version
                if (this.IsOldSmkVersion)
                {
                    var additionalFields = new Dictionary<string, object>
                    {
                        { "OldSMKField1", this.OldSMKField1 },
                        { "OldSMKField2", this.OldSMKField2 }
                    };

                    shift.AdditionalFields = this.smkStrategy.FormatAdditionalFields(additionalFields);
                }

                // Save to database
                await this.databaseService.SaveMedicalShiftAsync(shift);

                await Application.Current.MainPage.DisplayAlert(
                    "Sukces",
                    this.ShiftId > 0 ? "Dyżur został pomyślnie zaktualizowany." : "Dyżur został pomyślnie dodany.",
                    "OK");

                // Navigate back
                await this.OnCancelAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving shift: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Błąd",
                    "Nie udało się zapisać dyżuru. Spróbuj ponownie.",
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