using BankingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Data.EntityConfigurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> modelBuilder)
        {
            modelBuilder.HasIndex(x => x.AccountNumber).IsUnique();

            modelBuilder.OwnsOne(x => x.Balance).WithOwner();
        }
    }
}
