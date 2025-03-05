using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Courses
{
    [QueryProperty(nameof(CourseId), "courseId")]
    public class CourseDetailsViewModel : BaseViewModel
    {
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private int courseId;
        private Course course;
        private string courseName;
        private string courseType;
        private string courseNumber;
        private string institutionName;
        private DateTime completionDate;
        private int year;
        private int courseSequenceNumber;
        private bool hasCertificate;
        private string certificateNumber;
        private DateTime? certificateDate;
        private string syncStatusText;
        private bool isNotSynced;
        private string additionalDetails;
        private string moduleInfo;

        public CourseDetailsViewModel(
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja komend
            this.EditCommand = new AsyncRelayCommand(this.OnEditAsync);
            this.DeleteCommand = new AsyncRelayCommand(this.OnDeleteAsync);
            this.GoBackCommand = new AsyncRelayCommand(this.OnGoBackAsync);

            // Inicjalizacja właściwości
            this.Title = "Szczegóły kursu";
        }

        // Właściwości
        public int CourseId
        {
            get => this.courseId;
            set
            {
                this.SetProperty(ref this.courseId, value);
                this.LoadCourseAsync(value).ConfigureAwait(false);
            }
        }

        public Course Course
        {
            get => this.course;
            set => this.SetProperty(ref this.course, value);
        }

        public string CourseName
        {
            get => this.courseName;
            set => this.SetProperty(ref this.courseName, value);
        }

        public string CourseType
        {
            get => this.courseType;
            set => this.SetProperty(ref this.courseType, value);
        }

        public string CourseNumber
        {
            get => this.courseNumber;
            set => this.SetProperty(ref this.courseNumber, value);
        }

        public string InstitutionName
        {
            get => this.institutionName;
            set => this.SetProperty(ref this.institutionName, value);
        }

        public DateTime CompletionDate
        {
            get => this.completionDate;
            set => this.SetProperty(ref this.completionDate, value);
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

        public string SyncStatusText
        {
            get => this.syncStatusText;
            set => this.SetProperty(ref this.syncStatusText, value);
        }

        public bool IsNotSynced
        {
            get => this.isNotSynced;
            set => this.SetProperty(ref this.isNotSynced, value);
        }

        public string AdditionalDetails
        {
            get => this.additionalDetails;
            set => this.SetProperty(ref this.additionalDetails, value);
        }

        public string ModuleInfo
        {
            get => this.moduleInfo;
            set => this.SetProperty(ref this.moduleInfo, value);
        }

        public string FormattedCompletionDate => this.CompletionDate.ToString("d");

        public string CertificateInfo
        {
            get
            {
                if (this.HasCertificate)
                {
                    string info = "Certyfikat: " + this.CertificateNumber;
                    if (this.CertificateDate.HasValue)
                    {
                        info += " z dnia " + this.CertificateDate.Value.ToString("d");
                    }

                    return info;
                }

                return "Brak certyfikatu";
            }
        }

        // Komendy
        public ICommand EditCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand GoBackCommand { get; }

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
                this.Course = await this.databaseService.GetCourseAsync(courseId);
                if (this.Course == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono kursu.",
                        "OK");
                    await this.OnGoBackAsync();
                    return;
                }

                // Ustawienie właściwości
                this.CourseName = this.Course.CourseName;
                this.CourseType = this.Course.CourseType;
                this.CourseNumber = this.Course.CourseNumber;
                this.InstitutionName = this.Course.InstitutionName;
                this.CompletionDate = this.Course.CompletionDate;
                this.Year = this.Course.Year;
                this.CourseSequenceNumber = this.Course.CourseSequenceNumber;
                this.HasCertificate = this.Course.HasCertificate;
                this.CertificateNumber = this.Course.CertificateNumber;
                this.CertificateDate = this.Course.CertificateDate;

                // Ustawienie statusu synchronizacji
                this.IsNotSynced = this.Course.SyncStatus != SyncStatus.Synced;
                this.SyncStatusText = this.GetSyncStatusText(this.Course.SyncStatus);

                // Pobierz dodatkowe informacje o module, jeśli dostępne
                if (this.Course.ModuleId.HasValue && this.Course.ModuleId.Value > 0)
                {
                    var module = await this.databaseService.GetModuleAsync(this.Course.ModuleId.Value);
                    if (module != null)
                    {
                        this.ModuleInfo = $"Moduł: {module.Name}";
                    }
                    else
                    {
                        this.ModuleInfo = string.Empty;
                    }
                }
                else
                {
                    this.ModuleInfo = string.Empty;
                }

                // Parsowanie dodatkowych pól
                if (!string.IsNullOrEmpty(this.Course.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            this.Course.AdditionalFields);

                        this.AdditionalDetails = string.Join("\n", additionalFields.Select(kv => $"{kv.Key}: {kv.Value}"));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd parsowania pól dodatkowych: {ex.Message}");
                        this.AdditionalDetails = string.Empty;
                    }
                }
                else
                {
                    this.AdditionalDetails = string.Empty;
                }
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

        private string GetSyncStatusText(SyncStatus status)
        {
            return status switch
            {
                SyncStatus.NotSynced => "Nie zsynchronizowano z SMK",
                SyncStatus.Synced => "Zsynchronizowano z SMK",
                SyncStatus.SyncFailed => "Synchronizacja nie powiodła się",
                SyncStatus.Modified => "Zsynchronizowano, ale potem zmodyfikowano",
                _ => "Nieznany status",
            };
        }

        private async Task OnEditAsync()
        {
            if (this.Course != null)
            {
                await Shell.Current.GoToAsync($"AddEditCourse?courseId={this.Course.CourseId}");
            }
        }

        private async Task OnDeleteAsync()
        {
            if (this.Course == null)
            {
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdź usunięcie",
                "Czy na pewno chcesz usunąć ten kurs? Tej operacji nie można cofnąć.",
                "Tak, usuń",
                "Anuluj");

            if (confirm)
            {
                try
                {
                    await this.databaseService.DeleteCourseAsync(this.Course);
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        "Kurs został pomyślnie usunięty.",
                        "OK");
                    await this.OnGoBackAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania kursu: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się usunąć kursu. Spróbuj ponownie.",
                        "OK");
                }
            }
        }

        private async Task OnGoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}