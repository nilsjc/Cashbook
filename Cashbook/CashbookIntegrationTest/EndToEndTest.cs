using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.database;
using WebAPI.endpoints;
using Microsoft.AspNetCore.Mvc;

namespace CashbookIntegrationTest
{
    public class EndToEndTest
    {
        const string SalaryAccount = "Lön";
        const string BankAccount = "Bankkonto";
        const string GroceryAccount = "Livsmedel";
        const string RentAccount = "Hyra";
        [Fact]
        public async Task Skapande_Av_Konto()
        {
            SqliteConnection? connection = null;
            IDatabaseService dbService = CreateTestDatabaseService(connection);
            PostAccountDTO account = new()
            {
                Name = SalaryAccount,
                Type = AccountType.Check
            };
            await AccountAPI.CreateAccount(account, new WebAPI.validators.AccountCreateValidator(), dbService);
            connection?.Close();
            var returnValue = await AccountAPI.GetAccounts(dbService);
            var result = returnValue as OkObjectResult;
            var accounts = result?.Value as List<GetAccountDTO>;
            Assert.True(accounts?[0].Name == SalaryAccount);
            Assert.True(accounts?[0].Amount == 0);

        }

        [Fact]
        public async Task Saldo_Konton_Transaktioner()
        {
            SqliteConnection? connection = null;
            IDatabaseService dbService = CreateTestDatabaseService(connection);

            // skapa konton
            List<PostAccountDTO> accountsToCreate = new()
            {
                new PostAccountDTO { Name = SalaryAccount, Type = AccountType.Income },
                new PostAccountDTO { Name = BankAccount, Type = AccountType.Check },
                new PostAccountDTO { Name = GroceryAccount, Type = AccountType.Expense },
                new PostAccountDTO { Name = RentAccount, Type = AccountType.Expense }
            };
            foreach (var account in accountsToCreate)
            {
                await AccountAPI.CreateAccount(account, new WebAPI.validators.AccountCreateValidator(), dbService);
            }

            // skapa transaktioner
            List<TransactionRequestDTO> transactionsToCreate = new()
            {
                new TransactionRequestDTO { FromAccount = SalaryAccount, ToAccount = BankAccount, Amount = 1000 },
                new TransactionRequestDTO { FromAccount = BankAccount, ToAccount = GroceryAccount, Amount = 50 },
                new TransactionRequestDTO { FromAccount = BankAccount, ToAccount = RentAccount, Amount = 250 },
                new TransactionRequestDTO { FromAccount = SalaryAccount, ToAccount = BankAccount, Amount = 1000 },
                new TransactionRequestDTO { FromAccount = BankAccount, ToAccount = RentAccount, Amount = 250 }
            };
            foreach (var transaction in transactionsToCreate)
            {
                await dbService.CreateTransactionAsync(transaction.FromAccount, transaction.ToAccount, transaction.Amount);
            }
            var returnValue = await AccountAPI.GetAccounts(dbService);
            var result = returnValue as OkObjectResult;
            var accounts = result?.Value as List<GetAccountDTO>;
            Assert.True(accounts?.Where(a => a.Name == BankAccount).First().Amount == 1450);
            Assert.True(accounts?.Where(a => a.Name == GroceryAccount).First().Amount == -50);
            Assert.True(accounts?.Where(a => a.Name == RentAccount).First().Amount == -500);
            Assert.True(accounts?.Where(a => a.Name == SalaryAccount).First().Amount == 2000);
        }

    public static EFDatabaseService CreateTestDatabaseService(SqliteConnection? connection)
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var services = new ServiceCollection();
            services.AddDbContextFactory<CashBookDbContext>(opts =>
            {
                opts.UseSqlite(connection);
            });
            var provider = services.BuildServiceProvider();

            var factory = provider.GetRequiredService<IDbContextFactory<CashBookDbContext>>();
            using (var ctx = factory.CreateDbContext())
            {
                ctx.Database.EnsureCreated();
            }

            return new EFDatabaseService(factory);
        }
    }
}