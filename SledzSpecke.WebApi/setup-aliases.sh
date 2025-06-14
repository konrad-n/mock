#!/bin/bash

# Add helpful aliases for safe building

echo "# SledzSpecke Safe Build Aliases" >> ~/.bashrc
echo "alias build='cd /home/ubuntu/projects/mock/SledzSpecke.WebApi && ./safe-build.sh'" >> ~/.bashrc
echo "alias clean='cd /home/ubuntu/projects/mock/SledzSpecke.WebApi && ./cleanup-resources.sh'" >> ~/.bashrc
echo "alias cleanfull='cd /home/ubuntu/projects/mock/SledzSpecke.WebApi && ./cleanup-resources.sh --full'" >> ~/.bashrc
echo "alias runapi='cd /home/ubuntu/projects/mock/SledzSpecke.WebApi && MSBUILDDISABLENODEREUSE=1 dotnet run --project src/SledzSpecke.Api'" >> ~/.bashrc
echo "alias killbuild='pkill -9 -f \"MSBuild|VBCSCompiler\"'" >> ~/.bashrc

echo "✓ Aliases added to ~/.bashrc"
echo ""
echo "Available commands after reloading shell:"
echo "  build     - Safe build with resource limits"
echo "  clean     - Quick cleanup of processes"
echo "  cleanfull - Full cleanup including build artifacts"
echo "  runapi    - Run API with resource limits"
echo "  killbuild - Emergency kill all build processes"
echo ""
echo "Run 'source ~/.bashrc' to activate aliases"