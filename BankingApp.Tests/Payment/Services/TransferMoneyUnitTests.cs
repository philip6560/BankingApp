using BankingApp.Data.Entities.Common.ValueObjects;
using BankingApp.Data.Entities;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Common.Dtos;
using BankingApp.Services.Payment.Dtos;
using BankingApp.Services.Payment.Utils;
using BankingApp.Services.Payment;
using NSubstitute;
using Shouldly;
using BankingApp.Data.Entities.Common.Enums;
using MockQueryable;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ExceptionExtensions;

namespace BankingApp.Tests.Payment.Services;

public class TransferMoneyUnitTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PaymentService _paymentService;

    public TransferMoneyUnitTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _paymentService = new PaymentService(_unitOfWork);
    }

    [Fact]
    public async Task GivenValidRequest_ShouldTransferSuccessfully()
    {
        // Arrange
        var userId = 1L;
        var request = new TransferMoneyRequest
        {
            AccountNumber = "1234567890",
            Amount = new MoneyDto { Amount = 100.00M, Currency = "NGN" }
        };

        var senderAccount = GetSenderAccount(userId);

        var recipientAccount = GetRecipientAccount(accountNumber: request.AccountNumber);

        var accounts = new List<Data.Entities.Account>
            {
                senderAccount,
                recipientAccount
            }.BuildMock();

        _unitOfWork.AccountRepository
            .GetAll()
            .Returns(accounts);

        Transaction? capturedSenderTransaction = null;
        Transaction? capturedRecipientTransaction = null;

        await _unitOfWork.TransactionRepository
            .InsertAsync(Arg.Do<Transaction>(t =>
            {
                if (t.Status == TransactionStatus.Debit)
                    capturedSenderTransaction = t;
                else
                    capturedRecipientTransaction = t;
            }));

        // Act
        var result = await _paymentService.TransferMoney(userId, request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldBeTrue();

        // Verify account updates
        await _unitOfWork.AccountRepository
            .Received(2)
            .UpdateAsync(Arg.Any<Data.Entities.Account>());

        senderAccount.Balance.Amount.ShouldBe(400.00M);
        recipientAccount.Balance.Amount.ShouldBe(300.00M);

        // Verify transaction entries
        await _unitOfWork.TransactionRepository
            .Received(2)
            .InsertAsync(Arg.Any<Transaction>());

        capturedSenderTransaction.ShouldNotBeNull();
        capturedSenderTransaction.ShouldSatisfyAllConditions(
            () => capturedSenderTransaction.AccountOwnerId.ShouldBe(senderAccount.Id),
            () => capturedSenderTransaction.SenderId.ShouldBe(senderAccount.Id),
            () => capturedSenderTransaction.RecipientId.ShouldBe(recipientAccount.Id),
            () => capturedSenderTransaction.Amount.Amount.ShouldBe(request.Amount.Amount),
            () => capturedSenderTransaction.Status.ShouldBe(TransactionStatus.Debit)
        );

        capturedRecipientTransaction.ShouldNotBeNull();
        capturedRecipientTransaction.ShouldSatisfyAllConditions(
            () => capturedRecipientTransaction.AccountOwnerId.ShouldBe(recipientAccount.Id),
            () => capturedRecipientTransaction.SenderId.ShouldBe(senderAccount.Id),
            () => capturedRecipientTransaction.RecipientId.ShouldBe(recipientAccount.Id),
            () => capturedRecipientTransaction.Amount.Amount.ShouldBe(request.Amount.Amount),
            () => capturedRecipientTransaction.Status.ShouldBe(TransactionStatus.Credit),
            () => capturedRecipientTransaction.ReferenceNumber.ShouldBe(capturedSenderTransaction.ReferenceNumber)
        );

        await _unitOfWork
            .Received(1)
            .SaveChangesAsync();
    }

    [Fact]
    public async Task GivenRecipientNotFound_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1L;
        var request = new TransferMoneyRequest
        {
            AccountNumber = "nonexistent",
            Amount = new MoneyDto { Amount = 100.00M, Currency = "NGN" }
        };

        var senderAccount = GetSenderAccount(userId);

        var accounts = new List<Data.Entities.Account> { senderAccount }.BuildMock();

        _unitOfWork.AccountRepository
            .GetAll()
            .Returns(accounts);

        // Act
        var result = await _paymentService.TransferMoney(userId, request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(PaymentServicesErrors.AccountNotFound(request.AccountNumber).Description);

        await _unitOfWork.AccountRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Data.Entities.Account>());

        await _unitOfWork.TransactionRepository
            .DidNotReceive()
            .InsertAsync(Arg.Any<Transaction>());
    }

    [Fact]
    public async Task GivenInsufficientFunds_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1L;
        var request = new TransferMoneyRequest
        {
            AccountNumber = "1234567890",
            Amount = new MoneyDto { Amount = 1000.00M, Currency = "NGN" }
        };

        var senderAccount = GetSenderAccount(userId);

        var recipientAccount = GetRecipientAccount(accountNumber: request.AccountNumber);

        var accounts = new List<Data.Entities.Account>
            {
                senderAccount,
                recipientAccount
            }.BuildMock();

        _unitOfWork.AccountRepository
            .GetAll()
            .Returns(accounts);

        // Act
        var result = await _paymentService.TransferMoney(userId, request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(PaymentServicesErrors.InsufficientFunds.Description);
    }

    [Fact]
    public async Task GivenCurrencyMismatch_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1L;
        var request = new TransferMoneyRequest
        {
            AccountNumber = "1234567890",
            Amount = new MoneyDto { Amount = 100.00M, Currency = "EUR" }
        };

        var senderAccount = GetSenderAccount(userId);

        var recipientAccount = GetRecipientAccount(accountNumber: request.AccountNumber);

        var accounts = new List<Data.Entities.Account>
            {
                senderAccount,
                recipientAccount
            }.BuildMock();

        _unitOfWork.AccountRepository
            .GetAll()
            .Returns(accounts);

        // Act
        var result = await _paymentService.TransferMoney(userId, request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(
            PaymentServicesErrors.CurrencyMismatch(senderAccount.Balance.Currency, request.Amount.Currency).Description
        );
    }

    [Fact]
    public async Task GivenConcurrencyConflict_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1L;
        var request = new TransferMoneyRequest
        {
            AccountNumber = "1234567890",
            Amount = new MoneyDto { Amount = 100.00M, Currency = "NGN" }
        };

        var senderAccount = GetSenderAccount(userId);

        var recipientAccount = GetRecipientAccount(accountNumber: request.AccountNumber);

        var accounts = new List<Data.Entities.Account>
            {
                senderAccount,
                recipientAccount
            }.BuildMock();

        _unitOfWork.AccountRepository
            .GetAll()
            .Returns(accounts);

        _unitOfWork.SaveChangesAsync()
            .ThrowsAsync(new DbUpdateConcurrencyException());

        // Act
        var result = await _paymentService.TransferMoney(userId, request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(PaymentServicesErrors.ConcurrencyUpdate.Description);
    }

    private Data.Entities.Account GetSenderAccount(long userId) 
    {
        return new Data.Entities.Account
        {
            Id = 1,
            Customer = new Customer { UserId = userId },
            AccountNumber = "0987654321",
            Balance = new Money(500.00M, "NGN")
        };
    }
    
    private Data.Entities.Account GetRecipientAccount(string accountNumber, bool addCustomer = true) 
    {
        var account = new Data.Entities.Account
        {
            Id = 2,
            AccountNumber = accountNumber,
            Balance = new Money(200.00M, "NGN")
        };

        if (addCustomer) 
        {
            account.Customer = new Customer { UserId = 2 };
        }

        return account;
    }
}
