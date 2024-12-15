using BankingApp.Data.Entities;
using BankingApp.Data.Entities.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data
{
    public class BankingAppDbContext(DbContextOptions<BankingAppDbContext> options) : DbContext(options)
    {

        public DbSet<User> Users { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingAppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresEnum<TransactionStatus>();
        }
    }
}
