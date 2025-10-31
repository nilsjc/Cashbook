using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

            // Act
            var result = await AccountAPI.CreateAccount(account, new WebAPI.validators.AccountCreateValidator());

            // Assert
            Assert.NotNull(result);
            var badRequest = result as BadRequest<List<string>>;
            Assert.Equal("Invalid account type.", badRequest?.Value?[0]);
        }
    }
}