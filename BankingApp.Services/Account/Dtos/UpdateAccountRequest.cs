namespace BankingApp.Services.Account.Dtos
{
    public record UpdateAccountRequest
    {
        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? Address { get; init; }
    }
}
