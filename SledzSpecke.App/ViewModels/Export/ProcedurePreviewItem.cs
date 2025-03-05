namespace SledzSpecke.App.ViewModels.Export
{
    public class ProcedurePreviewItem
    {
        public DateTime Date { get; set; }
        public required string Code { get; set; }
        public required string OperatorCode { get; set; }
        public required string Location { get; set; }
        public bool IsAlternate { get; set; }
    }
}
