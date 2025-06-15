# Multi-stage build for optimal size and security
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first (for better layer caching)
COPY ["SledzSpecke.WebApi.sln", "./"]
COPY ["src/SledzSpecke.Api/SledzSpecke.Api.csproj", "src/SledzSpecke.Api/"]
COPY ["src/SledzSpecke.Application/SledzSpecke.Application.csproj", "src/SledzSpecke.Application/"]
COPY ["src/SledzSpecke.Core/SledzSpecke.Core.csproj", "src/SledzSpecke.Core/"]
COPY ["src/SledzSpecke.Infrastructure/SledzSpecke.Infrastructure.csproj", "src/SledzSpecke.Infrastructure/"]

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR "/src/src/SledzSpecke.Api"
RUN dotnet build "SledzSpecke.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SledzSpecke.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN groupadd -g 1000 appuser && \
    useradd -r -u 1000 -g appuser appuser

WORKDIR /app

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Create directories for logs with proper permissions
RUN mkdir -p /app/logs && \
    chown -R appuser:appuser /app

# Health check configuration
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/monitoring/health || exit 1

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Entry point with migration support
ENTRYPOINT ["dotnet", "SledzSpecke.Api.dll"]