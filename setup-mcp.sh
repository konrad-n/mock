#!/bin/bash

# SledzSpecke MCP Setup Script
# This script helps set up and test MCP (Model Context Protocol) integration

set -e

echo "🔌 SledzSpecke MCP Setup"
echo "========================"

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Function to print colored messages
print_success() { echo -e "${GREEN}✓ $1${NC}"; }
print_warning() { echo -e "${YELLOW}⚠ $1${NC}"; }
print_error() { echo -e "${RED}✗ $1${NC}"; }

# Check if claude is installed
if ! command -v claude &> /dev/null; then
    print_error "Claude Code CLI not found. Please install with: npm install -g @anthropic-ai/claude-code"
    exit 1
fi

# Check if MCP config exists
if [ ! -f "mcp-config.json" ]; then
    print_error "mcp-config.json not found in current directory"
    exit 1
fi

print_success "Found mcp-config.json"

# Install MCP servers
echo ""
echo "📦 Installing MCP servers..."
echo ""

# Install filesystem server
echo "Installing filesystem server..."
if npx -y @modelcontextprotocol/server-filesystem --help &> /dev/null; then
    print_success "Filesystem server ready"
else
    print_error "Failed to install filesystem server"
fi

# Install PostgreSQL server
echo "Installing PostgreSQL server..."
if npx -y @modelcontextprotocol/server-postgres --help &> /dev/null; then
    print_success "PostgreSQL server ready"
else
    print_error "Failed to install PostgreSQL server"
fi

# Install GitHub server
echo "Installing GitHub server..."
if npx -y @modelcontextprotocol/server-github --help &> /dev/null; then
    print_success "GitHub server ready"
else
    print_error "Failed to install GitHub server"
fi

# Install memory server
echo "Installing memory server..."
if npx -y @modelcontextprotocol/server-memory --help &> /dev/null; then
    print_success "Memory server ready"
else
    print_error "Failed to install memory server"
fi

# Check environment variables
echo ""
echo "🔐 Checking environment..."
echo ""

if [ -z "$GITHUB_TOKEN" ]; then
    print_warning "GITHUB_TOKEN not set. GitHub MCP server will have limited functionality."
    echo "   Set it with: export GITHUB_TOKEN='your-token-here'"
else
    print_success "GITHUB_TOKEN is set"
fi

if [ -z "$ANTHROPIC_API_KEY" ]; then
    print_error "ANTHROPIC_API_KEY not set. Claude Code will not work."
    echo "   Set it with: export ANTHROPIC_API_KEY='your-key-here'"
    exit 1
else
    print_success "ANTHROPIC_API_KEY is set"
fi

# Test MCP connection
echo ""
echo "🧪 Testing MCP connection..."
echo ""

echo "Testing filesystem access..."
if claude --mcp-config mcp-config.json -p "List files in /home/ubuntu/projects/mock" \
    --allowedTools "mcp__filesystem__list_directory" \
    --output-format json 2>/dev/null | grep -q "result"; then
    print_success "Filesystem MCP server working"
else
    print_error "Filesystem MCP server test failed"
fi

# Create test queries
echo ""
echo "📝 Example MCP queries for SledzSpecke:"
echo ""

cat << 'EOF'
# 1. Check API health
claude --mcp-config mcp-config.json -p "Check if SledzSpecke API is running and healthy" \
  --allowedTools "mcp__filesystem__read_file"

# 2. Query recent users
claude --mcp-config mcp-config.json -p "Show the 5 most recent user registrations" \
  --allowedTools "mcp__postgres__query"

# 3. Check GitHub builds
claude --mcp-config mcp-config.json -p "Show status of recent GitHub Actions builds" \
  --allowedTools "mcp__github__list_workflows,mcp__github__get_workflow_runs"

# 4. Store important info
claude --mcp-config mcp-config.json -p "Remember: Production database is sledzspecke_db on localhost" \
  --allowedTools "mcp__memory__store"

# 5. Debug a specific issue
claude --mcp-config mcp-config.json -p "Debug why medical shifts are not saving correctly" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__memory__recall"
EOF

echo ""
print_success "MCP setup complete! You can now use Claude Code with MCP integration."
echo ""
echo "Try running one of the example queries above to get started."