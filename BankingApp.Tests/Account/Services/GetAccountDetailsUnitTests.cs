using BankingApp.Data.Entities;
using BankingApp.Data.Entities.Common.ValueObjects;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Account;
using BankingApp.Services.Account.Dtos;
using BankingApp.Services.Account.Utils;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using NSubstitute;
using Shouldly;

namespace BankingApp.Tests.Account.Services;

public class GetAccountDetailsUnitTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AccountService _accountService;

    public GetAccountDetailsUnitTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _passwordHasher = Substitute.For<IPasswordHasher<User>>();
        _accountService = new AccountService(_unitOfWork, _passwordHasher);
    }

    [Fact]
    public async Task GivenAccountExists_ShouldReturnAccountDetails()
    {
        // Arrange
        var userId = 1L;
        var request = new GetAccountDetailsRequest { UserId = userId };

        _unitOfWork.UserRepository.GetAll().Returns(GetUsers(userId));

        _unitOfWork.CustomerRepository.GetAll().Returns(GetCustomers(userId));

        _unitOfWork.AccountRepository.GetAll().Returns(GetAccounts());

        // Act
        var result = await _accountService.GetAccountDetails(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldSatisfyAllConditions(
            () => result.Data.FullName.ShouldBe("John Doe"),
            () => result.Data.EmailAddress.ShouldBe("test@example.com"),
            () => result.Data.AccountNumber.ShouldBe("1234567890"),
            () => result.Data.Address.ShouldBe("123 Test St"),
            () => result.Data.CurrentBalance.Amount.ShouldBe(1000.00M)
        );
    }

    [Fact]
    public async Task GivenAccountDoesNotExist_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = 999L;
        var request = new GetAccountDetailsRequest { UserId = userId };

        var emptyQueryable = new List<User>().BuildMock();

        _unitOfWork.UserRepository
            .GetAll()
            .Returns(emptyQueryable);

        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(new List<Customer>().BuildMock());

        _unitOfWork.AccountRepository
            .GetAll()
            .Returns(new List<Data.Entities.Account>().BuildMock());

        // Act
        var result = await _accountService.GetAccountDetails(request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(AccountServiceErrors.AccountNotFound("Details").Description);
    }

    private static IQueryable<User> GetUsers(long userId) 
        => new List<User>
        {
            new() {
                Id = userId,
                EmailAddress = "test@example.com"
            }
        }.BuildMock();

    private static IQueryable<Customer> GetCustomers(long userId)
        => new List<Customer>
        {
            new() {
                Id = 1,
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Test St"
            }
        }.BuildMock();

    private static IQueryable<Data.Entities.Account> GetAccounts()
        => new List<Data.Entities.Account>
        {
            new() {
                Id = 1,
                CustomerId = 1,
                Balance = new Money(1000.00M),
                AccountNumber = "1234567890"
            }
        }.BuildMock();
}
