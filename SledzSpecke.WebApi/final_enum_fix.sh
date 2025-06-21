#\!/bin/bash

# Check which ProcedureStatus ProcedureBase is using
echo "Checking ProcedureBase imports..."
grep -E "using.*Enums < /dev/null | using.*ValueObjects" src/SledzSpecke.Core/Entities/ProcedureBase.cs

# ProcedureBase uses Enums, so specifications should also use Enums.ProcedureStatus
# Fix ProcedureSpecifications to use the correct enum
sed -i 's/using SledzSpecke\.Core\.ValueObjects;/using SledzSpecke.Core.ValueObjects;\nusing SledzSpecke.Core.Enums;/g' \
    src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs

# Then qualify the enum types to avoid ambiguity
sed -i 's/ProcedureStatus _status;/Enums.ProcedureStatus _status;/g' \
    src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs

sed -i 's/ProcedureStatus status)/Enums.ProcedureStatus status)/g' \
    src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs

sed -i 's/ProcedureStatus\.Completed/Enums.ProcedureStatus.Completed/g' \
    src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs

sed -i 's/ProcedureStatus\.Approved/Enums.ProcedureStatus.Approved/g' \
    src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs

# Fix ProcedureExecutionType too
sed -i 's/ProcedureExecutionType _executionType;/Enums.ProcedureExecutionType _executionType;/g' \
    src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs

sed -i 's/ProcedureExecutionType executionType)/Enums.ProcedureExecutionType executionType)/g' \
    src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs

# Fix SyncStatus
sed -i 's/SyncStatus\.NotSynced/Enums.SyncStatus.NotSynced/g' \
    src/SledzSpecke.Core/Specifications/ProcedureSpecifications.cs

echo "Enum namespace fixes applied\!"
