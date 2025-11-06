using WebAPI.endpoints;
using WebAPI.database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace CashbookIntegrationTest
{
    public class RaceConditionTest
    {
        private readonly ITestOutputHelper output;

        public RaceConditionTest(ITestOutputHelper output)
        {
            this.output = output;

        }
        const string SalaryAccount = "Lön";
        const string BankAccount = "Bankkonto";

        [Fact]
        public async Task TestConcurrencyHandling()
        {
            SqliteConnection? connection = null;
            var dbService = CreateTestDatabaseService(connection);
            await dbService.CreateAccountAsync(SalaryAccount, AccountType.Income);
            await dbService.CreateAccountAsync(BankAccount, AccountType.Check);

            int threads = 1000;
            var tasks = Enumerable.Range(0, threads).Select(_ =>
                Task.Run(async () =>
                {
                    await dbService.CreateTransactionAsync(SalaryAccount, BankAccount, 1);
                })
            ).ToArray();
            await Task.WhenAll(tasks);
            var result = await dbService.GetAllAccountsAsync();
            Assert.True(result?.Data?.Where(a => a.Name == BankAccount).First().Amount == threads);
            connection?.Close();

        }
        public static EFDataBaseServiceThreadsafe CreateTestDatabaseService(SqliteConnection? connection)
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

            return new EFDataBaseServiceThreadsafe(factory);
        }
    }
}