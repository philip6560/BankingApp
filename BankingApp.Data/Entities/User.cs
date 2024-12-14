using BankingApp.Data.Entities.Common;
using BankingApp.Data.Entities.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace BankingApp.Data.Entities
{
    public class User : BaseEntity<long>
    {
        [EmailAddress]
        [StringLength(ValidationConstants.MaxEmailLength,
            MinimumLength = ValidationConstants.MinEmailLength)]
        public string Email { get; set; }

        [StringLength(ValidationConstants.MaxPasswordLength, 
            MinimumLength = ValidationConstants.MinPasswordLength)]
        public string Password { get; set; }
    }
}
