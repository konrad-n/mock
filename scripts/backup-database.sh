#!/bin/bash
# backup-database.sh - Automatic PostgreSQL backup script for SledzSpecke
# This script should be run on the VPS and added to crontab

set -e

# Configuration
BACKUP_DIR="/var/backups/postgresql"
DB_NAME="sledzspecke_db"
DB_USER="sledzspecke_user"
DB_PASSWORD="SledzSpecke123!"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
RETENTION_DAYS=7
LOG_FILE="/var/log/sledzspecke/backup.log"

# Function to log messages
log_message() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Create directories if they don't exist
mkdir -p "$BACKUP_DIR"
mkdir -p "$(dirname "$LOG_FILE")"

log_message "Starting database backup for $DB_NAME"

# Set PostgreSQL password
export PGPASSWORD="$DB_PASSWORD"

# Create backup
BACKUP_FILE="$BACKUP_DIR/sledzspecke_backup_${TIMESTAMP}.sql.gz"

# Perform the backup
if pg_dump -U "$DB_USER" -h localhost "$DB_NAME" | gzip > "$BACKUP_FILE"; then
    log_message "Backup created successfully: $BACKUP_FILE"
    
    # Get backup size
    BACKUP_SIZE=$(du -h "$BACKUP_FILE" | cut -f1)
    log_message "Backup size: $BACKUP_SIZE"
    
    # Verify backup integrity
    if gzip -t "$BACKUP_FILE" 2>/dev/null; then
        log_message "Backup integrity verified"
    else
        log_message "ERROR: Backup file is corrupted!"
        exit 1
    fi
else
    log_message "ERROR: Backup failed!"
    exit 1
fi

# Clean up old backups
log_message "Cleaning up old backups (older than $RETENTION_DAYS days)"
DELETED_COUNT=0
while IFS= read -r old_backup; do
    if [[ -f "$old_backup" ]]; then
        rm -f "$old_backup"
        log_message "Deleted old backup: $(basename "$old_backup")"
        ((DELETED_COUNT++))
    fi
done < <(find "$BACKUP_DIR" -name "sledzspecke_backup_*.sql.gz" -mtime +$RETENTION_DAYS -type f)

if [[ $DELETED_COUNT -eq 0 ]]; then
    log_message "No old backups to delete"
else
    log_message "Deleted $DELETED_COUNT old backup(s)"
fi

# List current backups
BACKUP_COUNT=$(find "$BACKUP_DIR" -name "sledzspecke_backup_*.sql.gz" -type f | wc -l)
log_message "Total backups remaining: $BACKUP_COUNT"

# Unset password
unset PGPASSWORD

log_message "Backup process completed successfully"

# Optional: Send notification on failure
# You can add email/Slack notification here if needed

exit 0