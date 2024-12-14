using System.ComponentModel.DataAnnotations;

namespace BankingApp.Services.Account.Dtos
{
    public record GetBeneficiaryRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; init; }
    }
}
