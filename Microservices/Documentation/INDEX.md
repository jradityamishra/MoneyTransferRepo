# ?? Complete Documentation Index

## ?? Quick Navigation

### For Testing & API Usage
- **[Postman Quick Start Guide](Postman_Quick_Start.md)** - Step-by-step guide to test the APIs
- **[API Documentation](API_Documentation.md)** - Complete API reference with all endpoints
- **[README](README.md)** - Overview of all testing resources

### For Automation & CI/CD
- **[Newman CLI Guide](Newman_CLI_Guide.md)** - Command-line testing and automation

### For Troubleshooting
- **[Authentication Fix Guide](Authentication_Fix_Guide.md)** - Resolving 401 Unauthorized errors
- **[Transaction Balance Fix Guide](Transaction_Balance_Fix_Guide.md)** - Resolving 400 Bad Request on transfers

---

## ?? Files & Resources

### Postman Collection Files
Located in: `Postman/`

| File | Description | Usage |
|------|-------------|-------|
| `MoneyTransfer_Microservices.postman_collection.json` | Complete API collection with 35+ requests | Import into Postman |
| `MoneyTransfer_Development.postman_environment.json` | Environment variables for development | Import into Postman |

### Documentation Files
Located in: `Documentation/`

| File | Purpose | Target Audience |
|------|---------|-----------------|
| `README.md` | Overview and navigation | Everyone |
| `API_Documentation.md` | Complete API reference | Developers, QA |
| `Postman_Quick_Start.md` | Step-by-step testing guide | Testers, QA |
| `Newman_CLI_Guide.md` | Automation and CI/CD | DevOps, Automation Engineers |
| `Authentication_Fix_Guide.md` | Troubleshooting 401 errors | Developers |
| `Transaction_Balance_Fix_Guide.md` | Troubleshooting 400 errors on transfers | Developers |

---

## ?? Getting Started (30 seconds)

### 1. Import to Postman
```
File ? Import ? Select files:
  - Postman/MoneyTransfer_Microservices.postman_collection.json
  - Postman/MoneyTransfer_Development.postman_environment.json
```

### 2. Select Environment
```
Top-right dropdown ? "Money Transfer - Development"
```

### 3. Run First Test
```
1. Authentication ? Register User ? Send
2. Authentication ? Login ? Send (saves token automatically)
3. Account Management ? Create Account ? Send
4. Transactions ? Initiate Transfer ? Send
```

---

## ?? Documentation Guide

### For First-Time Users
**Start Here:** `Postman_Quick_Start.md`

This guide walks you through:
- ? Importing the collection
- ? Starting the services
- ? Running your first requests
- ? Understanding the test flow
- ? Common issues and solutions

### For API Integration
**Read:** `API_Documentation.md`

Comprehensive reference including:
- ??? Architecture diagrams
- ?? All API endpoints with examples
- ?? Request/response formats
- ?? Error codes and handling
- ?? Authentication flow
- ?? Best practices

### For Test Automation
**See:** `Newman_CLI_Guide.md`

Learn how to:
- ?? Run tests from command line
- ?? Integrate with CI/CD pipelines
- ?? Generate test reports
- ?? Use Docker for testing
- ?? Configure GitHub Actions, Azure DevOps, Jenkins

### For Troubleshooting
**Check:** `Authentication_Fix_Guide.md`

Covers:
- ?? 401 Unauthorized errors
- ?? JWT token forwarding
- ??? Microservices authentication
- ? Testing the fix
- ?? Security best practices

---

## ?? Learning Path

### Level 1: Beginner
1. Read `Postman_Quick_Start.md`
2. Import Postman collection
3. Run basic authentication flow
4. Create accounts and test transfers

### Level 2: Intermediate
1. Review `API_Documentation.md`
2. Test all endpoints
3. Understand error scenarios
4. Use Collection Runner

### Level 3: Advanced
1. Study `Newman_CLI_Guide.md`
2. Set up automation
3. Integrate with CI/CD
4. Performance testing

---

## ??? Architecture Overview

```
????????????????????????????????????????????????
?         Client Applications                   ?
?    (Postman, Web, Mobile, Desktop)           ?
????????????????????????????????????????????????
                    ?
                    ?
????????????????????????????????????????????????
?       Ocelot API Gateway (Port: 7000)        ?
?  Routes: /auth, /account, /transaction       ?
????????????????????????????????????????????????
        ?           ?           ?          ?
    ?????           ?           ?          ??????
    ?               ?           ?               ?
??????????    ???????????  ????????????  ????????????
?  User  ?    ? Account ?  ?Transaction? ?Notification?
? :7001  ?    ?  :7002  ?  ?  :7003    ? ?  :7005    ?
??????????    ???????????  ????????????  ????????????
```

---

## ?? Key Features

### 1. Automatic Token Management
- Login automatically saves JWT token
- All subsequent requests use saved token
- No manual copy-paste needed

### 2. Comprehensive Test Coverage
- 35+ API endpoints
- 5 microservices
- All CRUD operations
- Error scenarios included

### 3. Built-in Validation
Every request includes test scripts:
- ? Response time validation
- ? Status code checking
- ? Data integrity verification
- ? Auto-save important values

### 4. Production-Ready
- Complete documentation
- CI/CD integration examples
- Security best practices
- Error handling patterns

---

## ?? Collection Structure

```
Money Transfer Microservices Collection
?
??? 1. Authentication (3 requests)
?   ??? Test Auth Service
?   ??? Register User
?   ??? Login
?
??? 2. User Management (10 requests)
?   ??? Get All Users
?   ??? Get User By ID
?   ??? Create/Update/Delete User
?   ??? Change Password
?   ??? Role Management
?
??? 3. Account Management (11 requests)
?   ??? CRUD Operations
?   ??? Balance Management
?   ??? Status Management
?   ??? Get Balance
?
??? 4. Transactions (5 requests)
?   ??? Initiate Transfer
?   ??? Get Transaction
?   ??? Transaction History
?   ??? Filtered Transactions
?   ??? Cancel Transaction
?
??? 5. Notifications (1 request)
    ??? Test Notification Service
```

---

## ?? Test Scenarios

### ? Included in Collection

1. **Basic Flow**
   - User registration
   - Login and authentication
   - Account creation
   - Money transfer
   - Transaction history

2. **Error Handling**
   - Insufficient balance
   - Invalid account
   - Unauthorized access
   - Invalid data

3. **Account Management**
   - Balance updates
   - Account locking/unlocking
   - Status changes

4. **User Management**
   - Password changes
   - Role assignment
   - User updates

---

## ?? Environment Variables

### Automatically Managed

| Variable | Description | Auto-saved? |
|----------|-------------|-------------|
| `auth_token` | JWT authentication token | ? After login |
| `refresh_token` | Token refresh | ? After login |
| `user_id` | Current user ID | ? After login/register |
| `account_number` | Account number | ? After account creation |
| `from_account` | Source account | ? After first account |
| `to_account` | Destination account | ? After second account |
| `transaction_id` | Transaction ID | ? After transfer |

### Pre-configured

| Variable | Value | Purpose |
|----------|-------|---------|
| `gateway_url` | https://localhost:7000 | API Gateway endpoint |
| `user_service_url` | https://localhost:7001 | User service |
| `account_service_url` | https://localhost:7002 | Account service |
| `transaction_service_url` | https://localhost:7003 | Transaction service |
| `notification_service_url` | https://localhost:7005 | Notification service |

---

## ?? Common Issues & Solutions

### Issue: 401 Unauthorized
**Solution:** Run Login request to get fresh token
**Details:** See `Authentication_Fix_Guide.md`

### Issue: Service not responding
**Solution:** Verify all 5 services are running
**Check:** Ports 7000, 7001, 7002, 7003, 7005

### Issue: SSL Certificate error
**Solution:** Disable SSL verification in Postman (development only)
**Path:** Settings ? General ? SSL certificate verification

### Issue: User already exists
**Solution:** Use different email or login with existing credentials

### Issue: 400 Bad Request on Transfer
**Solution:** Ensure sender has sufficient balance and accounts are active
**Details:** See `Transaction_Balance_Fix_Guide.md`

---

## ?? Testing Metrics

### Response Time Validation
All requests validate: Response time < 5000ms

### Coverage
- **Services**: 5/5 (100%)
- **Endpoints**: 35+ covered
- **HTTP Methods**: GET, POST, PUT, DELETE
- **Auth Scenarios**: Bearer token authentication

---

## ?? Security Notes

### Development Environment
- ? LocalDB with trusted connection
- ? Self-signed SSL certificates
- ? JWT tokens with 24-hour expiration
- ? Test data included

### Production Checklist
- ?? Use proper SSL certificates
- ?? Store secrets in Azure Key Vault
- ?? Implement rate limiting
- ?? Enable CORS properly
- ?? Use strong JWT secrets
- ?? Implement logging and monitoring

---

## ?? Support & Resources

### Getting Help
1. Check the troubleshooting section in each document
2. Review service logs in `Logs/` directories
3. Check database connectivity
4. Verify all services are running

### Useful Links
- Postman Documentation: https://learning.postman.com/
- Newman CLI: https://github.com/postmanlabs/newman
- Ocelot Gateway: https://ocelot.readthedocs.io/
- .NET 8 Documentation: https://docs.microsoft.com/en-us/dotnet/

---

## ?? Quick Reference Card

### Essential Commands

**Start Testing:**
```bash
# Import collection to Postman
# Select environment
# Run: Authentication ? Login
```

**Run from CLI:**
```bash
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json
```

**Start Services:**
```bash
dotnet run (in each service directory)
```

### Essential URLs

| Purpose | URL |
|---------|-----|
| API Gateway | https://localhost:7000 |
| User Service | https://localhost:7001 |
| Account Service | https://localhost:7002 |
| Transaction Service | https://localhost:7003 |
| Notification Service | https://localhost:7005 |

### Essential Endpoints

| Action | Endpoint |
|--------|----------|
| Register | POST /auth/register-user |
| Login | POST /auth/login |
| Create Account | POST /account |
| Transfer Money | POST /transaction/transfer |
| Get Transactions | GET /transaction/account/{id} |

---

## ?? Checklist

### Before Testing
- [ ] All 5 services running
- [ ] Postman collection imported
- [ ] Environment selected
- [ ] Database connections working

### First Test Run
- [ ] Register user
- [ ] Login (token saved)
- [ ] Create two accounts
- [ ] Initiate transfer
- [ ] Verify balances

### After Testing
- [ ] Review test results
- [ ] Check service logs
- [ ] Verify database records
- [ ] Document any issues

---

## ?? You're All Set!

Choose your starting point:
- **New to the project?** ? Start with `Postman_Quick_Start.md`
- **Need API details?** ? Check `API_Documentation.md`
- **Setting up automation?** ? Read `Newman_CLI_Guide.md`
- **Facing auth issues?** ? See `Authentication_Fix_Guide.md`
- **Trouble with transfers?** ? Consult `Transaction_Balance_Fix_Guide.md`

---

**Version**: 1.0  
**Last Updated**: 2024  
**Framework**: .NET 8  
**Gateway**: Ocelot  

**Happy Testing! ??**
