namespace BankingApp.Services.Common.Dtos
{
    public class PaginatedResponse<T>
    {
        public long TotalCount { get; set; }

        public ICollection<T> Items { get; set; } = [];
    }
}
