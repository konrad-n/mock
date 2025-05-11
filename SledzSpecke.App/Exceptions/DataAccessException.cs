namespace SledzSpecke.App.Exceptions
{
    public class DataAccessException : AppBaseException
    {
        public DataAccessException(
            string message,
            string userFriendlyMessage = "Wystąpił problem z dostępem do danych.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}