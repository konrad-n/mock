#\!/bin/bash

# 1. In Internship.cs, convert ValueObjects.ProcedureExecutionType to Enums.ProcedureExecutionType
# when calling Create methods
sed -i 's/var procedure = ProcedureOldSmk\.Create(procedureId, moduleId, InternshipId, date, year, code, name, location, executionType, supervisorName);/var procedure = ProcedureOldSmk.Create(procedureId, moduleId, InternshipId, date, year, code, name, location, (Enums.ProcedureExecutionType)Enum.Parse(typeof(Enums.ProcedureExecutionType), executionType.ToString()), supervisorName);/g' \
    src/SledzSpecke.Core/Entities/Internship.cs

sed -i 's/var procedure = ProcedureNewSmk\.Create(procedureId, moduleId, InternshipId, date, code, procedureName, location, executionType, supervisorName, procedureRequirementId);/var procedure = ProcedureNewSmk.Create(procedureId, moduleId, InternshipId, date, code, procedureName, location, (Enums.ProcedureExecutionType)Enum.Parse(typeof(Enums.ProcedureExecutionType), executionType.ToString()), supervisorName, procedureRequirementId);/g' \
    src/SledzSpecke.Core/Entities/Internship.cs

# 2. Fix ProcedureSpecifications - they're likely using value objects
# Check what types are actually being used
echo "Checking procedure entity types..."
grep -A5 "public.*ProcedureStatus" src/SledzSpecke.Core/Entities/ProcedureBase.cs  < /dev/null |  head -10

# 3. For InternshipEnhanced line 266 - check what's being compared
echo "Checking InternshipEnhanced line 266..."
sed -n '260,270p' src/SledzSpecke.Core/Entities/InternshipEnhanced.cs

echo "Fixes partially applied - manual intervention needed for some issues"
