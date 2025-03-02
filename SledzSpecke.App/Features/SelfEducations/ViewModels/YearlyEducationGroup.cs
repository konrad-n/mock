using System.Collections.ObjectModel;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.SelfEducations.ViewModels
{

    public class YearlyEducationGroup : ObservableCollection<SelfEducation>
    {
        public YearlyEducationGroup(int year, int totalDays, ObservableCollection<SelfEducation> items)
            : base(items)
        {
            this.Year = year;
            this.TotalDays = totalDays;
            this.Header = $"Rok {year}";
            this.Summary = $"Wykorzystano: {totalDays}/6 dni";
        }

        public int Year { get; private set; }

        public int TotalDays { get; private set; }

        public string Header { get; private set; }

        public string Summary { get; private set; }
    }
}
