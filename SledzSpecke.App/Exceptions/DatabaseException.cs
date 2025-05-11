namespace SledzSpecke.App.Exceptions
{
    public class DatabaseException : DataAccessException
    {
        public DatabaseException(
            string message,
            string userFriendlyMessage = "Wystąpił problem z bazą danych.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
