namespace SledzSpecke.App.Exceptions
{
    public class BusinessRuleViolationException : DomainLogicException
    {
        public BusinessRuleViolationException(
            string message,
            string userFriendlyMessage = "Naruszono regułę biznesową.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}