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

        public Course Course => this._course;

        public CourseDetailsViewModel(ILogger<CourseDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczegóły kursu";
        }

        public void Initialize(Course course, ModuleType currentModule, Func<Course, Task> onSaveCallback)
        {
            this._onSaveCallback = onSaveCallback;

            if (course == null)
            {
                // Nowy kurs
                this._course = new Course
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    IsRequired = true,
                };

                this.PageTitle = "Dodaj kurs";
                this.ModulePickerSelectedIndex = currentModule == ModuleType.Basic ? 0 : 1;
                this.StatusPickerSelectedIndex = 0; // Domyślnie "Oczekujący"
            }
            else
            {
                // Edycja istniejącego kursu
                this._course = course;
                this.PageTitle = "Szczegóły kursu";
                this.DurationDays = course.DurationDays.ToString();

                // Ustawienie pickerów
                this.ModulePickerSelectedIndex = course.Module == ModuleType.Basic ? 0 : 1;

                // Ustawienie statusu
                if (course.IsCompleted)
                {
                    this.StatusPickerSelectedIndex = 3; // Ukończony
                    this.IsCompletionDateVisible = true;
                    this.IsCompletionVisible = true;
                    if (course.CompletionDate.HasValue)
                    {
                        this.CompletionDate = course.CompletionDate.Value;
                    }
                }
                else if (course.IsAttended)
                {
                    this.StatusPickerSelectedIndex = 2; // Zarejestrowany
                    this.IsDateVisible = true;
                    if (course.ScheduledDate.HasValue)
                    {
                        this.CourseDate = course.ScheduledDate.Value;
                    }
                }
                else if (course.ScheduledDate.HasValue)
                {
                    this.StatusPickerSelectedIndex = 1; // Zaplanowany
                    this.IsDateVisible = true;
                    this.CourseDate = course.ScheduledDate.Value;
                }
                else
                {
                    this.StatusPickerSelectedIndex = 0; // Oczekujący
                }
            }
        }

        [RelayCommand]
        public void UpdateModuleType(int selectedIndex)
        {
            if (this._course != null)
            {
                this._course.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
            }
        }

        [RelayCommand]
        public void UpdateStatus(int selectedIndex)
        {
            if (this._course == null) return;

            switch (selectedIndex)
            {
                case 0: // Oczekujący
                    this.IsDateVisible = false;
                    this.IsCompletionDateVisible = false;
                    this.IsCompletionVisible = false;
                    this._course.ScheduledDate = null;
                    this._course.IsRegistered = false;
                    this._course.IsAttended = false;
                    this._course.IsCompleted = false;
                    this._course.CompletionDate = null;
                    break;
                case 1: // Zaplanowany
                    this.IsDateVisible = true;
                    this.IsCompletionDateVisible = false;
                    this.IsCompletionVisible = false;
                    this._course.ScheduledDate = this.CourseDate;
                    this._course.IsRegistered = false;
                    this._course.IsAttended = false;
                    this._course.IsCompleted = false;
                    this._course.CompletionDate = null;
                    break;
                case 2: // Zarejestrowany
                    this.IsDateVisible = true;
                    this.IsCompletionDateVisible = false;
                    this.IsCompletionVisible = false;
                    this._course.ScheduledDate = this.CourseDate;
                    this._course.IsRegistered = true;
                    this._course.IsAttended = true;
                    this._course.IsCompleted = false;
                    this._course.CompletionDate = null;
                    break;
                case 3: // Ukończony
                    this.IsDateVisible = false;
                    this.IsCompletionDateVisible = true;
                    this.IsCompletionVisible = true;
                    this._course.IsRegistered = true;
                    this._course.IsAttended = true;
                    this._course.IsCompleted = true;
                    this._course.CompletionDate = this.CompletionDate;
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
                    this._course.CertificateFilePath = fileResult.FullPath;
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Plik został dodany pomyślnie.", "OK");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error adding attachment");
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
            if (string.IsNullOrWhiteSpace(this._course.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa kursu jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(this.DurationDays, out int duration) || duration <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawny czas trwania kursu.", "OK");
                return;
            }

            this._course.DurationDays = duration;

            // Dodatkowe ustawienia w zależności od statusu
            if (this.IsDateVisible && this._course.ScheduledDate == null)
            {
                this._course.ScheduledDate = this.CourseDate;
            }

            if (this.IsCompletionDateVisible && this._course.CompletionDate == null)
            {
                this._course.CompletionDate = this.CompletionDate;
            }

            if (this._onSaveCallback != null)
            {
                await this._onSaveCallback(this._course);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}