using BankingApp.Data.Entities.Common;
using BankingApp.Data.Entities.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.Data.Entities
{
    public class Customer : BaseEntity<long>
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [StringLength(ValidationConstants.MaxNameLength,
            MinimumLength = ValidationConstants.MinNameLength)]
        public string FirstName { get; set; }

        [StringLength(ValidationConstants.MaxNameLength,
            MinimumLength = ValidationConstants.MinNameLength)]
        public string LastName { get; set; }

        [StringLength(ValidationConstants.MaxAddressLength,
            MinimumLength = ValidationConstants.MinAddressLength)]
        public string? Address { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }
}
