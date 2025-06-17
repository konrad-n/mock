namespace SledzSpecke.Application.Pipeline;

public interface IMessageExecutionStep
{
    Task ExecuteAsync<TMessage>(TMessage message, Func<Task> next, CancellationToken cancellationToken = default)
        where TMessage : class;
}