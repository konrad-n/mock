using System.Collections.ObjectModel;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.ViewModels.Procedures
{
    // Klasa pomocnicza do grupowania procedur
    public class ProcedureGrouping : ObservableCollection<Procedure>
    {
        public string GroupName { get; private set; }

        public ProcedureGrouping(string groupName, List<Procedure> procedures) : base(procedures)
        {
            this.GroupName = groupName;
        }
    }
}