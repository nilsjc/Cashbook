using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.endpoints
{
    public record AccountDTO
    {
        public string Name { get; set; }
        public AccountType Type { get; set; }
    }
    public enum AccountType
    {
        Income,
        Expense,
        Check
    }

}