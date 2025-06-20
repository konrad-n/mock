-- SledzSpecke Procedure Migration Verification Script
-- Run this after migration to verify data integrity

-- 1. Count comparison
SELECT 
    'Old Procedures' as "Type",
    COUNT(*) as "Count"
FROM "Procedures"
UNION ALL
SELECT 
    'New Realizations' as "Type",
    COUNT(*) as "Count"
FROM "ProcedureRealizations"
UNION ALL
SELECT 
    'Requirements Created' as "Type",
    COUNT(*) as "Count"
FROM "ProcedureRequirements";

-- 2. Check for orphaned procedures (not migrated)
SELECT 
    p."Id",
    p."Code",
    p."Date",
    p."UserId",
    p."ModuleId",
    'Not migrated' as "Status"
FROM "Procedures" p
WHERE NOT EXISTS (
    SELECT 1 
    FROM "ProcedureRealizations" pr
    INNER JOIN "ProcedureRequirements" req ON pr."RequirementId" = req."Id"
    WHERE req."Code" = p."Code" 
    AND pr."UserId" = p."UserId"
    AND pr."Date" = p."Date"
)
LIMIT 20;

-- 3. Requirements summary by module
SELECT 
    m."Name" as "Module",
    pr."SmkVersion" as "SMK Version",
    COUNT(DISTINCT pr."Code") as "Unique Procedures",
    SUM(pr."RequiredAsOperator") as "Total Required A",
    SUM(pr."RequiredAsAssistant") as "Total Required B"
FROM "ProcedureRequirements" pr
INNER JOIN "Modules" m ON pr."ModuleId" = m."Id"
GROUP BY m."Name", pr."SmkVersion"
ORDER BY m."Name";

-- 4. User realization summary
SELECT 
    u."Email",
    COUNT(DISTINCT pr."RequirementId") as "Unique Procedures",
    COUNT(*) as "Total Realizations",
    COUNT(CASE WHEN pr."Role" = 0 THEN 1 END) as "As Operator",
    COUNT(CASE WHEN pr."Role" = 1 THEN 1 END) as "As Assistant"
FROM "ProcedureRealizations" pr
INNER JOIN "Users" u ON pr."UserId" = u."Id"
GROUP BY u."Email"
ORDER BY COUNT(*) DESC
LIMIT 10;

-- 5. Data integrity checks
-- Check for realizations without valid requirements
SELECT COUNT(*) as "Orphaned Realizations"
FROM "ProcedureRealizations" pr
WHERE NOT EXISTS (
    SELECT 1 FROM "ProcedureRequirements" req 
    WHERE req."Id" = pr."RequirementId"
);

-- Check for requirements without module
SELECT COUNT(*) as "Requirements without Module"
FROM "ProcedureRequirements"
WHERE "ModuleId" NOT IN (SELECT "Id" FROM "Modules");

-- Check for realizations without valid user
SELECT COUNT(*) as "Realizations without User"
FROM "ProcedureRealizations"
WHERE "UserId" NOT IN (SELECT "Id" FROM "Users");

-- 6. Migration audit summary (if audit table exists)
SELECT 
    COUNT(*) as "Total Mappings",
    COUNT(DISTINCT "OldProcedureId") as "Unique Old Procedures",
    COUNT(DISTINCT "NewRequirementId") as "Unique Requirements",
    COUNT(DISTINCT "NewRealizationId") as "Unique Realizations"
FROM "ProcedureMigrationAudit"
WHERE EXISTS (
    SELECT 1 FROM information_schema.tables 
    WHERE table_name = 'ProcedureMigrationAudit'
);

-- 7. Sample migrated data
SELECT 
    'Sample Migration' as "Type",
    p."Code" as "Old Code",
    p."Date",
    req."Name" as "Requirement Name",
    pr."Role",
    u."Email" as "User"
FROM "Procedures" p
INNER JOIN "ProcedureRequirements" req ON req."Code" = p."Code" AND req."ModuleId" = p."ModuleId"
INNER JOIN "ProcedureRealizations" pr ON pr."RequirementId" = req."Id" AND pr."UserId" = p."UserId" AND pr."Date" = p."Date"
INNER JOIN "Users" u ON u."Id" = p."UserId"
LIMIT 10;