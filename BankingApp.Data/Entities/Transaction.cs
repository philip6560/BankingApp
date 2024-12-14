using BankingApp.Data.Entities.Common;
using BankingApp.Data.Entities.Common.Constants;
using BankingApp.Data.Entities.Common.Enums;
using BankingApp.Data.Entities.Common.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.Data.Entities
{
    public class Transaction : BaseEntity<long>
    {
        [StringLength(ValidationConstants.ReferenceNumberLength, 
            MinimumLength = ValidationConstants.ReferenceNumberLength)]
        public string ReferenceNumber { get; set; }

        public Money Amount { get; set; }

        public long AccountOwnerId { get; set; }

        [ForeignKey(nameof(AccountOwnerId))]
        public Account AccountOwner { get; set; }

        public long SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public Account Sender { get; set; }

        public long RecipientId { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public Account Recipient { get; set; }

        public TransactionStatus Status { get; set; }
    }
}
