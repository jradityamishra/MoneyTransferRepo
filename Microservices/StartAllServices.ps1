# Start all microservices
Write-Host "Starting all microservices..." -ForegroundColor Green

# Start UserMicroservices
Write-Host "`nStarting UserMicroservices on port 5001..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot'; dotnet run --project UserMicroservices/UserMicroservices.csproj"

# Wait a bit before starting next service
Start-Sleep -Seconds 2

# Start AccountMicroservices
Write-Host "Starting AccountMicroservices on port 5002..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot'; dotnet run --project AccountMicroservices/AccountMicroservices.csproj"

# Wait a bit before starting next service
Start-Sleep -Seconds 2

# Start TransactionMicroservices
Write-Host "Starting TransactionMicroservices on port 5003..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot'; dotnet run --project TransactionMicroservices/TransactionMicroservices.csproj"

# Wait a bit before starting next service
Start-Sleep -Seconds 2

# Start NotificationMicroservices
Write-Host "Starting NotificationMicroservices on port 5004..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot'; dotnet run --project NotificationMicroservices/NotificationMicroservices.csproj"

# Wait a bit before starting gateway
Start-Sleep -Seconds 3

# Start Ocelot API Gateway
Write-Host "Starting Ocelot API Gateway on port 5000..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot'; dotnet run --project OcelotApiGateway/OcelotApiGateway.csproj"

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "All services are starting!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "`nAPI Endpoints:" -ForegroundColor White
Write-Host "  Ocelot API Gateway:        http://localhost:5000" -ForegroundColor Yellow
Write-Host "  UserMicroservices:         http://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host "  AccountMicroservices:      http://localhost:5002/swagger" -ForegroundColor Cyan
Write-Host "  TransactionMicroservices:  http://localhost:5003/swagger" -ForegroundColor Cyan
Write-Host "  NotificationMicroservices: http://localhost:5004/swagger" -ForegroundColor Cyan
Write-Host "`nPress any key to exit this window (services will continue running)..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
