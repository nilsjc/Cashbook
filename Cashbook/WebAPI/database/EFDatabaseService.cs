using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.endpoints;

namespace WebAPI.database
{
    public class EFDatabaseService : IDatabaseService
    {
        public async Task<ServiceResult<string>> CreateAccountAsync(string name, AccountType type)
        {
            // create account
            using (var db = new CashBookContext())
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
    }
}