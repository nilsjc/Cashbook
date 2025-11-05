using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.database
{
    public class CashBookDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public CashBookDbContext(DbContextOptions<CashBookDbContext> options) : base(options)
        {
        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlite("Data Source=cashbook.db");
        // }
    }
    
    public class Account
    {
        [Key]
        public string Name { get; set; } = "";
        public int AccountType { get; set; }
        public int Amount { get; set; }
    }
}