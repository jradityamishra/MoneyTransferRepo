# MoneyTransferRepo

A microservices-based money transfer system built with **.NET 8.0**, implementing modern distributed architecture patterns with an API Gateway for centralized routing.

## 🏗️ Architecture Overview

This project demonstrates a **microservices architecture** where each service is independently deployable and maintains its own database. The system uses **Ocelot API Gateway** as the single entry point for all client requests.

### Microservices

| Service | Port | Responsibility | Database |
|---------|------|----------------|----------|
| **OcelotApiGateway** | 7000 | API Gateway & Request Routing | N/A |
| **UserMicroservices** | 7001 | User authentication & profiles | UserDatabase |
| **AccountMicroservices** | 7002 | Account management & balances | AccountDatabase |
| **TransactionMicroservices** | 7003 | Money transfers & history | TransactionDatabase |
| **NotificationMicroservices** | 7005 | Email/SMS notifications | NotificationDatabase |

## 🚀 Technology Stack

- **.NET 8.0** - Modern cross-platform framework
- **ASP.NET Core Web API** - RESTful API services
- **Entity Framework Core 8.0** - ORM for database operations
- **SQL Server (LocalDB)** - Relational database
- **Ocelot** - API Gateway library
- **Swagger/OpenAPI** - API documentation

## 📋 Prerequisites

Before running this project, ensure you have:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express, or Developer Edition)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) (optional)
- [PowerShell 5.1+](https://docs.microsoft.com/powershell/) (for startup script)

## 🔧 Installation

### 1. Clone the Repository

```bash
git clone https://github.com/jradityamishra/MoneyTransferRepo.git
cd MoneyTransferRepo
```

### 2. Configure Database Connection Strings

Each microservice has its own `appsettings.json` file. Verify the connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database={ServiceName}Database;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Note**: Replace `localhost` with your SQL Server instance name if different.

### 3. Apply Database Migrations

For each microservice with a database, run migrations:

```powershell
# From the Microservices directory
cd Microservices

# UserMicroservices
dotnet ef database update --project UserMicroservices

# AccountMicroservices
dotnet ef database update --project AccountMicroservices

# TransactionMicroservices
dotnet ef database update --project TransactionMicroservices

# NotificationMicroservices
dotnet ef database update --project NotificationMicroservices
```

### 4. Restore NuGet Packages

```powershell
dotnet restore MoneyTransferMicroservices.sln
```

## ▶️ Running the Application

### Option 1: PowerShell Script (Recommended)

The easiest way to start all services:

```powershell
cd Microservices
.\StartAllServices.ps1
```

This script will:
- Launch each microservice in a separate PowerShell window
- Start services in the correct order
- Display all service URLs

**Service URLs:**
- **API Gateway**: `https://localhost:7000`
- **UserMicroservices**: `https://localhost:7001/swagger`
- **AccountMicroservices**: `https://localhost:7002/swagger`
- **TransactionMicroservices**: `https://localhost:7003/swagger`
- **NotificationMicroservices**: `https://localhost:7005/swagger`

### Option 2: Visual Studio

1. Open `MoneyTransferMicroservices.sln`
2. Right-click the solution → **Configure Startup Projects**
3. Select **Multiple startup projects**
4. Set all projects to **Start**
5. Press **F5** to run

### Option 3: Individual Services (Terminal)

Run each service in a separate terminal:

```powershell
# Terminal 1 - UserMicroservices
dotnet run --project UserMicroservices

# Terminal 2 - AccountMicroservices
dotnet run --project AccountMicroservices

# Terminal 3 - TransactionMicroservices
dotnet run --project TransactionMicroservices

# Terminal 4 - NotificationMicroservices
dotnet run --project NotificationMicroservices

# Terminal 5 - OcelotApiGateway
dotnet run --project OcelotApiGateway
```

## 🔍 Testing the APIs

### Using Swagger UI

Each microservice includes Swagger UI (Development mode only):

- Navigate to `https://localhost:{port}/swagger`
- Explore available endpoints
- Execute test requests directly from the browser

### Using HTTP Files

Each microservice includes a `.http` file for testing:

```http
# UserMicroservices.http
GET https://localhost:7001/api/auth/test
```

Use the **REST Client** extension in VS Code to execute these requests.

### Using the API Gateway

All requests can be routed through the gateway:

```http
# Instead of: https://localhost:7001/api/auth/test
# Use gateway: https://localhost:7000/auth/test

GET https://localhost:7000/auth/test
GET https://localhost:7000/accounts/test
GET https://localhost:7000/transaction/test
GET https://localhost:7000/notification/test
```

## 🗂️ Project Structure

```
MoneyTransferRepo/
├── .github/
│   └── copilot-instructions.md    # Development guidelines
├── Microservices/
│   ├── Directory.Build.props       # Shared build configuration
│   ├── MoneyTransferMicroservices.sln
│   ├── StartAllServices.ps1        # Launch script
│   │
│   ├── UserMicroservices/
│   │   ├── Controllers/            # API endpoints
│   │   ├── Data/                   # Database context
│   │   ├── Model/
│   │   │   ├── DTO/               # Data Transfer Objects
│   │   │   └── Entity/            # Database entities
│   │   ├── Migrations/            # EF Core migrations
│   │   ├── Services/              # Business logic
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── AccountMicroservices/       # Similar structure
│   ├── TransactionMicroservices/   # Similar structure
│   ├── NotificationMicroservices/  # Similar structure
│   │
│   └── OcelotApiGateway/
│       ├── ocelot.json            # Route configuration
│       └── Program.cs
│
└── README.md
```

## 🔄 API Gateway Routing

The Ocelot gateway routes requests to the appropriate microservice:

| Gateway Route | → | Downstream Service | Endpoint |
|---------------|---|-------------------|----------|
| `/auth/*` | → | UserMicroservices | `/api/auth/*` |
| `/accounts/*` | → | AccountMicroservices | `/api/account/*` |
| `/transaction/*` | → | TransactionMicroservices | `/api/transaction/*` |
| `/notification/*` | → | NotificationMicroservices | `/api/notification/*` |

**Example Request Flow:**
```
Client → https://localhost:7000/auth/login
  ↓
Ocelot Gateway (Port 7000)
  ↓
UserMicroservices → https://localhost:7001/api/auth/login
  ↓
Database (UserDatabase)
  ↓
Response → Client
```

## 🛠️ Development Workflow

### Adding a New Endpoint

1. **Create the endpoint** in the appropriate controller:
```csharp
[HttpGet("example")]
public async Task<IActionResult> GetExample()
{
    return Ok("Example response");
}
```

2. **Test via Swagger**: Navigate to the service's Swagger UI
3. **Test via Gateway**: Verify routing through Ocelot
4. **Update documentation**: Add to `.http` file or documentation

### Creating a New Migration

```powershell
# Add migration
dotnet ef migrations add MigrationName --project UserMicroservices

# Apply migration
dotnet ef database update --project UserMicroservices
```

### Building the Solution

```powershell
# Build all projects
dotnet build MoneyTransferMicroservices.sln

# Build specific project
dotnet build UserMicroservices/UserMicroservices.csproj
```

## 🎯 Code Quality

This project enforces strict code quality standards via `Directory.Build.props`:

- ✅ **Nullable reference types** enabled
- ✅ **Warnings treated as errors**
- ✅ **Code style enforcement** during build
- ✅ **.NET analyzers** enabled
- ✅ **Code optimization** in Release mode
- ✅ **Unused code trimming** in Release mode

## 🐛 Troubleshooting

### Port Already in Use

```powershell
# Find process using port
netstat -ano | findstr :7001

# Kill process
taskkill /PID <process_id> /F
```

### Database Connection Issues

1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure database exists (run migrations)
4. Test connection with SQL Server Management Studio

### Service Not Responding

1. Check service logs in the terminal/PowerShell window
2. Verify port configuration in `Properties/launchSettings.json`
3. Ensure no firewall is blocking the port
4. Check for compilation errors: `dotnet build`

### Gateway 404 Errors

1. Verify the downstream service is running
2. Check `ocelot.json` route configuration
3. Ensure ports match in both `ocelot.json` and service `launchSettings.json`

## 🚧 Future Enhancements

- [ ] **JWT Authentication** - Secure API endpoints
- [ ] **Docker Support** - Containerize microservices
- [ ] **Message Queue** - RabbitMQ/Azure Service Bus integration
- [ ] **Health Checks** - Service monitoring endpoints
- [ ] **Logging** - Structured logging with Serilog
- [ ] **Unit Tests** - Comprehensive test coverage
- [ ] **CI/CD Pipeline** - Automated build and deployment
- [ ] **API Versioning** - Support multiple API versions
- [ ] **Rate Limiting** - Protect against abuse
- [ ] **Caching** - Redis for performance optimization

## 📖 Documentation

- **Copilot Instructions**: See `.github/copilot-instructions.md` for detailed coding standards
- **API Documentation**: Available via Swagger UI when running in Development mode
- **Architecture Guide**: Refer to the copilot instructions for architecture patterns

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/YourFeature`
3. Commit changes: `git commit -m '[ServiceName] Add feature'`
4. Push to branch: `git push origin feature/YourFeature`
5. Open a Pull Request

Please follow the coding standards outlined in `.github/copilot-instructions.md`.

## 📝 License

This project is provided as-is for educational and development purposes.

## 👥 Authors

- **Repository Owner**: [jradityamishra](https://github.com/jradityamishra)

## 🔗 Resources

- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Ocelot Gateway](https://ocelot.readthedocs.io/)
- [Microservices Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/)

---

**Last Updated**: November 27, 2025