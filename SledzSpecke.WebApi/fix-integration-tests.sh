#!/bin/bash
echo "🔧 SledzSpecke Integration Test Fix Script"
echo "=========================================="

echo "📊 Step 1: Analyzing compilation errors..."
mkdir -p test-fix-reports
dotnet build tests/SledzSpecke.Tests.Integration/SledzSpecke.Tests.Integration.csproj 2>&1 | grep -E "error CS" > test-fix-reports/errors.txt

echo "Found $(wc -l < test-fix-reports/errors.txt) compilation errors"
echo ""

echo "📈 Step 2: Categorizing errors by type..."
echo "Error types:"
cat test-fix-reports/errors.txt | sed -E 's/.*error (CS[0-9]+):.*/\1/' | sort | uniq -c | sort -nr | head -10
echo ""

echo "📁 Step 3: Files with most errors:"
cat test-fix-reports/errors.txt | cut -d'(' -f1 | sed 's/.*\///' | sort | uniq -c | sort -nr | head -10
echo ""

echo "🔄 Step 4: Applying automated fixes..."

echo "  - Replacing builder patterns..."
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/new UserBuilder()/TestDataFactory.CreateTestUser(/g' {} \;
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/new InternshipBuilder()/TestDataFactory.CreateTestInternship(/g' {} \;
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/new MedicalShiftBuilder()/TestDataFactory.CreateTestMedicalShift(/g' {} \;
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/new ProcedureBuilder()/TestDataFactory.CreateTestProcedure(/g' {} \;

echo "  - Fixing value object access..."
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/\.Email\.Should()/\.Email.Value.Should()/g' {} \;
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/new Duration(/new ShiftDuration(/g' {} \;
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/SpecializationType\.Medical/"Medical"/g' {} \;

echo "  - Fixing command constructors..."
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/new SignUp\s*{/new SignUp(/g' {} \;
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/new SignIn\s*{/new SignIn(/g' {} \;
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/new AddMedicalShift\s*{/new AddMedicalShift(/g' {} \;

echo "  - Fixing DTO references..."
find tests/SledzSpecke.Tests.Integration -name "*.cs" -type f -exec sed -i 's/AccessTokenDto/JwtDto/g' {} \;

echo ""
echo "✅ Automated fixes applied"
echo ""

echo "🔨 Building again to check progress..."
ERRORS_AFTER=$(dotnet build tests/SledzSpecke.Tests.Integration/SledzSpecke.Tests.Integration.csproj 2>&1 | grep -c "error CS")
echo "Remaining errors: $ERRORS_AFTER"

echo ""
echo "🎯 Next Steps:"
echo "1. Review test-fix-reports/errors.txt for specific errors"
echo "2. Fix remaining errors manually"
echo "3. Run individual test files as you fix them"
echo ""