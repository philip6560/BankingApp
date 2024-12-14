using BankingApp.Services.Common.Response;

namespace BankingApp.Services.Account.Utils
{
    public static class AccountServiceErrors
    {
        public static readonly Error BeneficiaryNotFound =
            new($"Account.Beneficiary", "Beneficiary account does not exist.");

        public static Error AccountNotFound (string source) => 
            new($"Account.{source}", "Account does not exist.");
    }
}
