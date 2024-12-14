namespace BankingApp.Services.Authentication.Dtos
{
    public class TokenGenerationRequest
    {
        public long UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}
