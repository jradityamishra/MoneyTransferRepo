# ?? Microservices Launcher - NO Console Windows!

## Problem Solved
This solution eliminates multiple console windows when running your microservices and provides a unified Swagger UI.

## ?? Files Created

1. **START-MICROSERVICES.bat** - Double-click to start all services
2. **STOP-MICROSERVICES.bat** - Double-click to stop all services
3. **start-microservices.ps1** - PowerShell script (runs automatically)
4. **stop-microservices.ps1** - PowerShell script (runs automatically)

## ?? How to Use

### Starting All Microservices

**Option 1: Double-Click (Easiest)**
```
Simply double-click: START-MICROSERVICES.bat
```

**Option 2: PowerShell**
```powershell
.\start-microservices.ps1
```

**Option 3: From PowerShell Terminal**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\start-microservices.ps1
```

### What Happens:
? All microservices start in the background (NO console windows)
? Builds the solution first
? Starts services in correct order with delays
? Automatically opens unified Swagger UI at: https://localhost:7000/swagger
? All services accessible through API Gateway

### Stopping All Microservices

**Option 1: Double-Click**
```
Double-click: STOP-MICROSERVICES.bat
```

**Option 2: Press Ctrl+C**
In the PowerShell window where you started the services

**Option 3: PowerShell**
```powershell
.\stop-microservices.ps1
```

## ?? Access Points

After starting:
- **Unified Swagger UI**: https://localhost:7000/swagger
- **API Gateway**: https://localhost:7000
- **Direct Access** (if needed):
  - User Service: https://localhost:7001
  - Account Service: https://localhost:7002
  - Transaction Service: https://localhost:7003
  - Notification Service: https://localhost:7005

## ?? Troubleshooting

### If you get execution policy error:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### If ports are already in use:
```
Run: STOP-MICROSERVICES.bat
Wait 5 seconds, then run: START-MICROSERVICES.bat
```

### If services don't start:
1. Check that all projects build successfully in Visual Studio
2. Ensure no other applications are using ports 7000-7005
3. Run `stop-microservices.ps1` to clean up any orphaned processes

## ?? Notes

- The script keeps running in the background
- Press **Ctrl+C** in the PowerShell window to stop all services
- Closing the PowerShell window will also stop all services
- Only ONE Swagger UI window opens (unified at API Gateway)
- All microservices run silently in the background

## ?? Benefits

? No multiple console windows cluttering your screen
? No multiple browser tabs opening
? Clean, unified Swagger interface
? Easy start/stop with one click
? Proper service startup order
? Automatic cleanup on exit

## ?? Advanced Usage

### Run in Background (Detached)
If you want services to run even after closing PowerShell:
```powershell
Start-Process powershell -ArgumentList "-NoExit", "-File", ".\start-microservices.ps1" -WindowStyle Hidden
```

### Check Running Services
```powershell
Get-Process dotnet | Where-Object {$_.Path -like "*MoneyTransferRepo*"}
```

---

**Enjoy your clean development environment! ??**
