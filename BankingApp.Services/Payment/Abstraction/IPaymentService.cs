using BankingApp.Services.Common.Dtos;
using BankingApp.Services.Common.Response;
using BankingApp.Services.Payment.Dtos;

namespace BankingApp.Services.Payment.Abstraction
{
    public interface IPaymentService
    {
        Task<Result<bool>> TransferMoney(long userId, TransferMoneyRequest request);

        Task<Result<PaginatedResponse<GetTransactionsResponse>>> GetTransactions(long userId, GetTransactionsRequest request);
    }
}
