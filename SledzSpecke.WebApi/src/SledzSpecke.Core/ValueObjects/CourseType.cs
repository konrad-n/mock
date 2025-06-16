namespace SledzSpecke.Core.ValueObjects;

/// <summary>
/// Types of courses in the SMK system
/// </summary>
public enum CourseType
{
    /// <summary>
    /// Specialization course (Kurs specjalizacyjny)
    /// Required course as part of specialization program
    /// </summary>
    Specialization = 1,
    
    /// <summary>
    /// Improvement course (Kurs doskonalący)
    /// Professional development course to improve skills
    /// </summary>
    Improvement = 2,
    
    /// <summary>
    /// Scientific course (Kurs naukowy)
    /// Research-oriented course, often related to PhD studies
    /// </summary>
    Scientific = 3,
    
    /// <summary>
    /// Certification course (Kurs atestacyjny)
    /// Course required for obtaining medical certifications
    /// </summary>
    Certification = 4
}