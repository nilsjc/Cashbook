namespace WebAPI.database
{
    public partial class Account
    {
        public void TransferTo(Account toAccount, int amount)
        {
            // om det här är en transaktion från inkomst öka summan
            if (this.AccountType == (int)endpoints.AccountType.Income)
            {
                this.Amount += amount;
            }
            else if (this.AccountType == (int)endpoints.AccountType.Expense)
            {
                // det här kontot är en utgiftskonto men har valts för en överföring
                // om det här kontot har ett minusvärde har det använts förut
                // om minusvärdet är mindre än eller lika med amount så kan vi
                // föra över summan till ett annat konto, i fall av misstag
                if(this.Amount <= -amount)
                {
                    this.Amount += amount;
                }
            }
            else // det här kontot är ett checkkonto som vi kan föra över från
            {
                this.Amount -= amount;
            }
            
            if(toAccount.AccountType == (int)endpoints.AccountType.Check)
            {
                toAccount.Amount += amount;
            }
            else
            {
                toAccount.Amount -= amount;
            }            
        }
    }
}