using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.endpoints
{
    public record PostAccountDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public AccountType Type { get; set; }
    }

    public record GetAccountDTO
    {
        public string Name { get; set; }
        public int Amount { get; set; }
    }
    public class TransactionRequestDTO
    {
        [Required]
        public string FromAccount { get; set; }
        [Required]
        public string ToAccount { get; set; }
        [Required]
        public int Amount { get; set; }
    }
    public enum AccountType
    {
        Income,
        Expense,
        Check
    }

    public record ExistingAccount
    {
        public string Name { get; set; }
        public AccountType Type { get; set; }
        public int Amount { get; set; }
    }



}