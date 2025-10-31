using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebAPI.database;

namespace WebAPI.endpoints
{
    public static class AccountAPI
    {
        public static void RegisterAccountEndpoints(this WebApplication app)
        {
            app.MapPost("/account", CreateAccount)
            .WithName("CreateAccount")
            .WithOpenApi();

            app.MapGet("/accounts", GetAccounts)
            .WithName("GetAccounts")
            .WithOpenApi();
        }

        public static async Task<IResult> CreateAccount(AccountDTO account, IValidator<AccountDTO> validator)
        {
            var validationResult = await validator.ValidateAsync(account);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(errors);
            }

            // create account
            using (var db = new CashBookContext())
            {
                string name = (account.Name.Length > 10 ? account.Name.Substring(0, 10) : account.Name)
                    .ToLower();

                // check duplicates
                var existingAccount = await db.Accounts.FindAsync(name);
                if (existingAccount != null)
                {
                    return Results.Conflict($"An account with the name '{name}' already exists.");
                }

                var newAccount = new Account
                {
                    Name = name,
                    AccountType = (int)account.Type,
                    Amount = 0
                };

                db.Accounts.Add(newAccount);
                await db.SaveChangesAsync();

            }

            return Results.Ok(account);
        }
        
        public static async Task<IResult> GetAccounts()
        {
            using (var db = new CashBookContext())
            {
                var accounts = await db.Accounts
                    .Select(a => new AccountDTO
                    {
                        Name = a.Name,
                        Type = (AccountType)a.AccountType
                    })
                    .ToListAsync();

                return Results.Ok(accounts);
            }
        }
    }
}