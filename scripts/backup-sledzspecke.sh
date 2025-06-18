#!/bin/bash

# SledzSpecke Backup Script
# Backs up PostgreSQL database and application files

# Configuration
BACKUP_DIR="/var/backups/sledzspecke"
DB_NAME="sledzspecke_db"
DB_USER="postgres"
APP_DIR="/var/www/sledzspecke-api"
LOGS_DIR="/var/log/sledzspecke"
RETENTION_DAYS=7

# Create backup directory if it doesn't exist
mkdir -p "$BACKUP_DIR"

# Generate timestamp
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Backup PostgreSQL database
echo "Starting database backup..."
sudo -u postgres pg_dump "$DB_NAME" | gzip > "$BACKUP_DIR/db_backup_$TIMESTAMP.sql.gz"
if [ $? -eq 0 ]; then
    echo "Database backup completed successfully"
else
    echo "Database backup failed"
    exit 1
fi

# Backup application files
echo "Starting application files backup..."
tar -czf "$BACKUP_DIR/app_backup_$TIMESTAMP.tar.gz" -C /var/www sledzspecke-api
if [ $? -eq 0 ]; then
    echo "Application files backup completed successfully"
else
    echo "Application files backup failed"
    exit 1
fi

# Backup logs
echo "Starting logs backup..."
tar -czf "$BACKUP_DIR/logs_backup_$TIMESTAMP.tar.gz" -C /var/log sledzspecke
if [ $? -eq 0 ]; then
    echo "Logs backup completed successfully"
else
    echo "Logs backup failed"
fi

# Backup Docker volumes
echo "Starting Docker volumes backup..."
docker run --rm -v seq-data:/data -v "$BACKUP_DIR":/backup alpine tar -czf /backup/seq_data_$TIMESTAMP.tar.gz -C /data .
docker run --rm -v grafana-data:/data -v "$BACKUP_DIR":/backup alpine tar -czf /backup/grafana_data_$TIMESTAMP.tar.gz -C /data .
docker run --rm -v prometheus-data:/data -v "$BACKUP_DIR":/backup alpine tar -czf /backup/prometheus_data_$TIMESTAMP.tar.gz -C /data .

# Backup nginx configuration
echo "Starting nginx configuration backup..."
tar -czf "$BACKUP_DIR/nginx_config_$TIMESTAMP.tar.gz" -C /etc/nginx sites-available sites-enabled

# Clean up old backups
echo "Cleaning up old backups..."
find "$BACKUP_DIR" -name "*.gz" -type f -mtime +$RETENTION_DAYS -delete

# List current backups
echo "Current backups:"
ls -lh "$BACKUP_DIR"/*.gz | tail -10

# Calculate backup size
BACKUP_SIZE=$(du -sh "$BACKUP_DIR" | cut -f1)
echo "Total backup size: $BACKUP_SIZE"

echo "Backup completed at $(date)"