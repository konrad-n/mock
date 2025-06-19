#!/bin/bash

echo "Testing Direct Import of Specialization Templates"
echo "==============================================="
echo ""

# Check current state
echo "1. Current templates in database:"
sudo -u postgres psql sledzspecke_db -c "SELECT id, code, name, version, is_active FROM \"SpecializationTemplates\" ORDER BY code, version;" 2>/dev/null

echo ""
echo "2. Available JSON files:"
ls -la /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Api/Data/SpecializationTemplates/*.json

echo ""
echo "3. Testing template import via direct SQL..."

# Read cardiology_new.json and insert into database
JSON_FILE="/home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Api/Data/SpecializationTemplates/cardiology_new.json"
if [ -f "$JSON_FILE" ]; then
    # Extract code and name from JSON
    CODE=$(jq -r '.code' "$JSON_FILE")
    NAME=$(jq -r '.name' "$JSON_FILE")
    VERSION="CMKP 2023"
    
    # Escape JSON content for SQL
    JSON_CONTENT=$(cat "$JSON_FILE" | jq -c . | sed "s/'/''/g")
    
    echo "   Importing: $CODE - $NAME (version: $VERSION)"
    
    # Insert into database
    sudo -u postgres psql sledzspecke_db << EOF 2>/dev/null
INSERT INTO "SpecializationTemplates" (code, name, version, json_content, is_active, created_at)
VALUES ('$CODE', '$NAME', '$VERSION', '$JSON_CONTENT', true, NOW())
ON CONFLICT (code, version) DO NOTHING;
EOF

    if [ $? -eq 0 ]; then
        echo "   ✓ Import successful"
    else
        echo "   ✗ Import failed (may already exist)"
    fi
fi

echo ""
echo "4. Checking templates after import:"
sudo -u postgres psql sledzspecke_db -c "SELECT id, code, name, version, is_active, created_at FROM \"SpecializationTemplates\" ORDER BY code, version;" 2>/dev/null

echo ""
echo "5. Testing if specializations can access templates:"
sudo -u postgres psql sledzspecke_db -c "
SELECT s.id, s.name, s.smk_version, st.code, st.version as template_version
FROM \"Specializations\" s
LEFT JOIN \"SpecializationTemplates\" st 
  ON LOWER(s.code) = st.code 
  AND ((s.smk_version = 'old' AND st.version = 'CMKP 2014') 
       OR (s.smk_version = 'new' AND st.version = 'CMKP 2023'))
ORDER BY s.id;" 2>/dev/null

echo ""
echo "Test completed!"