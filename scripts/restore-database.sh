#!/bin/bash
# restore-database.sh - Restore PostgreSQL database from backup
# Usage: ./restore-database.sh [backup_file]

set -e

# Configuration
DB_NAME="sledzspecke_db"
DB_USER="sledzspecke_user"
DB_PASSWORD="SledzSpecke123!"
BACKUP_DIR="/var/backups/postgresql"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${YELLOW}SledzSpecke Database Restore Tool${NC}"
echo "=================================="

# Check if backup file is provided
if [ $# -eq 0 ]; then
    echo "Available backups:"
    echo ""
    ls -lh "$BACKUP_DIR"/sledzspecke_backup_*.sql.gz 2>/dev/null | tail -10 || {
        echo -e "${RED}No backups found in $BACKUP_DIR${NC}"
        exit 1
    }
    echo ""
    echo "Usage: $0 <backup_file>"
    echo "Example: $0 $BACKUP_DIR/sledzspecke_backup_20240315_020000.sql.gz"
    exit 1
fi

BACKUP_FILE="$1"

# Verify backup file exists
if [ ! -f "$BACKUP_FILE" ]; then
    echo -e "${RED}Error: Backup file not found: $BACKUP_FILE${NC}"
    exit 1
fi

echo -e "${YELLOW}WARNING: This will restore the database from:${NC}"
echo "$BACKUP_FILE"
echo ""
echo -e "${RED}This will OVERWRITE the current database!${NC}"
echo ""
read -p "Are you sure you want to continue? (yes/no): " confirm

if [ "$confirm" != "yes" ]; then
    echo "Restore cancelled."
    exit 0
fi

# Stop the API service
echo "Stopping SledzSpecke API..."
sudo systemctl stop sledzspecke-api

# Set PostgreSQL password
export PGPASSWORD="$DB_PASSWORD"

# Create a current backup before restore
echo "Creating safety backup of current database..."
SAFETY_BACKUP="/var/backups/postgresql/sledzspecke_safety_$(date +%Y%m%d_%H%M%S).sql.gz"
pg_dump -U "$DB_USER" -h localhost "$DB_NAME" | gzip > "$SAFETY_BACKUP"
echo -e "${GREEN}Safety backup created: $SAFETY_BACKUP${NC}"

# Drop and recreate database
echo "Preparing database for restore..."
psql -U "$DB_USER" -h localhost postgres << EOF
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = '$DB_NAME'
  AND pid <> pg_backend_pid();
DROP DATABASE IF EXISTS $DB_NAME;
CREATE DATABASE $DB_NAME OWNER $DB_USER;
EOF

# Restore from backup
echo "Restoring database..."
if gzip -dc "$BACKUP_FILE" | psql -U "$DB_USER" -h localhost "$DB_NAME"; then
    echo -e "${GREEN}Database restored successfully!${NC}"
else
    echo -e "${RED}Restore failed! Attempting to restore safety backup...${NC}"
    gzip -dc "$SAFETY_BACKUP" | psql -U "$DB_USER" -h localhost "$DB_NAME"
    echo -e "${YELLOW}Safety backup restored${NC}"
    exit 1
fi

# Unset password
unset PGPASSWORD

# Start the API service
echo "Starting SledzSpecke API..."
sudo systemctl start sledzspecke-api

# Wait for service to start
sleep 5

# Check service status
if sudo systemctl is-active --quiet sledzspecke-api; then
    echo -e "${GREEN}API service started successfully${NC}"
else
    echo -e "${RED}API service failed to start! Check logs:${NC}"
    echo "sudo journalctl -u sledzspecke-api -n 50"
    exit 1
fi

echo ""
echo -e "${GREEN}Database restore completed successfully!${NC}"
echo ""
echo "Next steps:"
echo "1. Verify application functionality"
echo "2. Check logs: sudo journalctl -u sledzspecke-api -f"
echo "3. Keep safety backup for a few days: $SAFETY_BACKUP"