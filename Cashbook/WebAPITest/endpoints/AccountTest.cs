using WebAPI.endpoints;

namespace WebAPITest.endpoints.AccountDataTypes
{
    public class AccountTest
    {
        [Fact]
        public async Task CreateAccount_ShouldReturnOk()
        {
            // Arrange
            var account = new AccountDTO
            {
                Name = "Nisses konto",
                Type = (AccountType)5
            };

            // Act
            var result = await Account.CreateAccount(account);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<AccountDTO>>(result);
        }
    }
}