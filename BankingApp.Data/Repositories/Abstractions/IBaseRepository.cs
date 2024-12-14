namespace BankingApp.Data.Repositories.Abstractions
{
    public interface IBaseRepository<TEntity>
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> GetByIdAsync(object id);

        Task InsertAsync(TEntity entity);

        Task UpdateAsync(TEntity entityToUpdate);

        Task DeleteAsync(object id);

        Task DeleteAsync(TEntity entityToDelete);
    }
}
