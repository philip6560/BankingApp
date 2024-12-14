namespace BankingApp.Services.Account.Dtos
{
    public record GetBeneficiaryResponse
    {
        public string FullName { get; init; }

        public string EmailAddress { get; init; }

        public string AccountNumber { get; init; }
    }
}
