using BankingApp.Data.Entities;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Common;
using BankingApp.Services.Common.Dtos;
using BankingApp.Services.Common.Response;
using BankingApp.Services.Payment.Abstraction;
using BankingApp.Services.Payment.Dtos;
using Microsoft.EntityFrameworkCore;
using BankingApp.Data.Entities.Common.Enums;
using BankingApp.Services.Payment.Utils;

namespace BankingApp.Services.Payment
{
    public class PaymentService(IUnitOfWork unitOfWork) : IPaymentService
    {
        public async Task<Result<PaginatedResponse<GetTransactionsResponse>>> GetTransactions(long userId, GetTransactionsRequest request)
        {
            var query = unitOfWork.TransactionRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.AccountOwner.Customer.UserId == userId)
                .Select(x => new GetTransactionsResponse
                {
                    From = x.Sender.Customer.FullName,
                    To = x.Recipient.Customer.FullName,
                    Amount = x.Amount.Amount,
                    CreatedAt = x.CreatedAt,
                    Status = x.Status,
                });

            var totalCount = await query.CountAsync();

            var result = totalCount <= 0 ? [] : await query.Skip(request.Skip)
                .Take(request.MaxCount).ToListAsync();

            return Result<PaginatedResponse<GetTransactionsResponse>>
                .Success(new PaginatedResponse<GetTransactionsResponse> { TotalCount = totalCount, Items = result});
        }

        public async Task<Result<bool>> TransferMoney(long userId, TransferMoneyRequest request)
        {
            var accounts = await unitOfWork.AccountRepository
                .GetAll()
                .Where(x => x.Customer.UserId == userId || x.AccountNumber == request.AccountNumber)
                .ToListAsync();

            var recipientAccount = accounts.Where(x => x.AccountNumber == request.AccountNumber).FirstOrDefault();

            if (recipientAccount == null) 
            {
                return Result<bool>
                    .Failure(PaymentServicesErrors.AccountNotFound(request.AccountNumber));
            }

            var senderAccount = accounts.Where(x => x.AccountNumber != request.AccountNumber).First();

            if (senderAccount!.Balance.Currency != request.Amount.Currency) 
            {
                return Result<bool>
                    .Failure(PaymentServicesErrors.CurrencyMismatch(senderAccount.Balance.Currency, request.Amount.Currency));
            }

            if (senderAccount!.Balance < request.Amount.ToMoney()) 
            {
                return Result<bool>.Failure(PaymentServicesErrors.InsufficientFunds);
            }

            await InitiateMoneyTransfer(senderAccount, recipientAccount, request);

            await CreateTransactionEntry(senderAccount, recipientAccount, request);

            await unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        private async Task InitiateMoneyTransfer(
            Data.Entities.Account senderAccount,
            Data.Entities.Account recipientAccount,
            TransferMoneyRequest request)
        {
            var lastModifiedAt = DateTime.UtcNow;
            senderAccount.Balance -= request.Amount.ToMoney();
            senderAccount.LastModifiedAt = lastModifiedAt;
            await unitOfWork.AccountRepository.UpdateAsync(senderAccount);

            recipientAccount.Balance += request.Amount.ToMoney();
            recipientAccount.LastModifiedAt = lastModifiedAt;
            await unitOfWork.AccountRepository.UpdateAsync(recipientAccount);
        }

        private async Task CreateTransactionEntry(
            Data.Entities.Account senderAccount,
            Data.Entities.Account recipientAccount,
            TransferMoneyRequest request) 
        {
            var senderTransactionEntry = new Transaction
            {
                AccountOwnerId = senderAccount.Id,
                SenderId = senderAccount.Id,
                RecipientId = recipientAccount.Id,
                Amount = request.Amount.ToMoney(),
                Status = TransactionStatus.Debit,
                ReferenceNumber = GenerateTransactionReferenceNumber(),
            };
            await unitOfWork.TransactionRepository.InsertAsync(senderTransactionEntry);


            var recipientTransactionEntry = new Transaction
            {
                AccountOwnerId = recipientAccount.Id,
                SenderId = senderAccount.Id,
                RecipientId = recipientAccount.Id,
                Amount = request.Amount.ToMoney(),
                Status = TransactionStatus.Credit,
                ReferenceNumber = senderTransactionEntry.ReferenceNumber,
            };
            await unitOfWork.TransactionRepository.InsertAsync(recipientTransactionEntry);
        }

        private string GenerateTransactionReferenceNumber() => $"{Guid.NewGuid().ToString().Substring(1, 17).Replace("_", "-")}";
    }
}
