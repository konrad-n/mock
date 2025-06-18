namespace SledzSpecke.Application.Abstractions;

public interface IHasEvents
{
    IEnumerable<object> GetEvents();
    void ClearEvents();
}