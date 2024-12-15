using BankingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Data.EntityConfigurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> modelBuilder)
        {
            modelBuilder.HasIndex(x => x.ReferenceNumber);

            modelBuilder.OwnsOne(x => x.Amount).WithOwner();
        }
    }
}
