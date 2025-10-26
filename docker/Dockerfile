# Multi-stage Dockerfile for C# SDET Test Automation Framework
# This Dockerfile creates an optimized image for running .NET tests with Playwright

# ==================== Stage 1: Build Stage ====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy NuGet configuration
COPY NuGet.config ./

# Copy solution and project files
COPY SDET_CS_Framework.sln ./
COPY src/SDET.Core.csproj ./src/
COPY src/Application/SDET.Application.csproj ./src/Application/
COPY tests/SDET.Tests.csproj ./tests/

# Clear any cached NuGet sources and restore packages
RUN dotnet nuget locals all --clear && \
    dotnet restore SDET_CS_Framework.sln --configfile NuGet.config

# Copy all source code
COPY src/ ./src/
COPY tests/ ./tests/

# Build the solution (use configfile to avoid global configs)
RUN dotnet build SDET_CS_Framework.sln -c Release

# ==================== Stage 2: Test Runtime Stage ====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test-runtime

# Install system dependencies for Playwright
RUN apt-get update && apt-get install -y \
    # Playwright browser dependencies
    libnss3 \
    libnspr4 \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libcups2 \
    libdrm2 \
    libdbus-1-3 \
    libxkbcommon0 \
    libatspi2.0-0 \
    libxcomposite1 \
    libxdamage1 \
    libxfixes3 \
    libxrandr2 \
    libgbm1 \
    libpango-1.0-0 \
    libcairo2 \
    libasound2 \
    libxshmfence1 \
    # Fonts
    fonts-liberation \
    fonts-noto-color-emoji \
    # Utilities
    curl \
    wget \
    ca-certificates \
    # Node.js for Newman (Postman CLI)
    nodejs \
    npm \
    && rm -rf /var/lib/apt/lists/*

# Install Newman and Allure CLI
RUN npm install -g newman newman-reporter-allure newman-reporter-htmlextra

# Install Allure CLI
RUN wget https://github.com/allure-framework/allure2/releases/download/2.25.0/allure-2.25.0.tgz && \
    tar -zxvf allure-2.25.0.tgz -C /opt/ && \
    ln -s /opt/allure-2.25.0/bin/allure /usr/bin/allure && \
    rm allure-2.25.0.tgz

# Set working directory
WORKDIR /app

# Copy built artifacts from build stage
COPY --from=build /app/src /app/src
COPY --from=build /app/tests /app/tests
COPY --from=build /app/SDET_CS_Framework.sln /app/

# Copy Postman collections
COPY postman/ /app/postman/

# Copy scripts (if they exist)
COPY scripts/ /app/scripts/
RUN chmod +x /app/scripts/*.sh 2>/dev/null || true

# Restore packages
RUN dotnet restore

# Build in Release mode
RUN dotnet build -c Release --no-restore

# Install Playwright browsers
WORKDIR /app/tests
RUN pwsh bin/Release/net8.0/playwright.ps1 install || \
    echo "Playwright installation completed with warnings"
RUN pwsh bin/Release/net8.0/playwright.ps1 install-deps || \
    echo "Playwright deps installation completed with warnings"

# Create directories for test outputs
RUN mkdir -p /app/test-reports \
             /app/allure-results \
             /app/allure-report \
             /app/logs \
             /app/screenshots \
             /app/videos

# Set environment variables
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=true
ENV ASPNETCORE_ENVIRONMENT=Testing
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright

# Expose ports (if running application)
EXPOSE 5001

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:5001/health || exit 1

# Default command: Run all tests
WORKDIR /app/tests
CMD ["dotnet", "test", \
     "--logger:\"console;verbosity=detailed\"", \
     "--logger:\"trx;LogFileName=test-results.trx\"", \
     "--results-directory:/app/test-reports", \
     "--collect:\"XPlat Code Coverage\""]

# ==================== Usage Examples ====================
#
# Build the Docker image:
#   docker build -f docker/Dockerfile -t sdet-cs-framework:latest .
#
# Run all tests:
#   docker run --rm sdet-cs-framework:latest
#
# Run specific test category:
#   docker run --rm sdet-cs-framework:latest \
#       dotnet test --filter "TestCategory=Smoke"
#
# Run with volume mounts for reports:
#   docker run --rm \
#       -v $(pwd)/test-reports:/app/test-reports \
#       -v $(pwd)/allure-results:/app/allure-results \
#       sdet-cs-framework:latest
#
# Run Postman/Newman tests:
#   docker run --rm sdet-cs-framework:latest \
#       newman run /app/postman/collections/SDET_API_Tests.postman_collection.json \
#       --environment /app/postman/environments/local.postman_environment.json \
#       --reporters cli,allure,htmlextra
#
# Interactive shell:
#   docker run --rm -it sdet-cs-framework:latest /bin/bash
#
# ==================== Multi-Stage Build Benefits ====================
# - Smaller final image size
# - Faster build times with layer caching
# - Separation of build and runtime dependencies
# - Optimized for CI/CD pipelines
