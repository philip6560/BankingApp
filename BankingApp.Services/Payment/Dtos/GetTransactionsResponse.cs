using BankingApp.Data.Entities.Common.Enums;

namespace BankingApp.Services.Payment.Dtos
{
    public record GetTransactionsResponse
    {
        public string From { get; init; }

        public string To { get; init; }

        public decimal Amount { get; init; }

        public TransactionStatus Status { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}
