namespace BankingApp.Services.Account.Dtos
{
    public record GetAccountDetailsResponse
    {
        public string FullName { get; init; }

        public string AccountNumber { get; init; }

        public string EmailAddress { get; init; }

        public string? Address { get; init; }
    }
}
