#!/bin/bash

# Test script to demonstrate CMKP import functionality

echo "Testing CMKP Specialization Import System"
echo "========================================"
echo ""

# Check if specialization templates exist in the database
echo "1. Checking existing specialization templates in database..."
sudo -u postgres psql sledzspecke_db -c "SELECT code, name, version FROM \"SpecializationTemplates\" ORDER BY code, version;" 2>/dev/null || echo "No templates found or database error"

echo ""
echo "2. Checking JSON template files..."
ls -la /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Api/Data/SpecializationTemplates/*.json 2>/dev/null | wc -l | xargs -I {} echo "Found {} JSON template files"

echo ""
echo "3. Sample specializations that would be imported:"
echo "   - alergologia - Alergologia"
echo "   - anestezjologia - Anestezjologia i intensywna terapia"
echo "   - chirurgia-ogolna - Chirurgia ogólna"
echo "   - dermatologia - Dermatologia i wenerologia"
echo "   - endokrynologia - Endokrynologia"
echo "   - kardiologia - Kardiologia (already exists)"
echo "   - psychiatria - Psychiatria (already exists)"
echo "   ... and 60+ more specializations"

echo ""
echo "4. Import process would:"
echo "   - Download PDFs from CMKP website"
echo "   - Parse PDFs to extract specialization structure"
echo "   - Generate JSON templates"
echo "   - Import templates to database"
echo "   - Create specialization records for users"

echo ""
echo "5. API endpoints for managing templates:"
echo "   GET    /api/admin/specialization-templates"
echo "   GET    /api/admin/specialization-templates/{code}/{version}"
echo "   POST   /api/admin/specialization-templates/import"
echo "   POST   /api/admin/specialization-templates/import-bulk"
echo "   PUT    /api/admin/specialization-templates/{code}/{version}"
echo "   DELETE /api/admin/specialization-templates/{code}/{version}"

echo ""
echo "Test completed."