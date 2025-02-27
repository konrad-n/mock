using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.ViewModels.Reports
{
    public partial class ReportsViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IDutyService _dutyService;
        private readonly IProcedureService _procedureService;
        private readonly IExcelExportService _excelExportService;

        public ReportsViewModel(
            IUserService userService,
            IDutyService dutyService,
            IProcedureService procedureService,
            IExcelExportService excelExportService)
        {
            _userService = userService;
            _dutyService = dutyService;
            _procedureService = procedureService;
            _excelExportService = excelExportService;

            Title = "Eksport danych";
            ExportTypes = new ObservableCollection<string>
            {
                "Wykonane procedury",
                "Dyżury",
                "Kursy",
                "Staże",
                "Wszystkie dane"
            };

            StartDate = DateTime.Today.AddMonths(-6);
            EndDate = DateTime.Today;
        }

        [ObservableProperty]
        private ObservableCollection<string> exportTypes;

        [ObservableProperty]
        private string selectedExportType;

        [ObservableProperty]
        private DateTime startDate;

        [ObservableProperty]
        private DateTime endDate;

        [ObservableProperty]
        private bool dataPreviewAvailable;

        [ObservableProperty]
        private string previewSummary;

        partial void OnSelectedExportTypeChanged(string value)
        {
            LoadDataPreviewAsync();
        }

        private async Task LoadDataPreviewAsync()
        {
            if (IsBusy || string.IsNullOrEmpty(SelectedExportType))
                return;

            try
            {
                IsBusy = true;

                switch (SelectedExportType)
                {
                    case "Wykonane procedury":
                        await PreviewProceduresAsync();
                        break;
                    case "Dyżury":
                        await PreviewDutiesAsync();
                        break;
                    case "Kursy":
                        await PreviewCoursesAsync();
                        break;
                    case "Staże":
                        await PreviewInternshipsAsync();
                        break;
                    case "Wszystkie dane":
                        await PreviewAllDataAsync();
                        break;
                }

                DataPreviewAvailable = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się wczytać podglądu danych: {ex.Message}", "OK");
                DataPreviewAvailable = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PreviewProceduresAsync()
        {
            var procedures = await _procedureService.GetUserProceduresAsync();
            procedures = procedures.Where(p => p.ExecutionDate >= StartDate && p.ExecutionDate <= EndDate).ToList();

            var user = await _userService.GetCurrentUserAsync();

            PreviewSummary = $"Znaleziono {procedures.Count} procedur w wybranym zakresie dat.\n" +
                             $"Dane zostaną wyeksportowane w formacie kompatybilnym z SMK.";
        }

        private async Task PreviewDutiesAsync()
        {
            var duties = await _dutyService.GetUserDutiesAsync(StartDate);
            duties = duties.Where(d => d.StartTime >= StartDate && d.StartTime <= EndDate).ToList();

            PreviewSummary = $"Znaleziono {duties.Count} dyżurów w wybranym zakresie dat.\n" +
                             $"Dane zostaną wyeksportowane w formacie kompatybilnym z SMK.";
        }

        private async Task PreviewCoursesAsync()
        {
            var courses = await Task.FromResult(new List<Course>()); // Zastąp faktycznym wywołaniem

            PreviewSummary = $"Znaleziono {courses.Count} kursów w wybranym zakresie dat.\n" +
                             $"Dane zostaną wyeksportowane w formacie kompatybilnym z SMK.";
        }

        private async Task PreviewInternshipsAsync()
        {
            var internships = await Task.FromResult(new List<Internship>()); // Zastąp faktycznym wywołaniem

            PreviewSummary = $"Znaleziono {internships.Count} staży w wybranym zakresie dat.\n" +
                             $"Dane zostaną wyeksportowane w formacie kompatybilnym z SMK.";
        }

        private async Task PreviewAllDataAsync()
        {
            var procedures = await _procedureService.GetUserProceduresAsync();
            procedures = procedures.Where(p => p.ExecutionDate >= StartDate && p.ExecutionDate <= EndDate).ToList();

            var duties = await _dutyService.GetUserDutiesAsync(StartDate);
            duties = duties.Where(d => d.StartTime >= StartDate && d.StartTime <= EndDate).ToList();

            PreviewSummary = $"Znaleziono:\n" +
                             $"- {procedures.Count} procedur\n" +
                             $"- {duties.Count} dyżurów\n" +
                             $"- 0 kursów\n" +
                             $"- 0 staży\n" +
                             $"w wybranym zakresie dat.\n\n" +
                             $"Dane zostaną wyeksportowane w formacie kompatybilnym z SMK.";
        }

        [RelayCommand]
        private async Task ExportToExcelAsync()
        {
            if (IsBusy || string.IsNullOrEmpty(SelectedExportType))
                return;

            try
            {
                IsBusy = true;

                // Pobierz dane do eksportu
                string fileName = "";

                switch (SelectedExportType)
                {
                    case "Wykonane procedury":
                        var procedures = await _procedureService.GetUserProceduresAsync();
                        procedures = procedures.Where(p => p.ExecutionDate >= StartDate && p.ExecutionDate <= EndDate).ToList();
                        fileName = await _excelExportService.ExportProceduresToExcelAsync(procedures);
                        break;
                    case "Dyżury":
                        var duties = await _dutyService.GetUserDutiesAsync(StartDate);
                        duties = duties.Where(d => d.StartTime >= StartDate && d.StartTime <= EndDate).ToList();
                        fileName = await _excelExportService.ExportDutiesToExcelAsync(duties);
                        break;
                    case "Kursy":
                        // Implementacja dla kursów
                        var courses = new List<Course>();
                        fileName = "Kursy - eksport nie został zaimplementowany";
                        break;
                    case "Staże":
                        // Implementacja dla staży
                        var internships = new List<Internship>();
                        fileName = "Staże - eksport nie został zaimplementowany";
                        break;
                    case "Wszystkie dane":
                        var allProcedures = await _procedureService.GetUserProceduresAsync();
                        allProcedures = allProcedures.Where(p => p.ExecutionDate >= StartDate && p.ExecutionDate <= EndDate).ToList();

                        var allDuties = await _dutyService.GetUserDutiesAsync(StartDate);
                        allDuties = allDuties.Where(d => d.StartTime >= StartDate && d.StartTime <= EndDate).ToList();

                        fileName = await _excelExportService.ExportAllDataToExcelAsync(allProcedures, allDuties);
                        break;
                }

                await Shell.Current.DisplayAlert("Sukces",
                    $"Dane zostały wyeksportowane do pliku:\n{fileName}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd",
                    $"Nie udało się wyeksportować danych: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
