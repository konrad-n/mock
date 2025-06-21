#\!/bin/bash

# 1. Fix CourseRequirementService - convert enum to value object
sed -i 's/specialization\.SmkVersion,/new ValueObjects.SmkVersion(specialization.SmkVersion.ToString()),/g' \
    src/SledzSpecke.Core/DomainServices/CourseRequirementService.cs

# 2. Fix the class definition InternshipCompletionRequirements to use enum
# Find the class and check what type SmkVersion property should be
grep -A5 "class InternshipCompletionRequirements" src/SledzSpecke.Core/DomainServices/InternshipCompletionService.cs

# 3. Fix the procedure creation calls in Internship.cs
# Check what the Create methods expect
echo "Checking ProcedureOldSmk.Create signature..."
grep -B2 -A3 "static.*Create" src/SledzSpecke.Core/Entities/ProcedureOldSmk.cs  < /dev/null |  head -10

echo "Script complete - manual checks needed for some files"
