#!/bin/bash

# SledzSpecke Quick Start Script
# This script starts both the backend API and frontend development server

echo "🚀 Starting SledzSpecke Application..."
echo ""

# Check if PostgreSQL is running
if ! pg_isready -q; then
    echo "❌ PostgreSQL is not running. Please start PostgreSQL first."
    echo "   On Ubuntu/Debian: sudo systemctl start postgresql"
    echo "   On macOS: brew services start postgresql"
    exit 1
fi

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
if ! command_exists dotnet; then
    echo "❌ .NET SDK is not installed. Please install .NET 9 SDK from:"
    echo "   https://dotnet.microsoft.com/download/dotnet/9.0"
    exit 1
fi

if ! command_exists node; then
    echo "❌ Node.js is not installed. Please install Node.js 18+ from:"
    echo "   https://nodejs.org/"
    exit 1
fi

# Start backend
echo "📦 Starting Backend API..."
cd SledzSpecke.WebApi

# Check if migrations are needed
if ! dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api --no-build 2>/dev/null; then
    echo "⚠️  Running database migrations..."
    dotnet ef database update -p src/SledzSpecke.Infrastructure -s src/SledzSpecke.Api
fi

# Start API in background
dotnet run --project src/SledzSpecke.Api &
API_PID=$!
echo "✅ Backend API started (PID: $API_PID)"
echo "   API URL: http://localhost:5000"
echo "   Swagger: http://localhost:5000/swagger"
echo ""

# Wait for API to be ready
echo "⏳ Waiting for API to be ready..."
while ! curl -s http://localhost:5000/swagger/index.html > /dev/null; do
    sleep 2
done
echo "✅ API is ready!"
echo ""

# Start frontend
echo "📦 Starting Frontend..."
cd ../SledzSpecke-Frontend/packages/web

# Install dependencies if needed
if [ ! -d "node_modules" ]; then
    echo "📦 Installing frontend dependencies..."
    npm install
fi

# Start frontend
echo "🌐 Starting development server..."
npm run dev &
FRONTEND_PID=$!
echo "✅ Frontend started (PID: $FRONTEND_PID)"
echo "   Frontend URL: http://localhost:5173"
echo ""

# Show test credentials
echo "🔐 Test Credentials:"
echo "   Username: testuser"
echo "   Password: Test123!"
echo ""

echo "✨ SledzSpecke is running!"
echo ""
echo "Press Ctrl+C to stop all services..."

# Function to cleanup on exit
cleanup() {
    echo ""
    echo "🛑 Stopping services..."
    kill $API_PID 2>/dev/null
    kill $FRONTEND_PID 2>/dev/null
    echo "👋 Goodbye!"
    exit 0
}

# Set up trap to cleanup on Ctrl+C
trap cleanup INT

# Wait indefinitely
while true; do
    sleep 1
done