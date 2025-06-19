# SMK Official Documentation

This directory should contain the official Polish government SMK (System Monitorowania Kształcenia) PDF documentation.

## Required Files

1. **old-smk-official.pdf** - Documentation for the Old SMK system (pre-2023)
2. **new-smk-official.pdf** - Documentation for the New SMK system (2023+)

## File Naming Convention

Please ensure files are named exactly as above for the MCP tools to work correctly.

## Security Note

These PDFs contain official government documentation and should not be committed to version control. Add them to `.gitignore`:

```
docs/smk-pdfs/*.pdf
```

## Usage

Once PDFs are in place, use the SMK compliance helper:

```bash
# Analyze PDFs
../../../smk-compliance-helper.sh analyze-old
../../../smk-compliance-helper.sh analyze-new

# Extract field mappings
../../../smk-compliance-helper.sh extract-fields

# Generate compliance report
../../../smk-compliance-helper.sh compliance-report
```

## MCP Integration

These PDFs are accessible via MCP using the `mcp-smk-config.json` configuration:

```bash
claude --mcp-config ../../mcp-smk-config.json -p "Analyze SMK PDFs" \
  --allowedTools "mcp__smk-docs__read_file"
```