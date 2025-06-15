#!/bin/bash
# setup-backup-cron.sh - Setup automatic database backups on VPS
# Run this script on the VPS to configure automatic backups

echo "Setting up automatic database backups for SledzSpecke..."

# Copy backup script to system location
sudo cp backup-database.sh /usr/local/bin/backup-sledzspecke.sh
sudo chmod +x /usr/local/bin/backup-sledzspecke.sh

# Create backup directories
sudo mkdir -p /var/backups/postgresql
sudo mkdir -p /var/log/sledzspecke

# Set proper permissions
sudo chown postgres:postgres /var/backups/postgresql
sudo chown www-data:www-data /var/log/sledzspecke

# Add to crontab (runs daily at 2 AM)
echo "Adding backup script to crontab..."
(sudo crontab -l 2>/dev/null || true; echo "0 2 * * * /usr/local/bin/backup-sledzspecke.sh >> /var/log/sledzspecke/backup-cron.log 2>&1") | sudo crontab -

# Test the backup script
echo "Testing backup script..."
sudo /usr/local/bin/backup-sledzspecke.sh

if [ $? -eq 0 ]; then
    echo "✅ Backup test successful!"
    echo "✅ Automatic backups configured to run daily at 2:00 AM"
    echo ""
    echo "Recent backups:"
    ls -lh /var/backups/postgresql/sledzspecke_backup_*.sql.gz | tail -5
else
    echo "❌ Backup test failed! Check /var/log/sledzspecke/backup.log for details"
    exit 1
fi

echo ""
echo "To check backup logs:"
echo "  sudo tail -f /var/log/sledzspecke/backup.log"
echo ""
echo "To list all backups:"
echo "  ls -lh /var/backups/postgresql/sledzspecke_backup_*.sql.gz"
echo ""
echo "To manually run backup:"
echo "  sudo /usr/local/bin/backup-sledzspecke.sh"