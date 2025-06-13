using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record CreatePublication(
    SpecializationId SpecializationId,
    UserId UserId,
    PublicationType Type,
    string Title,
    DateTime PublicationDate,
    string? Authors = null,
    string? Journal = null,
    string? Publisher = null) : ICommand;