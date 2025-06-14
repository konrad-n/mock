#!/bin/bash

# Setup automatic cleanup of dotnet build processes

echo "=== Setting up automatic cleanup ==="
echo ""

# Create the cleanup script for cron
cat > ~/cleanup-dotnet-processes.sh << 'EOF'
#!/bin/bash
# Kill MSBuild processes older than 10 minutes
for pid in $(ps aux | grep MSBuild.dll | grep -v grep | awk '{print $2}'); do
    # Get process start time
    start_time=$(ps -o etimes= -p $pid 2>/dev/null | tr -d ' ')
    if [ ! -z "$start_time" ] && [ "$start_time" -gt 600 ]; then
        kill -9 $pid 2>/dev/null && echo "$(date): Killed old MSBuild process $pid" >> ~/dotnet-cleanup.log
    fi
done

# Kill VBCSCompiler processes older than 10 minutes
for pid in $(ps aux | grep VBCSCompiler | grep -v grep | awk '{print $2}'); do
    start_time=$(ps -o etimes= -p $pid 2>/dev/null | tr -d ' ')
    if [ ! -z "$start_time" ] && [ "$start_time" -gt 600 ]; then
        kill -9 $pid 2>/dev/null && echo "$(date): Killed old VBCSCompiler process $pid" >> ~/dotnet-cleanup.log
    fi
done
EOF

chmod +x ~/cleanup-dotnet-processes.sh

# Add to crontab (runs every 5 minutes)
echo "Adding cleanup job to crontab..."
(crontab -l 2>/dev/null; echo "*/5 * * * * /home/ubuntu/cleanup-dotnet-processes.sh") | crontab -

echo "✓ Automatic cleanup configured"
echo ""
echo "The system will now automatically kill MSBuild and Roslyn processes"
echo "that are older than 10 minutes, checking every 5 minutes."
echo ""
echo "To view cleanup logs: tail -f ~/dotnet-cleanup.log"
echo "To remove this job: crontab -e (and delete the line)"