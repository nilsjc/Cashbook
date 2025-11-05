namespace WebAPI.database
{
    public partial class Account
    {
        public void TransferTo(Account toAccount, int amount)
        {
            if (this.AccountType == (int)endpoints.AccountType.Income)
            {
                this.Amount += amount;
            }else
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