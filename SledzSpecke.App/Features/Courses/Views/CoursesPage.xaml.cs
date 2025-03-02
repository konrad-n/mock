using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Courses.ViewModels;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Courses.Views
{
    public partial class CoursesPage : BaseContentPage
    {
        private readonly ISpecializationService specializationService;
        private CoursesViewModel viewModel;

        public CoursesPage(ISpecializationService specializationService)
        {
            this.specializationService = specializationService;
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<CoursesViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();

                // Po inicjalizacji wyświetl kursy dla domyślnego modułu
                this.DisplayCourses(this.viewModel.CurrentModule);
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się załadować danych kursów.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in CoursesPage: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Odśwież dane kursów przy każdym pokazaniu strony
            if (this.viewModel != null)
            {
                this.viewModel.LoadSpecializationDataAsync().ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        this.DisplayCourses(this.viewModel.CurrentModule);
                    });
                });
            }
        }

        private void DisplayCourses(ModuleType moduleType)
        {
            this.CoursesLayout.Children.Clear();
            this.NoModuleSelectedLabel.IsVisible = false;

            // Ustawienie aktywnego przycisku
            if (moduleType == ModuleType.Basic)
            {
                this.BasicModuleButton.BackgroundColor = new Color(8, 32, 68);
                this.BasicModuleButton.TextColor = Colors.White;
                this.SpecialisticModuleButton.BackgroundColor = new Color(228, 240, 245);
                this.SpecialisticModuleButton.TextColor = Colors.Black;
            }
            else
            {
                this.BasicModuleButton.BackgroundColor = new Color(228, 240, 245);
                this.BasicModuleButton.TextColor = Colors.Black;
                this.SpecialisticModuleButton.BackgroundColor = new Color(8, 32, 68);
                this.SpecialisticModuleButton.TextColor = Colors.White;
            }

            var courses = this.viewModel.GetFilteredCourses();

            if (courses.Count == 0)
            {
                this.CoursesLayout.Children.Add(new Label
                {
                    Text = "Brak kursów do wyświetlenia",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0),
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
                    Style = course.IsCompleted ? (Style)this.Resources["CompletedCourseStyle"] :
                            course.ScheduledDate.HasValue ? (Style)this.Resources["PlannedCourseStyle"] :
                            (Style)this.Resources["PendingCourseStyle"],
                };

                var statusIndicator = new BoxView
                {
                    Color = course.IsCompleted ? Colors.Green :
                            course.ScheduledDate.HasValue ? Colors.Orange :
                            new Color(84, 126, 158),
                    WidthRequest = 16,
                    HeightRequest = 16,
                    CornerRadius = 8,
                    VerticalOptions = LayoutOptions.Center,
                };

                var titleLabel = new Label
                {
                    Text = course.Name,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                };

                var descriptionLabel = new Label
                {
                    Text = $"Czas trwania: {course.DurationDays} dni",
                    FontSize = 14,
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
                                new Color(84, 126, 158),
                };

                var detailsButton = new Button
                {
                    Text = "Szczegóły",
                    HeightRequest = 35,
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 0, 0),
                    BackgroundColor = new Color(36, 193, 222),
                    TextColor = Colors.White,
                    CommandParameter = course.Id,
                };
                detailsButton.Clicked += this.OnCourseDetailsClicked;

                var headerLayout = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = new GridLength(30) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                };
                headerLayout.Add(statusIndicator, 0, 0);
                headerLayout.Add(titleLabel, 1, 0);

                var contentLayout = new VerticalStackLayout
                {
                    Children = { headerLayout, descriptionLabel, statusLabel, detailsButton },
                };

                frame.Content = contentLayout;
                this.CoursesLayout.Children.Add(frame);
            }
        }

        private async void OnCourseDetailsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int courseId)
            {
                var course = this.viewModel.Specialization.RequiredCourses.FirstOrDefault(c => c.Id == courseId);
                if (course != null)
                {
                    await this.Navigation.PushAsync(new CourseDetailsPage(course, this.viewModel.CurrentModule, this.OnCourseUpdated));
                }
            }
        }

        private async void OnBasicModuleButtonClicked(object sender, EventArgs e)
        {
            this.viewModel.SelectBasicModule();
            this.DisplayCourses(this.viewModel.CurrentModule);
        }

        private async void OnSpecialisticModuleButtonClicked(object sender, EventArgs e)
        {
            this.viewModel.SelectSpecialisticModule();
            this.DisplayCourses(this.viewModel.CurrentModule);
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new CourseDetailsPage(null, this.viewModel.CurrentModule, this.OnCourseAdded));
        }

        private async Task OnCourseAdded(Course course)
        {
            await this.specializationService.SaveCourseAsync(course);
            await this.viewModel.LoadSpecializationDataAsync();
            this.DisplayCourses(this.viewModel.CurrentModule);
        }

        private async Task OnCourseUpdated(Course course)
        {
            await this.specializationService.SaveCourseAsync(course);
            await this.viewModel.LoadSpecializationDataAsync();
            this.DisplayCourses(this.viewModel.CurrentModule);
        }
    }
}