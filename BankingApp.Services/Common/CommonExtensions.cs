using BankingApp.Data.Entities.Common.ValueObjects;
using BankingApp.Services.Common.Dtos;

namespace BankingApp.Services.Common
{
    public static class CommonExtensions
    {
        public static Money ToMoney(this MoneyDto dto)
        {
            if (dto == null)
            {
                return new Money(0);
            }
            return new Money
            {
                Amount = dto.Amount,
                Currency = dto.Currency,
            };
        }

        public static MoneyDto ToMoneyDto(this Money money)
        {
            if (money == null)
            {
                return new MoneyDto
                {
                    Amount = 0,
                    Currency = "NGN"
                };

            }
            return new MoneyDto
            {
                Amount = money.Amount,
                Currency = money.Currency,
            };
        }
    }
}
