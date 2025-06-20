-- SledzSpecke Procedure Data Migration Script
-- Migrates from old procedure structure (ProcedureBase/ProcedureOldSmk/ProcedureNewSmk) 
-- to new structure (ProcedureRequirement/ProcedureRealization)
-- 
-- WARNING: Run this script AFTER the schema migration has been applied
-- Make sure to backup your database before running this script!

BEGIN TRANSACTION;

-- Step 1: Create procedure requirements from existing procedures
-- We'll create requirements based on unique procedure codes per module

INSERT INTO "ProcedureRequirements" ("Id", "ModuleId", "Code", "Name", "RequiredAsOperator", "RequiredAsAssistant", "SmkVersion", "Year", "CreatedAt", "UpdatedAt")
SELECT DISTINCT ON (p."Code", p."ModuleId")
    nextval('"ProcedureRequirements_Id_seq"'::regclass),
    p."ModuleId",
    p."Code",
    COALESCE(p."ProcedureGroup", p."Code"), -- Use ProcedureGroup as Name if available, otherwise Code
    CASE 
        WHEN p."SmkVersion" = 'old' THEN 1  -- Old SMK typically requires 1 as operator
        ELSE COALESCE(pn."RequiredCountCodeA", 1) -- New SMK has specific requirements
    END,
    CASE 
        WHEN p."SmkVersion" = 'old' THEN 0  -- Old SMK doesn't typically have assistant requirements
        ELSE COALESCE(pn."RequiredCountCodeB", 0)
    END,
    p."SmkVersion",
    CASE 
        WHEN p."SmkVersion" = 'old' THEN po."Year"
        ELSE NULL
    END,
    NOW(),
    NOW()
FROM "Procedures" p
LEFT JOIN "ProcedureOldSmk" po ON p."Id" = po."Id"
LEFT JOIN "ProcedureNewSmk" pn ON p."Id" = pn."Id"
WHERE p."Code" IS NOT NULL AND p."ModuleId" IS NOT NULL
ORDER BY p."Code", p."ModuleId", p."Id";

-- Step 2: Create procedure realizations from existing procedure instances
-- Map each procedure instance to a realization

WITH requirement_mapping AS (
    -- Create a mapping of old procedures to new requirements
    SELECT 
        p."Id" as old_id,
        pr."Id" as requirement_id,
        p."UserId",
        p."Date",
        p."Location",
        p."ExecutionType",
        p."SmkVersion",
        po."Year" as old_year
    FROM "Procedures" p
    LEFT JOIN "ProcedureOldSmk" po ON p."Id" = po."Id"
    INNER JOIN "ProcedureRequirements" pr ON 
        pr."Code" = p."Code" AND 
        pr."ModuleId" = p."ModuleId" AND
        pr."SmkVersion" = p."SmkVersion" AND
        (pr."Year" IS NULL OR pr."Year" = po."Year")
)
INSERT INTO "ProcedureRealizations" ("Id", "RequirementId", "UserId", "Date", "Location", "Role", "Year", "CreatedAt", "UpdatedAt")
SELECT 
    nextval('"ProcedureRealizations_Id_seq"'::regclass),
    rm.requirement_id,
    rm."UserId",
    rm."Date",
    rm."Location",
    CASE 
        WHEN rm."ExecutionType" = 0 THEN 0  -- CodeA = Operator
        WHEN rm."ExecutionType" = 1 THEN 1  -- CodeB = Assistant
        ELSE 0 -- Default to Operator
    END,
    rm.old_year,
    NOW(),
    NOW()
FROM requirement_mapping rm;

-- Step 3: Create an audit table to track the migration
CREATE TABLE IF NOT EXISTS "ProcedureMigrationAudit" (
    "Id" SERIAL PRIMARY KEY,
    "OldProcedureId" INT NOT NULL,
    "NewRequirementId" INT,
    "NewRealizationId" INT,
    "MigratedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Insert audit records
INSERT INTO "ProcedureMigrationAudit" ("OldProcedureId", "NewRequirementId", "NewRealizationId")
SELECT 
    p."Id",
    pr."Id",
    preal."Id"
FROM "Procedures" p
INNER JOIN "ProcedureRequirements" pr ON 
    pr."Code" = p."Code" AND 
    pr."ModuleId" = p."ModuleId"
LEFT JOIN "ProcedureRealizations" preal ON 
    preal."RequirementId" = pr."Id" AND
    preal."UserId" = p."UserId" AND
    preal."Date" = p."Date";

-- Step 4: Verify migration
DO $$
DECLARE
    old_count INT;
    new_count INT;
    requirement_count INT;
BEGIN
    SELECT COUNT(*) INTO old_count FROM "Procedures";
    SELECT COUNT(*) INTO new_count FROM "ProcedureRealizations";
    SELECT COUNT(*) INTO requirement_count FROM "ProcedureRequirements";
    
    RAISE NOTICE 'Migration Summary:';
    RAISE NOTICE '  Old procedures: %', old_count;
    RAISE NOTICE '  New realizations: %', new_count;
    RAISE NOTICE '  Requirements created: %', requirement_count;
    
    IF old_count != new_count THEN
        RAISE WARNING 'Count mismatch! Old: %, New: %', old_count, new_count;
    END IF;
END $$;

-- Step 5: Update any foreign key references
-- Note: You may need to update other tables that reference the old Procedures table

-- Uncomment to commit the transaction when ready
-- COMMIT;

-- If something went wrong, rollback:
-- ROLLBACK;