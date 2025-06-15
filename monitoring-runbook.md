# SledzSpecke Monitoring Runbook

## Overview
SledzSpecke uses structured logging with Serilog to provide comprehensive monitoring and debugging capabilities.

## Quick Start

### Viewing Logs
Use the provided log viewer script:

```bash
# View recent errors
./view-logs.sh errors

# View recent logs
./view-logs.sh recent

# Search for specific terms
./view-logs.sh search "user@example.com"

# View log statistics
./view-logs.sh stats
```

### Log Locations
- **Application logs**: `/var/log/sledzspecke/api-YYYY-MM-DD.log`
- **Structured JSON logs**: `/var/log/sledzspecke/structured-YYYY-MM-DD.json`
- **System logs**: `sudo journalctl -u sledzspecke-api`

## Common Troubleshooting

### Finding Registration Errors
```bash
# Search for registration failures
./view-logs.sh search "Registration failed"

# Get detailed error with correlation ID
sudo journalctl -u sledzspecke-api | grep "CorrelationId.*<ID_HERE>"
```

### Checking API Health
```bash
# Check if API is running
systemctl status sledzspecke-api

# Check recent errors
./view-logs.sh errors

# Check API response times
sudo journalctl -u sledzspecke-api | grep "Request completed" | tail -10
```

## Error Codes Reference

| Code | Description | Solution |
|------|-------------|----------|
| AUTH_INVALID_CREDENTIALS | Invalid login credentials | Check username/password |
| USER_EMAIL_IN_USE | Email already registered | Use different email |
| USER_USERNAME_IN_USE | Username already taken | Choose different username |
| USER_REGISTRATION_FAILED | General registration failure | Check logs for details |
| SPEC_NOT_FOUND | Specialization doesn't exist | Verify specialization ID |
| VALIDATION_FAILED | Input validation error | Check request format |

## Log Analysis

### Using jq for JSON logs
```bash
# Pretty print structured logs
tail -f /var/log/sledzspecke/structured-*.json | jq '.'

# Filter by log level
cat /var/log/sledzspecke/structured-*.json | jq 'select(.Level == "Error")'

# Extract specific fields
cat /var/log/sledzspecke/structured-*.json | jq '{time: .Timestamp, message: .MessageTemplate, user: .UserId}'
```

### Common Queries

#### Find all errors for a user
```bash
sudo journalctl -u sledzspecke-api | grep "UserId.*123" | grep "ERR"
```

#### Count errors by type
```bash
cat /var/log/sledzspecke/structured-*.json | jq -r '.ExceptionType' | sort | uniq -c | sort -rn
```

#### Track request performance
```bash
sudo journalctl -u sledzspecke-api | grep "Request completed" | grep -oP "ElapsedTime: \K\d+" | awk '{sum+=$1; count++} END {print "Average:", sum/count, "ms"}'
```

## Monitoring Best Practices

1. **Check logs daily** - Review error trends
2. **Set up alerts** - Monitor for critical errors
3. **Archive old logs** - Logs are kept for 7 days
4. **Use correlation IDs** - Track requests across the system
5. **Monitor disk space** - Logs can grow quickly

## Emergency Procedures

### API Not Responding
1. Check service status: `systemctl status sledzspecke-api`
2. Check recent errors: `./view-logs.sh errors`
3. Restart service: `sudo systemctl restart sledzspecke-api`
4. Check disk space: `df -h`
5. Check memory: `free -h`

### High Error Rate
1. Identify error pattern: `./view-logs.sh errors`
2. Check specific endpoint: `./view-logs.sh search "/api/endpoint"`
3. Review recent deployments
4. Check database connectivity
5. Monitor resource usage

### Disk Space Issues
```bash
# Check log sizes
du -sh /var/log/sledzspecke/*

# Remove old logs (older than 7 days)
find /var/log/sledzspecke -name "*.log" -mtime +7 -delete
find /var/log/sledzspecke -name "*.json" -mtime +7 -delete
```

## Integration with Development

### Local Development
Set environment variable for detailed logging:
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

### Testing Error Scenarios
```bash
# Test invalid registration
curl -X POST http://localhost:5000/api/auth/sign-up \
  -H "Content-Type: application/json" \
  -d '{"email":"invalid-email","username":"test","password":"123"}'

# Check the logs
./view-logs.sh search "invalid-email"
```

## Contact
For monitoring issues or questions, check:
- Application logs first
- GitHub issues: https://github.com/konrad-n/mock/issues
- Documentation: /home/ubuntu/projects/mock/CLAUDE.md