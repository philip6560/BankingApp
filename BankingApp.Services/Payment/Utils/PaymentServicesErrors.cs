using BankingApp.Services.Common.Response;

namespace BankingApp.Services.Payment.Utils
{
    public static class PaymentServicesErrors
    {
        public static Error AccountNotFound(string accountNumber)
            => new("Payment.TransferMoney", $"Account with account number {accountNumber} does not exist.");

        public static Error CurrencyMismatch(string senderCurrency, string recipientCurrency) =>
            new("Payment.TransferMoney",
                $"Transfer request cannot be made between sender " +
                $"currency {senderCurrency} and recipient currency {recipientCurrency}");

        public static Error InsufficientFunds =
            new("Payment.TransferMoney", "Insufficient funds.");
    }
}
