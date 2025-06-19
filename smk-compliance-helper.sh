#!/bin/bash

# SledzSpecke SMK Compliance Helper
# Tools for ensuring 1:1 compliance with official SMK systems

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Base command for SMK MCP
SMK_MCP="claude --mcp-config /home/ubuntu/projects/mock/mcp-smk-config.json -p"

usage() {
    echo -e "${BLUE}SledzSpecke SMK Compliance Helper${NC}"
    echo "=================================="
    echo ""
    echo "Usage: $0 <command> [args]"
    echo ""
    echo "PDF Analysis Commands:"
    echo "  analyze-old         - Analyze Old SMK PDF for field mappings"
    echo "  analyze-new         - Analyze New SMK PDF for field mappings"
    echo "  compare-versions    - Compare Old vs New SMK requirements"
    echo "  extract-fields      - Extract all field mappings from PDFs"
    echo ""
    echo "Excel Export Commands:"
    echo "  implement-export    - Implement Excel export based on PDFs"
    echo "  validate-format     - Validate Excel format compliance"
    echo "  generate-test       - Generate test Excel files"
    echo "  test-chrome-ext     - Test Chrome extension compatibility"
    echo ""
    echo "Validation Commands:"
    echo "  check-fields        - Check for missing SMK fields"
    echo "  validate-schema     - Validate DB schema against SMK"
    echo "  compliance-report   - Generate full compliance report"
    echo ""
    echo "Storage Commands:"
    echo "  store-mappings      - Store field mappings in memory"
    echo "  recall-mappings     - Recall stored field mappings"
    echo "  list-requirements   - List all SMK requirements"
    echo ""
    exit 1
}

# Check if claude is available
if ! command -v claude &> /dev/null; then
    echo -e "${RED}Error: Claude Code CLI not installed${NC}"
    echo "Install with: npm install -g @anthropic-ai/claude-code"
    exit 1
fi

# Check for PDFs
check_pdfs() {
    local pdf_dir="/home/ubuntu/projects/mock/docs/smk-pdfs"
    if [ ! -f "$pdf_dir/old-smk-official.pdf" ] || [ ! -f "$pdf_dir/new-smk-official.pdf" ]; then
        echo -e "${YELLOW}Warning: SMK PDFs not found in $pdf_dir${NC}"
        echo "Please place the official SMK PDFs:"
        echo "  - old-smk-official.pdf (Old SMK system)"
        echo "  - new-smk-official.pdf (New SMK system)"
    fi
}

# Execute command
case "$1" in
    analyze-old)
        echo -e "${BLUE}Analyzing Old SMK PDF...${NC}"
        check_pdfs
        $SMK_MCP "Analyze the Old SMK PDF and extract: 1) All Excel sheet names, 2) Column headers for each sheet, 3) Data formats, 4) Required vs optional fields" \
            --allowedTools "mcp__smk-docs__read_file,mcp__memory__store" \
            --max-turns 5
        ;;
    
    analyze-new)
        echo -e "${BLUE}Analyzing New SMK PDF...${NC}"
        check_pdfs
        $SMK_MCP "Analyze the New SMK PDF and extract: 1) All Excel sheet names, 2) Column headers for each sheet, 3) Data formats, 4) Required vs optional fields" \
            --allowedTools "mcp__smk-docs__read_file,mcp__memory__store" \
            --max-turns 5
        ;;
    
    compare-versions)
        echo -e "${BLUE}Comparing Old vs New SMK versions...${NC}"
        check_pdfs
        $SMK_MCP "Compare Old and New SMK PDFs: What fields changed? What new requirements exist? What was removed?" \
            --allowedTools "mcp__smk-docs__read_file,mcp__memory__store" \
            --max-turns 5
        ;;
    
    extract-fields)
        echo -e "${BLUE}Extracting all field mappings...${NC}"
        check_pdfs
        $SMK_MCP "Extract and store complete field mappings from both SMK PDFs. Create a comprehensive mapping: SledzSpecke field -> Excel column -> Format" \
            --allowedTools "mcp__smk-docs__read_file,mcp__memory__store" \
            --max-turns 10
        ;;
    
    implement-export)
        echo -e "${BLUE}Implementing Excel export...${NC}"
        version="${2:-new}"
        $SMK_MCP "Implement Excel export service for $version SMK format based on stored PDF specifications. Use ClosedXML library." \
            --allowedTools "mcp__smk-docs__read_file,mcp__smk-docs__write_file,mcp__memory__recall,mcp__postgres__schema" \
            --max-turns 10
        ;;
    
    validate-format)
        echo -e "${BLUE}Validating Excel format compliance...${NC}"
        $SMK_MCP "Validate our Excel export implementation matches SMK requirements exactly. Check: sheet names, column headers, data formats" \
            --allowedTools "mcp__smk-docs__read_file,mcp__postgres__query,mcp__memory__recall"
        ;;
    
    generate-test)
        echo -e "${BLUE}Generating test Excel files...${NC}"
        version="${2:-new}"
        $SMK_MCP "Generate test Excel file with sample data for $version SMK format. Include all required sheets and realistic Polish medical data." \
            --allowedTools "mcp__smk-docs__write_file,mcp__postgres__query,mcp__memory__recall"
        ;;
    
    test-chrome-ext)
        echo -e "${BLUE}Testing Chrome extension compatibility...${NC}"
        $SMK_MCP "Generate Excel test files specifically for Chrome extension import testing. Include edge cases and Polish characters." \
            --allowedTools "mcp__smk-docs__write_file,mcp__memory__recall"
        ;;
    
    check-fields)
        echo -e "${BLUE}Checking for missing SMK fields...${NC}"
        $SMK_MCP "Compare our database schema with SMK requirements. List any missing fields we need to add." \
            --allowedTools "mcp__smk-docs__read_file,mcp__postgres__schema,mcp__memory__recall"
        ;;
    
    validate-schema)
        echo -e "${BLUE}Validating database schema...${NC}"
        $SMK_MCP "Validate our complete database schema has all fields required for SMK export. Check data types and constraints." \
            --allowedTools "mcp__postgres__schema,mcp__memory__recall"
        ;;
    
    compliance-report)
        echo -e "${BLUE}Generating compliance report...${NC}"
        $SMK_MCP "Generate comprehensive SMK compliance report: 1) Field coverage, 2) Format compliance, 3) Missing features, 4) Recommendations" \
            --allowedTools "mcp__smk-docs__read_file,mcp__postgres__schema,mcp__memory__recall" \
            --max-turns 10
        ;;
    
    store-mappings)
        echo -e "${BLUE}Storing field mappings...${NC}"
        if [ -z "$2" ]; then
            echo -e "${RED}Error: Please provide mapping data${NC}"
            echo "Example: $0 store-mappings 'Medical shifts: Date->A column (DD.MM.YYYY)'"
            exit 1
        fi
        shift
        mappings="$*"
        $SMK_MCP "Store these SMK field mappings: $mappings" \
            --allowedTools "mcp__memory__store"
        ;;
    
    recall-mappings)
        echo -e "${BLUE}Recalling field mappings...${NC}"
        category="${2:-all}"
        $SMK_MCP "Recall SMK field mappings for: $category" \
            --allowedTools "mcp__memory__recall,mcp__memory__search"
        ;;
    
    list-requirements)
        echo -e "${BLUE}Listing all SMK requirements...${NC}"
        $SMK_MCP "List all stored SMK requirements and field mappings organized by category" \
            --allowedTools "mcp__memory__list,mcp__memory__recall"
        ;;
    
    *)
        if [ -n "$1" ]; then
            echo -e "${RED}Unknown command: $1${NC}"
        fi
        usage
        ;;
esac