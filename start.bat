@echo off
REM SledzSpecke Quick Start Script for Windows
REM This script starts both the backend API and frontend development server

echo Starting SledzSpecke Application...
echo.

REM Check if dotnet is available
where dotnet >nul 2>nul
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed. Please install .NET 9 SDK from:
    echo        https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)

REM Check if node is available
where node >nul 2>nul
if %errorlevel% neq 0 (
    echo ERROR: Node.js is not installed. Please install Node.js 18+ from:
    echo        https://nodejs.org/
    pause
    exit /b 1
)

echo Starting Backend API...
cd SledzSpecke.WebApi

REM Start API in new window
start "SledzSpecke API" cmd /k "dotnet run --project src/SledzSpecke.Api"

echo Backend API starting...
echo    API URL: http://localhost:5000
echo    Swagger: http://localhost:5000/swagger
echo.

REM Wait a bit for API to start
timeout /t 5 /nobreak >nul

echo Starting Frontend...
cd ..\SledzSpecke-Frontend\packages\web

REM Check if node_modules exists
if not exist "node_modules" (
    echo Installing frontend dependencies...
    call npm install
)

REM Start frontend in new window
start "SledzSpecke Frontend" cmd /k "npm run dev"

echo Frontend starting...
echo    Frontend URL: http://localhost:5173
echo.

echo ============================================
echo SledzSpecke is starting!
echo.
echo Test Credentials:
echo    Username: testuser
echo    Password: Test123!
echo.
echo Both API and Frontend are running in separate windows.
echo Close those windows to stop the services.
echo ============================================

pause