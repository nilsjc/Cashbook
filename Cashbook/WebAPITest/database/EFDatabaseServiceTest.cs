// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using WebAPI.endpoints;

// namespace WebAPITest.database
// {
//     public class EFDatabaseServiceTest
//     {
//         [Fact]
//         public async Task CreateTransactionAsync_InsufficientFunds_ReturnsFailure()
//         {
//             // Arrange
//             var dbService = new EFDatabaseService();
//             var fromAccount = "ExpenseAccount";
//             var toAccount = "IncomeAccount";
//             var initialFromAmount = 50;
//             var transactionAmount = 100;

//             // Create accounts
//             await dbService.CreateAccountAsync(fromAccount, AccountType.Expense);
//             await dbService.CreateAccountAsync(toAccount, AccountType.Income);

//     }
// }