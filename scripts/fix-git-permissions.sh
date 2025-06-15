#!/bin/bash
# Fix Git permissions on VPS

echo "Fixing Git permissions in /home/ubuntu/sledzspecke..."

# Fix ownership
sudo chown -R ubuntu:ubuntu /home/ubuntu/sledzspecke
sudo chown -R ubuntu:ubuntu /home/ubuntu/sledzspecke/.git

# Fix permissions
find /home/ubuntu/sledzspecke/.git -type d -exec chmod 755 {} \;
find /home/ubuntu/sledzspecke/.git -type f -exec chmod 644 {} \;

# Make Git hooks executable if they exist
if [ -d "/home/ubuntu/sledzspecke/.git/hooks" ]; then
    chmod +x /home/ubuntu/sledzspecke/.git/hooks/*
fi

echo "Git permissions fixed!"

# Test git operations
cd /home/ubuntu/sledzspecke
git status

echo "If you still have issues, try:"
echo "  rm -rf /home/ubuntu/sledzspecke"
echo "  git clone https://github.com/konrad-n/mock.git sledzspecke"