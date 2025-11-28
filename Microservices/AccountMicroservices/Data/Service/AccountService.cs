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

    public async Task<Account?> GetByIdAsync(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> CreateAsync(Account account)
    {
        account.CreatedAt = DateTime.UtcNow;
        account.UpdatedAt = DateTime.UtcNow;
            _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<bool> UpdateAsync(int id, Account updatedAccount)
    {
        var existing = await _context.Accounts.FindAsync(id);
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

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Accounts.FindAsync(id);
        if (existing == null) return false;

            _context.Accounts.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
}