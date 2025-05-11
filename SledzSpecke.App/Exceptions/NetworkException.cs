namespace SledzSpecke.App.Exceptions
{
    public class NetworkException : AppBaseException
    {
        public NetworkException(
            string message,
            string userFriendlyMessage = "Wystąpił problem z połączeniem sieciowym. Sprawdź swoje połączenie i spróbuj ponownie.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}