namespace SledzSpecke.Core.ValueObjects;

/// <summary>
/// Role of a person in a publication
/// </summary>
public enum PublicationRole
{
    /// <summary>
    /// Primary author of the publication
    /// </summary>
    Author = 1,
    
    /// <summary>
    /// Co-author of the publication
    /// </summary>
    CoAuthor = 2
}