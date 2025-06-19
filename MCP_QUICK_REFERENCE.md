# SledzSpecke MCP Quick Reference

## 🚀 Quick Start

1. **Setup MCP** (first time only):
   ```bash
   ./setup-mcp.sh
   ```

2. **Use the helper script**:
   ```bash
   ./mcp-sledzspecke.sh health
   ./mcp-sledzspecke.sh errors
   ./mcp-sledzspecke.sh users
   ```

## 📋 Common MCP Commands

### System Health
```bash
# Quick health check
./mcp-sledzspecke.sh health

# Full system analysis
./mcp-sledzspecke.sh full-check

# Check recent errors
./mcp-sledzspecke.sh errors
```

### Database Queries
```bash
# Recent users
./mcp-sledzspecke.sh users

# Medical shifts analysis
./mcp-sledzspecke.sh shifts

# Custom SQL query
./mcp-sledzspecke.sh query "SELECT COUNT(*) FROM \"MedicalShifts\" WHERE \"Date\" > NOW() - INTERVAL '7 days'"
```

### Debugging
```bash
# Debug specific issue
./mcp-sledzspecke.sh debug "user cannot log in"

# Debug with full context
claude --mcp-config mcp-config.json -p "User jan.kowalski@wum.edu.pl reports medical shifts not saving" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__memory__recall" \
  --max-turns 5
```

### GitHub Integration
```bash
# Check builds
./mcp-sledzspecke.sh builds

# Detailed build analysis
claude --mcp-config mcp-config.json -p "Why did the last GitHub Actions build fail?" \
  --allowedTools "mcp__github__get_workflow_runs,mcp__filesystem__read_file"
```

### Memory Operations
```bash
# Store important info
./mcp-sledzspecke.sh remember "Medical shifts must not exceed 48 hours per week"

# Recall information
./mcp-sledzspecke.sh recall "SMK rules"
```

## 🔧 Direct MCP Usage

### Basic Pattern
```bash
claude --mcp-config mcp-config.json -p "<your prompt>" \
  --allowedTools "<comma-separated-tools>"
```

### Available Tools

#### Filesystem
- `mcp__filesystem__read_file`
- `mcp__filesystem__write_file`
- `mcp__filesystem__list_directory`
- `mcp__filesystem__search_files`

#### PostgreSQL
- `mcp__postgres__query`
- `mcp__postgres__analyze`
- `mcp__postgres__schema`

#### GitHub
- `mcp__github__list_workflows`
- `mcp__github__get_workflow_runs`
- `mcp__github__list_pull_requests`
- `mcp__github__create_issue`
- `mcp__github__get_file_content`

#### Memory
- `mcp__memory__store`
- `mcp__memory__recall`
- `mcp__memory__search`
- `mcp__memory__list`

## 🎯 Production Scenarios

### Morning Health Check
```bash
claude --mcp-config mcp-config.json -p "Morning health check: API status, overnight errors, new registrations, pending issues" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__github__get_workflow_runs,mcp__memory__recall" \
  --max-turns 3
```

### User Support
```bash
claude --mcp-config mcp-config.json -p "User ID 12345 cannot see their medical shifts - investigate and fix" \
  --allowedTools "mcp__postgres__query,mcp__filesystem__read_file,mcp__memory__recall" \
  --max-turns 5
```

### Performance Analysis
```bash
claude --mcp-config mcp-config.json -p "Analyze API performance: slow queries, response times, bottlenecks" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__analyze" \
  --max-turns 3
```

### Deployment Verification
```bash
claude --mcp-config mcp-config.json -p "Verify latest deployment: build status, API health, database migrations" \
  --allowedTools "mcp__github__get_workflow_runs,mcp__filesystem__read_file,mcp__postgres__query"
```

## 🔐 Security Best Practices

1. **Always specify exact tools needed**
2. **Use read-only tools for analysis**
3. **Be careful with write operations**
4. **Never expose sensitive data in prompts**

### Safe Read-Only Analysis
```bash
--allowedTools "mcp__postgres__query,mcp__filesystem__read_file,mcp__github__list*"
```

### Dangerous Operations (use with caution)
```bash
--allowedTools "mcp__filesystem__write_file,mcp__postgres__execute"
```

## 📊 Advanced Usage

### Batch Analysis
```bash
# Analyze multiple aspects in one command
claude --mcp-config mcp-config.json -p "Complete analysis: errors, performance, user issues, database health" \
  --allowedTools "mcp__filesystem__read_file,mcp__postgres__query,mcp__memory__recall" \
  --max-turns 10 \
  --output-format json > analysis-$(date +%Y%m%d).json
```

### Continuous Monitoring
```bash
# Run periodic health checks
while true; do
  ./mcp-sledzspecke.sh health
  sleep 3600  # Check every hour
done
```

### Integration with Scripts
```bash
# Use in bash scripts
result=$(claude --mcp-config mcp-config.json -p "Count active users" \
  --allowedTools "mcp__postgres__query" \
  --output-format json)
  
user_count=$(echo $result | jq -r '.result')
echo "Active users: $user_count"
```

## 📋 SMK Compliance

### Quick SMK Commands
```bash
# Analyze official PDFs
./smk-compliance-helper.sh analyze-old
./smk-compliance-helper.sh analyze-new

# Extract all field mappings
./smk-compliance-helper.sh extract-fields

# Generate test Excel files
./smk-compliance-helper.sh generate-test new
./smk-compliance-helper.sh generate-test old

# Full compliance check
./smk-compliance-helper.sh compliance-report
```

### SMK MCP Examples
```bash
# Analyze PDF for specific module
claude --mcp-config mcp-smk-config.json -p "Extract Excel format for medical shifts from New SMK PDF" \
  --allowedTools "mcp__smk-docs__read_file,mcp__memory__store"

# Implement Excel export
claude --mcp-config mcp-smk-config.json -p "Implement Excel export for procedures matching Old SMK format" \
  --allowedTools "mcp__smk-docs__read_file,mcp__smk-docs__write_file,mcp__memory__recall"

# Validate compliance
claude --mcp-config mcp-smk-config.json -p "Validate our medical shifts data matches SMK requirements" \
  --allowedTools "mcp__postgres__query,mcp__memory__recall"
```

## 🆘 Troubleshooting

### MCP Not Working?
1. Check if claude is installed: `which claude`
2. Verify API key: `echo $ANTHROPIC_API_KEY`
3. Test MCP config: `claude --mcp-config mcp-config.json -p "test" --verbose`

### Common Errors
- **"Tool not found"**: Add tool to `--allowedTools`
- **"Permission denied"**: Check file/database permissions
- **"Connection failed"**: Verify server configuration in `mcp-config.json`

### Get Help
```bash
# List all available MCP tools
claude --mcp-config mcp-config.json -p "List all available MCP tools" --verbose

# Test specific MCP server
claude --mcp-config mcp-config.json -p "Test filesystem MCP server" \
  --allowedTools "mcp__filesystem__list_directory" \
  --verbose
```