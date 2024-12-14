namespace BankingApp.Data.Entities.Abstractions
{
    public interface IBaseEntity<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
