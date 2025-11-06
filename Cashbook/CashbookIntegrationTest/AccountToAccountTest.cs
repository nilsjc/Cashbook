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
        const string BankAccount = "Bankkonto";
        const string GroceryAccount = "Livsmedel";
        const string RentAccount = "Hyra";

        private readonly ITestOutputHelper output;

        public AccountToAccountTest(ITestOutputHelper output)
        {
            this.output = output;

        }

        [Fact]
        public async Task Konton_Utan_Transaktioner()
        {
            SqliteConnection? connection = null;
            var dbService = CreateTestDatabaseService(connection);

            await dbService.CreateAccountAsync(GroceryAccount, AccountType.Expense);
            await dbService.CreateAccountAsync(BankAccount, AccountType.Check);
            await dbService.CreateAccountAsync(RentAccount, AccountType.Expense);
            await dbService.CreateAccountAsync(SalaryAccount, AccountType.Income);

            var result = await dbService.GetAllAccountsAsync();

            var names = result!.Data!.Select(a => a.Name).ToList();

            var expected = names
                .OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            // Kontrollera om kontona returneras i alfabetisk ordning
            Assert.Equal(expected, names);
            // Kontrollera att alla konton har saldo 0
            Assert.True(result?.Data?.Where(a => a.Name == BankAccount).First().Amount == 0);
            Assert.True(result?.Data?.Where(a => a.Name == GroceryAccount).First().Amount == 0);
            Assert.True(result?.Data?.Where(a => a.Name == RentAccount).First().Amount == 0);
            Assert.True(result?.Data?.Where(a => a.Name == SalaryAccount).First().Amount == 0);

            connection?.Close();
        }

        [Fact]
        public async Task Skapa_Dubbla_Konton_Med_Samma_Namn()
        {
            SqliteConnection? connection = null;
            var dbService = CreateTestDatabaseService(connection);

            var result1 = await dbService.CreateAccountAsync(GroceryAccount, AccountType.Expense);
            var result2 = await dbService.CreateAccountAsync(GroceryAccount, AccountType.Expense);

            Assert.True(result1.Success);
            Assert.False(result2.Success);
            Assert.Equal($"An account with the name '{GroceryAccount}' already exists.", result2.Error);

            connection?.Close();
        }

        [Fact]
        public async Task Saldo_Konton_Transaktioner()
        {
            SqliteConnection? connection = null;
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
            output.WriteLine(result?.Data?.ToString());

            Assert.True(result?.Data?.Where(a => a.Name == BankAccount).First().Amount == 1450);
            Assert.True(result?.Data?.Where(a => a.Name == GroceryAccount).First().Amount == -50);
            Assert.True(result?.Data?.Where(a => a.Name == RentAccount).First().Amount == -500);
            Assert.True(result?.Data?.Where(a => a.Name == SalaryAccount).First().Amount == 2000);

            connection?.Close();

        }

        [Fact]
        public async Task Korrigering_Av_Transaktioner()
        {
            SqliteConnection? connection = null;
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
            output.WriteLine(result?.Data?.ToString());

            Assert.True(result?.Data?.Where(a => a.Name == BankAccount).First().Amount == 900);
            Assert.True(result?.Data?.Where(a => a.Name == RentAccount).First().Amount == 0);
            Assert.True(result?.Data?.Where(a => a.Name == GroceryAccount).First().Amount == -100);
            Assert.True(result?.Data?.Where(a => a.Name == SalaryAccount).First().Amount == 1000);

            connection?.Close();


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