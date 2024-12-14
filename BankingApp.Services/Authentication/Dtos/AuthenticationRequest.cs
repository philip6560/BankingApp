using System.ComponentModel.DataAnnotations;

namespace BankingApp.Services.Authentication.Dtos
{
    public record AuthenticationRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; init; }

        [Required]
        public string Password { get; init; }
    }
}
