﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    [QueryProperty(nameof(ProcedureId), nameof(ProcedureId))]
    [QueryProperty(nameof(IsEditString), "IsEdit")]
    [QueryProperty(nameof(DateString), "Date")]
    [QueryProperty(nameof(YearString), "Year")]
    [QueryProperty(nameof(CodeString), "Code")]
    [QueryProperty(nameof(PerformingPerson), "PerformingPerson")]
    [QueryProperty(nameof(Location), "Location")]
    [QueryProperty(nameof(PatientInitials), "PatientInitials")]
    [QueryProperty(nameof(PatientGenderString), "PatientGender")]
    [QueryProperty(nameof(AssistantData), "AssistantData")]
    [QueryProperty(nameof(ProcedureGroup), "ProcedureGroup")]
    [QueryProperty(nameof(InternshipIdString), "InternshipId")]
    [QueryProperty(nameof(InternshipNameString), "InternshipName")]
    [QueryProperty(nameof(RequirementId), nameof(RequirementId))]
    public class AddEditOldSMKProcedureViewModel : BaseViewModel
    {
        private readonly IProcedureService procedureService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private bool isEdit;
        private string procedureId;
        private string requirementId;
        private RealizedProcedureOldSMK procedure;
        private ObservableCollection<KeyValuePair<string, string>> codeOptions;
        private ObservableCollection<KeyValuePair<string, string>> yearOptions;
        private ObservableCollection<KeyValuePair<string, string>> genderOptions;
        private ObservableCollection<Internship> availableInternships;
        private Internship selectedInternship;
        private KeyValuePair<string, string> selectedCode;
        private KeyValuePair<string, string> selectedYear;
        private KeyValuePair<string, string> selectedGender;
        private User currentUser;
        private bool isInitialized;

        private string isEditString;
        private string dateString;
        private string yearString;
        private string codeString;
        private string patientGenderString;
        private string internshipIdString;
        private string internshipNameString;
        private string performingPerson;
        private string location;
        private string patientInitials;
        private string assistantData;
        private string procedureGroup;
        private bool isInternshipSelectionEnabled;
        private string internshipSelectionHint;

        public AddEditOldSMKProcedureViewModel(
            IProcedureService procedureService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.procedureService = procedureService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.CodeOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.YearOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.GenderOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.AvailableInternships = new ObservableCollection<Internship>();

            this.Procedure = new RealizedProcedureOldSMK
            {
                Date = DateTime.Now,
                Code = "A - operator",
                PatientGender = "K",
                Year = 1,
                SyncStatus = SyncStatus.NotSynced,
                PerformingPerson = string.Empty,
            };

            this.PerformingPerson = string.Empty;
            this.Location = string.Empty;
            this.PatientInitials = string.Empty;
            this.AssistantData = string.Empty;
            this.ProcedureGroup = string.Empty;
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);
            this.isInitialized = false;
        }

        #region Query Property Setters

        // Query property setters remain unchanged
        public string IsEditString
        {
            set
            {
                this.isEditString = value;
                if (bool.TryParse(value, out bool result))
                {
                    this.IsEdit = result;
                    this.Title = this.IsEdit ? "Edytuj procedurę" : "Dodaj procedurę";
                }
            }
        }

        public string DateString
        {
            set
            {
                this.dateString = value;
                if (DateTime.TryParse(value, out DateTime result))
                {
                    this.Procedure.Date = result;
                }
            }
        }

        public string YearString
        {
            set
            {
                this.yearString = value;
                if (int.TryParse(value, out int result))
                {
                    this.Procedure.Year = result;
                }
            }
        }

        public string CodeString
        {
            set
            {
                this.codeString = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.Procedure.Code = value;
                }
            }
        }

        public string PatientGenderString
        {
            set
            {
                this.patientGenderString = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.Procedure.PatientGender = value;
                }
            }
        }

        public string InternshipIdString
        {
            set
            {
                this.internshipIdString = value;
                if (int.TryParse(value, out int result))
                {
                    this.Procedure.InternshipId = result;
                }
            }
        }

        public string InternshipNameString
        {
            set
            {
                this.internshipNameString = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.Procedure.InternshipName = value;
                }
            }
        }

        #endregion

        #region Properties

        public string ProcedureId
        {
            set
            {
                this.procedureId = value;
            }
        }

        public string RequirementId
        {
            set
            {
                this.requirementId = value;
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public RealizedProcedureOldSMK Procedure
        {
            get => this.procedure;
            set
            {
                if (this.SetProperty(ref this.procedure, value))
                {
                    this.SyncPropertiesFromProcedure();
                }
            }
        }

        public string PerformingPerson
        {
            get => this.performingPerson;
            set
            {
                if (this.SetProperty(ref this.performingPerson, value))
                {
                    this.Procedure.PerformingPerson = value;
                }
            }
        }

        public string Location
        {
            get => this.location;
            set
            {
                if (this.SetProperty(ref this.location, value))
                {
                    this.Procedure.Location = value;
                }
            }
        }

        public string PatientInitials
        {
            get => this.patientInitials;
            set
            {
                if (this.SetProperty(ref this.patientInitials, value))
                {
                    this.Procedure.PatientInitials = value;
                }
            }
        }

        public string AssistantData
        {
            get => this.assistantData;
            set
            {
                if (this.SetProperty(ref this.assistantData, value))
                {
                    this.Procedure.AssistantData = value;
                }
            }
        }

        public string ProcedureGroup
        {
            get => this.procedureGroup;
            set
            {
                if (this.SetProperty(ref this.procedureGroup, value))
                {
                    this.Procedure.ProcedureGroup = value;
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> CodeOptions
        {
            get => this.codeOptions;
            set => this.SetProperty(ref this.codeOptions, value);
        }

        public ObservableCollection<KeyValuePair<string, string>> YearOptions
        {
            get => this.yearOptions;
            set => this.SetProperty(ref this.yearOptions, value);
        }

        public ObservableCollection<KeyValuePair<string, string>> GenderOptions
        {
            get => this.genderOptions;
            set => this.SetProperty(ref this.genderOptions, value);
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
                    this.Procedure.InternshipId = value.InternshipId;
                    this.Procedure.InternshipName = value.InternshipName;
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedCode
        {
            get => this.selectedCode;
            set
            {
                if (this.SetProperty(ref this.selectedCode, value))
                {
                    if (!string.IsNullOrEmpty(value.Key))
                    {
                        this.Procedure.Code = value.Key;
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value))
                {
                    if (!string.IsNullOrEmpty(value.Key) && int.TryParse(value.Key, out int year))
                    {
                        this.Procedure.Year = year;
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedGender
        {
            get => this.selectedGender;
            set
            {
                if (this.SetProperty(ref this.selectedGender, value))
                {
                    if (!string.IsNullOrEmpty(value.Key))
                    {
                        this.Procedure.PatientGender = value.Key;
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public User CurrentUser
        {
            get => this.currentUser;
            set => this.SetProperty(ref this.currentUser, value);
        }

        public bool IsInternshipSelectionEnabled
        {
            get => this.isInternshipSelectionEnabled;
            set => this.SetProperty(ref this.isInternshipSelectionEnabled, value);
        }

        public string InternshipSelectionHint
        {
            get => this.internshipSelectionHint;
            set => this.SetProperty(ref this.internshipSelectionHint, value);
        }

        #endregion

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void SyncPropertiesFromProcedure()
        {
            if (this.procedure != null)
            {
                this.PerformingPerson = this.procedure.PerformingPerson ?? string.Empty;
                this.Location = this.procedure.Location ?? string.Empty;
                this.PatientInitials = this.procedure.PatientInitials ?? string.Empty;
                this.AssistantData = this.procedure.AssistantData ?? string.Empty;
                this.ProcedureGroup = this.procedure.ProcedureGroup ?? string.Empty;
            }
        }

        public async Task InitializeAsync()
        {
            if (this.IsBusy || this.isInitialized)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                this.CurrentUser = await this.authService.GetCurrentUserAsync();
                if (this.CurrentUser == null)
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono aktywnego użytkownika.");
                }

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Active specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                if (this.IsEdit && this.Procedure.ProcedureId == 0)
                {
                    int procId = 0;
                    if (!string.IsNullOrEmpty(this.procedureId) && int.TryParse(this.procedureId, out procId) && procId > 0)
                    {
                        await this.LoadProcedureAsync(procId);
                    }
                }
                else if (!this.IsEdit)
                {
                    if (this.CurrentUser != null && string.IsNullOrEmpty(this.Procedure.PerformingPerson))
                    {
                        this.Procedure.PerformingPerson = this.CurrentUser.Name;
                        this.PerformingPerson = this.CurrentUser.Name;
                    }
                }

                this.LoadDropdownOptions(specialization?.DurationYears ?? 6);
                await this.LoadInternshipsAsync();
                this.SynchronizePickersWithProcedure();

                if (!this.IsEdit && !string.IsNullOrEmpty(this.requirementId) && int.TryParse(this.requirementId, out int reqId))
                {
                    await this.LoadRequirementDataAsync(reqId);
                }

                if (!this.IsEdit && string.IsNullOrEmpty(this.Procedure.Location))
                {
                    await this.LoadLastLocationAsync();
                }

                this.SyncPropertiesFromProcedure();
                ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                this.isInitialized = true;
            }, "Wystąpił problem podczas inicjalizacji formularza procedury.");

            this.IsBusy = false;
        }

        private void LoadDropdownOptions(int yearsFromSpecialization)
        {
            this.CodeOptions.Clear();
            this.CodeOptions.Add(new KeyValuePair<string, string>("A - operator", "A - operator"));
            this.CodeOptions.Add(new KeyValuePair<string, string>("B - asysta", "B - asysta"));

            this.GenderOptions.Clear();
            this.GenderOptions.Add(new KeyValuePair<string, string>("K", "K"));
            this.GenderOptions.Add(new KeyValuePair<string, string>("M", "M"));

            this.YearOptions.Clear();
            for (int i = 1; i <= yearsFromSpecialization; i++)
            {
                this.YearOptions.Add(new KeyValuePair<string, string>(i.ToString(), $"Rok {i}"));
            }
        }

        private void SynchronizePickersWithProcedure()
        {
            var codeItem = this.CodeOptions.FirstOrDefault(c => c.Key == this.Procedure.Code);
            if (codeItem.Key != null)
            {
                this.SelectedCode = codeItem;
            }
            else if (this.CodeOptions.Count > 0)
            {
                this.SelectedCode = this.CodeOptions.First();
            }

            var yearItem = this.YearOptions.FirstOrDefault(y => y.Key == this.Procedure.Year.ToString());
            if (yearItem.Key != null)
            {
                this.SelectedYear = yearItem;
            }
            else if (this.YearOptions.Count > 0)
            {
                this.SelectedYear = this.YearOptions.First();
            }

            var genderItem = this.GenderOptions.FirstOrDefault(g => g.Key == this.Procedure.PatientGender);
            if (genderItem.Key != null)
            {
                this.SelectedGender = genderItem;
            }
            else if (this.GenderOptions.Count > 0)
            {
                this.SelectedGender = this.GenderOptions.First();
            }

            if (this.Procedure.InternshipId > 0)
            {
                var internship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == this.Procedure.InternshipId);
                if (internship != null)
                {
                    this.SelectedInternship = internship;
                }
                else if (this.AvailableInternships.Count > 0)
                {
                    this.SelectedInternship = this.AvailableInternships.First();
                }
            }
            else if (this.AvailableInternships.Count > 0)
            {
                this.SelectedInternship = this.AvailableInternships.First();
            }
        }

        private async Task LoadInternshipsAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule == null)
                {
                    throw new ResourceNotFoundException(
                        "Current module not found",
                        "Nie znaleziono aktualnego modułu specjalizacji.");
                }

                var internships = await this.specializationService.GetInternshipsAsync(moduleId: currentModule?.ModuleId);

                if (this.CurrentUser.SmkVersion == SmkVersion.Old)
                {
                    bool isBasicModule = currentModule?.Type == ModuleType.Basic;
                    internships = internships.Where(i =>
                        (isBasicModule && i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)) ||
                        (!isBasicModule && !i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                this.AvailableInternships.Clear();
                foreach (var internship in internships)
                {
                    this.AvailableInternships.Add(internship);
                }

                if (this.AvailableInternships.Count == 0)
                {
                    throw new BusinessRuleViolationException(
                        "No internships available",
                        "Nie znaleziono dostępnych staży. Dodaj staż przed dodaniem procedury.");
                }
            }, "Wystąpił problem podczas wczytywania dostępnych staży.");
        }

        private async Task LoadRequirementDataAsync(int reqId)
        {
            await SafeExecuteAsync(async () =>
            {
                var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync();
                var requirement = requirements.FirstOrDefault(r => r.Id == reqId);

                if (requirement != null)
                {
                    if (this.CurrentUser.SmkVersion == SmkVersion.New)
                    {
                        if (requirement.InternshipId.HasValue)
                        {
                            var internship = this.AvailableInternships.FirstOrDefault(i =>
                                i.InternshipId == requirement.InternshipId.Value);
                            if (internship != null)
                            {
                                this.SelectedInternship = internship;
                                this.IsInternshipSelectionEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        bool isBasicInternship = requirement.Type.Contains("podstawowy", StringComparison.OrdinalIgnoreCase);
                        var matchingInternship = this.AvailableInternships.FirstOrDefault(i =>
                            (isBasicInternship && i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)) ||
                            (!isBasicInternship && i.InternshipName.Contains(requirement.Type, StringComparison.OrdinalIgnoreCase)));

                        if (matchingInternship != null)
                        {
                            this.SelectedInternship = matchingInternship;
                            this.IsInternshipSelectionEnabled = false;
                        }
                        else
                        {
                            var filteredInternships = this.AvailableInternships.Where(i =>
                                (isBasicInternship && i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)) ||
                                (!isBasicInternship && !i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                            this.AvailableInternships.Clear();
                            foreach (var internship in filteredInternships)
                            {
                                this.AvailableInternships.Add(internship);
                            }

                            this.IsInternshipSelectionEnabled = true;
                        }
                    }

                    this.InternshipSelectionHint = this.IsInternshipSelectionEnabled
                        ? "Wybierz staż z listy"
                        : "Staż jest przypisany automatycznie do tej procedury";
                }
                else
                {
                    throw new ResourceNotFoundException(
                        $"Requirement with ID {reqId} not found",
                        "Nie znaleziono wymagania procedurowego o podanym identyfikatorze.");
                }
            }, "Wystąpił problem podczas wczytywania danych wymagania procedurowego.");
        }

        private async Task LoadProcedureAsync(int procedureId)
        {
            await SafeExecuteAsync(async () =>
            {
                var loadedProcedure = await this.procedureService.GetOldSMKProcedureAsync(procedureId);
                if (loadedProcedure != null)
                {
                    this.IsEdit = true;
                    this.Title = "Edytuj procedurę";
                    this.Procedure = loadedProcedure;
                    this.PerformingPerson = loadedProcedure.PerformingPerson ?? string.Empty;
                    this.Location = loadedProcedure.Location ?? string.Empty;
                    this.PatientInitials = loadedProcedure.PatientInitials ?? string.Empty;
                    this.AssistantData = loadedProcedure.AssistantData ?? string.Empty;
                    this.ProcedureGroup = loadedProcedure.ProcedureGroup ?? string.Empty;
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj procedurę";
                    throw new ResourceNotFoundException(
                        $"Procedure with ID {procedureId} not found",
                        "Nie znaleziono procedury o podanym identyfikatorze.");
                }
            }, "Wystąpił problem podczas wczytywania procedury.");
        }

        private async Task LoadLastLocationAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono danych użytkownika.");
                }

                var lastProcedures = await this.procedureService.GetOldSMKProceduresAsync();

                if (lastProcedures.Count > 0)
                {
                    this.Procedure.Location = lastProcedures[0].Location;
                    this.Location = lastProcedures[0].Location;
                }
            }, "Wystąpił problem podczas wczytywania danych ostatniej lokalizacji.");
        }

        private async Task<bool> ValidateInputsAsync()
        {
            if (this.Procedure == null)
            {
                return false;
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(this.Procedure.Code))
            {
                throw new InvalidInputException("Code is required", "Kod procedury jest wymagany.");
            }

            if (string.IsNullOrWhiteSpace(this.Location))
            {
                throw new InvalidInputException("Location is required", "Lokalizacja jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(this.PerformingPerson))
            {
                throw new InvalidInputException("Performing person is required", "Osoba wykonująca jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(this.PatientInitials))
            {
                throw new InvalidInputException("Patient initials are required", "Inicjały pacjenta są wymagane.");
            }

            if (string.IsNullOrWhiteSpace(this.Procedure.PatientGender))
            {
                throw new InvalidInputException("Patient gender is required", "Płeć pacjenta jest wymagana.");
            }

            if (this.SelectedInternship == null)
            {
                throw new InvalidInputException("Internship is required", "Wybór stażu jest wymagany.");
            }

            return true;
        }

        public async Task OnSaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    await ValidateInputsAsync();

                    this.Procedure.PerformingPerson = this.PerformingPerson;
                    this.Procedure.Location = this.Location;
                    this.Procedure.PatientInitials = this.PatientInitials;
                    this.Procedure.AssistantData = this.AssistantData;
                    this.Procedure.ProcedureGroup = this.ProcedureGroup;

                    if (this.Procedure.SpecializationId <= 0 && this.CurrentUser != null)
                    {
                        this.Procedure.SpecializationId = this.CurrentUser.SpecializationId;
                    }

                    this.Procedure.InternshipId = this.SelectedInternship.InternshipId;
                    this.Procedure.InternshipName = this.SelectedInternship.InternshipName;

                    if (!string.IsNullOrEmpty(this.requirementId) && int.TryParse(this.requirementId, out int reqId))
                    {
                        this.Procedure.ProcedureRequirementId = reqId;
                    }

                    bool success = await this.procedureService.SaveOldSMKProcedureAsync(this.Procedure);

                    if (success)
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            this.IsEdit ? "Procedura została zaktualizowana." : "Procedura została dodana.",
                            "OK");

                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to save procedure",
                            "Nie udało się zapisać procedury. Sprawdź poprawność danych.");
                    }
                }, "Wystąpił problem podczas zapisywania procedury.");
            }
            catch (InvalidInputException ex)
            {
                await this.dialogService.DisplayAlertAsync("Błąd walidacji", ex.UserFriendlyMessage, "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("..");
            }, "Wystąpił problem podczas anulowania edycji procedury.");
        }
    }
}
