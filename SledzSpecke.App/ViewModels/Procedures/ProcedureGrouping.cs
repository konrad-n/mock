using System.Collections.ObjectModel;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.ViewModels.Procedures
{
    // Klasa pomocnicza do grupowania procedur
    public class ProcedureGrouping : ObservableCollection<ProcedureViewModel>
    {
        public string GroupName { get; private set; }

        public ProcedureGrouping(string groupName, List<Procedure> procedures) : base(procedures.Select(p => ProcedureViewModel.FromModel(p)))
        {
            this.GroupName = groupName;
        }
    }
}