# PowerShell Script to Launch All Microservices Silently
# Only opens unified Swagger UI

Write-Host "Starting Money Transfer Microservices..." -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green

# Get the script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionPath = $scriptPath

# Array to store background jobs
$jobs = @()

# Function to start a microservice in background
function Start-Microservice {
    param(
        [string]$ProjectName,
        [string]$ProjectPath,
        [string]$Port
    )
    
    Write-Host "Starting $ProjectName on port $Port..." -ForegroundColor Yellow
    
    $job = Start-Process -FilePath "dotnet" `
        -ArgumentList "run --project `"$ProjectPath`" --no-build" `
        -WorkingDirectory (Split-Path -Parent $ProjectPath) `
        -WindowStyle Hidden `
        -PassThru
    
    return $job
}

# Build all projects first
Write-Host "`nBuilding solution..." -ForegroundColor Cyan
dotnet build "$solutionPath\Microservices.slnx" --configuration Debug

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nBuild failed! Please fix errors and try again." -ForegroundColor Red
    exit 1
}

Write-Host "Build successful!`n" -ForegroundColor Green

# Start all microservices silently
Write-Host "Launching microservices in background..." -ForegroundColor Cyan

$jobs += Start-Microservice -ProjectName "UserMicroservices" `
    -ProjectPath "$solutionPath\UserMicroservices\UserMicroservices.csproj" `
    -Port "7001"
Start-Sleep -Seconds 2

$jobs += Start-Microservice -ProjectName "AccountMicroservices" `
    -ProjectPath "$solutionPath\AccountMicroservices\AccountMicroservices.csproj" `
    -Port "7002"
Start-Sleep -Seconds 2

$jobs += Start-Microservice -ProjectName "TransactionMicroservices" `
    -ProjectPath "$solutionPath\TransactionMicroservices\TransactionMicroservices.csproj" `
    -Port "7003"
Start-Sleep -Seconds 2

$jobs += Start-Microservice -ProjectName "NotificationMicroservices" `
    -ProjectPath "$solutionPath\NotificationMicroservices\NotificationMicroservices.csproj" `
    -Port "7005"
Start-Sleep -Seconds 3

$jobs += Start-Microservice -ProjectName "OcelotApiGateway" `
    -ProjectPath "$solutionPath\OcelotApiGateway\OcelotApiGateway.csproj" `
    -Port "7000"
Start-Sleep -Seconds 5

# Open unified Swagger UI in browser
Write-Host "`nOpening Unified Swagger UI..." -ForegroundColor Green
Start-Process "https://localhost:7000/swagger"

Write-Host "`n==========================================" -ForegroundColor Green
Write-Host "All microservices are running!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host "`nAccess points:" -ForegroundColor Cyan
Write-Host "  Unified Swagger UI: https://localhost:7000/swagger" -ForegroundColor White
Write-Host "  API Gateway:        https://localhost:7000" -ForegroundColor White
Write-Host "`nPress Ctrl+C to stop all services..." -ForegroundColor Yellow

# Keep script running and handle cleanup
try {
    while ($true) {
        Start-Sleep -Seconds 1
        # Check if any process has exited
        foreach ($job in $jobs) {
            if ($job.HasExited) {
                Write-Host "`nWarning: A microservice has stopped unexpectedly!" -ForegroundColor Red
            }
        }
    }
}
finally {
    Write-Host "`nStopping all microservices..." -ForegroundColor Yellow
    foreach ($job in $jobs) {
        if (-not $job.HasExited) {
            Stop-Process -Id $job.Id -Force -ErrorAction SilentlyContinue
        }
    }
    Write-Host "All services stopped." -ForegroundColor Green
}
