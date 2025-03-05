namespace SledzSpecke.App.ViewModels.Export
{
    // Klasy pomocnicze do prezentacji danych w podglądzie
    public class CoursePreviewItem
    {
        public string CourseName { get; set; }
        public string InstitutionName { get; set; }

        public DateTime CompletionDate { get; set; }

        public string Status { get; set; }

        public bool IsAlternate { get; set; }
    }
}
