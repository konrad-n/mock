using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.App.ViewModels.Courses
{
    public partial class CoursesViewModel : BaseViewModel
    {
        private readonly ICourseService _courseService;
        
        public CoursesViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            Title = "Kursy";
            Courses = new ObservableCollection<Course>();
            AvailableYears = new ObservableCollection<int>();
        }

        [ObservableProperty]
        private ObservableCollection<Course> courses;
        
        [ObservableProperty]
        private ObservableCollection<int> availableYears;
        
        [ObservableProperty]
        private int selectedYear;
        
        [ObservableProperty]
        private double completionPercentage;
        
        [ObservableProperty]
        private Dictionary<string, (int Required, int Completed)> yearProgress;
        
        [ObservableProperty]
        private ObservableCollection<CourseDefinition> recommendedCourses;

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;
            
            try
            {
                IsBusy = true;
                
                // Załaduj kursy
                var userCourses = await _courseService.GetUserCoursesAsync();
                Courses.Clear();
                foreach (var course in userCourses)
                {
                    Courses.Add(course);
                }
                
                // Załaduj dostępne lata
                var allCourses = await _courseService.GetRequiredCoursesAsync();
                var years = allCourses.Select(c => c.RecommendedYear).Distinct().OrderBy(y => y).ToList();
                AvailableYears.Clear();
                AvailableYears.Add(0); // 0 oznacza "Wszystkie"
                foreach (var year in years)
                {
                    AvailableYears.Add(year);
                }
                
                // Załaduj postępy
                CompletionPercentage = await _courseService.GetCourseProgressAsync();
                YearProgress = await _courseService.GetCourseProgressByYearAsync();
                
                // Załaduj rekomendowane kursy
                var recommended = await _courseService.GetRecommendedCoursesForCurrentYearAsync();
                RecommendedCourses = new ObservableCollection<CourseDefinition>(recommended);
                
                // Ustaw domyślne filtrowanie
                SelectedYear = 0; // Wszystkie
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSelectedYearChanged(int value)
        {
            _ = FilterCoursesAsync();
        }

        private async Task FilterCoursesAsync()
        {
            if (IsBusy) return;
            
            try
            {
                IsBusy = true;
                var allCourses = await _courseService.GetUserCoursesAsync();
                
                // Filtruj według roku
                if (SelectedYear != 0) // 0 oznacza "Wszystkie"
                {
                    // Potrzebujemy definicji kursów, aby znać ich przypisany rok
                    var requiredCourses = await _courseService.GetRequiredCoursesAsync();
                    var coursesByYear = requiredCourses
                        .Where(c => c.RecommendedYear == SelectedYear)
                        .Select(c => c.Id)
                        .ToHashSet();
                    
                    allCourses = allCourses
                        .Where(c => coursesByYear.Contains(c.CourseDefinitionId))
                        .ToList();
                }
                
                Courses.Clear();
                foreach (var course in allCourses)
                {
                    Courses.Add(course);
                }
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zafiltrować danych: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddCourseAsync()
        {
            await Shell.Current.GoToAsync("course/add");
        }

        [RelayCommand]
        private async Task ViewCourseAsync(int id)
        {
            await Shell.Current.GoToAsync($"course/edit?id={id}");
        }

        [RelayCommand]
        private async Task ViewCourseDetailsAsync(int id)
        {
            await Shell.Current.GoToAsync($"course/details?id={id}");
        }
    }
}
