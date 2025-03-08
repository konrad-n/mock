namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class ExaminationInfo
    {
        public string ExamType { get; set; }
        public List<ExaminationComponent> Components { get; set; }
    }
}
