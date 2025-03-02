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
        private Course course;
        private Func<Course, Task> onSaveCallback;

        [ObservableProperty]
        private string pageTitle;

        [ObservableProperty]
        private string durationDays;

        [ObservableProperty]
        private DateTime courseDate = DateTime.Now.AddDays(30);

        [ObservableProperty]
        private DateTime completionDate = DateTime.Now;

        [ObservableProperty]
        private bool isDateVisible;

        [ObservableProperty]
        private bool isCompletionDateVisible;

        [ObservableProperty]
        private bool isCompletionVisible;

        [ObservableProperty]
        private int modulePickerSelectedIndex;

        [ObservableProperty]
        private int statusPickerSelectedIndex;

        public CourseDetailsViewModel(ILogger<CourseDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczegóły kursu";
        }

        public Course Course => this.course;

        public void Initialize(Course course, ModuleType currentModule, Func<Course, Task> onSaveCallback)
        {
            this.onSaveCallback = onSaveCallback;

            if (course == null)
            {
                // Nowy kurs
                this.course = new Course
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
                this.course = course;
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
            if (this.course != null)
            {
                this.course.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
            }
        }

        [RelayCommand]
        public void UpdateStatus(int selectedIndex)
        {
            if (this.course == null)
            {
                return;
            }

            switch (selectedIndex)
            {
                case 0: // Oczekujący
                    this.IsDateVisible = false;
                    this.IsCompletionDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.course.ScheduledDate = null;
                    this.course.IsRegistered = false;
                    this.course.IsAttended = false;
                    this.course.IsCompleted = false;
                    this.course.CompletionDate = null;
                    break;
                case 1: // Zaplanowany
                    this.IsDateVisible = true;
                    this.IsCompletionDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.course.ScheduledDate = this.CourseDate;
                    this.course.IsRegistered = false;
                    this.course.IsAttended = false;
                    this.course.IsCompleted = false;
                    this.course.CompletionDate = null;
                    break;
                case 2: // Zarejestrowany
                    this.IsDateVisible = true;
                    this.IsCompletionDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.course.ScheduledDate = this.CourseDate;
                    this.course.IsRegistered = true;
                    this.course.IsAttended = true;
                    this.course.IsCompleted = false;
                    this.course.CompletionDate = null;
                    break;
                case 3: // Ukończony
                    this.IsDateVisible = false;
                    this.IsCompletionDateVisible = true;
                    this.IsCompletionVisible = true;
                    this.course.IsRegistered = true;
                    this.course.IsAttended = true;
                    this.course.IsCompleted = true;
                    this.course.CompletionDate = this.CompletionDate;
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
                    this.course.CertificateFilePath = fileResult.FullPath;
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Plik został dodany pomyślnie.", "OK");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error adding attachment");
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
            if (string.IsNullOrWhiteSpace(this.course.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa kursu jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(this.DurationDays, out int duration) || duration <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawny czas trwania kursu.", "OK");
                return;
            }

            this.course.DurationDays = duration;

            // Dodatkowe ustawienia w zależności od statusu
            if (this.IsDateVisible && this.course.ScheduledDate == null)
            {
                this.course.ScheduledDate = this.CourseDate;
            }

            if (this.IsCompletionDateVisible && this.course.CompletionDate == null)
            {
                this.course.CompletionDate = this.CompletionDate;
            }

            if (this.onSaveCallback != null)
            {
                await this.onSaveCallback(this.course);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}