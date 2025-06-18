namespace SledzSpecke.Application.Pipeline;

public class MessageContext
{
    public object Message { get; }
    public Type MessageType { get; }
    public Dictionary<string, object> Metadata { get; }
    public Guid CorrelationId { get; }
    public DateTime Timestamp { get; }
    
    public MessageContext(object message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        MessageType = message.GetType();
        Metadata = new Dictionary<string, object>();
        CorrelationId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }
    
    public T GetMessage<T>() where T : class
    {
        return Message as T 
            ?? throw new InvalidOperationException($"Message is not of type {typeof(T).Name}");
    }
}