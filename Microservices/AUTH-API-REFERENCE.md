# Authentication Microservice - Quick Reference

## ?? Available Endpoints

### Base URL
- **Development:** `https://localhost:7001/api/auth`
- **Via API Gateway:** `https://localhost:7000/auth`

---

## Endpoints

### 1. Health Check
```http
GET /api/auth
```
**Description:** Test if the authentication service is running

**Response:**
```json
"Authentication Microservice is working!"
```

---

### 2. Register User
```http
POST /api/auth/register
Content-Type: application/json
```

**Request Body:**
```json
{
  "username": "string (3-20 chars)",
  "email": "string (valid email)",
  "password": "string (min 8 chars)"
}
```

**Success Response (200):**
```json
{
  "status": "Success",
  "message": "User registered successfully!"
}
```

**Error Response (500):**
```json
{
  "status": "Error",
  "message": "User already exists!"
}
```

**Notes:**
- ? Automatically assigns "User" role
- ? Validates email format
- ? Checks username uniqueness

---

### 3. Login
```http
POST /api/auth/login
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Success Response (200):**
```json
{
  "token": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "guid-guid-guid-guid",
    "expiresAt": "2024-12-05T10:30:00Z"
  },
  "user": {
    "id": "user-guid",
    "userName": "johndoe",
    "email": "john@example.com",
    "emailConfirmed": false,
    "phoneNumber": null,
    "phoneNumberConfirmed": false,
    "twoFactorEnabled": false,
    "lockoutEnd": null,
    "lockoutEnabled": false,
    "accessFailedCount": 0,
    "status": "Unlock",
    "createdAt": "2024-12-03T10:30:00Z",
    "updatedAt": null
  }
}
```

**Error Response (401):**
```json
{
  "status": "Error",
  "message": "Invalid email or password"
}
```

**Notes:**
- ? Returns JWT token (valid for 24 hours)
- ? Returns refresh token (valid for 6 months)
- ? Includes user roles in JWT claims

---

### 4. Check User Exists
```http
GET /api/auth/check-user/{id}
```

**Parameters:**
- `id` (string, required) - User ID to check

**Success Response (200):**
```json
{
  "exists": true
}
```

**Notes:**
- ? Used by other microservices
- ? No authentication required
- ? Returns true/false based on user existence

---

## JWT Token Claims

The JWT token includes the following claims:

```javascript
{
  "nameid": "user-id",
  "unique_name": "username",
  "email": "user@example.com",
  "sub": "user@example.com",
  "jti": "token-id",
  "role": ["User"], // or ["Admin", "User"]
  "nbf": 1701600000,
  "exp": 1701686400,
  "iat": 1701600000,
  "iss": "https://localhost:44386/",
  "aud": "User"
}
```

---

## Using JWT Token

### In Request Headers
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Example with cURL
```bash
curl -X GET https://localhost:7002/api/account \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### Example with JavaScript
```javascript
fetch('https://localhost:7002/api/account', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});
```

---

## User Roles

### Default Role
- **User** - Automatically assigned to all new registrations

### Admin Role
- **Admin** - Created via database seeding
- Default admin credentials:
  - Email: `admin@system.com`
  - Password: `Admin@123`

---

## Error Codes

| Status Code | Meaning |
|-------------|---------|
| 200 | Success |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid credentials) |
| 500 | Internal Server Error |

---

## Postman Collection Example

```json
{
  "info": {
    "name": "Authentication Microservice",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Register",
      "request": {
        "method": "POST",
        "header": [{"key": "Content-Type", "value": "application/json"}],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"username\": \"testuser\",\n  \"email\": \"test@test.com\",\n  \"password\": \"Test1234\"\n}"
        },
        "url": {
          "raw": "https://localhost:7001/api/auth/register",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7001",
          "path": ["api", "auth", "register"]
        }
      }
    },
    {
      "name": "Login",
      "request": {
        "method": "POST",
        "header": [{"key": "Content-Type", "value": "application/json"}],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"email\": \"test@test.com\",\n  \"password\": \"Test1234\"\n}"
        },
        "url": {
          "raw": "https://localhost:7001/api/auth/login",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7001",
          "path": ["api", "auth", "login"]
        }
      }
    }
  ]
}
```

---

## Common Issues

### 1. "User already exists"
**Solution:** Use a different username or email

### 2. "Invalid email or password"
**Solution:** Check credentials and ensure user is registered

### 3. Token expired
**Solution:** Login again to get a new token

### 4. CORS errors
**Solution:** Configure CORS in Program.cs if accessing from a web app

---

## Testing Commands

### cURL Examples

**Register:**
```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@test.com","password":"Test1234"}'
```

**Login:**
```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test1234"}'
```

**Check User:**
```bash
curl -X GET https://localhost:7001/api/auth/check-user/USER-ID-HERE
```

---

## Security Notes

? **What's Secured:**
- Passwords are hashed (ASP.NET Core Identity)
- JWT tokens are signed
- HTTPS enforced
- Security stamps for token invalidation

? **What's Not Implemented:**
- Email verification
- Password reset via email
- Two-factor authentication
- Account lockout (can be enabled in Program.cs)
- Rate limiting

---

**Last Updated:** 2024-12-03
**Version:** 1.0
**Microservice:** Authentication Only
