using BankingApp.Data.Entities;
using BankingApp.Data.Entities.Common.Enums;
using BankingApp.Data.Entities.Common.ValueObjects;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Payment;
using BankingApp.Services.Payment.Dtos;
using MockQueryable;
using NSubstitute;
using Shouldly;

namespace BankingApp.Tests.Payment.Services;

public class GetTransactionsUnitTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PaymentService _paymentService;

    public GetTransactionsUnitTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _paymentService = new PaymentService(_unitOfWork);
    }

    [Fact]
    public async Task GivenTransactionsExist_ShouldReturnPaginatedTransactions()
    {
        // Arrange
        var userId = 1L;
        var request = new GetTransactionsRequest
        {
            Skip = 0,
            MaxCount = 10
        };

        var customer = new Customer
        {
            Id = 1,
            UserId = userId,
            FirstName = "John",
            LastName = "Doe"
        };

        var recipientCustomer = new Customer
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith"
        };

        var account = new Data.Entities.Account
        {
            Id = 1,
            Customer = customer
        };

        var recipientAccount = new Data.Entities.Account
        {
            Id = 2,
            Customer = recipientCustomer
        };

        var transactions = new List<Transaction>
            {
                new() {
                    AccountOwner = account,
                    Sender = account,
                    Recipient = recipientAccount,
                    Amount = new Money(100.00M, "USD"),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Status = TransactionStatus.Debit
                },
                new() {
                    AccountOwner = account,
                    Sender = recipientAccount,
                    Recipient = account,
                    Amount = new Money(50.00M, "USD"),
                    CreatedAt = DateTime.UtcNow,
                    Status = TransactionStatus.Credit
                }
            }.BuildMock();

        _unitOfWork.TransactionRepository
            .GetAll()
            .Returns(transactions);

        // Act
        var result = await _paymentService.GetTransactions(userId, request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.TotalCount.ShouldBe(2);
        result.Data.Items.Count.ShouldBe(2);

        var firstTransaction = result.Data.Items[0];
        firstTransaction.ShouldSatisfyAllConditions(
            () => firstTransaction.From.ShouldBe("John Doe"),
            () => firstTransaction.To.ShouldBe("Jane Smith"),
            () => firstTransaction.Amount.Amount.ShouldBe(100.00M),
            () => firstTransaction.Status.ShouldBe(TransactionStatus.Debit)
        );

        var secondTransaction = result.Data.Items[1];
        secondTransaction.ShouldSatisfyAllConditions(
            () => secondTransaction.From.ShouldBe("Jane Smith"),
            () => secondTransaction.To.ShouldBe("John Doe"),
            () => secondTransaction.Amount.Amount.ShouldBe(50.00M),
            () => secondTransaction.Status.ShouldBe(TransactionStatus.Credit)
        );
    }

    [Fact]
    public async Task GivenNoTransactions_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = 1L;
        var request = new GetTransactionsRequest
        {
            Skip = 0,
            MaxCount = 10
        };

        var emptyTransactions = new List<Transaction>().BuildMock();

        _unitOfWork.TransactionRepository
            .GetAll()
            .Returns(emptyTransactions);

        // Act
        var result = await _paymentService.GetTransactions(userId, request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.TotalCount.ShouldBe(0);
        result.Data.Items.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 5)]
    [InlineData(15, 3)]
    public async Task GivenPagination_ShouldReturnCorrectPage(int skip, int maxCount)
    {
        // Arrange
        var userId = 1L;
        var request = new GetTransactionsRequest
        {
            Skip = skip,
            MaxCount = maxCount
        };

        var customer = new Customer
        {
            Id = 1,
            UserId = userId,
            FirstName = "John",
            LastName = "Doe"
        };

        var account = new Data.Entities.Account
        {
            Id = 1,
            Customer = customer
        };

        // Create 20 test transactions
        var transactions = Enumerable.Range(1, 20)
            .Select(i => new Transaction
            {
                AccountOwner = account,
                Sender = account,
                Recipient = account,
                Amount = new Money(i * 100M, "USD"),
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                Status = TransactionStatus.Debit
            })
            .BuildMock();

        _unitOfWork.TransactionRepository
            .GetAll()
            .Returns(transactions);

        // Act
        var result = await _paymentService.GetTransactions(userId, request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.TotalCount.ShouldBe(20);
        result.Data.Items.Count.ShouldBe(Math.Min(maxCount, 20 - skip));
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(0, 0)]
    [InlineData(0, -1)]
    public async Task GivenInvalidPagination_ShouldReturnEmptyList(int skip, int maxCount)
    {
        // Arrange
        var userId = 1L;
        var request = new GetTransactionsRequest
        {
            Skip = skip,
            MaxCount = maxCount
        };

        _unitOfWork.TransactionRepository
            .GetAll()
            .Returns(new List<Transaction>().BuildMock());

        // Act
        var result = await _paymentService.GetTransactions(userId, request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.TotalCount.ShouldBe(0);
        result.Data.Items.ShouldBeEmpty();
    }
}
