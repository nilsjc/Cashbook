using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebAPI.endpoints;

namespace WebAPI.database
{
    public class EFDatabaseService : IDatabaseService
    {
        private readonly IDbContextFactory<CashBookDbContext> _dbContextFactory;
        public EFDatabaseService(IDbContextFactory<CashBookDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<ServiceResult<string>> CreateAccountAsync(string name, AccountType type)
        {
            // create account
            using (var db = _dbContextFactory.CreateDbContext())
            {
                // check duplicates
                var existingAccount = await db.Accounts.FindAsync(name);
                if (existingAccount != null)
                {
                    return ServiceResult<string>.Fail($"An account with the name '{name}' already exists.");
                }

                var newAccount = new Account
                {
                    Name = name,
                    AccountType = (int)type,
                    Amount = 0
                };

                db.Accounts.Add(newAccount);
                await db.SaveChangesAsync();
                return ServiceResult<string>.Ok(name);
            }
        }



        public async Task<ServiceResult<IEnumerable<GetAccountDTO>>> GetAllAccountsAsync()
        {
            using var db = _dbContextFactory.CreateDbContext();
            var accounts = await db.Accounts
                .OrderBy(a => a.Name)
                .Select(a => new GetAccountDTO
                {
                    Name = a.Name,
                    Amount = a.Amount
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<GetAccountDTO>>.Ok(accounts);
        }

        public async Task<ServiceResult<string>> CreateTransactionAsync(string fromAccount, string toAccount, int amount)
        {
            if(fromAccount == toAccount)
            {
                return ServiceResult<string>.Fail("Cannot transfer to the same account.");
            }
            using var context = _dbContextFactory.CreateDbContext();
            using var tx = await context.Database.BeginTransactionAsync();
            try
            {
                var fromAcc = await context.Accounts.FindAsync(fromAccount);
                var toAcc = await context.Accounts.FindAsync(toAccount);

                if (fromAcc == null || toAcc == null)
                    return ServiceResult<string>.Fail("One or both accounts do not exist.");

                fromAcc.TransferTo(toAcc, amount);

                await context.SaveChangesAsync();
                await tx.CommitAsync();
                return ServiceResult<string>.Ok("Transaction completed successfully.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ServiceResult<string>.Fail($"Transaction failed: {ex.Message}");
            }
        }
    }
}