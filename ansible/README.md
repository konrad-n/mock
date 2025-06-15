# SledzSpecke Ansible Infrastructure as Code

This Ansible playbook automates the deployment and configuration of the SledzSpecke application stack.

## Prerequisites

1. Install Ansible on your local machine:
```bash
pip install ansible
```

2. Install required collections:
```bash
ansible-galaxy collection install -r requirements.yml
```

3. Set up SSH key authentication to your VPS:
```bash
ssh-copy-id ubuntu@51.77.59.184
```

## Usage

### Deploy everything:
```bash
ansible-playbook -i inventory.yml playbook.yml
```

### Deploy specific components:
```bash
# Only database
ansible-playbook -i inventory.yml playbook.yml --tags postgres

# Only API
ansible-playbook -i inventory.yml playbook.yml --tags dotnet,deploy

# Only frontend
ansible-playbook -i inventory.yml playbook.yml --tags nodejs,nginx

# Only monitoring
ansible-playbook -i inventory.yml playbook.yml --tags monitoring
```

### Dry run (check mode):
```bash
ansible-playbook -i inventory.yml playbook.yml --check
```

### With vault for secrets:
```bash
# Create vault file
ansible-vault create vault.yml

# Run with vault
ansible-playbook -i inventory.yml playbook.yml --ask-vault-pass
```

## What it configures:

1. **Security**:
   - UFW firewall
   - Fail2ban
   - Unattended upgrades
   - SSH hardening

2. **Database**:
   - PostgreSQL 16
   - Automatic backups
   - Performance tuning

3. **API**:
   - .NET 9 runtime
   - Systemd service
   - Health checks
   - Database migrations

4. **Frontend**:
   - Node.js 20
   - Nginx with SSL
   - Rate limiting
   - Security headers

5. **Monitoring**:
   - Seq for logging
   - Prometheus for metrics
   - Grafana for visualization
   - Node exporter
   - Health checks

## Variables

Edit `inventory.yml` to customize:
- Database credentials
- JWT secrets
- Domain names
- Monitoring passwords

## Backup and Restore

The playbook sets up automatic daily backups at 2 AM. To manually backup:
```bash
ansible sledzspecke -i inventory.yml -m shell -a "/usr/local/bin/backup-sledzspecke.sh"
```

## Troubleshooting

Check deployment status:
```bash
ansible sledzspecke -i inventory.yml -m shell -a "systemctl status sledzspecke-api"
```

View logs:
```bash
ansible sledzspecke -i inventory.yml -m shell -a "tail -n 50 /var/log/sledzspecke/api-$(date +%Y-%m-%d).log"
```