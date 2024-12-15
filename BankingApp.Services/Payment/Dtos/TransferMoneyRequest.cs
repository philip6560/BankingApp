using BankingApp.Data.Entities.Common.Constants;
using BankingApp.Services.Common.Dtos;
using System.ComponentModel.DataAnnotations;

namespace BankingApp.Services.Payment.Dtos
{
    public record TransferMoneyRequest
    {
        [StringLength(ValidationConstants.AccountNumberLength,
            MinimumLength = ValidationConstants.AccountNumberLength)]
        public string AccountNumber { get; init; }

        public MoneyDto Amount { get; init; }
    }
}
