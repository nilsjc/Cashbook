using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.endpoints
{
    public record PostAccountDTO
    {
        public string Name { get; set; }
        public AccountType Type { get; set; }
    }

    public record GetAccountDTO
    {
        public string Name { get; set; }
        public int Amount { get; set; }
    }
    public enum AccountType
    {
        Income,
        Expense,
        Check
    }

}