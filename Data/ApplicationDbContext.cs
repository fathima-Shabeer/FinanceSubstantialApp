using FinanceSubstantialApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagementApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed some initial data for testing
            modelBuilder.Entity<ChartOfAccount>().HasData(
                new ChartOfAccount { Id = 1, AccountNumber = "1010", AccountName = "Cash", AccountType = AccountType.Asset },
                new ChartOfAccount { Id = 2, AccountNumber = "1200", AccountName = "Accounts Receivable", AccountType = AccountType.Asset },
                new ChartOfAccount { Id = 3, AccountNumber = "2010", AccountName = "Accounts Payable", AccountType = AccountType.Liability },
                new ChartOfAccount { Id = 4, AccountNumber = "3010", AccountName = "Owner's Equity", AccountType = AccountType.Equity },
                new ChartOfAccount { Id = 5, AccountNumber = "4010", AccountName = "Sales Revenue", AccountType = AccountType.Revenue },
                new ChartOfAccount { Id = 6, AccountNumber = "5010", AccountName = "Rent Expense", AccountType = AccountType.Expense }
            );
        }
    }
}