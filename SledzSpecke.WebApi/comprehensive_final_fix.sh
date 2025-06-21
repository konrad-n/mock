#\!/bin/bash

# 1. Fix ModuleType enum value
sed -i 's/ModuleType\.Specialized/ModuleType.Specialistic/g' \
    src/SledzSpecke.Core/Specifications/ModuleSpecifications.cs

# 2. Fix all remaining module.Id references
sed -i 's/module\.Id\b/module.ModuleId/g' \
    src/SledzSpecke.Core/DomainServices/ProcedureValidationService.cs

# 3. Fix Duration references in ModuleProgressionService
sed -i 's/shift\.Duration\.TotalMinutes/shift.TotalMinutes/g' \
    src/SledzSpecke.Core/DomainServices/ModuleProgressionService.cs

# 4. Fix SmkVersion comparison in ModuleProgressionService
sed -i 's/specialization\.SmkVersion == SmkVersion\.Old/specialization.SmkVersion == Enums.SmkVersion.Old/g' \
    src/SledzSpecke.Core/DomainServices/ModuleProgressionService.cs

# 5. Look for exact lines in CourseRequirementService that need fixing
# First, let's see what's at lines 301 and 355
echo "Checking CourseRequirementService lines 301 and 355..."
sed -n '301p' src/SledzSpecke.Core/DomainServices/CourseRequirementService.cs
sed -n '355p' src/SledzSpecke.Core/DomainServices/CourseRequirementService.cs

# 6. Check InternshipCompletionService line 219
echo "Checking InternshipCompletionService line 219..."
sed -n '219p' src/SledzSpecke.Core/DomainServices/InternshipCompletionService.cs

# 7. Check the exact ProcedureExecutionType issues in Internship.cs
echo "Checking Internship.cs lines 357 and 395..."
sed -n '357p' src/SledzSpecke.Core/Entities/Internship.cs
sed -n '395p' src/SledzSpecke.Core/Entities/Internship.cs

echo "Initial checks complete\!"
