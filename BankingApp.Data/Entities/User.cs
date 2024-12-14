using BankingApp.Data.Entities.Common;
using BankingApp.Data.Entities.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.Data.Entities
{
    [Table("User")]
    public class User : BaseEntity<long>
    {
        [EmailAddress]
        [StringLength(ValidationConstants.MaxEmailLength,
            MinimumLength = ValidationConstants.MinEmailLength)]
        public string EmailAddress { get; set; }

        [StringLength(ValidationConstants.MaxPasswordLength, 
            MinimumLength = ValidationConstants.MinPasswordLength)]
        public string Password { get; set; }
    }
}
