using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using WebAPI.endpoints;

namespace WebAPI.database
{
    public class EFDataBaseServiceThreadsafe : IDatabaseService
    {
        private readonly IDbContextFactory<CashBookDbContext> _dbContextFactory;
        // Per-account semaphores to serialize operations on the same account names within this process.
        // This is an application-level lock and works only within the same process.
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _accountLocks = new();

        public EFDataBaseServiceThreadsafe(IDbContextFactory<CashBookDbContext> dbContextFactory)
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

        public async Task<ServiceResult<string>> CreateTransactionAsync(string fromAccount, string toAccount, int amount)
        {
            if (fromAccount == toAccount)
            {
                return ServiceResult<string>.Fail("Cannot transfer to the same account.");
            }

            // Acquire locks for both accounts in a deterministic order to avoid deadlocks.
            var firstKey = string.CompareOrdinal(fromAccount, toAccount) <= 0 ? fromAccount : toAccount;
            var secondKey = firstKey == fromAccount ? toAccount : fromAccount;

            var firstLock = _accountLocks.GetOrAdd(firstKey, _ => new SemaphoreSlim(1, 1));
            var secondLock = _accountLocks.GetOrAdd(secondKey, _ => new SemaphoreSlim(1, 1));

            await firstLock.WaitAsync();
            await secondLock.WaitAsync();

            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                using var tx = await context.Database.BeginTransactionAsync();
                try
                {
                    var fromAcc = await context.Accounts.FindAsync(fromAccount);
                    var toAcc = await context.Accounts.FindAsync(toAccount);

                    if (fromAcc == null || toAcc == null)
                        return ServiceResult<string>.Fail("One or both accounts do not exist.");

                    // Delegate business logic to the domain entity
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
            finally
            {
                // Release locks in reverse order
                secondLock.Release();
                firstLock.Release();
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
    }
}