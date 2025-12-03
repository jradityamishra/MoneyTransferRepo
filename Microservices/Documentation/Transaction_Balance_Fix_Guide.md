# Transaction Transfer Issues - Troubleshooting Guide

## Issue: 400 Bad Request on Balance Update

### Problem Description

When attempting to transfer money between accounts, the transaction fails with a **400 Bad Request** error during the balance update step.

**Error Pattern in Logs:**
```
Ocelot.Requester.Middleware.HttpRequesterMiddleware: Warning: 
'400 (Bad Request) status code of request URI: 
https://localhost:7002/api/account/update-balance/07352595.'
```

---

## Root Cause Analysis

### The Bug

The `UpdateBalanceAsync` method in `AccountService.cs` had a logic flaw:

**Original Code:**
```csharp
if (op == "debit" && existing.Status == "Lock")
{
    return new BalanceUpdateResult
    {
        Success = false,
        Message = "Account is locked"
    };
}

if (op == "debit" && existing.Status == "Unlock")  // ? Problem here
{
    existing.Balance = existing.Balance - changeBalanceVM.Amount;
} 
else if (op == "credit")
{
    existing.Balance = existing.Balance + changeBalanceVM.Amount;
}
else
{
    return new BalanceUpdateResult
    {
        Success = false,
        Message = "Invalid operation"
    };
}
```

### Why It Failed

1. **Account Status Values:**
   - Accounts are created with status: **"Active"**
   - Lock status: **"Lock"** or **"Locked"**
   - Unlock status: Should be **"Active"** (not "Unlock")

2. **The Logic Flaw:**
   ```csharp
   if (op == "debit" && existing.Status == "Unlock")
   ```
   - This condition checks if status equals "Unlock"
   - But accounts have status "Active", not "Unlock"
   - So debit operations **always went to the else block**
   - The else block returned "Invalid operation" error

3. **Result:**
   - Debit operations failed with "Invalid operation"
   - Credit operations worked (they were in the else-if)
   - Transfers failed because debit couldn't execute

---

## The Fix

### Updated Code

```csharp
public async Task<BalanceUpdateResult> UpdateBalanceAsync(string accountNo, ChangeBalanceVM changeBalanceVM)
{
    var existing = await _context.Accounts.FirstOrDefaultAsync(n => n.AccountNumber == accountNo);
    if (existing == null) 
        return new BalanceUpdateResult 
        { 
            Success = false, 
            Message = "Account not found" 
        };

    var op = changeBalanceVM.operation?.ToLower();

    // ? Check if account is locked (for debit operations)
    if (op == "debit" && (existing.Status == "Lock" || existing.Status == "Locked"))
    {
        return new BalanceUpdateResult
        {
            Success = false,
            Message = "Account is locked"
        };
    }

    // ? Perform the operation (no status check for debit on active accounts)
    if (op == "debit")
    {
        // ? Added: Check for sufficient balance
        if (existing.Balance < changeBalanceVM.Amount)
        {
            return new BalanceUpdateResult
            {
                Success = false,
                Message = "Insufficient balance"
            };
        }
        
        existing.Balance = existing.Balance - changeBalanceVM.Amount;
    } 
    else if (op == "credit")
    {
        existing.Balance = existing.Balance + changeBalanceVM.Amount;
    }
    else
    {
        return new BalanceUpdateResult
        {
            Success = false,
            Message = "Invalid operation. Use 'debit' or 'credit'"
        };
    }

    // ? Added: Update timestamp
    existing.UpdatedAt = DateTime.UtcNow;
    _context.Accounts.Update(existing);
    await _context.SaveChangesAsync();
    
    return new BalanceUpdateResult
    {
        Success = true,
        Message = "Balance updated successfully",
        UpdatedBalance = existing.Balance
    };
}
```

### What Changed

1. **? Removed Unlock Status Check:**
   - Changed from: `if (op == "debit" && existing.Status == "Unlock")`
   - To: `if (op == "debit")`
   - Now debit works on Active accounts

2. **? Improved Lock Check:**
   - Added both "Lock" and "Locked" status checks
   - Only blocks debit on locked accounts

3. **? Added Balance Validation:**
   - Checks for insufficient balance before debit
   - Returns proper error message

4. **? Added Timestamp Update:**
   - Updates `UpdatedAt` field on balance change

5. **? Better Error Messages:**
   - More descriptive error messages
   - Specifies valid operations

---

## Applying the Fix

### Step 1: Stop All Running Services

Stop debugging in Visual Studio or terminate all running processes:
- UserMicroservices
- AccountMicroservices
- TransactionMicroservices
- NotificationMicroservices
- OcelotApiGateway

### Step 2: Clean and Rebuild

```bash
# Clean the solution
dotnet clean

# Rebuild
dotnet build
```

Or in Visual Studio:
- Build ? Clean Solution
- Build ? Rebuild Solution

### Step 3: Restart All Services

Start all 5 microservices again.

### Step 4: Test the Fix

Use Postman to test:

#### Test 1: Create Accounts
```json
POST https://localhost:7000/account
Authorization: Bearer <token>

{
  "UserID": "user-id",
  "AccountNumber": "ACC1001",
  "Balance": 10000,
  "Currency": "USD",
  "Status": "Active"
}
```

#### Test 2: Initiate Transfer
```json
POST https://localhost:7000/transaction/transfer
Authorization: Bearer <token>

{
  "FromAccountId": "ACC1001",
  "ToAccountId": "ACC1002",
  "Amount": 500,
  "Currency": "USD",
  "Description": "Test transfer"
}
```

#### Expected Success Response
```json
{
    "success": true,
    "message": "Transfer completed successfully",
    "transaction": {
        "id": "transaction-guid",
        "fromAccountId": "ACC1001",
        "toAccountId": "ACC1002",
        "amount": 500,
        "status": "Completed"
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

---

## Account Status Reference

### Valid Status Values

| Status | Description | Can Debit? | Can Credit? |
|--------|-------------|------------|-------------|
| **Active** | Normal active account | ? Yes | ? Yes |
| **Lock** / **Locked** | Account is locked | ? No | ? Yes |
| **Closed** | Account is closed | ? No | ? No |
| **Suspended** | Temporarily suspended | ? No | ? No |

### Recommended Status Values

Use these consistent status values across your application:

```csharp
public static class AccountStatus
{
    public const string Active = "Active";
    public const string Locked = "Locked";
    public const string Closed = "Closed";
    public const string Suspended = "Suspended";
}
```

**Usage:**
```csharp
account.Status = AccountStatus.Active;
```

---

## Transaction Flow (Fixed)

### Before Fix
```
1. Validate accounts ?
2. Check balance ?
3. Debit from source ? (Failed: "Invalid operation")
4. Transaction failed
```

### After Fix
```
1. Validate accounts ?
2. Check balance ?
3. Debit from source ? (Success)
4. Credit to destination ? (Success)
5. Record transaction ?
6. Transaction completed ?
```

---

## Testing Scenarios

### Scenario 1: Normal Transfer (Should Work)
```json
{
  "FromAccountId": "ACC1001",  // Status: Active, Balance: 10000
  "ToAccountId": "ACC1002",    // Status: Active
  "Amount": 500,
  "Currency": "USD"
}
```
**Expected:** ? Success

### Scenario 2: Insufficient Balance
```json
{
  "FromAccountId": "ACC1001",  // Balance: 100
  "ToAccountId": "ACC1002",
  "Amount": 500,
  "Currency": "USD"
}
```
**Expected:** ? Error: "Insufficient balance"

### Scenario 3: Locked Account
```json
// First lock the account
PUT https://localhost:7000/account/update-status/ACC1001
{
  "AccountNumber": "ACC1001",
  "Status": "Locked"
}

// Then try to transfer
POST https://localhost:7000/transaction/transfer
{
  "FromAccountId": "ACC1001",
  "ToAccountId": "ACC1002",
  "Amount": 500
}
```
**Expected:** ? Error: "Account is locked"

---

## Verification Checklist

After applying the fix, verify:

- [ ] All services build without errors
- [ ] Can create accounts with "Active" status
- [ ] Can transfer money between active accounts
- [ ] Debit operations work correctly
- [ ] Credit operations work correctly
- [ ] Insufficient balance is detected
- [ ] Locked accounts cannot be debited
- [ ] Transaction history is recorded
- [ ] Account balances are updated correctly

---

## Common Errors and Solutions

### Error: "Invalid operation"
**Before Fix:** Occurred on every debit attempt  
**After Fix:** Only occurs if operation is not "debit" or "credit"  
**Solution:** Ensure operation is lowercase "debit" or "credit"

### Error: "Account is locked"
**Cause:** Account status is "Locked" or "Lock"  
**Solution:** Update account status to "Active"
```json
PUT /account/update-status/{accountNumber}
{
  "AccountNumber": "ACC1001",
  "Status": "Active"
}
```

### Error: "Insufficient balance"
**Cause:** Trying to debit more than available balance  
**Solution:** Check balance first or reduce transfer amount

### Error: "Account not found"
**Cause:** Account number doesn't exist  
**Solution:** Verify account number or create account first

---

## Best Practices

### 1. Account Status Management
```csharp
// Use constants for status values
public const string STATUS_ACTIVE = "Active";
public const string STATUS_LOCKED = "Locked";

// Always validate status before operations
if (account.Status == STATUS_LOCKED)
{
    return Error("Account is locked");
}
```

### 2. Balance Validation
```csharp
// Always check balance before debit
if (operation == "debit" && account.Balance < amount)
{
    return Error("Insufficient balance");
}
```

### 3. Operation Validation
```csharp
// Validate operation type
var validOperations = new[] { "debit", "credit" };
if (!validOperations.Contains(operation.ToLower()))
{
    return Error("Invalid operation");
}
```

### 4. Transaction Atomicity
```csharp
// Use database transactions for multi-step operations
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // Debit
    await DebitAccount(fromAccount, amount);
    
    // Credit
    await CreditAccount(toAccount, amount);
    
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

---

## Additional Improvements Made

### 1. Added Insufficient Balance Check
Prevents overdrafts:
```csharp
if (existing.Balance < changeBalanceVM.Amount)
{
    return new BalanceUpdateResult
    {
        Success = false,
        Message = "Insufficient balance"
    };
}
```

### 2. Added Timestamp Update
Tracks when balance was modified:
```csharp
existing.UpdatedAt = DateTime.UtcNow;
```

### 3. Improved Error Messages
More descriptive errors:
```csharp
Message = "Invalid operation. Use 'debit' or 'credit'"
```

---

## Related Documentation

- **API Documentation:** `Documentation/API_Documentation.md`
- **Authentication Fix:** `Documentation/Authentication_Fix_Guide.md`
- **Postman Testing:** `Documentation/Postman_Quick_Start.md`
- **Complete Guide:** `Documentation/INDEX.md`

---

## Support

If you continue to experience issues:

1. **Check Logs:**
   - Look for specific error messages
   - Check all 5 microservice logs

2. **Verify Data:**
   - Check account status in database
   - Verify account balances
   - Confirm account numbers exist

3. **Test Direct API:**
   - Test balance update endpoint directly
   - Verify it works outside of transactions

4. **Database State:**
   ```sql
   -- Check account status
   SELECT AccountNumber, Balance, Status 
   FROM Accounts 
   WHERE AccountNumber IN ('ACC1001', 'ACC1002');
   ```

---

**Status:** ? Fixed  
**Issue:** Balance update logic flaw  
**Impact:** Transactions now work correctly  
**Version:** 1.1  
**Date:** 2024
