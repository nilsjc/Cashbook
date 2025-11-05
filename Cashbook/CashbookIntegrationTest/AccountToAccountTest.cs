using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using WebAPI.endpoints;
using Xunit;
using WebAPI.database;

namespace CashbookIntegrationTest
{
    public class AccountToAccountTest
    {
        const string SalaryAccount = "Lön";
        const string IncomeAccount = "Inkomst";
        const string BankAccount = "Bankkonto";
        const string GroceryAccount = "Livsmedel";
        const string RentAccount = "Hyra";

        public AccountToAccountTest()
        {

        }

        [Theory]
        [InlineData(SalaryAccount, BankAccount, 1000)]
        [InlineData(BankAccount, GroceryAccount, 50)]
        [InlineData(BankAccount, RentAccount, 250)]
        [InlineData(IncomeAccount, BankAccount, 1000)]
        [InlineData(BankAccount, RentAccount, 250)]

        public async Task Test1(string fromAcount, string toAccount, int amount)
        {
            // create accounts in database
            IDatabaseService dbService = new EFDatabaseService();
            await dbService.CreateAccountAsync(GroceryAccount, AccountType.Expense);
            await dbService.CreateAccountAsync(BankAccount, AccountType.Check);
            await dbService.CreateAccountAsync(RentAccount, AccountType.Expense);
            await dbService.CreateAccountAsync(SalaryAccount, AccountType.Income);
            await dbService.CreateAccountAsync(IncomeAccount, AccountType.Income);

            Assert.True(true);
        }
    }
    
    public class MockDatabaseService : IDatabaseService
    {
        List<ExistingAccount> accounts = new List<ExistingAccount>();
        public async Task<ServiceResult<string>> CreateAccountAsync(string name, AccountType type)
        {
            accounts.Add(new ExistingAccount
            {
                Name = name,
                Type = type,
                Amount = 0
            });
            return ServiceResult<string>.Ok("Transaction completed successfully.");
        }

        public async Task<ServiceResult<string>> CreateTransactionAsync(string fromAccount, string toAccount, int amount)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult<IEnumerable<GetAccountDTO>>> GetAllAccountsAsync()
        {
            var result = accounts
            .OrderBy(a => a.Name)
            .Select(a => new GetAccountDTO
            {
                Name = a.Name,
                Amount = a.Amount
            });
            return ServiceResult<IEnumerable<GetAccountDTO>>.Ok(result);
        }
    }
}