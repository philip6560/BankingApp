using System.ComponentModel.DataAnnotations;

namespace BankingApp.Services.Common.Dtos
{
    public record MoneyDto
    {
        public MoneyDto()
        {

        }
        public MoneyDto(decimal amount, string currency = "NGN")
        {
            Amount = amount;
            Currency = currency;
        }

        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a valid price with at most 2 decimal places.")]
        public decimal Amount { get; set; }
        [Required]
        public string Currency { get; set; }
    }
}
