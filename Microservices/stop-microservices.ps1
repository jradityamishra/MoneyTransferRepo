# PowerShell Script to Stop All Running Microservices

Write-Host "Stopping all microservices..." -ForegroundColor Yellow

# Find and kill all dotnet processes running our microservices
$processes = Get-Process dotnet -ErrorAction SilentlyContinue | Where-Object {
    $_.Path -like "*MoneyTransferRepo*"
}

if ($processes) {
    Write-Host "Found $($processes.Count) microservice process(es)" -ForegroundColor Cyan
    
    foreach ($process in $processes) {
        Write-Host "Stopping process: $($process.Id)" -ForegroundColor Gray
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host "`nAll microservices stopped successfully!" -ForegroundColor Green
} else {
    Write-Host "No running microservices found." -ForegroundColor Yellow
}

# Also kill any remaining processes on specific ports
$ports = @(5001, 5002, 5003, 5005, 5000, 7001, 7002, 7003, 7005, 7000)

foreach ($port in $ports) {
    $netstat = netstat -ano | Select-String ":$port\s" | Select-Object -First 1
    if ($netstat) {
        $processId = ($netstat -split '\s+')[-1]
        if ($processId -and $processId -ne '0') {
            Write-Host "Killing process on port $port (PID: $processId)" -ForegroundColor Gray
            Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
        }
    }
}

Write-Host "`nCleanup complete!" -ForegroundColor Green
