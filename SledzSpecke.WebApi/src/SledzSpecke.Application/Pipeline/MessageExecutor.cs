using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Pipeline.Steps;

namespace SledzSpecke.Application.Pipeline;

public sealed class MessageExecutor : IMessageExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IMessageExecutionStep> _steps;

    public MessageExecutor(IServiceProvider serviceProvider, IEnumerable<IMessageExecutionStep> steps)
    {
        _serviceProvider = serviceProvider;
        _steps = steps.OrderBy(s => GetStepOrder(s.GetType()));
    }

    public async Task ExecuteCommandAsync<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        
        async Task HandlerCall() => await handler.HandleAsync(command);
        
        await ExecuteWithPipeline(command, HandlerCall, CancellationToken.None);
    }

    public async Task<TResult> ExecuteCommandAsync<TCommand, TResult>(TCommand command)
        where TCommand : class, ICommand<TResult>
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        
        TResult? result = default;
        async Task HandlerCall() => result = await handler.HandleAsync(command);
        
        await ExecuteWithPipeline(command, HandlerCall, CancellationToken.None);
        
        return result!;
    }

    public async Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult>
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        
        TResult? result = default;
        async Task HandlerCall() => result = await handler.HandleAsync(query);
        
        await ExecuteWithPipeline(query, HandlerCall, CancellationToken.None);
        
        return result!;
    }

    private async Task ExecuteWithPipeline<TMessage>(TMessage message, Func<Task> handler, CancellationToken cancellationToken)
        where TMessage : class
    {
        if (!_steps.Any())
        {
            await handler();
            return;
        }

        var stepsList = _steps.ToList();
        var currentIndex = -1;

        async Task Next()
        {
            currentIndex++;
            if (currentIndex < stepsList.Count)
            {
                await stepsList[currentIndex].ExecuteAsync(message, Next, cancellationToken);
            }
            else
            {
                await handler();
            }
        }

        await Next();
    }

    private static int GetStepOrder(Type stepType)
    {
        // Define the order of execution steps
        return stepType.Name switch
        {
            nameof(ValidationExecutionStep) => 1,
            nameof(LoggingExecutionStep) => 2,
            "TransactionExecutionStep" => 3,
            nameof(TimingExecutionStep) => 4,
            _ => 99
        };
    }
}