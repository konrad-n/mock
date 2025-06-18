namespace SledzSpecke.Application.Services;

public interface IOutboxService
{
    Task<Guid> StoreAsync<TEvent>(TEvent @event, Dictionary<string, object>? metadata = null) 
        where TEvent : class;
}