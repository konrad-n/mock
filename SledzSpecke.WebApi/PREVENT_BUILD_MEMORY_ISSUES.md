# Preventing MSBuild/Roslyn Memory Issues

## The Problem
MSBuild and Roslyn (VBCSCompiler) processes can accumulate during development because:
1. **Node Reuse**: MSBuild keeps processes alive to speed up subsequent builds
2. **Compiler Server**: Roslyn compiler server stays resident in memory
3. **Parallel Builds**: Multiple MSBuild nodes spawn for parallel compilation
4. **No Timeout**: Processes don't automatically terminate

## The Solution

### 1. Environment Configuration (.env file)
```bash
MSBUILDDISABLENODEREUSE=1        # Prevents MSBuild process reuse
MSBUILDNODECOUNT=1               # Limits to single process
DOTNET_CLI_USE_MSBUILD_SERVER=0  # Disables MSBuild server
DOTNET_COMPILER_SERVER_TIMEOUT=600 # 10-minute timeout for Roslyn
```

### 2. Safe Build Script (safe-build.sh)
- Kills existing processes before building
- Applies environment limits
- Cleans up immediately after build

### 3. Automatic Cleanup (via cron)
- Runs every 5 minutes
- Kills processes older than 10 minutes
- Logs all cleanup actions

### 4. Manual Controls
- `killbuild` - Emergency kill command
- `clean` - Quick cleanup
- `cleanfull` - Complete cleanup

## How to Use

### Initial Setup (One Time)
```bash
# 1. Setup automatic cleanup
./setup-auto-cleanup.sh

# 2. Setup command aliases
./setup-aliases.sh
source ~/.bashrc
```

### Daily Development
```bash
# Use safe build instead of dotnet build
./safe-build.sh

# Or use the alias
build

# Run API safely
runapi

# Quick cleanup when needed
clean
```

### Emergency Recovery
```bash
# If system is slow
killbuild
clean

# If very bad
cleanfull
```

## Verification

Check if prevention is working:
```bash
# Should show minimal dotnet processes
ps aux | grep -E "(MSBuild|VBCSCompiler)" | grep -v grep

# Should show good memory availability
free -h
```

## Why This Works

1. **No Process Accumulation**: Processes are killed after each build
2. **Resource Limits**: Only one build process at a time
3. **Automatic Cleanup**: Cron job prevents long-running processes
4. **Immediate Feedback**: Scripts show memory before/after

## Additional Tips

1. **Use Incremental Builds**: 
   ```bash
   build --no-restore  # When dependencies haven't changed
   ```

2. **Build Specific Projects**:
   ```bash
   dotnet build src/SledzSpecke.Api/SledzSpecke.Api.csproj
   ```

3. **Monitor Resources**:
   ```bash
   watch -n 1 'free -h; echo; ps aux | grep -E "(MSBuild|VBC)" | grep -v grep'
   ```

4. **Check Cleanup Logs**:
   ```bash
   tail -f ~/dotnet-cleanup.log
   ```

## Troubleshooting

If issues persist:
1. Check if .env is loaded: `echo $MSBUILDDISABLENODEREUSE`
2. Verify cron job: `crontab -l`
3. Check process ages: `ps aux | grep MSBuild`
4. Review cleanup log: `cat ~/dotnet-cleanup.log`

With these measures in place, MSBuild/Roslyn process accumulation should never happen again!