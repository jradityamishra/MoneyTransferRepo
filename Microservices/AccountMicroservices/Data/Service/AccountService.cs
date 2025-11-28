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
    public async Task<bool> UpdateBalanceAsync(string accountNo,ChangeBalanceVM changeBalanceVM )
    {
        var existing = await _context.Accounts.FirstOrDefaultAsync(n => n.AccountNumber == accountNo);
        if (existing == null) return false;

        
        existing.Balance = changeBalanceVM.Amount;
 

        _context.Accounts.Update(existing);
        await _context.SaveChangesAsync();
        return true;
    }
    }
}