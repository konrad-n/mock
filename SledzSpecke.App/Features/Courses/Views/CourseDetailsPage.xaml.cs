using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Views
{
    public partial class CourseDetailsPage : ContentPage
    {
        private Course _course;
        private ModuleType _currentModule;
        private Func<Course, Task> _onSaveCallback;

        public string PageTitle { get; set; }
        public string DurationDays { get; set; }
        public DateTime CourseDate { get; set; } = DateTime.Now.AddDays(30);
        public DateTime CompletionDate { get; set; } = DateTime.Now;
        public bool IsDateVisible { get; set; }
        public bool IsCompletionDateVisible { get; set; }
        public bool IsCompletionVisible { get; set; }

        public Course Course => _course;

        public CourseDetailsPage(Course course, ModuleType currentModule, Func<Course, Task> onSaveCallback)
        {
            InitializeComponent();
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
            }
            else
            {
                // Edycja istniejącego kursu
                _course = course;
                PageTitle = "Szczegóły kursu";
                DurationDays = course.DurationDays.ToString();

                // Ustawienie pickerów
                ModulePicker.SelectedIndex = course.Module == ModuleType.Basic ? 0 : 1;

                // Ustawienie statusu
                if (course.IsCompleted)
                {
                    StatusPicker.SelectedIndex = 3; // Ukończony
                    IsCompletionDateVisible = true;
                    IsCompletionVisible = true;
                    if (course.CompletionDate.HasValue)
                        CompletionDate = course.CompletionDate.Value;
                }
                else if (course.IsAttended)
                {
                    StatusPicker.SelectedIndex = 2; // Zarejestrowany
                    IsDateVisible = true;
                    if (course.ScheduledDate.HasValue)
                        CourseDate = course.ScheduledDate.Value;
                }
                else if (course.ScheduledDate.HasValue)
                {
                    StatusPicker.SelectedIndex = 1; // Zaplanowany
                    IsDateVisible = true;
                    CourseDate = course.ScheduledDate.Value;
                }
                else
                {
                    StatusPicker.SelectedIndex = 0; // Oczekujący
                }
            }

            BindingContext = this;
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            _course.Module = ModulePicker.SelectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
        }

        private void OnStatusPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (StatusPicker.SelectedIndex)
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
            OnPropertyChanged(nameof(IsDateVisible));
            OnPropertyChanged(nameof(IsCompletionDateVisible));
            OnPropertyChanged(nameof(IsCompletionVisible));
        }

        private async void OnAddAttachmentClicked(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await FilePicker.PickAsync();
                if (fileResult != null)
                {
                    _course.CertificateFilePath = fileResult.FullPath;
                    await DisplayAlert("Sukces", "Plik został dodany pomyślnie.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem z wyborem pliku: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(_course.Name))
            {
                await DisplayAlert("Błąd", "Nazwa kursu jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(DurationDays, out int duration) || duration <= 0)
            {
                await DisplayAlert("Błąd", "Wprowadź poprawny czas trwania kursu.", "OK");
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

            await Navigation.PopAsync();
        }
    }
}