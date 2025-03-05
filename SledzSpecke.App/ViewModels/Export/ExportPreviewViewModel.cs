using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Export;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Export
{
    public class ExportPreviewViewModel : BaseViewModel
    {
        private readonly IExportService exportService;
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private string previewDescription;
        private string smkVersionInfo;
        private bool formatForOldSmk;
        private DateTime startDate;
        private DateTime endDate;
        private int? moduleId;
        private bool showCourses;
        private bool showShifts;
        private bool showProcedures;
        private bool showInternships;
        private ExportOptions exportOptions;
        private string selectedTab = "Kursy"; // Domyślna zakładka

        private ObservableCollection<CoursePreviewItem> coursesPreviews;
        private ObservableCollection<ShiftPreviewItem> shiftsPreviews;
        private ObservableCollection<ProcedurePreviewItem> proceduresPreviews;
        private ObservableCollection<InternshipPreviewItem> internshipsPreviews;

        public ExportPreviewViewModel(
            IExportService exportService,
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.exportService = exportService;
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja tytułu
            this.Title = "Podgląd eksportu";

            // Inicjalizacja kolekcji
            this.CoursesPreviews = new ObservableCollection<CoursePreviewItem>();
            this.ShiftsPreviews = new ObservableCollection<ShiftPreviewItem>();
            this.ProceduresPreviews = new ObservableCollection<ProcedurePreviewItem>();
            this.InternshipsPreviews = new ObservableCollection<InternshipPreviewItem>();

            // Inicjalizacja komend
            this.BackCommand = new AsyncRelayCommand(this.OnBackAsync);
            this.ContinueExportCommand = new AsyncRelayCommand(this.OnContinueExportAsync);
            this.SelectTabCommand = new RelayCommand<string>(this.OnSelectTab);
        }

        // Właściwości
        public string SelectedTab
        {
            get => this.selectedTab;
            set => this.SetProperty(ref this.selectedTab, value);
        }

        public string PreviewDescription
        {
            get => this.previewDescription;
            set => this.SetProperty(ref this.previewDescription, value);
        }

        public string SmkVersionInfo
        {
            get => this.smkVersionInfo;
            set => this.SetProperty(ref this.smkVersionInfo, value);
        }

        public bool ShowCourses
        {
            get => this.showCourses;
            set => this.SetProperty(ref this.showCourses, value);
        }

        public bool ShowShifts
        {
            get => this.showShifts;
            set => this.SetProperty(ref this.showShifts, value);
        }

        public bool ShowProcedures
        {
            get => this.showProcedures;
            set => this.SetProperty(ref this.showProcedures, value);
        }

        public bool ShowInternships
        {
            get => this.showInternships;
            set => this.SetProperty(ref this.showInternships, value);
        }

        public ObservableCollection<CoursePreviewItem> CoursesPreviews
        {
            get => this.coursesPreviews;
            set => this.SetProperty(ref this.coursesPreviews, value);
        }

        public ObservableCollection<ShiftPreviewItem> ShiftsPreviews
        {
            get => this.shiftsPreviews;
            set => this.SetProperty(ref this.shiftsPreviews, value);
        }

        public ObservableCollection<ProcedurePreviewItem> ProceduresPreviews
        {
            get => this.proceduresPreviews;
            set => this.SetProperty(ref this.proceduresPreviews, value);
        }

        public ObservableCollection<InternshipPreviewItem> InternshipsPreviews
        {
            get => this.internshipsPreviews;
            set => this.SetProperty(ref this.internshipsPreviews, value);
        }

        // Komendy
        public ICommand BackCommand { get; }

        public ICommand ContinueExportCommand { get; }

        public ICommand SelectTabCommand { get; }

        // Metoda inicjalizacji z parametrami eksportu
        public async Task InitializeAsync(ExportOptions options)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Zapisujemy opcje eksportu
                this.exportOptions = options;
                this.formatForOldSmk = options.FormatForOldSMK;
                this.startDate = options.StartDate;
                this.endDate = options.EndDate;
                this.moduleId = options.ModuleId;

                // Ustawiamy opis podglądu
                string dateRange = $"{this.startDate:dd.MM.yyyy} - {this.endDate:dd.MM.yyyy}";
                this.PreviewDescription = $"Dane w zakresie dat: {dateRange}";

                // Ustawiamy informację o wersji SMK
                this.SmkVersionInfo = this.formatForOldSmk ? 
                    "Format danych: Stara wersja SMK" : 
                    "Format danych: Nowa wersja SMK";

                // Ustawiamy widoczność zakładek
                this.ShowCourses = options.IncludeCourses;
                this.ShowShifts = options.IncludeShifts;
                this.ShowProcedures = options.IncludeProcedures;
                this.ShowInternships = options.IncludeInternships;

                // Pobieramy aktualną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new Exception("Nie znaleziono aktywnej specjalizacji");
                }

                // Wczytujemy dane do podglądu
                await this.LoadPreviewDataAsync(specialization.SpecializationId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji podglądu eksportu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania danych podglądu.",
                    "OK");
                await this.OnBackAsync();
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        // Ładowanie danych do podglądu
        private async Task LoadPreviewDataAsync(int specializationId)
        {
            // Pobieramy dane z bazy i przygotowujemy podgląd
            if (this.ShowCourses)
            {
                await this.LoadCoursesPreviewsAsync(specializationId);
            }

            if (this.ShowShifts)
            {
                await this.LoadShiftsPreviewsAsync(specializationId);
            }

            if (this.ShowProcedures)
            {
                await this.LoadProceduresPreviewsAsync(specializationId);
            }

            if (this.ShowInternships)
            {
                await this.LoadInternshipsPreviewsAsync(specializationId);
            }
        }

        private async Task LoadCoursesPreviewsAsync(int specializationId)
        {
            this.CoursesPreviews.Clear();

            try
            {
                var courses = this.moduleId.HasValue
                    ? await this.databaseService.GetCoursesAsync(moduleId: this.moduleId)
                    : await this.databaseService.GetCoursesAsync(specializationId: specializationId);

                // Filtruj po datach, jeśli podano zakres dat
                if (this.startDate != DateTime.MinValue && this.endDate != DateTime.MinValue)
                {
                    courses = courses.Where(c => c.CompletionDate >= this.startDate && c.CompletionDate <= this.endDate).ToList();
                }

                // Dodaj elementy do podglądu
                for (int i = 0; i < courses.Count; i++)
                {
                    var course = courses[i];
                    string status;

                    if (this.formatForOldSmk)
                    {
                        // Status w starym SMK
                        status = !course.RequiresApproval ? 
                            "Zaliczony elektronicznie" : 
                            string.IsNullOrEmpty(course.RecognitionType) ? "Wymaga akceptacji" : "Uznany";
                    }
                    else
                    {
                        // Status w nowym SMK
                        status = course.HasCertificate ? "Posiada certyfikat" : "Ukończony";
                    }

                    this.CoursesPreviews.Add(new CoursePreviewItem
                    {
                        CourseName = course.CourseName,
                        InstitutionName = course.InstitutionName,
                        CompletionDate = course.CompletionDate,
                        Status = status,
                        IsAlternate = i % 2 == 1
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania podglądu kursów: {ex.Message}");
            }
        }

        private async Task LoadShiftsPreviewsAsync(int specializationId)
        {
            this.ShiftsPreviews.Clear();

            try
            {
                // Pobieramy wszystkie staże dla specjalizacji lub modułu
                var internships = this.moduleId.HasValue
                    ? await this.databaseService.GetInternshipsAsync(moduleId: this.moduleId)
                    : await this.databaseService.GetInternshipsAsync(specializationId: specializationId);

                var allShifts = new List<MedicalShift>();

                // Pobieramy dyżury dla każdego stażu
                foreach (var internship in internships)
                {
                    var shifts = await this.databaseService.GetMedicalShiftsAsync(internship.InternshipId);
                    if (shifts != null && shifts.Count > 0)
                    {
                        allShifts.AddRange(shifts);
                    }
                }

                // Filtrujemy po datach
                if (this.startDate != DateTime.MinValue && this.endDate != DateTime.MinValue)
                {
                    allShifts = allShifts.Where(s => s.Date >= this.startDate && s.Date <= this.endDate).ToList();
                }

                // Sortujemy po dacie (od najnowszych)
                allShifts = allShifts.OrderByDescending(s => s.Date).ToList();

                // Dodajemy elementy do podglądu
                for (int i = 0; i < allShifts.Count; i++)
                {
                    var shift = allShifts[i];
                    string duration = shift.Minutes > 0 ? 
                        $"{shift.Hours}:{shift.Minutes:D2}h" : 
                        $"{shift.Hours}h";

                    this.ShiftsPreviews.Add(new ShiftPreviewItem
                    {
                        Date = shift.Date,
                        Location = shift.Location,
                        Duration = duration,
                        Year = shift.Year.ToString(),
                        IsAlternate = i % 2 == 1
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania podglądu dyżurów: {ex.Message}");
            }
        }

        private async Task LoadProceduresPreviewsAsync(int specializationId)
        {
            this.ProceduresPreviews.Clear();

            try
            {
                // Pobieramy wszystkie staże dla specjalizacji lub modułu
                var internships = this.moduleId.HasValue
                    ? await this.databaseService.GetInternshipsAsync(moduleId: this.moduleId)
                    : await this.databaseService.GetInternshipsAsync(specializationId: specializationId);

                var allProcedures = new List<Procedure>();

                // Pobieramy procedury dla każdego stażu
                foreach (var internship in internships)
                {
                    var procedures = await this.databaseService.GetProceduresAsync(internship.InternshipId);
                    if (procedures != null && procedures.Count > 0)
                    {
                        allProcedures.AddRange(procedures);
                    }
                }

                // Filtrujemy po datach
                if (this.startDate != DateTime.MinValue && this.endDate != DateTime.MinValue)
                {
                    allProcedures = allProcedures.Where(p => p.Date >= this.startDate && p.Date <= this.endDate).ToList();
                }

                // Sortujemy po dacie (od najnowszych)
                allProcedures = allProcedures.OrderByDescending(p => p.Date).ToList();

                // Dodajemy elementy do podglądu
                for (int i = 0; i < allProcedures.Count; i++)
                {
                    var procedure = allProcedures[i];
                    
                    string operatorCode = procedure.OperatorCode;
                    if (this.formatForOldSmk)
                    {
                        // W starym SMK operator ma inne oznaczenia
                        operatorCode = procedure.OperatorCode == "A" ? "Operator" : "Asysta";
                    }

                    this.ProceduresPreviews.Add(new ProcedurePreviewItem
                    {
                        Date = procedure.Date,
                        Code = procedure.Code,
                        OperatorCode = operatorCode,
                        Location = procedure.Location,
                        IsAlternate = i % 2 == 1
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania podglądu procedur: {ex.Message}");
            }
        }

        private async Task LoadInternshipsPreviewsAsync(int specializationId)
        {
            this.InternshipsPreviews.Clear();

            try
            {
                var internships = this.moduleId.HasValue
                    ? await this.databaseService.GetInternshipsAsync(moduleId: this.moduleId)
                    : await this.databaseService.GetInternshipsAsync(specializationId: specializationId);

                // Filtrujemy po datach
                if (this.startDate != DateTime.MinValue && this.endDate != DateTime.MinValue)
                {
                    internships = internships.Where(i =>
                        (i.StartDate >= this.startDate && i.StartDate <= this.endDate) ||
                        (i.EndDate >= this.startDate && i.EndDate <= this.endDate) ||
                        (i.StartDate <= this.startDate && i.EndDate >= this.endDate)
                    ).ToList();
                }

                // Sortujemy po dacie rozpoczęcia (od najnowszych)
                internships = internships.OrderByDescending(i => i.StartDate).ToList();

                // Dodajemy elementy do podglądu
                for (int i = 0; i < internships.Count; i++)
                {
                    var internship = internships[i];
                    
                    this.InternshipsPreviews.Add(new InternshipPreviewItem
                    {
                        InternshipName = internship.InternshipName,
                        InstitutionName = internship.InstitutionName,
                        StartDate = internship.StartDate,
                        EndDate = internship.EndDate,
                        IsAlternate = i % 2 == 1
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania podglądu staży: {ex.Message}");
            }
        }

        // Metody obsługi komend
        private async Task OnBackAsync()
        {
            // Wracamy do strony eksportu bez wykonywania eksportu
            await Shell.Current.GoToAsync("..");
        }

        private void OnSelectTab(string tabName)
        {
            if (!string.IsNullOrEmpty(tabName))
            {
                this.SelectedTab = tabName;
            }
        }

        private async Task OnContinueExportAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Wyświetlamy komunikat o rozpoczęciu eksportu
                await this.dialogService.DisplayAlertAsync(
                    "Eksport danych",
                    "Rozpoczynanie eksportu danych do pliku Excel...",
                    "OK");

                // Wykonujemy eksport używając zapisanych wcześniej opcji
                string filePath = await this.exportService.ExportToExcelAsync(this.exportOptions);

                // Informujemy o zakończeniu eksportu
                bool shareNow = await this.dialogService.DisplayAlertAsync(
                    "Eksport zakończony",
                    $"Dane zostały pomyślnie wyeksportowane do pliku Excel.\nPlik zapisano w: {filePath}\n\nCzy chcesz udostępnić plik?",
                    "Tak",
                    "Nie");

                if (shareNow)
                {
                    // Udostępniamy plik eksportu
                    await this.exportService.ShareExportFileAsync(filePath);
                }

                // Wracamy do strony głównej aplikacji
                await Shell.Current.GoToAsync("//Dashboard");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas eksportu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd eksportu",
                    $"Wystąpił problem podczas eksportu danych: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
