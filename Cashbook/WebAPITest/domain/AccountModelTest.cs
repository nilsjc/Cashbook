using WebAPI.database;

namespace WebAPITest.domain
{
    public class AccountTest
    {
        [Fact]
        public void Income_To_Expense_Transaction()
        {
            Account fromAccount = new Account
            {
                Name = "Income",
                AccountType = 0, // income
                Amount = 0
            };
            Account toAccount = new Account
            {
                Name = "Expense",
                AccountType = 1, // expense
                Amount = 0
            };
            // Simulera överföring
            fromAccount.TransferTo(toAccount, 500);
            // Kontrollera saldon efter överföring
            Assert.Equal(500, fromAccount.Amount);
            Assert.Equal(-500, toAccount.Amount);
        }
        [Fact]
        public void Expense_To_Expense_Transaction()
        {
            Account fromAccount = new Account
            {
                Name = "Expense_1",
                AccountType = 1, // income
                Amount = -500
            };
            Account toAccount = new Account
            {
                Name = "Expense_2",
                AccountType = 1, // expense
                Amount = 0
            };
            // Simulera överföring
            fromAccount.TransferTo(toAccount, 500);
            // Kontrollera saldon efter överföring
            Assert.Equal(0, fromAccount.Amount);
            Assert.Equal(-500, toAccount.Amount);
        }
        [Fact]
        public void Income_To_Check_Transaction()
        {
            Account fromAccount1 = new Account
            {
                Name = "Income",
                AccountType = 0, // income
                Amount = 0
            };
            Account fromAccount2 = new Account
            {
                Name = "Income",
                AccountType = 0, // income
                Amount = 0
            };
            Account myBankAccount = new Account
            {
                Name = "Check",
                AccountType = 2, // check
                Amount = 0
            };
            // Simulera överföring
            fromAccount1.TransferTo(myBankAccount, 500);
            fromAccount2.TransferTo(myBankAccount, 500);
            // Kontrollera saldon efter överföring
            Assert.Equal(500, fromAccount1.Amount);
            Assert.Equal(500, fromAccount2.Amount);
            Assert.Equal(1000, myBankAccount.Amount);
        }
    }
}