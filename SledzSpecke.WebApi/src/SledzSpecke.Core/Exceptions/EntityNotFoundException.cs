namespace SledzSpecke.Core.Exceptions;

public class EntityNotFoundException : Exception
{
    public string EntityName { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityName, object entityId) 
        : base($"{entityName} with id '{entityId}' was not found.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public EntityNotFoundException(string message) : base(message)
    {
        EntityName = string.Empty;
        EntityId = string.Empty;
    }
}