namespace SledzSpecke.Core.ValueObjects;

/// <summary>
/// Types of self-education activities in the SMK system
/// </summary>
public enum SelfEducationType
{
    /// <summary>
    /// Literature study (Studiowanie piśmiennictwa)
    /// Reading and studying medical literature
    /// </summary>
    LiteratureStudy = 1,
    
    /// <summary>
    /// Conference (Konferencja)
    /// Attending medical conferences
    /// </summary>
    Conference = 2,
    
    /// <summary>
    /// Scientific meeting (Posiedzenie naukowe)
    /// Participating in scientific meetings or seminars
    /// </summary>
    ScientificMeeting = 3,
    
    /// <summary>
    /// Publication (Publikacja)
    /// Publishing scientific papers or articles
    /// </summary>
    Publication = 4,
    
    /// <summary>
    /// Workshop (Warsztaty)
    /// Participating in practical workshops
    /// </summary>
    Workshop = 5
}