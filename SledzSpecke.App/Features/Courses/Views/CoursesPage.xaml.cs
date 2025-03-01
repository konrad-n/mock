using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Courses.ViewModels;
using SledzSpecke.App.Services;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Courses.Views
{
    public partial class CoursesPage : BaseContentPage
    {
        private CoursesViewModel _viewModel;
        private readonly ISpecializationService _specializationService;

        public CoursesPage(ISpecializationService specializationService)
        {
            _specializationService = specializationService;
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<CoursesViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();

                // Po inicjalizacji wyświetl kursy dla domyślnego modułu
                DisplayCourses(_viewModel.CurrentModule);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się załadować danych kursów.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in CoursesPage: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Odśwież dane kursów przy każdym pokazaniu strony
            if (_viewModel != null)
            {
                _viewModel.LoadSpecializationDataAsync().ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        DisplayCourses(_viewModel.CurrentModule);
                    });
                });
            }
        }

        private void DisplayCourses(ModuleType moduleType)
        {
            CoursesLayout.Children.Clear();
            NoModuleSelectedLabel.IsVisible = false;

            // Ustawienie aktywnego przycisku
            if (moduleType == ModuleType.Basic)
            {
                BasicModuleButton.BackgroundColor = new Color(8, 32, 68);
                BasicModuleButton.TextColor = Colors.White;
                SpecialisticModuleButton.BackgroundColor = new Color(228, 240, 245);
                SpecialisticModuleButton.TextColor = Colors.Black;
            }
            else
            {
                BasicModuleButton.BackgroundColor = new Color(228, 240, 245);
                BasicModuleButton.TextColor = Colors.Black;
                SpecialisticModuleButton.BackgroundColor = new Color(8, 32, 68);
                SpecialisticModuleButton.TextColor = Colors.White;
            }

            var courses = _viewModel.GetFilteredCourses();

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
                            new Color(84, 126, 158),
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
                           course.IsAttended ? "Zarejestrowany" :
                           course.ScheduledDate.HasValue ? $"Zaplanowany na: {course.ScheduledDate.Value.ToString("dd.MM.yyyy")}" :
                           "Oczekujący",
                    FontSize = 14,
                    TextColor = course.IsCompleted ? Colors.Green :
                                course.ScheduledDate.HasValue ? Colors.Orange :
                                new Color(84, 126, 158)
                };

                var detailsButton = new Button
                {
                    Text = "Szczegóły",
                    HeightRequest = 35,
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 0, 0),
                    BackgroundColor = new Color(36, 193, 222),
                    TextColor = Colors.White,
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

        private async void OnCourseDetailsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int courseId)
            {
                var course = _viewModel.Specialization.RequiredCourses.FirstOrDefault(c => c.Id == courseId);
                if (course != null)
                {
                    await Navigation.PushAsync(new CourseDetailsPage(course, _viewModel.CurrentModule, OnCourseUpdated));
                }
            }
        }

        private async void OnBasicModuleButtonClicked(object sender, EventArgs e)
        {
            _viewModel.SelectBasicModule();
            DisplayCourses(_viewModel.CurrentModule);
        }

        private async void OnSpecialisticModuleButtonClicked(object sender, EventArgs e)
        {
            _viewModel.SelectSpecialisticModule();
            DisplayCourses(_viewModel.CurrentModule);
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseDetailsPage(null, _viewModel.CurrentModule, OnCourseAdded));
        }

        // Metody callback zostają bez zmian, aby zachować pełną funkcjonalność
        private async Task OnCourseAdded(Course course)
        {
            await _specializationService.SaveCourseAsync(course);
            await _viewModel.LoadSpecializationDataAsync();
            DisplayCourses(_viewModel.CurrentModule);
        }

        private async Task OnCourseUpdated(Course course)
        {
            await _specializationService.SaveCourseAsync(course);
            await _viewModel.LoadSpecializationDataAsync();
            DisplayCourses(_viewModel.CurrentModule);
        }
    }
}