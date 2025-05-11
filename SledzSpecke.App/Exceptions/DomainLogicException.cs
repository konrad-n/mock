namespace SledzSpecke.App.Exceptions
{
    public class DomainLogicException : AppBaseException
    {
        public DomainLogicException(
            string message,
            string userFriendlyMessage = "Wystąpił błąd logiki biznesowej.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}