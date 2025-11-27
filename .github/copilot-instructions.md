# MoneyTransferRepo - GitHub Copilot Instructions

## Project Overview

This is a **microservices-based money transfer system** built with **.NET 8.0**, implementing a distributed architecture pattern with an API Gateway (Ocelot) as the single entry point for all client requests.

### Architecture Pattern
- **Microservices Architecture**: Independent, loosely-coupled services
- **API Gateway Pattern**: Centralized routing and load balancing via Ocelot
- **Database-per-Service Pattern**: Each microservice maintains its own database
- **RESTful APIs**: HTTP/HTTPS communication between services

---

## Microservices Components

### 1. **UserMicroservices** (Port 7001)
- **Responsibility**: User authentication, registration, and profile management
- **Database**: `UserDatabase`
- **Key Endpoints**: `/api/auth/*`
- **Gateway Route**: `/auth/*`

### 2. **AccountMicroservices** (Port 7002)
- **Responsibility**: Bank account creation, management, and balance tracking
- **Database**: `AccountDatabase`
- **Key Endpoints**: `/api/account/*`
- **Gateway Route**: `/accounts/*`

### 3. **TransactionMicroservices** (Port 7003)
- **Responsibility**: Money transfer processing, transaction history, and validation
- **Database**: `TransactionDatabase`
- **Key Endpoints**: `/api/transaction/*`
- **Gateway Route**: `/transaction/*`

### 4. **NotificationMicroservices** (Port 7005)
- **Responsibility**: Email/SMS notifications for transactions and account activities
- **Database**: `NotificationDatabase`
- **Key Endpoints**: `/api/notification/*`
- **Gateway Route**: `/notification/*`

### 5. **OcelotApiGateway** (Port 7000)
- **Responsibility**: Request routing, load balancing, and API composition
- **Configuration**: `ocelot.json`
- **Base URL**: `https://localhost:7000`

---

## Technology Stack

### Backend
- **.NET 8.0** - Target framework
- **ASP.NET Core Web API** - RESTful services
- **Entity Framework Core 8.0** - ORM for database operations
- **SQL Server** - Relational database (LocalDB for development)
- **Ocelot** - API Gateway library
- **Swagger/OpenAPI** - API documentation (available in Development mode)

### Tools & Libraries
- **Microsoft.EntityFrameworkCore.SqlServer** - SQL Server provider
- **Microsoft.EntityFrameworkCore.Tools** - Migrations and scaffolding
- **Swashbuckle.AspNetCore** - Swagger UI generation

---

## Project Structure

```
MoneyTransferRepo/
├── Microservices/
│   ├── UserMicroservices/
│   │   ├── Controllers/        # API endpoints (AuthController)
│   │   ├── Data/               # DatabaseContext
│   │   ├── Model/
│   │   │   ├── DTO/           # Data Transfer Objects
│   │   │   └── Entity/        # Database entities
│   │   ├── Migrations/        # EF Core migrations
│   │   └── Program.cs         # Application entry point
│   │
│   ├── AccountMicroservices/
│   │   ├── Controllers/        # API endpoints (AccountController)
│   │   ├── Data/               # DatabaseContext
│   │   └── Program.cs
│   │
│   ├── TransactionMicroservices/
│   │   ├── Controllers/        # API endpoints
│   │   ├── Data/               # DatabaseContext
│   │   └── Program.cs
│   │
│   ├── NotificationMicroservices/
│   │   ├── Controllers/        # API endpoints (NotificationController)
│   │   ├── Data/               # DatabaseContext
│   │   └── Program.cs
│   │
│   ├── OcelotApiGateway/
│   │   ├── ocelot.json        # Route configuration
│   │   └── Program.cs
│   │
│   ├── MoneyTransferMicroservices.sln  # Solution file
│   └── StartAllServices.ps1   # PowerShell script to launch all services
```

---

## Coding Standards & Best Practices

### General Guidelines
- **Follow C# naming conventions**: PascalCase for classes/methods, camelCase for parameters/variables
- **Use async/await**: All I/O operations should be asynchronous
- **Dependency Injection**: Use constructor injection for all dependencies
- **Null safety**: Nullable reference types are enabled (`<Nullable>enable</Nullable>`)
- **SOLID principles**: Single responsibility, dependency inversion, interface segregation

### API Controller Standards
```csharp
[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    // Use dependency injection
    private readonly DatabaseContext _context;
    
    public ExampleController(DatabaseContext context)
    {
        _context = context;
    }
    
    // Use async/await
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _context.Examples.FindAsync(id);
        return result == null ? NotFound() : Ok(result);
    }
}
```

### Entity/Model Standards
- Use **Data Annotations** for validation and schema definition
- Create separate **DTO classes** for API requests/responses
- Entity classes in `Model/Entity/` directory
- DTOs in `Model/DTO/` directory

### Database Context Standards
```csharp
public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) 
        : base(options) { }
    
    // DbSets for entities
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity relationships and constraints
        base.OnModelCreating(modelBuilder);
    }
}
```

### Configuration Management
- **Development settings**: `appsettings.Development.json`
- **Production settings**: `appsettings.json`
- **Connection strings**: Always use configuration, never hardcode
- **Secrets**: Use User Secrets for sensitive data (not in source control)

### Error Handling
- Use proper HTTP status codes (200, 201, 400, 404, 500)
- Return meaningful error messages
- Log exceptions appropriately
- Use try-catch blocks for external operations

### Code Quality Settings (Directory.Build.props)
All projects enforce:
- `<Nullable>enable</Nullable>` - Nullable reference types
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` - Fail build on warnings
- `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` - Code style validation
- `<EnableNETAnalyzers>true</EnableNETAnalyzers>` - Static code analysis
- `<Trim>true</Trim>` - Trim unused code (Release mode)
- `<Optimize>true</Optimize>` - Code optimization (Release mode)

---

## Database Configuration

### Connection Strings Format
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database={ServiceName}Database;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Databases
- **UserDatabase** - User authentication and profiles
- **AccountDatabase** - Bank accounts and balances
- **TransactionDatabase** - Transaction records
- **NotificationDatabase** - Notification logs and templates

### Entity Framework Migrations
```bash
# Create migration
dotnet ef migrations add MigrationName --project {ServiceName}

# Update database
dotnet ef database update --project {ServiceName}
```

---

## API Gateway Routing (Ocelot)

### Route Pattern
```json
{
  "DownstreamPathTemplate": "/api/{controller}/{everything}",
  "DownstreamScheme": "https",
  "DownstreamHostAndPorts": [{"Host": "localhost", "Port": 7001}],
  "UpstreamPathTemplate": "/{controller}/{everything}",
  "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE"]
}
```

### Gateway Access
- **Base URL**: `https://localhost:7000`
- **Example**: `GET https://localhost:7000/auth/login` → Routes to `https://localhost:7001/api/auth/login`

---

## Development Workflow

### Running the Application
1. **Option 1 - PowerShell Script** (Recommended):
   ```powershell
   cd Microservices
   .\StartAllServices.ps1
   ```

2. **Option 2 - Individual Services**:
   ```powershell
   dotnet run --project UserMicroservices
   dotnet run --project AccountMicroservices
   dotnet run --project TransactionMicroservices
   dotnet run --project NotificationMicroservices
   dotnet run --project OcelotApiGateway
   ```

3. **Option 3 - Visual Studio**:
   - Open `MoneyTransferMicroservices.sln`
   - Configure multiple startup projects
   - Press F5 to debug all services

### Testing APIs
- **Swagger UI**: Each service has Swagger at `https://localhost:{port}/swagger` (Development only)
- **HTTP Files**: Use `.http` files in each project for testing
- **Gateway**: Test through Ocelot at `https://localhost:7000`

### Adding New Endpoints
1. Create controller method in appropriate service
2. Test endpoint via Swagger/HTTP file
3. Verify routing through API Gateway
4. Update documentation

---

## Common Patterns to Follow

### Controller Action Pattern
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var entity = new Entity { /* map from dto */ };
    _context.Entities.Add(entity);
    await _context.SaveChangesAsync();
    
    return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
}
```

### Service Layer Pattern (Recommended)
- Create `Services/` folder in each microservice
- Implement business logic in service classes
- Controllers should only handle HTTP concerns
- Inject services via dependency injection

### Repository Pattern (Optional)
- Abstract data access in repository interfaces
- Implement repository classes for each entity
- Simplifies testing and maintenance

---

## Security Considerations

### Current State
- HTTPS enabled (UseHttpsRedirection)
- Authorization middleware registered
- TrustServerCertificate enabled for development

### Recommendations for Production
- Implement JWT authentication in UserMicroservices
- Add authorization policies to controllers
- Enable CORS with specific origins
- Use API keys or OAuth for gateway access
- Implement rate limiting
- Add request validation and sanitization
- Use environment-specific connection strings
- Implement health checks for all services

---

## Testing Guidelines

### Unit Testing
- Test business logic in service classes
- Mock DbContext using InMemory database
- Test validation logic
- Target 80%+ code coverage

### Integration Testing
- Test API endpoints end-to-end
- Use WebApplicationFactory
- Test database operations with test database
- Test service-to-service communication

### API Testing
- Use HTTP files for manual testing
- Test all CRUD operations
- Verify error handling
- Test gateway routing

---

## Deployment Considerations

### Container Orchestration (Future)
- Each microservice can be containerized with Docker
- Use Docker Compose for local development
- Kubernetes for production orchestration

### Service Discovery (Future Enhancement)
- Consider Consul or Eureka for dynamic service discovery
- Replace hardcoded ports with service registry

### Monitoring & Logging
- Implement structured logging (Serilog recommended)
- Add application insights or similar monitoring
- Implement distributed tracing (OpenTelemetry)
- Add health check endpoints

---

## Contribution Guidelines

### Before Submitting Code
1. Ensure all services build without warnings
2. Run and pass all unit tests
3. Test endpoints through Swagger and Gateway
4. Update migrations if database schema changed
5. Follow coding standards outlined above
6. Add XML documentation comments for public APIs

### Commit Message Format
```
[ServiceName] Brief description of change

Detailed explanation if needed
- Bullet points for multiple changes
- Reference issue numbers if applicable
```

### Pull Request Checklist
- [ ] Code builds without warnings/errors
- [ ] All tests pass
- [ ] API endpoints tested via Swagger
- [ ] Gateway routing verified
- [ ] Database migrations created (if needed)
- [ ] Documentation updated (if needed)

---

## Troubleshooting

### Common Issues

**Port Already in Use**
- Check for running instances: `netstat -ano | findstr :{port}`
- Kill process: `taskkill /PID {pid} /F`

**Database Connection Failed**
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database exists (run migrations)

**Service Not Responding**
- Check service logs in terminal
- Verify correct port in launchSettings.json
- Ensure no firewall blocking

**Gateway 404 Errors**
- Verify ocelot.json routing configuration
- Ensure downstream service is running
- Check downstream service port matches configuration

---

## Future Enhancements

### Planned Features
- [ ] JWT authentication and authorization
- [ ] Message queue integration (RabbitMQ/Azure Service Bus)
- [ ] CQRS pattern for complex operations
- [ ] Event sourcing for transaction history
- [ ] Redis caching for performance
- [ ] Docker containerization
- [ ] CI/CD pipeline setup
- [ ] Comprehensive unit and integration tests
- [ ] API versioning strategy
- [ ] Rate limiting and throttling

---

## Resources

### Documentation
- [ASP.NET Core Web APIs](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Ocelot Gateway](https://ocelot.readthedocs.io/)
- [Microservices Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/)

### Team Contacts
- **Repository Owner**: jradityamishra
- **Repository**: MoneyTransferRepo

---

*Last Updated: November 27, 2025*
