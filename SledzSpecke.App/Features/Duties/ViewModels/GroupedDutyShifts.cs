using System.Collections.ObjectModel;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Duties.ViewModels
{
    public class GroupedDutyShifts : ObservableCollection<DutyShift>
    {
        public GroupedDutyShifts(string title, string subtitle, ObservableCollection<DutyShift> items)
            : base(items)
        {
            this.Title = title;
            this.Subtitle = subtitle;
        }

        public string Title { get; private set; }

        public string Subtitle { get; private set; }
    }
}
