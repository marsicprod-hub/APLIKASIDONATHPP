#!/bin/bash
# Build and test script for HppDonatApp
# This script builds all projects and runs tests

set -e

echo "🔨 Building HppDonatApp solution..."
echo ""

echo "Building Core layer..."
dotnet build HppDonatApp.Core/HppDonatApp.Core.csproj -c Release

echo ""
echo "Building Data layer..."
dotnet build HppDonatApp.Data/HppDonatApp.Data.csproj -c Release

echo ""
echo "Building Services layer..."
dotnet build HppDonatApp.Services/HppDonatApp.Services.csproj -c Release

echo ""
echo "Building Tests project..."
dotnet build HppDonatApp.Tests/HppDonatApp.Tests.csproj -c Release

echo ""
echo "✅ All projects built successfully!"
echo ""
echo "🧪 Running unit tests..."
dotnet test HppDonatApp.Tests/HppDonatApp.Tests.csproj -c Release --logger "console;verbosity=minimal"

echo ""
echo "✅ All tests passed!"
echo ""
echo "📊 Build Summary:"
echo "   - Core: Core business logic and pricing engine"
echo "   - Data: EF Core persistence with SQLite"
echo "   - Services: Application services and file I/O"
echo "   - Tests: 30+ comprehensive unit tests"
echo ""
echo "Note: UI project (HppDonatApp) requires Windows for full build."
echo "    On Windows, run: dotnet build HppDonatApp.sln -c Release"
