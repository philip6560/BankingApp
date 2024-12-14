using BankingApp.Data.Entities.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace BankingApp.Services.Account.Dtos
{
    public record CreateAccountRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(ValidationConstants.MaxEmailLength)]
        public string EmailAddress { get; init; }
        
        [Required]
        [MaxLength(ValidationConstants.MaxNameLength)]
        public string FirstName { get; init; }
        
        [Required]
        [MaxLength(ValidationConstants.MaxNameLength)]
        public string LastName { get; init; }
        
        [Required]
        [StringLength(ValidationConstants.MaxPasswordLength,
            MinimumLength = ValidationConstants.MinPasswordLength)]
        public string Password { get; init; }
    }
}
