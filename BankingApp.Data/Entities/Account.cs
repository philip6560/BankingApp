using BankingApp.Data.Entities.Common;
using BankingApp.Data.Entities.Common.Constants;
using BankingApp.Data.Entities.Common.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.Data.Entities
{
    [Table("Accounts")]
    public class Account : BaseEntity<long>
    {
        [StringLength(ValidationConstants.AccountNumberLength, 
            MinimumLength = ValidationConstants.AccountNumberLength)]
        public string AccountNumber { get; set; }

        public Money Balance { get; set; }

        public long CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }
}
