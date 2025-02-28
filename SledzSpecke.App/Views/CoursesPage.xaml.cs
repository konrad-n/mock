using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Views
{
    public partial class CoursesPage : ContentPage
    {
        private Specialization _specialization;
        private ModuleType _currentModule = ModuleType.Basic;

        public CoursesPage()
        {
            InitializeComponent();
            LoadDataAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                // Pobieramy dane z bazy, nie z seedera
                _specialization = await App.SpecializationService.GetSpecializationAsync();
                DisplayCourses(_currentModule);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
            }
        }

        private void DisplayCourses(ModuleType moduleType)
        {
            _currentModule = moduleType;
            CoursesLayout.Children.Clear();
            NoModuleSelectedLabel.IsVisible = false;

            // Ustawienie aktywnego przycisku
            if (moduleType == ModuleType.Basic)
            {
                BasicModuleButton.BackgroundColor = Colors.DarkBlue;
                BasicModuleButton.TextColor = Colors.White;
                SpecialisticModuleButton.BackgroundColor = Colors.LightGray;
                SpecialisticModuleButton.TextColor = Colors.Black;
            }
            else
            {
                BasicModuleButton.BackgroundColor = Colors.LightGray;
                BasicModuleButton.TextColor = Colors.Black;
                SpecialisticModuleButton.BackgroundColor = Colors.DarkBlue;
                SpecialisticModuleButton.TextColor = Colors.White;
            }

            var courses = _specialization.RequiredCourses
                .Where(c => c.Module == moduleType)
                .OrderBy(c => c.IsCompleted)
                .ToList();

            if (courses.Count == 0)
            {
                CoursesLayout.Children.Add(new Label
                {
                    Text = "Brak kursów do wyświetlenia",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            foreach (var course in courses)
            {
                var frame = new Frame
                {
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 10),
                    CornerRadius = 5,
                    Style = course.IsCompleted ? (Style)Resources["CompletedCourseStyle"] :
                            course.ScheduledDate.HasValue ? (Style)Resources["PlannedCourseStyle"] :
                            (Style)Resources["PendingCourseStyle"]
                };

                var statusIndicator = new BoxView
                {
                    Color = course.IsCompleted ? Colors.Green :
                            course.ScheduledDate.HasValue ? Colors.Orange :
                            Colors.Gray,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    CornerRadius = 8,
                    VerticalOptions = LayoutOptions.Center
                };

                var titleLabel = new Label
                {
                    Text = course.Name,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16
                };

                var descriptionLabel = new Label
                {
                    Text = $"Czas trwania: {course.DurationDays} dni",
                    FontSize = 14
                };

                var statusLabel = new Label
                {
                    Text = course.IsCompleted ? "Ukończony" :
                           course.ScheduledDate.HasValue ? $"Zaplanowany na: {course.ScheduledDate.Value.ToString("dd.MM.yyyy")}" :
                           "Oczekujący",
                    FontSize = 14,
                    TextColor = course.IsCompleted ? Colors.Green :
                                course.ScheduledDate.HasValue ? Colors.Orange :
                                Colors.Gray
                };

                var detailsButton = new Button
                {
                    Text = "Szczegóły",
                    HeightRequest = 35,
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 0, 0),
                    CommandParameter = course.Id
                };
                detailsButton.Clicked += OnCourseDetailsClicked;

                var headerLayout = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(30) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
                };
                headerLayout.Add(statusIndicator, 0, 0);
                headerLayout.Add(titleLabel, 1, 0);

                var contentLayout = new VerticalStackLayout
                {
                    Children = { headerLayout, descriptionLabel, statusLabel, detailsButton }
                };

                frame.Content = contentLayout;
                CoursesLayout.Children.Add(frame);
            }
        }

        private void OnBasicModuleButtonClicked(object sender, EventArgs e)
        {
            DisplayCourses(ModuleType.Basic);
        }

        private void OnSpecialisticModuleButtonClicked(object sender, EventArgs e)
        {
            DisplayCourses(ModuleType.Specialistic);
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseDetailsPage(null, _currentModule, OnCourseAdded));
        }

        private async void OnCourseDetailsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int courseId)
            {
                var course = _specialization.RequiredCourses.FirstOrDefault(c => c.Id == courseId);
                if (course != null)
                {
                    await Navigation.PushAsync(new CourseDetailsPage(course, _currentModule, OnCourseUpdated));
                }
            }
        }

        // Zmiana metod callback na async Task
        private async Task OnCourseAdded(Course course)
        {
            await App.SpecializationService.SaveCourseAsync(course);
            LoadDataAsync();
        }

        private async Task OnCourseUpdated(Course course)
        {
            await App.SpecializationService.SaveCourseAsync(course);
            LoadDataAsync();
        }
    }
}