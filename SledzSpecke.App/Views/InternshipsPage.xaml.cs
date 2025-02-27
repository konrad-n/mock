using SledzSpecke.Services;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Views
{
    public partial class InternshipsPage : ContentPage
    {
        private Specialization _specialization;
        private ModuleType _currentModule = ModuleType.Basic;



        public InternshipsPage()
        {
            InitializeComponent();
            _specialization = DataSeeder.SeedHematologySpecialization();
            DisplayInternships(_currentModule);
        }

        private void DisplayInternships(ModuleType moduleType)
        {
            _currentModule = moduleType;
            InternshipsLayout.Children.Clear();
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

            var internships = _specialization.RequiredInternships
                .Where(i => i.Module == moduleType)
                .OrderBy(i => i.IsCompleted)
                .ToList();

            if (internships.Count == 0)
            {
                InternshipsLayout.Children.Add(new Label
                {
                    Text = "Brak staży do wyświetlenia",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            foreach (var internship in internships)
            {
                bool isCurrentInternship = internship.StartDate.HasValue &&
                                           !internship.EndDate.HasValue;

                var frame = new Frame
                {
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 10),
                    CornerRadius = 5,
                    Style = internship.IsCompleted ? (Style)Resources["CompletedInternshipStyle"] :
                            isCurrentInternship ? (Style)Resources["CurrentInternshipStyle"] :
                            internship.StartDate.HasValue ? (Style)Resources["PlannedInternshipStyle"] :
                            (Style)Resources["PendingInternshipStyle"]
                };

                var statusIndicator = new BoxView
                {
                    Color = internship.IsCompleted ? Colors.Green :
                            isCurrentInternship ? Colors.Blue :
                            internship.StartDate.HasValue ? Colors.Orange :
                            Colors.Gray,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    CornerRadius = 8,
                    VerticalOptions = LayoutOptions.Center
                };

                var titleLabel = new Label
                {
                    Text = internship.Name,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16
                };

                var durationLabel = new Label
                {
                    Text = $"Czas trwania: {internship.DurationWeeks} tygodni ({internship.WorkingDays} dni roboczych)",
                    FontSize = 14
                };

                string statusText;
                if (internship.IsCompleted)
                    statusText = "Ukończony";
                else if (isCurrentInternship)
                    statusText = $"W trakcie od: {internship.StartDate.Value.ToString("dd.MM.yyyy")}";
                else if (internship.StartDate.HasValue)
                    statusText = $"Zaplanowany na: {internship.StartDate.Value.ToString("dd.MM.yyyy")}";
                else
                    statusText = "Oczekujący";

                var statusLabel = new Label
                {
                    Text = statusText,
                    FontSize = 14,
                    TextColor = internship.IsCompleted ? Colors.Green :
                                isCurrentInternship ? Colors.Blue :
                                internship.StartDate.HasValue ? Colors.Orange :
                                Colors.Gray
                };

                var locationLabel = new Label
                {
                    Text = !string.IsNullOrEmpty(internship.Location) ? $"Miejsce: {internship.Location}" : "Miejsce: Nieokreślone",
                    FontSize = 14,
                    IsVisible = !string.IsNullOrEmpty(internship.Location) || isCurrentInternship || internship.IsCompleted
                };

                var supervisorLabel = new Label
                {
                    Text = !string.IsNullOrEmpty(internship.SupervisorName) ? $"Kierownik: {internship.SupervisorName}" : "Kierownik: Nieokreślony",
                    FontSize = 14,
                    IsVisible = !string.IsNullOrEmpty(internship.SupervisorName) || isCurrentInternship || internship.IsCompleted
                };

                var detailsButton = new Button
                {
                    Text = "Szczegóły",
                    HeightRequest = 35,
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 0, 0),
                    CommandParameter = internship.Id
                };
                detailsButton.Clicked += OnInternshipDetailsClicked;

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
                    Children = { headerLayout, durationLabel, statusLabel }
                };

                if (locationLabel.IsVisible)
                    contentLayout.Children.Add(locationLabel);

                if (supervisorLabel.IsVisible)
                    contentLayout.Children.Add(supervisorLabel);

                contentLayout.Children.Add(detailsButton);

                frame.Content = contentLayout;
                InternshipsLayout.Children.Add(frame);
            }
        }

        private void OnBasicModuleButtonClicked(object sender, EventArgs e)
        {
            DisplayInternships(ModuleType.Basic);
        }

        private void OnSpecialisticModuleButtonClicked(object sender, EventArgs e)
        {
            DisplayInternships(ModuleType.Specialistic);
        }

        private async void OnAddInternshipClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InternshipDetailsPage(null, _currentModule, OnInternshipAdded));
        }

        private async void OnInternshipDetailsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int internshipId)
            {
                var internship = _specialization.RequiredInternships.FirstOrDefault(i => i.Id == internshipId);
                if (internship != null)
                {
                    await Navigation.PushAsync(new InternshipDetailsPage(internship, _currentModule, OnInternshipUpdated));
                }
            }
        }

        private void OnInternshipAdded(Internship internship)
        {
            _specialization.RequiredInternships.Add(internship);
            DisplayInternships(_currentModule);
        }

        private void OnInternshipUpdated(Internship internship)
        {
            var existingInternship = _specialization.RequiredInternships.FirstOrDefault(i => i.Id == internship.Id);
            if (existingInternship != null)
            {
                var index = _specialization.RequiredInternships.IndexOf(existingInternship);
                _specialization.RequiredInternships[index] = internship;
            }
            DisplayInternships(_currentModule);
        }
    }
}