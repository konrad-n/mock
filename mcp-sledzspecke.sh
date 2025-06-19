#!/bin/bash

# SledzSpecke MCP Helper Script
# Quick commands for common MCP operations

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Base command
MCP_CMD="claude --mcp-config /home/ubuntu/projects/mock/mcp-config.json -p"

# Function to print usage
usage() {
    echo -e "${BLUE}SledzSpecke MCP Helper${NC}"
    echo "======================="
    echo ""
    echo "Usage: $0 <command> [args]"
    echo ""
    echo "Commands:"
    echo "  health          - Check API health and status"
    echo "  errors          - Show recent errors from logs"
    echo "  users           - Show recent user registrations"
    echo "  shifts          - Analyze medical shifts data"
    echo "  builds          - Check GitHub Actions build status"
    echo "  debug <issue>   - Debug a specific issue"
    echo "  query <sql>     - Run a custom SQL query"
    echo "  remember <info> - Store information in memory"
    echo "  recall <topic>  - Recall stored information"
    echo "  full-check      - Run complete system health check"
    echo ""
    exit 1
}

# Check if claude is available
if ! command -v claude &> /dev/null; then
    echo -e "${RED}Error: Claude Code CLI not installed${NC}"
    echo "Install with: npm install -g @anthropic-ai/claude-code"
    exit 1
fi

# Check arguments
if [ $# -eq 0 ]; then
    usage
fi

# Execute command based on argument
case "$1" in
    health)
        echo -e "${BLUE}Checking SledzSpecke API health...${NC}"
        $MCP_CMD "Check SledzSpecke API health: is it running, any recent errors, response times" \
            --allowedTools "mcp__filesystem__read_file,mcp__filesystem__list_directory"
        ;;
    
    errors)
        echo -e "${BLUE}Analyzing recent errors...${NC}"
        $MCP_CMD "Find and analyze all errors from SledzSpecke logs in the last 24 hours" \
            --allowedTools "mcp__filesystem__read_file,mcp__filesystem__search_files"
        ;;
    
    users)
        echo -e "${BLUE}Checking recent user registrations...${NC}"
        $MCP_CMD "Show the 10 most recent user registrations with their details" \
            --allowedTools "mcp__postgres__query"
        ;;
    
    shifts)
        echo -e "${BLUE}Analyzing medical shifts...${NC}"
        $MCP_CMD "Analyze medical shifts: total count, average hours, validation errors" \
            --allowedTools "mcp__postgres__query"
        ;;
    
    builds)
        echo -e "${BLUE}Checking GitHub Actions builds...${NC}"
        $MCP_CMD "Show the status of the last 5 GitHub Actions builds for SledzSpecke" \
            --allowedTools "mcp__github__list_workflows,mcp__github__get_workflow_runs"
        ;;
    
    debug)
        if [ -z "$2" ]; then
            echo -e "${RED}Error: Please specify what to debug${NC}"
            echo "Example: $0 debug 'user login issues'"
            exit 1
        fi
        shift
        issue="$*"
        echo -e "${BLUE}Debugging: $issue${NC}"
        $MCP_CMD "Debug this issue: $issue. Check logs, database, and any stored context" \
            --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__memory__recall" \
            --max-turns 5
        ;;
    
    query)
        if [ -z "$2" ]; then
            echo -e "${RED}Error: Please specify SQL query${NC}"
            echo "Example: $0 query 'SELECT COUNT(*) FROM \"Users\"'"
            exit 1
        fi
        shift
        query="$*"
        echo -e "${BLUE}Running SQL query...${NC}"
        $MCP_CMD "Run this SQL query on SledzSpecke database and explain the results: $query" \
            --allowedTools "mcp__postgres__query"
        ;;
    
    remember)
        if [ -z "$2" ]; then
            echo -e "${RED}Error: Please specify what to remember${NC}"
            echo "Example: $0 remember 'Production API runs on port 5000'"
            exit 1
        fi
        shift
        info="$*"
        echo -e "${BLUE}Storing information...${NC}"
        $MCP_CMD "Remember this about SledzSpecke: $info" \
            --allowedTools "mcp__memory__store"
        ;;
    
    recall)
        if [ -z "$2" ]; then
            echo -e "${RED}Error: Please specify topic to recall${NC}"
            echo "Example: $0 recall 'API configuration'"
            exit 1
        fi
        shift
        topic="$*"
        echo -e "${BLUE}Recalling information about: $topic${NC}"
        $MCP_CMD "What do you remember about: $topic" \
            --allowedTools "mcp__memory__recall,mcp__memory__search"
        ;;
    
    full-check)
        echo -e "${BLUE}Running complete system health check...${NC}"
        $MCP_CMD "Perform complete SledzSpecke health check: API status, recent errors, database health, user activity, GitHub builds" \
            --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__github__get_workflow_runs,mcp__memory__recall" \
            --max-turns 5
        ;;
    
    *)
        echo -e "${RED}Unknown command: $1${NC}"
        usage
        ;;
esac