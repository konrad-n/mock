using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Events;

public record ProcedureCreatedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public int ProcedureId { get; }
    public string Code { get; }
    public string Status { get; }
    public int UserId { get; }

    public ProcedureCreatedEvent(int procedureId, string code, string status, int userId)
    {
        ProcedureId = procedureId;
        Code = code;
        Status = status;
        UserId = userId;
    }
}