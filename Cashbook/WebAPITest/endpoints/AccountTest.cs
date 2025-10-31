using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.database;
using WebAPI.endpoints;

namespace WebAPITest.endpoints.AccountDataTypes
{
    public class AccountTest
    {
        [Fact]
        public async Task CreateAccountWithCorruptType_ShouldReturnBadRequest()
        {
            // Arrange
            var account = new AccountDTO
            {
                Name = "TestAcccount",
                Type = (AccountType)5
            };

            IDatabaseService dbService = new EmptyDbService();


            // Act
            var result = await AccountAPI.CreateAccount(account, new WebAPI.validators.AccountCreateValidator(), dbService);

            // Assert
            Assert.NotNull(result);
            var badRequest = result as BadRequest<List<string>>;
            Assert.Equal("Invalid account type.", badRequest?.Value?[0]);
        }

        [Fact]
        public async Task CreateAccountWithNoName_ShouldReturnBadRequest()
        {
            // Arrange
            var account = new AccountDTO
            {
                Name = string.Empty,
                Type = (AccountType)0
            };

            IDatabaseService dbService = new EmptyDbService();

            // Act
            var result = await AccountAPI.CreateAccount(account, new WebAPI.validators.AccountCreateValidator(), dbService);

            // Assert
            Assert.NotNull(result);
            var badRequest = result as BadRequest<List<string>>;
            Assert.Equal("Account name is required.", badRequest?.Value?[0]);
        }
    }

    public class EmptyDbService : IDatabaseService
    {
        public Task<ServiceResult<string>> CreateAccountAsync(string name, AccountType type)
        {
            throw new NotImplementedException();
        }
    }
}