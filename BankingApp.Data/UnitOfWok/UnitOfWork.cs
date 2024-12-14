using BankingApp.Data.Entities;
using BankingApp.Data.Repositories;
using BankingApp.Data.Repositories.Abstractions;
using BankingApp.Data.UnitOfWok.Abstractions;

namespace BankingApp.Data.UnitOfWok
{
    public class UnitOfWork(
        BankingAppDbContext dbContext,
        IBaseRepository<User> userRepository,
        IBaseRepository<Customer> customerRepository,
        IBaseRepository<Account> accountRepository,
        IBaseRepository<Transaction> transactionRepository
        ) : IUnitOfWork, IDisposable
    {
        private bool disposed = false;

        public IBaseRepository<User> UserRepository 
        {
            get { return userRepository ??= new BaseRepository<User>(dbContext); }
        }

        public IBaseRepository<Customer> CustomerRepository 
        {
            get { return customerRepository ??= new BaseRepository<Customer>(dbContext); }
        }

        public IBaseRepository<Account> AccountRepository 
        {
            get { return accountRepository ??= new BaseRepository<Account>(dbContext); }
        }

        public IBaseRepository<Transaction> TransactionRepository 
        {
            get { return transactionRepository ??= new BaseRepository<Transaction>(dbContext); }
        }

        public Task SaveChangesAsync()
        {
            return dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}
