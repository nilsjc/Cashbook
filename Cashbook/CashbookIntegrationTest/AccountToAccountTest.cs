using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using WebAPI.endpoints;
using Xunit;
using WebAPI.database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace CashbookIntegrationTest
{
    public class AccountToAccountTest
    {
        const string SalaryAccount = "Lön";
        const string IncomeAccount = "Inkomst";
        const string BankAccount = "Bankkonto";
        const string GroceryAccount = "Livsmedel";
        const string RentAccount = "Hyra";

        private readonly ITestOutputHelper output;

        public AccountToAccountTest(ITestOutputHelper output)
        {
            this.output = output;

        }

        [Fact]
        public async Task Test1()
        {
            SqliteConnection connection = null;
            var dbService = CreateTestDatabaseService(connection);

            // create accounts
            await dbService.CreateAccountAsync(GroceryAccount, AccountType.Expense);
            await dbService.CreateAccountAsync(BankAccount, AccountType.Check);
            await dbService.CreateAccountAsync(RentAccount, AccountType.Expense);
            await dbService.CreateAccountAsync(SalaryAccount, AccountType.Income);

            // create transactions
            await dbService.CreateTransactionAsync(SalaryAccount, BankAccount, 1000);
            await dbService.CreateTransactionAsync(BankAccount, GroceryAccount, 50);
            await dbService.CreateTransactionAsync(BankAccount, RentAccount, 250);
            await dbService.CreateTransactionAsync(SalaryAccount, BankAccount, 1000);
            await dbService.CreateTransactionAsync(BankAccount, RentAccount, 250);


            var result = await dbService.GetAllAccountsAsync();
            if (result?.Data is not null)
            {
                foreach (var account in result?.Data)
                {
                    output.WriteLine(" - {0}: {1}", account.Name, account.Amount);
                }
            }
            connection.Close();

            Assert.True(result?.Data?.Count() == 4);

        }

        [Fact]
        public async Task Test2()
        {
            SqliteConnection connection = null;
            var dbService = CreateTestDatabaseService(connection);

            // create accounts
            await dbService.CreateAccountAsync(GroceryAccount, AccountType.Expense);
            await dbService.CreateAccountAsync(BankAccount, AccountType.Check);
            await dbService.CreateAccountAsync(RentAccount, AccountType.Expense);
            await dbService.CreateAccountAsync(SalaryAccount, AccountType.Income);

            // create transactions
            await dbService.CreateTransactionAsync(SalaryAccount, BankAccount, 1000);
            await dbService.CreateTransactionAsync(BankAccount, RentAccount, 100);
            await dbService.CreateTransactionAsync(RentAccount, GroceryAccount, 100);

            var result = await dbService.GetAllAccountsAsync();
            if (result?.Data is not null)
            {
                foreach (var account in result?.Data)
                {
                    output.WriteLine(" - {0}: {1}", account.Name, account.Amount);
                }
            }
            connection.Close();

            Assert.True(result?.Data?.Count() == 4);

        }
        public static EFDatabaseService CreateTestDatabaseService(SqliteConnection connection)
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