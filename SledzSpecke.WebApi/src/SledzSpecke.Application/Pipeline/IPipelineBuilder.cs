using System;
using System.Threading.Tasks;

namespace SledzSpecke.Application.Pipeline;

public interface IPipelineBuilder<TContext>
{
    IPipelineBuilder<TContext> Use(Func<TContext, Func<Task>, Task> middleware);
    IPipelineBuilder<TContext> UseWhen(Predicate<TContext> condition, 
        Func<TContext, Func<Task>, Task> middleware);
    IPipelineBuilder<TContext> UseStep<TStep>() where TStep : IPipelineStep<TContext>;
    Func<TContext, Task> Build();
}

public interface IPipelineStep<TContext>
{
    Task ExecuteAsync(TContext context, Func<Task> next);
}