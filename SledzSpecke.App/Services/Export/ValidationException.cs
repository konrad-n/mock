namespace SledzSpecke.App.Services.Export
{
    /// <summary>
    /// Wyjątek zgłaszany przy błędach walidacji.
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
