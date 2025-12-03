using AccountMicroservices.Data.Model;
using AccountMicroservices.Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using UserMicroservices.Data;
using static Azure.Core.HttpHeader;

namespace AccountMicroservices.Data.Service
{
    public class AccountService 
    {

        private readonly DatabaseContext _context;

    public AccountService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        return await _context.Accounts.AsNoTracking().ToListAsync();
    }

    public async Task<Account?> GetByIdAsync(string accountNumber)
    {
        return await _context.Accounts.FirstOrDefaultAsync(n=>n.AccountNumber == accountNumber);
    }
        public async Task<int> getAccountBalanceAsync(string accountNumber)
        {
            var _account = await _context.Accounts.FirstOrDefaultAsync(n => n.AccountNumber == accountNumber);
            return  _account.Balance;
        }

        public async Task<Account> CreateAsync(Account account)
    {
        account.CreatedAt = DateTime.UtcNow;
        account.UpdatedAt = DateTime.UtcNow;
            _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<bool> UpdateAsync(string accountNo, Account updatedAccount)
    {
        var existing = await _context.Accounts.FirstOrDefaultAsync(n => n.AccountNumber == accountNo);
        if (existing == null) return false;

        existing.UserID = updatedAccount.UserID;
        existing.AccountNumber = updatedAccount.AccountNumber;
        existing.Balance = updatedAccount.Balance;
        existing.Currency = updatedAccount.Currency;
        existing.Status = updatedAccount.Status;
        existing.UpdatedAt = DateTime.UtcNow;

            _context.Accounts.Update(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string accountNumber)
    {
        var existing = await _context.Accounts.FirstOrDefaultAsync(n=>n.AccountNumber==accountNumber);
        if (existing == null) return false;

            _context.Accounts.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
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

            // Check if account is locked (for debit operations)
            if (op == "debit" && (existing.Status == "Lock" || existing.Status == "Locked"))
            {
                return new BalanceUpdateResult
                {
                    Success = false,
                    Message = "Account is locked"
                };
            }

            // Perform the operation
            if (op == "debit")
            {
                // Check for sufficient balance
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
        public async Task<bool> UpdateAccStatus(string accountNo, UpdateStatus updateStatus)
        {
            var existing = await _context.Accounts.FirstOrDefaultAsync(n => n.AccountNumber == accountNo);
            if (existing == null) return false;


            existing.Status = updateStatus.Status;


            _context.Accounts.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}