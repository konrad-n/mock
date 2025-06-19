#!/bin/bash

# CMKP Specialization Management Helper
# Tools for managing medical specialization requirements from CMKP

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Base paths
TEMPLATES_DIR="/home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Api/Data/SpecializationTemplates"
PDFS_DIR="/home/ubuntu/projects/mock/docs/cmkp-pdfs"
MCP_CMD="claude --mcp-config /home/ubuntu/projects/mock/mcp-cmkp-config.json -p"

usage() {
    echo -e "${BLUE}CMKP Specialization Management Helper${NC}"
    echo "====================================="
    echo ""
    echo "Usage: $0 <command> [args]"
    echo ""
    echo "Web Scraping Commands:"
    echo "  list-specializations <old|new>    - List all specializations from CMKP website"
    echo "  download-pdf <old|new> <name>      - Download specific specialization PDF"
    echo "  download-all <old|new>             - Download all specialization PDFs"
    echo ""
    echo "PDF Processing Commands:"
    echo "  parse-pdf <pdf-file>               - Parse PDF to JSON format"
    echo "  parse-all <old|new>                - Parse all PDFs in directory"
    echo "  validate-json <json-file>          - Validate JSON structure"
    echo ""
    echo "Database Commands:"
    echo "  import-json <json-file>            - Import JSON to database"
    echo "  import-all                         - Import all JSONs to database"
    echo "  list-imported                      - List imported specializations"
    echo ""
    echo "Analysis Commands:"
    echo "  compare-versions <specialization>  - Compare old vs new SMK"
    echo "  missing-specializations            - Find missing specializations"
    echo "  generate-template <pdf-file>       - Generate JSON template from PDF"
    echo ""
    echo "Examples:"
    echo "  $0 download-pdf new alergologia"
    echo "  $0 parse-pdf /docs/cmkp-pdfs/new-smk/alergologia.pdf"
    echo "  $0 import-all"
    echo ""
    exit 1
}

# Function to list specializations from CMKP website
list_specializations() {
    local version=$1
    local url=""
    
    if [ "$version" = "old" ]; then
        url="https://www.cmkp.edu.pl/ksztalcenie/podyplomowe/lekarze-i-lekarze-dentysci/modulowe-programy-specjalizacji-od-1-10-2014-aktualizacja-2018"
    elif [ "$version" = "new" ]; then
        url="https://www.cmkp.edu.pl/ksztalcenie/podyplomowe/lekarze-i-lekarze-dentysci/modulowe-programy-specjalizacji-lekarskich-2023"
    else
        echo -e "${RED}Error: Invalid version. Use 'old' or 'new'${NC}"
        exit 1
    fi
    
    echo -e "${BLUE}Fetching specializations from CMKP ($version SMK)...${NC}"
    $MCP_CMD "Fetch the CMKP website at $url and extract all specialization names and their PDF links (first link for each specialization)" \
        --allowedTools "mcp__web-scraper__fetch,mcp__memory__store"
}

# Function to download specific PDF
download_pdf() {
    local version=$1
    local name=$2
    local output_dir="$PDFS_DIR/${version}-smk"
    
    mkdir -p "$output_dir"
    
    echo -e "${BLUE}Downloading $name specialization PDF ($version SMK)...${NC}"
    $MCP_CMD "Download the PDF for $name specialization from CMKP $version SMK website and save to $output_dir/${name}.pdf" \
        --allowedTools "mcp__web-scraper__fetch,mcp__cmkp-docs__write_file,mcp__memory__recall"
}

# Function to parse PDF to JSON
parse_pdf() {
    local pdf_file=$1
    local output_file="${pdf_file%.pdf}.json"
    
    echo -e "${BLUE}Parsing PDF to JSON format...${NC}"
    $MCP_CMD "Parse the specialization PDF at $pdf_file and convert to JSON format matching the cardiology template structure. Extract: name, code, duration, modules, courses, internships, procedures" \
        --allowedTools "mcp__cmkp-docs__read_file,mcp__cmkp-docs__write_file,mcp__memory__recall" \
        --max-turns 10
}

# Function to validate JSON structure
validate_json() {
    local json_file=$1
    
    echo -e "${BLUE}Validating JSON structure...${NC}"
    $MCP_CMD "Validate the specialization JSON at $json_file matches our template structure. Check for: required fields, data types, module structure, course/internship/procedure arrays" \
        --allowedTools "mcp__cmkp-docs__read_file"
}

# Function to import JSON to database
import_json() {
    local json_file=$1
    
    echo -e "${BLUE}Importing JSON to database...${NC}"
    $MCP_CMD "Import the specialization from $json_file to the database. Create specialization template records with all modules, courses, internships, and procedures" \
        --allowedTools "mcp__cmkp-docs__read_file,mcp__postgres__query" \
        --max-turns 5
}

# Function to compare versions
compare_versions() {
    local spec_name=$1
    
    echo -e "${BLUE}Comparing old vs new SMK for $spec_name...${NC}"
    $MCP_CMD "Compare the old and new SMK versions of $spec_name specialization. Show differences in: duration, modules, courses, internships, procedures" \
        --allowedTools "mcp__cmkp-docs__read_file,mcp__memory__store"
}

# Function to find missing specializations
missing_specializations() {
    echo -e "${BLUE}Finding missing specializations...${NC}"
    $MCP_CMD "List all specializations from CMKP websites that are not yet in our SpecializationTemplates directory" \
        --allowedTools "mcp__cmkp-docs__list_directory,mcp__memory__recall,mcp__web-scraper__fetch" \
        --max-turns 5
}

# Function to generate template from PDF
generate_template() {
    local pdf_file=$1
    local template_name=$(basename "$pdf_file" .pdf)
    
    echo -e "${BLUE}Generating JSON template from PDF...${NC}"
    $MCP_CMD "Generate a complete specialization JSON template from $pdf_file. Use the exact structure from cardiology_old.json as reference. Extract all data comprehensively." \
        --allowedTools "mcp__cmkp-docs__read_file,mcp__cmkp-docs__write_file" \
        --max-turns 15
}

# Parse all PDFs in directory
parse_all() {
    local version=$1
    local pdf_dir="$PDFS_DIR/${version}-smk"
    
    if [ ! -d "$pdf_dir" ]; then
        echo -e "${RED}Error: Directory $pdf_dir not found${NC}"
        exit 1
    fi
    
    echo -e "${BLUE}Parsing all PDFs in $pdf_dir...${NC}"
    for pdf in "$pdf_dir"/*.pdf; do
        if [ -f "$pdf" ]; then
            echo -e "${GREEN}Processing: $(basename "$pdf")${NC}"
            parse_pdf "$pdf"
        fi
    done
}

# Import all JSONs
import_all() {
    echo -e "${BLUE}Importing all specialization JSONs...${NC}"
    for json in "$TEMPLATES_DIR"/*.json; do
        if [ -f "$json" ]; then
            echo -e "${GREEN}Importing: $(basename "$json")${NC}"
            import_json "$json"
        fi
    done
}

# List imported specializations
list_imported() {
    echo -e "${BLUE}Listing imported specializations...${NC}"
    $MCP_CMD "Query database for all imported specialization templates. Show: name, SMK version, module count, total duration" \
        --allowedTools "mcp__postgres__query"
}

# Download all specializations
download_all() {
    local version=$1
    
    echo -e "${BLUE}Downloading all specializations for $version SMK...${NC}"
    $MCP_CMD "Download all specialization PDFs from CMKP $version SMK website to $PDFS_DIR/${version}-smk/ directory" \
        --allowedTools "mcp__web-scraper__fetch,mcp__cmkp-docs__write_file,mcp__memory__recall" \
        --max-turns 50
}

# Execute command
case "$1" in
    list-specializations)
        list_specializations "$2"
        ;;
    download-pdf)
        download_pdf "$2" "$3"
        ;;
    download-all)
        download_all "$2"
        ;;
    parse-pdf)
        parse_pdf "$2"
        ;;
    parse-all)
        parse_all "$2"
        ;;
    validate-json)
        validate_json "$2"
        ;;
    import-json)
        import_json "$2"
        ;;
    import-all)
        import_all
        ;;
    list-imported)
        list_imported
        ;;
    compare-versions)
        compare_versions "$2"
        ;;
    missing-specializations)
        missing_specializations
        ;;
    generate-template)
        generate_template "$2"
        ;;
    *)
        if [ -n "$1" ]; then
            echo -e "${RED}Unknown command: $1${NC}"
        fi
        usage
        ;;
esac