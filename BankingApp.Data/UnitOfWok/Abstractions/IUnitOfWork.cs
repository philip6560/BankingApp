using BankingApp.Data.Entities;
using BankingApp.Data.Repositories.Abstractions;

namespace BankingApp.Data.UnitOfWok.Abstractions
{
    public interface IUnitOfWork
    {
        IBaseRepository<User> UserRepository { get; }

        IBaseRepository<Customer> CustomerRepository { get; }

        IBaseRepository<Account> AccountRepository { get; }

        IBaseRepository<Transaction> TransactionRepository { get; }

        Task SaveChangesAsync();
    }
}
