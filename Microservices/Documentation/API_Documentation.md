# Money Transfer Microservices - API Documentation

## Table of Contents
- [Overview](#overview)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Authentication](#authentication)
- [API Endpoints](#api-endpoints)
  - [Authentication Service](#authentication-service)
  - [User Management Service](#user-management-service)
  - [Account Management Service](#account-management-service)
  - [Transaction Service](#transaction-service)
  - [Notification Service](#notification-service)
- [Error Handling](#error-handling)
- [Postman Collection](#postman-collection)
- [Examples](#examples)

## Overview

This is a microservices-based money transfer application built with .NET 8. The application uses Ocelot API Gateway to route requests to different microservices.

### Technologies Used
- **.NET 8** - Framework
- **Ocelot** - API Gateway
- **JWT** - Authentication
- **Serilog** - Logging
- **Entity Framework Core** - ORM
- **SQL Server LocalDB** - Database

## Architecture

### Microservices Architecture Diagram

```
???????????????????????????????????????????????????
?           Client Applications                    ?
?     (Web, Mobile, Desktop, Postman)             ?
???????????????????????????????????????????????????
                      ?
                      ?
???????????????????????????????????????????????????
?         Ocelot API Gateway                       ?
?         (Port: 7000)                             ?
???????????????????????????????????????????????????
          ?           ?           ?          ?
    ???????           ?           ?          ???????
    ?                 ?           ?                ?
???????????     ???????????? ????????????  ????????????
?  User   ?     ? Account  ? ?Transaction? ?Notification?
? Service ?     ? Service  ? ? Service   ? ?  Service  ?
?  :7001  ?     ?  :7002   ? ?  :7003    ? ?  :7005    ?
???????????     ???????????? ????????????  ????????????
    ?               ?             ?              ?
    ?               ?             ?              ?
???????????     ???????????? ????????????  ????????????
? UserDb  ?     ?AccountDb ? ?Transaction? ?   Email   ?
?         ?     ?          ? ?    Db     ? ?   / SMS   ?
???????????     ???????????? ????????????  ????????????
```

### Service Ports

| Service | Port | Base URL |
|---------|------|----------|
| API Gateway | 7000 | https://localhost:7000 |
| User Service | 7001 | https://localhost:7001 |
| Account Service | 7002 | https://localhost:7002 |
| Transaction Service | 7003 | https://localhost:7003 |
| Notification Service | 7005 | https://localhost:7005 |

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB
- Postman (for testing)

### Running the Application

1. **Start all microservices:**
   ```bash
   # User Service
   cd UserMicroservices
   dotnet run

   # Account Service
   cd AccountMicroservices
   dotnet run

   # Transaction Service
   cd TransactionMicroservices
   dotnet run

   # Notification Service
   cd NotificationMicroservices
   dotnet run

   # API Gateway
   cd OcelotApiGateway
   dotnet run
   ```

2. **Database Setup:**
   - The databases will be auto-created on first run
   - Connection strings are configured in appsettings.json
   - LocalDB is used: `(localdb)\\MSSQLLocalDB`

### Base URL
All API requests should be made through the API Gateway:
```
https://localhost:7000
```

## Authentication

The application uses **JWT (JSON Web Tokens)** for authentication.

### Authentication Flow

1. **Register** a new user
2. **Login** to get JWT token
3. **Include token** in subsequent requests

### Token Usage
Add the JWT token to the Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

### Token Expiration
- Tokens expire after **1440 minutes (24 hours)**
- Refresh tokens expire after **6 months**

### Roles
- **User**: Standard user with access to their own accounts
- **Admin**: Administrative access (if needed)

---

## API Endpoints

### Authentication Service

Base Path: `/auth`

#### 1. Test Auth Service
**GET** `/auth`

Test if the authentication service is running.

**Response:**
```json
"User Microservice is working!"
```

---

#### 2. Register User
**POST** `/auth/register-user`

Register a new user in the system.

**Request Body:**
```json
{
  "Username": "john_doe",
  "Email": "john.doe@example.com",
  "Password": "Password@123",
  "Role": "User"
}
```

**Field Descriptions:**
- `Username` (required): Unique username
- `Email` (required): Valid email address
- `Password` (required): Strong password
- `Role` (required): "User" or "Admin"

**Success Response (200):**
```json
{
  "Status": "Success",
  "Message": "User created successfully!"
}
```

**Error Responses:**
- **500**: User already exists
- **500**: User creation failed

---

#### 3. Login
**POST** `/auth/login`

Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "Email": "john.doe@example.com",
  "Password": "Password@123"
}
```

**Success Response (200):**
```json
{
  "token": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "guid-guid",
    "expiresAt": "2024-12-25T10:00:00Z"
  },
  "user": {
    "id": "user-guid",
    "userName": "john_doe",
    "email": "john.doe@example.com"
  }
}
```

**Error Response (401):**
```
Unauthorized
```

---

#### 4. Check User Exists
**GET** `/auth/check-user/{id}`

Check if a user exists by ID.

**Parameters:**
- `id` (path): User ID

**Response:**
```json
{
  "Exists": true
}
```

---

### User Management Service

Base Path: `/user`

**Authorization Required**: Bearer Token

#### 1. Get All Users
**GET** `/user/get-all-users`

Retrieve all users in the system.

**Success Response (200):**
```json
[
  {
    "id": "user-guid",
    "userName": "john_doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890",
    "roles": ["User"]
  }
]
```

---

#### 2. Get User By ID
**GET** `/user/user-by-{id}`

Get user details by user ID.

**Parameters:**
- `id` (path): User ID

**Success Response (200):**
```json
{
  "id": "user-guid",
  "userName": "john_doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "roles": ["User"]
}
```

**Error Response (404):**
```json
{
  "message": "User not found"
}
```

---

#### 3. Get User By Email
**GET** `/user/user-by-email?email={email}`

Get user by email address.

**Query Parameters:**
- `email` (required): User's email address

**Success Response (200):**
```json
{
  "id": "user-guid",
  "userName": "john_doe",
  "email": "john.doe@example.com"
}
```

---

#### 4. Create User
**POST** `/user/create-user`

Create a new user (Admin function).

**Request Body:**
```json
{
  "UserName": "jane_doe",
  "Email": "jane.doe@example.com",
  "Password": "Password@123",
  "PhoneNumber": "+1234567890"
}
```

**Success Response (201):**
```json
{
  "id": "user-guid",
  "userName": "jane_doe",
  "email": "jane.doe@example.com"
}
```

---

#### 5. Update User
**PUT** `/user/update-user/{id}`

Update user information.

**Parameters:**
- `id` (path): User ID

**Request Body:**
```json
{
  "UserName": "john_doe_updated",
  "Email": "john.updated@example.com",
  "PhoneNumber": "+9876543210"
}
```

**Success Response (204):**
```
No Content
```

---

#### 6. Delete User
**DELETE** `/user/delete-user/{id}`

Delete a user from the system.

**Parameters:**
- `id` (path): User ID

**Success Response (204):**
```
No Content
```

---

#### 7. Change Password
**POST** `/user/change-password`

Change user password.

**Request Body:**
```json
{
  "UserId": "user-guid",
  "CurrentPassword": "Password@123",
  "NewPassword": "NewPassword@123"
}
```

**Success Response (200):**
```json
{
  "message": "Password changed"
}
```

---

#### 8. Reset Password
**POST** `/user/reset-password`

Reset user password using token.

**Request Body:**
```json
{
  "Email": "john.doe@example.com",
  "Token": "reset_token_here",
  "NewPassword": "NewPassword@123"
}
```

**Success Response (200):**
```json
{
  "message": "Password reset"
}
```

---

#### 9. Assign Role
**POST** `/user/assign-role`

Assign a role to a user.

**Request Body:**
```json
{
  "UserId": "user-guid",
  "Role": "Admin"
}
```

**Success Response (200):**
```json
{
  "message": "Role assigned"
}
```

---

#### 10. Remove Role
**POST** `/user/remove-role`

Remove a role from a user.

**Request Body:**
```json
{
  "UserId": "user-guid",
  "Role": "Admin"
}
```

**Success Response (200):**
```json
{
  "message": "Role removed"
}
```

---

### Account Management Service

Base Path: `/account`

**Authorization Required**: Bearer Token (User role)

#### 1. Test Account Service
**GET** `/account/test`

Test if the account service is running.

**Response:**
```json
"Account Microservice is working!"
```

---

#### 2. Get All Accounts
**GET** `/account`

Retrieve all accounts.

**Success Response (200):**
```json
[
  {
    "id": 1,
    "userID": "user-guid",
    "accountNumber": "ACC1001",
    "balance": 10000,
    "currency": "USD",
    "status": "Active",
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

---

#### 3. Get Account By Number
**GET** `/account/{accountNumber}`

Get account details by account number.

**Parameters:**
- `accountNumber` (path): Account number

**Success Response (200):**
```json
{
  "id": 1,
  "userID": "user-guid",
  "accountNumber": "ACC1001",
  "balance": 10000,
  "currency": "USD",
  "status": "Active",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

**Error Response (404):**
```
Not Found
```

---

#### 4. Get Account Balance
**GET** `/account/get-account-balance/{accountNumber}`

Get the current balance of an account.

**Parameters:**
- `accountNumber` (path): Account number

**Success Response (200):**
```json
10000
```

---

#### 5. Create Account
**POST** `/account`

Create a new account.

**Request Body:**
```json
{
  "UserID": "user-guid",
  "AccountNumber": "ACC1001",
  "Balance": 10000,
  "Currency": "USD",
  "Status": "Active"
}
```

**Field Descriptions:**
- `UserID` (required): User ID who owns the account
- `AccountNumber` (required): Unique account number
- `Balance`: Initial balance (default: 0)
- `Currency` (required): Currency code (e.g., USD, EUR)
- `Status` (required): Account status (Active, Locked, Closed)

**Success Response (200):**
```json
{
  "message": "Account created successfully!",
  "account": {
    "id": 1,
    "userID": "user-guid",
    "accountNumber": "ACC1001",
    "balance": 10000,
    "currency": "USD",
    "status": "Active"
  }
}
```

---

#### 6. Update Account
**PUT** `/account/{accountNumber}`

Update account details.

**Parameters:**
- `accountNumber` (path): Account number

**Request Body:**
```json
{
  "UserID": "user-guid",
  "AccountNumber": "ACC1001",
  "Balance": 15000,
  "Currency": "USD",
  "Status": "Active"
}
```

**Success Response (200):**
```json
"Account Updated"
```

**Error Response (404):**
```
Not Found
```

---

#### 7. Update Account Balance
**PUT** `/account/update-balance/{accountNumber}`

Update account balance (credit or debit).

**Parameters:**
- `accountNumber` (path): Account number

**Request Body:**
```json
{
  "AccountNumber": "ACC1001",
  "Amount": 500,
  "operation": "credit"
}
```

**Operation Types:**
- `credit`: Add amount to balance
- `debit`: Subtract amount from balance

**Success Response (200):**
```json
{
  "message": "Balance updated successfully",
  "balance": 10500
}
```

**Error Response (400):**
```json
{
  "message": "Insufficient balance"
}
```

---

#### 8. Update Account Status
**PUT** `/account/update-status/{accountNumber}`

Update account status.

**Parameters:**
- `accountNumber` (path): Account number

**Request Body:**
```json
{
  "AccountNumber": "ACC1001",
  "Status": "Locked"
}
```

**Status Types:**
- `Active`: Account is active
- `Locked`: Account is temporarily locked
- `Closed`: Account is closed
- `Suspended`: Account is suspended

**Success Response (200):**
```json
"Balance Updated Successfully!"
```

---

#### 9. Delete Account
**DELETE** `/account/{accountNumber}`

Delete an account.

**Parameters:**
- `accountNumber` (path): Account number

**Success Response (200):**
```json
"Account Deleted Successfully!"
```

**Error Response (404):**
```
Not Found
```

---

### Transaction Service

Base Path: `/transaction`

**Authorization Required**: Bearer Token (User role)

#### 1. Initiate Transfer
**POST** `/transaction/transfer`

Initiate a money transfer between two accounts.

**Request Body:**
```json
{
  "FromAccountId": "ACC1001",
  "ToAccountId": "ACC1002",
  "Amount": 500,
  "Currency": "USD",
  "Description": "Payment for services"
}
```

**Field Descriptions:**
- `FromAccountId` (required): Source account number
- `ToAccountId` (required): Destination account number
- `Amount` (required): Transfer amount (must be > 0)
- `Currency` (required): Currency code
- `Description`: Transaction description

**Success Response (200):**
```json
{
  "success": true,
  "message": "Transfer completed successfully",
  "transaction": {
    "id": "transaction-guid",
    "fromAccountId": "ACC1001",
    "toAccountId": "ACC1002",
    "amount": 500,
    "currency": "USD",
    "status": "Completed",
    "type": "Transfer",
    "description": "Payment for services",
    "reference": "TXN-12345",
    "initiatedAt": "2024-01-15T10:30:00Z",
    "completedAt": "2024-01-15T10:30:01Z"
  },
  "debitOperation": {
    "accountNumber": "ACC1001",
    "operationType": "debit",
    "amount": 500,
    "newBalance": 9500,
    "status": "Completed"
  },
  "creditOperation": {
    "accountNumber": "ACC1002",
    "operationType": "credit",
    "amount": 500,
    "newBalance": 5500,
    "status": "Completed"
  }
}
```

**Error Response (400):**
```json
{
  "success": false,
  "message": "Insufficient balance in source account"
}
```

**Error Response (400) - Validation:**
```json
"Invalid transfer request."
```

---

#### 2. Get Transaction By ID
**GET** `/transaction/{transactionId}`

Get transaction details by transaction ID.

**Parameters:**
- `transactionId` (path): Transaction ID (GUID)

**Success Response (200):**
```json
{
  "id": "transaction-guid",
  "fromAccountId": "ACC1001",
  "toAccountId": "ACC1002",
  "amount": 500,
  "currency": "USD",
  "status": "Completed",
  "type": "Transfer",
  "description": "Payment for services",
  "reference": "TXN-12345",
  "initiatedAt": "2024-01-15T10:30:00Z",
  "completedAt": "2024-01-15T10:30:01Z",
  "failureReason": null
}
```

**Error Response (404):**
```json
"Transaction with ID {transactionId} not found."
```

---

#### 3. Get Account Transactions (Paginated)
**GET** `/transaction/account/{accountId}?page=1&pageSize=20`

Get all transactions for an account with pagination.

**Parameters:**
- `accountId` (path): Account number
- `page` (query): Page number (default: 1)
- `pageSize` (query): Items per page (default: 20)

**Success Response (200):**
```json
[
  {
    "id": "transaction-guid-1",
    "fromAccountId": "ACC1001",
    "toAccountId": "ACC1002",
    "amount": 500,
    "currency": "USD",
    "status": "Completed",
    "type": "Transfer",
    "initiatedAt": "2024-01-15T10:30:00Z"
  },
  {
    "id": "transaction-guid-2",
    "fromAccountId": "ACC1003",
    "toAccountId": "ACC1001",
    "amount": 1000,
    "currency": "USD",
    "status": "Completed",
    "type": "Transfer",
    "initiatedAt": "2024-01-14T09:15:00Z"
  }
]
```

**Error Response (404):**
```json
"No transactions found for account {accountId}."
```

---

#### 4. Get Filtered Account Transactions
**GET** `/transaction/account/{accountId}/filtered?operation={operation}&startDate={startDate}&endDate={endDate}`

Get filtered transactions by operation type and date range.

**Parameters:**
- `accountId` (path): Account number
- `operation` (query): Operation type (credit/debit)
- `startDate` (query): Start date (format: YYYY-MM-DD)
- `endDate` (query): End date (format: YYYY-MM-DD)

**Example:**
```
/transaction/account/ACC1001/filtered?operation=credit&startDate=2024-01-01&endDate=2024-12-31
```

**Success Response (200):**
```json
[
  {
    "id": "transaction-guid",
    "fromAccountId": "ACC1003",
    "toAccountId": "ACC1001",
    "amount": 1000,
    "currency": "USD",
    "status": "Completed",
    "type": "Transfer",
    "initiatedAt": "2024-06-15T10:30:00Z"
  }
]
```

**Error Response (400):**
```json
"Start date cannot be after end date."
```

---

#### 5. Cancel Transaction
**POST** `/transaction/{transactionId}/cancel`

Cancel a pending transaction.

**Parameters:**
- `transactionId` (path): Transaction ID (GUID)

**Success Response (200):**
```json
"Transaction with ID {transactionId} has been cancelled."
```

**Error Response (404):**
```json
"Transaction with ID {transactionId} could not be cancelled or was not found."
```

---

### Notification Service

Base Path: `/notification`

**Authorization Required**: Bearer Token

#### 1. Test Notification Service
**GET** `/notification`

Test if the notification service is running.

**Response:**
```json
"Notification Microservice is working!"
```

---

## Error Handling

### HTTP Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 | Success |
| 201 | Created |
| 204 | No Content (Success with no response body) |
| 400 | Bad Request (Invalid input) |
| 401 | Unauthorized (Missing or invalid token) |
| 403 | Forbidden (Insufficient permissions) |
| 404 | Not Found (Resource doesn't exist) |
| 500 | Internal Server Error |

### Error Response Format

```json
{
  "message": "Error description",
  "errors": ["Error detail 1", "Error detail 2"]
}
```

### Common Errors

#### 401 Unauthorized
```json
"Unauthorized"
```
**Solution**: Ensure you have a valid JWT token in the Authorization header.

#### 400 Bad Request
```json
{
  "message": "User already exists!"
}
```
**Solution**: Check your request payload and ensure all required fields are present and valid.

#### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Internal server error: {error details}"
}
```
**Solution**: Check server logs for detailed error information.

---

## Postman Collection

### Importing the Collection

1. Open Postman
2. Click **Import**
3. Select the file: `MoneyTransfer_Microservices.postman_collection.json`
4. Import the environment: `MoneyTransfer_Development.postman_environment.json`

### Using the Collection

#### Step 1: Set Environment
Select **"Money Transfer - Development"** environment from the dropdown.

#### Step 2: Test Flow

1. **Register User**
   - Run: `1. Authentication > Register User`
   - This creates a new user

2. **Login**
   - Run: `1. Authentication > Login`
   - This automatically saves the JWT token

3. **Create Account**
   - Run: `3. Account Management > Create Account`
   - Creates first account (ACC1001)
   
4. **Create Second Account**
   - Run: `3. Account Management > Create Second Account`
   - Creates second account (ACC1002) for testing transfers

5. **Initiate Transfer**
   - Run: `4. Transactions > Initiate Transfer`
   - Transfers money from ACC1001 to ACC1002

6. **View Transaction History**
   - Run: `4. Transactions > Get Account Transactions`

### Environment Variables

The collection automatically manages these variables:
- `auth_token`: JWT token (auto-saved after login)
- `user_id`: User ID (auto-saved after login)
- `account_number`: Account number (auto-saved after account creation)
- `from_account`: Source account for transfers
- `to_account`: Destination account for transfers
- `transaction_id`: Transaction ID (auto-saved after transfer)

### Pre-Request Scripts

The collection includes scripts that:
- Log request URLs
- Validate response times
- Check for server errors

### Test Scripts

Automated tests check:
- Response time < 5000ms
- Status code is not 500
- Successful operations save data to environment variables

---

## Examples

### Complete Transaction Flow

#### 1. Register and Login

```bash
# Register
POST https://localhost:7000/auth/register-user
Content-Type: application/json

{
  "Username": "alice",
  "Email": "alice@example.com",
  "Password": "SecurePass@123",
  "Role": "User"
}

# Login
POST https://localhost:7000/auth/login
Content-Type: application/json

{
  "Email": "alice@example.com",
  "Password": "SecurePass@123"
}

Response:
{
  "token": {
    "token": "eyJhbG...",
    "refreshToken": "guid-guid",
    "expiresAt": "2024-12-26T10:00:00Z"
  },
  "user": {
    "id": "user-123",
    "userName": "alice",
    "email": "alice@example.com"
  }
}
```

#### 2. Create Accounts

```bash
# Create Account 1
POST https://localhost:7000/account
Authorization: Bearer eyJhbG...
Content-Type: application/json

{
  "UserID": "user-123",
  "AccountNumber": "ACC2001",
  "Balance": 50000,
  "Currency": "USD",
  "Status": "Active"
}

# Create Account 2
POST https://localhost:7000/account
Authorization: Bearer eyJhbG...
Content-Type: application/json

{
  "UserID": "user-123",
  "AccountNumber": "ACC2002",
  "Balance": 20000,
  "Currency": "USD",
  "Status": "Active"
}
```

#### 3. Check Balance

```bash
GET https://localhost:7000/account/get-account-balance/ACC2001
Authorization: Bearer eyJhbG...

Response: 50000
```

#### 4. Transfer Money

```bash
POST https://localhost:7000/transaction/transfer
Authorization: Bearer eyJhbG...
Content-Type: application/json

{
  "FromAccountId": "ACC2001",
  "ToAccountId": "ACC2002",
  "Amount": 5000,
  "Currency": "USD",
  "Description": "Payment for invoice #INV-001"
}

Response:
{
  "success": true,
  "message": "Transfer completed successfully",
  "transaction": {
    "id": "txn-guid-123",
    "fromAccountId": "ACC2001",
    "toAccountId": "ACC2002",
    "amount": 5000,
    "currency": "USD",
    "status": "Completed",
    "reference": "TXN-98765"
  },
  "debitOperation": {
    "accountNumber": "ACC2001",
    "operationType": "debit",
    "amount": 5000,
    "newBalance": 45000,
    "status": "Completed"
  },
  "creditOperation": {
    "accountNumber": "ACC2002",
    "operationType": "credit",
    "amount": 5000,
    "newBalance": 25000,
    "status": "Completed"
  }
}
```

#### 5. View Transaction History

```bash
# Get all transactions for an account
GET https://localhost:7000/transaction/account/ACC2001?page=1&pageSize=20
Authorization: Bearer eyJhbG...

# Get filtered transactions (only credits in a date range)
GET https://localhost:7000/transaction/account/ACC2001/filtered?operation=credit&startDate=2024-01-01&endDate=2024-12-31
Authorization: Bearer eyJhbG...
```

#### 6. Update Account Status

```bash
# Lock an account
PUT https://localhost:7000/account/update-status/ACC2001
Authorization: Bearer eyJhbG...
Content-Type: application/json

{
  "AccountNumber": "ACC2001",
  "Status": "Locked"
}
```

### Error Scenarios

#### Insufficient Balance

```bash
POST https://localhost:7000/transaction/transfer
Authorization: Bearer eyJhbG...
Content-Type: application/json

{
  "FromAccountId": "ACC2001",
  "ToAccountId": "ACC2002",
  "Amount": 100000,
  "Currency": "USD",
  "Description": "Large transfer"
}

Response (400):
{
  "success": false,
  "message": "Insufficient balance in source account"
}
```

#### Invalid Account

```bash
GET https://localhost:7000/account/INVALID_ACC
Authorization: Bearer eyJhbG...

Response (404): Not Found
```

#### Unauthorized Access

```bash
GET https://localhost:7000/account
# No Authorization header

Response (401): Unauthorized
```

---

## Best Practices

### Security
1. Always use HTTPS in production
2. Store JWT tokens securely
3. Never log sensitive information (passwords, tokens)
4. Implement rate limiting
5. Validate all inputs

### API Usage
1. Always include proper Authorization headers
2. Use pagination for large datasets
3. Handle errors gracefully
4. Implement retry logic for failed requests
5. Cache frequently accessed data

### Transaction Management
1. Always check account balance before initiating transfers
2. Use transaction IDs for tracking
3. Implement idempotency for transfer operations
4. Keep audit logs of all transactions
5. Handle concurrent transactions properly

---

## Support

For issues or questions:
- Check the logs in the `Logs/` directory of each microservice
- Review the Serilog database logs in the `Logs` table
- Ensure all services are running on their designated ports
- Verify database connections in appsettings.json

---

## Version History

- **v1.0** - Initial release
  - User authentication and management
  - Account management
  - Money transfers
  - Transaction history
  - Basic notification service

---

**Last Updated**: 2024
**API Version**: 1.0
**Framework**: .NET 8
