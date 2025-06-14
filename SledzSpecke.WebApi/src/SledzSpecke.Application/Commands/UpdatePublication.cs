using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record UpdatePublication(
    PublicationId PublicationId,
    string Title,
    string? Authors,
    string? Journal,
    string? Publisher,
    string? Abstract,
    string? Keywords,
    bool IsFirstAuthor,
    bool IsCorrespondingAuthor,
    bool IsPeerReviewed) : ICommand;