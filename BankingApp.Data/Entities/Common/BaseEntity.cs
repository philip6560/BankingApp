using BankingApp.Data.Entities.Abstractions;

namespace BankingApp.Data.Entities.Common
{
    public abstract class BaseEntity<TPrimaryKey> : IBaseEntity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }
        public virtual DateTime CreatedAt { get; set; }

        protected BaseEntity() 
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
