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

namespace SledzSpecke.App.ViewModels.Courses
{
    [QueryProperty(nameof(CourseId), "courseId")]
    [QueryProperty(nameof(ModuleId), "moduleId")]
    public class AddEditCourseViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private int courseId;
        private int? moduleId;
        private string courseName = string.Empty;
        private string courseType = string.Empty;
        private string courseNumber = string.Empty;
        private string institutionName = string.Empty;
        private DateTime completionDate = DateTime.Today;
        private int year = 1;
        private int courseSequenceNumber = 1;
        private bool hasCertificate;
        private string certificateNumber = string.Empty;
        private DateTime? certificateDate;
        private bool canSave;
        private string moduleInfo = string.Empty;
        private bool hasModules;

        private ObservableCollection<string> availableCourseTypes;
        private string selectedCourseType;
        private ObservableCollection<int> availableYears;

        public AddEditCourseViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja tytułu
            this.Title = "Nowy kurs";

            // Inicjalizacja komend
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, () => this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            // Inicjalizacja kolekcji
            this.AvailableCourseTypes = new ObservableCollection<string>
            {
                "Kurs wprowadzający",
                "Kurs: Orzecznictwo lekarskie",
                "Kurs: Profilaktyka i promocja zdrowia",
                "Kurs: Prawo medyczne",
                "Kurs: Ratownictwo medyczne",
                "Kurs: Diagnostyka obrazowa",
                "Kurs: Transfuzjologia kliniczna",
                "Kurs atestacyjny",
                "Kurs specjalizacyjny",
                "Inny kurs"
            };

            this.AvailableYears = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6 };
        }

        // Właściwości
        public int CourseId
        {
            get => this.courseId;
            set
            {
                if (this.SetProperty(ref this.courseId, value) && value > 0)
                {
                    // Aktualizacja tytułu dla trybu edycji
                    this.Title = "Edytuj kurs";

                    // Wczytanie danych kursu
                    this.LoadCourseAsync(value).ConfigureAwait(false);
                }
            }
        }

        public int? ModuleId
        {
            get => this.moduleId;
            set
            {
                if (this.SetProperty(ref this.moduleId, value) && value.HasValue)
                {
                    // Wczytaj dane o module
                    this.LoadModuleInfoAsync().ConfigureAwait(false);
                }
            }
        }

        public string CourseName
        {
            get => this.courseName;
            set
            {
                this.SetProperty(ref this.courseName, value);
                this.ValidateInput();
            }
        }

        public string CourseType
        {
            get => this.courseType;
            set
            {
                this.SetProperty(ref this.courseType, value);
                this.ValidateInput();
            }
        }

        public string CourseNumber
        {
            get => this.courseNumber;
            set
            {
                this.SetProperty(ref this.courseNumber, value);
                this.ValidateInput();
            }
        }

        public string InstitutionName
        {
            get => this.institutionName;
            set
            {
                this.SetProperty(ref this.institutionName, value);
                this.ValidateInput();
            }
        }

        public DateTime CompletionDate
        {
            get => this.completionDate;
            set
            {
                this.SetProperty(ref this.completionDate, value);
                this.ValidateInput();
            }
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public int CourseSequenceNumber
        {
            get => this.courseSequenceNumber;
            set => this.SetProperty(ref this.courseSequenceNumber, value);
        }

        public bool HasCertificate
        {
            get => this.hasCertificate;
            set => this.SetProperty(ref this.hasCertificate, value);
        }

        public string CertificateNumber
        {
            get => this.certificateNumber;
            set => this.SetProperty(ref this.certificateNumber, value);
        }

        public DateTime? CertificateDate
        {
            get => this.certificateDate;
            set => this.SetProperty(ref this.certificateDate, value);
        }

        public bool CanSave
        {
            get => this.canSave;
            set => this.SetProperty(ref this.canSave, value);
        }

        public ObservableCollection<string> AvailableCourseTypes
        {
            get => this.availableCourseTypes;
            set => this.SetProperty(ref this.availableCourseTypes, value);
        }

        public string SelectedCourseType
        {
            get => this.selectedCourseType;
            set
            {
                if (this.SetProperty(ref this.selectedCourseType, value))
                {
                    this.CourseType = value;
                    this.CourseName = value;
                }
            }
        }

        public ObservableCollection<int> AvailableYears
        {
            get => this.availableYears;
            set => this.SetProperty(ref this.availableYears, value);
        }

        public string ModuleInfo
        {
            get => this.moduleInfo;
            set => this.SetProperty(ref this.moduleInfo, value);
        }

        public bool HasModules
        {
            get => this.hasModules;
            set => this.SetProperty(ref this.hasModules, value);
        }

        // Komendy
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Metody
        private async Task LoadCourseAsync(int courseId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Wczytanie kursu
                var course = await this.databaseService.GetCourseAsync(courseId);
                if (course == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono kursu.",
                        "OK");
                    await this.OnCancelAsync();
                    return;
                }

                // Ustawienie moduleId na podstawie kursu
                this.moduleId = course.ModuleId;

                // Ustawienie właściwości
                this.CourseName = course.CourseName;
                this.CourseType = course.CourseType;
                this.CourseNumber = course.CourseNumber;
                this.InstitutionName = course.InstitutionName;
                this.CompletionDate = course.CompletionDate;
                this.Year = course.Year;
                this.CourseSequenceNumber = course.CourseSequenceNumber;
                this.HasCertificate = course.HasCertificate;
                this.CertificateNumber = course.CertificateNumber;
                this.CertificateDate = course.CertificateDate;

                // Spróbuj znaleźć odpowiadający typ kursu
                this.SelectedCourseType = this.AvailableCourseTypes
                    .FirstOrDefault(t => t == course.CourseType) ?? string.Empty;

                // Wczytaj dane o module, jeśli istnieje
                if (course.ModuleId.HasValue)
                {
                    await this.LoadModuleInfoAsync();
                }

                // Walidacja
                this.ValidateInput();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania kursu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować danych kursu.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadModuleInfoAsync()
        {
            try
            {
                if (!this.moduleId.HasValue)
                {
                    this.ModuleInfo = string.Empty;
                    return;
                }

                // Sprawdź, czy specjalizacja ma moduły
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                this.HasModules = specialization?.HasModules ?? false;

                // Jeśli specjalizacja ma moduły, pobierz dane modułu
                if (this.HasModules)
                {
                    var module = await this.databaseService.GetModuleAsync(this.moduleId.Value);
                    if (module != null)
                    {
                        this.ModuleInfo = $"Ten kurs będzie dodany do modułu: {module.Name}";
                    }
                }
                else
                {
                    this.ModuleInfo = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania informacji o module: {ex.Message}");
                this.ModuleInfo = string.Empty;
            }
        }

        private void ValidateInput()
        {
            // Sprawdź czy wszystkie wymagane pola są wypełnione
            this.CanSave = !string.IsNullOrWhiteSpace(this.CourseName)
                        && !string.IsNullOrWhiteSpace(this.CourseType)
                        && !string.IsNullOrWhiteSpace(this.InstitutionName);
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy || !this.CanSave)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobierz aktualną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new Exception("Nie znaleziono aktywnej specjalizacji.");
                }

                // Utwórz lub pobierz kurs
                Course course;
                if (this.CourseId > 0)
                {
                    course = await this.databaseService.GetCourseAsync(this.CourseId);
                    if (course == null)
                    {
                        throw new Exception("Nie znaleziono kursu do edycji.");
                    }

                    // Oznacz jako zmodyfikowane, jeśli było wcześniej zsynchronizowane
                    if (course.SyncStatus == SyncStatus.Synced)
                    {
                        course.SyncStatus = SyncStatus.Modified;
                    }
                }
                else
                {
                    course = new Course
                    {
                        SpecializationId = specialization.SpecializationId,
                        SyncStatus = SyncStatus.NotSynced,
                    };
                }

                // Aktualizacja właściwości
                course.CourseName = this.CourseName;
                course.CourseType = this.CourseType;
                course.CourseNumber = this.CourseNumber;
                course.InstitutionName = this.InstitutionName;
                course.CompletionDate = this.CompletionDate;
                course.Year = this.Year;
                course.CourseSequenceNumber = this.CourseSequenceNumber;
                course.HasCertificate = this.HasCertificate;
                course.CertificateNumber = this.CertificateNumber;
                course.CertificateDate = this.CertificateDate;

                // Ustaw moduł, jeśli specjalizacja ma moduły
                if (specialization.HasModules)
                {
                    // Jeśli przekazano moduleId, użyj go
                    if (this.moduleId.HasValue)
                    {
                        course.ModuleId = this.moduleId.Value;
                    }
                    // W przeciwnym razie użyj bieżącego modułu specjalizacji
                    else if (specialization.CurrentModuleId.HasValue)
                    {
                        course.ModuleId = specialization.CurrentModuleId.Value;
                    }
                }
                else
                {
                    course.ModuleId = null;
                }

                // Zapisz do bazy danych
                await this.databaseService.SaveCourseAsync(course);

                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.CourseId > 0
                        ? "Kurs został pomyślnie zaktualizowany."
                        : "Kurs został pomyślnie dodany.",
                    "OK");

                // Powrót
                await this.OnCancelAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisywania kursu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać kursu. Spróbuj ponownie.",
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