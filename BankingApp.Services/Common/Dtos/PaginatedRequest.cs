namespace BankingApp.Services.Common.Dtos
{
    public class PaginatedRequest
    {
        public int Skip { get; set; } = 0;

        public int MaxCount { get; set; } = 10;
    }
}
