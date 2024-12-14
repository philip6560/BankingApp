namespace BankingApp.Services.Account.Dtos
{
    public record GetAccountDetailsRequest
    {
        public long UserId { get; init; }
    }
}
