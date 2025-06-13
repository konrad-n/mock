using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Infrastructure.Time;

internal sealed class Clock : IClock
{
    public DateTime Current() => DateTime.UtcNow;
}