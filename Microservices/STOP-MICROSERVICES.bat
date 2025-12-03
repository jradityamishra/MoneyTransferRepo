# Batch file wrapper to stop all services
@echo off
echo Stopping All Microservices...
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0stop-microservices.ps1"

pause
