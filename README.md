# Money Transfer Microservices System

A comprehensive microservices-based money transfer application built with .NET 8, featuring user management, account operations, transactions, and notifications.

## 🏗️ Architecture

This solution consists of 5 microservices orchestrated through an API Gateway:

- **UserMicroservices** - User authentication and management
- **AccountMicroservices** - Account creation and management
- **TransactionMicroservices** - Money transfer and transaction processing
- **OcelotApiGateway** - API Gateway for routing and orchestration

## 🚀 Technology Stack

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API development
- **Entity Framework Core 8.0** - ORM for database operations
- **SQL Server** - Database management system
- **Ocelot** - API Gateway
- **Swagger/OpenAPI** - API documentation
- **C# 12.0** - Programming language

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or higher)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- Git

## 🔧 Installation & Setup

### 1. Clone the Repository

git clone <repository-url> cd MoneyTransferRepo


### 2. Configure Database Connection Strings

Update the `appsettings.json` file in each microservice with your SQL Server connection string:

{ "ConnectionStrings": { "DefaultConnection": "Server=localhost;Database=<DatabaseName>;Trusted_Connection=True;TrustServerCertificate=True" } }

**Database Names:**
- UserMicroservices: `UserDB`
- AccountMicroservices: `AccountDB`
- TransactionMicroservices: `TransactionDB`
- NotificationMicroservices: `NotificationDB`

### 3. Run Database Migrations

For each microservice with a database context, run:
add-migration "COMMENT"
update-database


### 4. Configure Multiple Startup Projects in Visual Studio

1. Right-click on the solution
2. Select **"Set Startup Projects..."**
3. Choose **"Multiple startup projects"**
4. Set all 5 projects to **"Start"**:
   - UserMicroservices
   - AccountMicroservices
   - TransactionMicroservices
   - NotificationMicroservices
   - OcelotApiGateway

### 5. Run the Application

Press `F5` in Visual Studio or run:
dotnet run --project UserMicroservices dotnet run --project AccountMicroservices dotnet run --project TransactionMicroservices dotnet run --project NotificationMicroservices dotnet run --project OcelotApiGateway


## 🌐 Service Endpoints & Port Configuration

### API Gateway

| Protocol | URL | Port | Description |
|----------|-----|------|-------------|
| HTTP | `http://localhost:5000` | 5000 | Gateway HTTP endpoint |
| HTTPS | `https://localhost:7000` | 7000 | Gateway HTTPS endpoint |

### UserMicroservices (Authentication & User Management)

| Protocol | URL | Port | IIS Express Port | SSL Port | Description |
|----------|-----|------|------------------|----------|-------------|
| HTTP | `http://localhost:5001` | 5001 | 48532 | - | User service HTTP |
| HTTPS | `https://localhost:7001` | 7001 | - | 44328 | User service HTTPS |
| Swagger | `http://localhost:5001/swagger` | 5001 | - | - | API documentation |

### AccountMicroservices (Account Management)

| Protocol | URL | Port | IIS Express Port | SSL Port | Description |
|----------|-----|------|------------------|----------|-------------|
| HTTP | `http://localhost:5002` | 5002 | 60880 | - | Account service HTTP |
| HTTPS | `https://localhost:7002` | 7002 | - | 44305 | Account service HTTPS |
| Swagger | `http://localhost:5002/swagger` | 5002 | - | - | API documentation |

### TransactionMicroservices (Money Transfer & Transactions)

| Protocol | URL | Port | IIS Express Port | SSL Port | Description |
|----------|-----|------|------------------|----------|-------------|
| HTTP | `http://localhost:5003` | 5003 | 11432 | - | Transaction service HTTP |
| HTTPS | `https://localhost:7003` | 7003 | - | 44367 | Transaction service HTTPS |
| Swagger | `http://localhost:5003/swagger` | 5003 | - | - | API documentation |

### NotificationMicroservices (User Notifications)

| Protocol | URL | Port | IIS Express Port | SSL Port | Description |
|----------|-----|------|------------------|----------|-------------|
| HTTP | `http://localhost:5005` | 5005 | 57134 | - | Notification service HTTP |
| HTTPS | `https://localhost:7005` | 7005 | - | 44310 | Notification service HTTPS |
| Swagger | `http://localhost:5005/swagger` | 5005 | - | - | API documentation |

### OcelotApiGateway (API Gateway)

| Protocol | URL | Port | IIS Express Port | SSL Port | Description |
|----------|-----|------|------------------|----------|-------------|
| HTTP | `http://localhost:5000` | 5000 | 57901 | - | Gateway HTTP |
| HTTPS | `https://localhost:7000` | 7000 | - | 44783 | Gateway HTTPS |

## 🔌 API Gateway Routes

Access all microservices through the API Gateway using these routes:

### Gateway Route Mappings

| Gateway URL | UpStreamtream Service | Downstream Port (HTTPS) | Description |
|-------------|-------------------|------------------------|-------------|
| `https://localhost:7000/auth/*` | UserMicroservices | 7001 | User authentication & management |
| `https://localhost:7000/account/*` | AccountMicroservices | 7002 | Account operations |
| `https://localhost:7000/transaction/*` | TransactionMicroservices | 7003 | Money transfers & transactions |
| `https://localhost:7000/notification/*` | NotificationMicroservices | 7005 | User notifications |

### Example API Calls
Via API Gateway (Recommended)
curl https://localhost:7000/api/auth/login curl https://localhost:7000/api/account/balance curl https://localhost:7000/api/transaction/transfer curl https://localhost:7000/api/notification/status
Direct Access (Development/Testing)
curl https://localhost:7001/api/auth/login curl https://localhost:7002/api/account/balance curl https://localhost:7003/api/transaction/transfer curl https://localhost:7005/api/notification/status

## 📊 Port Summary Table

| Service | HTTP Port | HTTPS Port | IIS Express | SSL Port |
|---------|-----------|------------|-------------|----------|
| **OcelotApiGateway** | 5000 | 7000 | 57901 | 44783 |
| **UserMicroservices** | 5001 | 7001 | 48532 | 44328 |
| **AccountMicroservices** | 5002 | 7002 | 60880 | 44305 |
| **TransactionMicroservices** | 5003 | 7003 | 11432 | 44367 |
| **NotificationMicroservices** | 5005 | 7005 | 57134 | 44310 |

> **Note:** Port 5004 is intentionally skipped to avoid previous conflicts.

## 📁 Project Structure

## 🔐 Security Considerations

- Enable HTTPS for production environments
- Implement authentication and authorization (JWT recommended)
- Use API keys or OAuth for gateway access
- Encrypt sensitive data in configuration files
- Apply rate limiting to prevent abuse

## 🧪 Testing

### Manual Testing with Swagger

Navigate to Swagger UI for each service:

- User Service: `http://localhost:5001/swagger`
- Account Service: `http://localhost:5002/swagger`
- Transaction Service: `http://localhost:5003/swagger`
- Notification Service: `http://localhost:5005/swagger`

### Testing via API Gateway

Use tools like Postman, curl, or any HTTP client:

