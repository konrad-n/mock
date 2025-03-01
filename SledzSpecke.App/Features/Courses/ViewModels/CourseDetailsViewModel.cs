using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Courses.ViewModels
{
    public partial class CourseDetailsViewModel : ViewModelBase
    {
        private Course _course;
        private ModuleType _currentModule;
        private Func<Course, Task> _onSaveCallback;

        [ObservableProperty]
        private string _pageTitle;

        [ObservableProperty]
        private string _durationDays;

        [ObservableProperty]
        private DateTime _courseDate = DateTime.Now.AddDays(30);

        [ObservableProperty]
        private DateTime _completionDate = DateTime.Now;

        [ObservableProperty]
        private bool _isDateVisible;

        [ObservableProperty]
        private bool _isCompletionDateVisible;

        [ObservableProperty]
        private bool _isCompletionVisible;

        [ObservableProperty]
        private int _modulePickerSelectedIndex;

        [ObservableProperty]
        private int _statusPickerSelectedIndex;

        public Course Course => _course;

        public CourseDetailsViewModel(ILogger<CourseDetailsViewModel> logger) : base(logger)
        {
            Title = "Szczegóły kursu";
        }

        public void Initialize(Course course, ModuleType currentModule, Func<Course, Task> onSaveCallback)
        {
            _currentModule = currentModule;
            _onSaveCallback = onSaveCallback;

            if (course == null)
            {
                // Nowy kurs
                _course = new Course
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    IsRequired = true
                };
                PageTitle = "Dodaj kurs";
                ModulePickerSelectedIndex = currentModule == ModuleType.Basic ? 0 : 1;
                StatusPickerSelectedIndex = 0; // Domyślnie "Oczekujący"
            }
            else
            {
                // Edycja istniejącego kursu
                _course = course;
                PageTitle = "Szczegóły kursu";
                DurationDays = course.DurationDays.ToString();

                // Ustawienie pickerów
                ModulePickerSelectedIndex = course.Module == ModuleType.Basic ? 0 : 1;

                // Ustawienie statusu
                if (course.IsCompleted)
                {
                    StatusPickerSelectedIndex = 3; // Ukończony
                    IsCompletionDateVisible = true;
                    IsCompletionVisible = true;
                    if (course.CompletionDate.HasValue)
                        CompletionDate = course.CompletionDate.Value;
                }
                else if (course.IsAttended)
                {
                    StatusPickerSelectedIndex = 2; // Zarejestrowany
                    IsDateVisible = true;
                    if (course.ScheduledDate.HasValue)
                        CourseDate = course.ScheduledDate.Value;
                }
                else if (course.ScheduledDate.HasValue)
                {
                    StatusPickerSelectedIndex = 1; // Zaplanowany
                    IsDateVisible = true;
                    CourseDate = course.ScheduledDate.Value;
                }
                else
                {
                    StatusPickerSelectedIndex = 0; // Oczekujący
                }
            }
        }

        [RelayCommand]
        public void UpdateModuleType(int selectedIndex)
        {
            if (_course != null)
            {
                _course.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
            }
        }

        [RelayCommand]
        public void UpdateStatus(int selectedIndex)
        {
            if (_course == null) return;

            switch (selectedIndex)
            {
                case 0: // Oczekujący
                    IsDateVisible = false;
                    IsCompletionDateVisible = false;
                    IsCompletionVisible = false;
                    _course.ScheduledDate = null;
                    _course.IsRegistered = false;
                    _course.IsAttended = false;
                    _course.IsCompleted = false;
                    _course.CompletionDate = null;
                    break;
                case 1: // Zaplanowany
                    IsDateVisible = true;
                    IsCompletionDateVisible = false;
                    IsCompletionVisible = false;
                    _course.ScheduledDate = CourseDate;
                    _course.IsRegistered = false;
                    _course.IsAttended = false;
                    _course.IsCompleted = false;
                    _course.CompletionDate = null;
                    break;
                case 2: // Zarejestrowany
                    IsDateVisible = true;
                    IsCompletionDateVisible = false;
                    IsCompletionVisible = false;
                    _course.ScheduledDate = CourseDate;
                    _course.IsRegistered = true;
                    _course.IsAttended = true;
                    _course.IsCompleted = false;
                    _course.CompletionDate = null;
                    break;
                case 3: // Ukończony
                    IsDateVisible = false;
                    IsCompletionDateVisible = true;
                    IsCompletionVisible = true;
                    _course.IsRegistered = true;
                    _course.IsAttended = true;
                    _course.IsCompleted = true;
                    _course.CompletionDate = CompletionDate;
                    break;
            }
        }

        [RelayCommand]
        private async Task AddAttachmentAsync()
        {
            try
            {
                var fileResult = await FilePicker.PickAsync();
                if (fileResult != null)
                {
                    _course.CertificateFilePath = fileResult.FullPath;
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Plik został dodany pomyślnie.", "OK");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attachment");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił problem z wyborem pliku: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(_course.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa kursu jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(DurationDays, out int duration) || duration <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawny czas trwania kursu.", "OK");
                return;
            }

            _course.DurationDays = duration;

            // Dodatkowe ustawienia w zależności od statusu
            if (IsDateVisible && _course.ScheduledDate == null)
            {
                _course.ScheduledDate = CourseDate;
            }

            if (IsCompletionDateVisible && _course.CompletionDate == null)
            {
                _course.CompletionDate = CompletionDate;
            }

            if (_onSaveCallback != null)
            {
                await _onSaveCallback(_course);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}