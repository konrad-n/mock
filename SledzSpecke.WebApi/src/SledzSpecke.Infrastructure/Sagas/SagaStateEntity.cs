using System;
using System.Collections.Generic;
using SledzSpecke.Core.Sagas;

namespace SledzSpecke.Infrastructure.Sagas;

public class SagaStateEntity
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string Data { get; set; } = "{}"; // JSON serialized
    public List<SagaStepEntity> Steps { get; set; } = new();
}

public class SagaStepEntity
{
    public Guid Id { get; set; }
    public Guid SagaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Navigation property
    public SagaStateEntity Saga { get; set; } = null!;
}