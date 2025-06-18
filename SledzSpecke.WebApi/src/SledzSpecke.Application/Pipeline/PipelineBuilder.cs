using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SledzSpecke.Application.Pipeline;

public class PipelineBuilder<TContext> : IPipelineBuilder<TContext>
{
    private readonly List<Func<TContext, Func<Task>, Task>> _components = new();
    private readonly IServiceProvider _serviceProvider;

    public PipelineBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPipelineBuilder<TContext> Use(Func<TContext, Func<Task>, Task> middleware)
    {
        _components.Add(middleware);
        return this;
    }

    public IPipelineBuilder<TContext> UseWhen(Predicate<TContext> condition,
        Func<TContext, Func<Task>, Task> middleware)
    {
        _components.Add(async (context, next) =>
        {
            if (condition(context))
            {
                await middleware(context, next);
            }
            else
            {
                await next();
            }
        });
        return this;
    }

    public IPipelineBuilder<TContext> UseStep<TStep>() where TStep : IPipelineStep<TContext>
    {
        _components.Add(async (context, next) =>
        {
            var step = _serviceProvider.GetRequiredService<TStep>();
            await step.ExecuteAsync(context, next);
        });
        return this;
    }

    public Func<TContext, Task> Build()
    {
        return context =>
        {
            var index = -1;

            async Task Next()
            {
                index++;
                if (index < _components.Count)
                {
                    await _components[index](context, Next);
                }
            }

            return Next();
        };
    }
}