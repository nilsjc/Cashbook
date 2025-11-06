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
    }
    
    public partial class Account
    {
        [Key]
        public string Name { get; set; } = "";
        public int AccountType { get; set; }
        public int Amount { get; set; }
        
        // Optimistic concurrency token. EF will use this to detect concurrent updates.
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}