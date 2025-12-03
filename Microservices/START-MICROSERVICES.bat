# Batch file wrapper for easier execution
@echo off
echo Starting Microservices with Unified Swagger...
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0start-microservices.ps1"

pause
