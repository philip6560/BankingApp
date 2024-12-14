using BankingApp.Services.Account.Dtos;
using BankingApp.Services.Common.Response;

namespace BankingApp.Services.Account.Abstraction
{
    public interface IAccountService
    {
        Task<Result<bool>> CreateAccount(CreateAccountRequest request);

        Task<Result<GetAccountDetailsResponse>> GetAccountDetails(long userId, GetAccountDetailsRequest request);

        Task<Result<bool>> UpdateAccount(long userId, UpdateAccountRequest request);

        Task<Result<GetBeneficiaryResponse>> GetBeneficiary(GetBeneficiaryRequest request);
    }
}
