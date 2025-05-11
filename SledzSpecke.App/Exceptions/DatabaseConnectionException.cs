namespace SledzSpecke.App.Exceptions
{
    public class DatabaseConnectionException : DataAccessException
    {
        public DatabaseConnectionException(
            string message,
            string userFriendlyMessage = "Nie można połączyć się z bazą danych.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}