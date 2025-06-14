# Development Optimization Guide

## Resource Monitoring

### Current VPS Specs
- RAM: 3.8 GB
- Swap: 4.0 GB
- CPU: 2 cores
- Disk: 77 GB (12% used)

### Memory Usage Issues
During heavy development, multiple MSBuild and Roslyn compiler processes can accumulate, consuming significant memory. This leads to:
- Slow builds
- System using swap memory
- Degraded performance

## Optimization Strategies

### 1. Regular Cleanup
Run the cleanup script periodically:
```bash
# Basic cleanup (kills processes, clears temp files)
./cleanup-resources.sh

# Full cleanup (also removes bin/obj directories)
./cleanup-resources.sh --full
```

### 2. Optimized Build Commands
```bash
# Use no-restore when dependencies haven't changed
dotnet build --no-restore

# Use configuration-specific builds
dotnet build -c Release  # Smaller binaries, optimized
dotnet build -c Debug    # Faster builds, larger binaries

# Build specific projects only
dotnet build src/SledzSpecke.Api/SledzSpecke.Api.csproj

# Use parallel builds wisely (limit on low-memory systems)
dotnet build -maxcpucount:1  # Limit to single CPU
```

### 3. Development Workflow Optimizations

#### Testing Strategy
- Test specific endpoints instead of full API runs
- Use `dotnet watch run` for auto-reload during development
- Kill the API process immediately after testing

#### Build Strategy
- Build only what changed: `dotnet build --no-dependencies`
- Use incremental builds
- Clean only when necessary

### 4. IDE/Editor Optimizations
If using VS Code or other IDEs remotely:
- Disable unnecessary extensions
- Limit OmniSharp memory usage
- Use lightweight editors for quick edits

### 5. Docker Alternative (Future)
Consider containerizing the application to:
- Isolate build environments
- Easy cleanup with container removal
- Consistent resource limits

## Monitoring Commands

```bash
# Check memory usage
free -h

# Check top processes
ps aux --sort=-%mem | head -20

# Check disk usage
df -h
du -sh ~/projects/mock/SledzSpecke.WebApi/*

# Monitor in real-time
htop  # If installed
watch -n 1 free -h
```

## Emergency Recovery

If system becomes unresponsive:
1. Kill all dotnet processes: `pkill -9 dotnet`
2. Clear swap: `sudo swapoff -a && sudo swapon -a` (requires sudo)
3. Restart dotnet services
4. Run cleanup script

## Preventive Measures

1. **Set Resource Limits**
   ```bash
   # Limit MSBuild processes
   export MSBUILDNODECOUNT=1
   ```

2. **Configure .NET**
   Create `~/.dotnet/tools/.store/.stage` with:
   ```json
   {
     "configProperties": {
       "System.GC.Server": false,
       "System.GC.Concurrent": true
     }
   }
   ```

3. **Use Build Cache**
   ```bash
   # Enable build cache
   export DOTNET_CLI_USE_MSBUILD_SERVER=1
   ```

## Development Best Practices

1. **Commit and Push Regularly**
   - Reduces need for long-running processes
   - Allows cleanup between sessions

2. **Use Feature Branches**
   - Smaller, focused builds
   - Easier to clean and restart

3. **Test Incrementally**
   - Run API only when needed
   - Use unit tests for quick feedback
   - Integration tests only for final validation

4. **Clean Workspace Daily**
   - Run cleanup script at end of day
   - Start fresh each session

Remember: A clean development environment is a fast development environment!