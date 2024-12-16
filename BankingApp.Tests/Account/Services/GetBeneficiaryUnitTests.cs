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

public class GetBeneficiaryUnitTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AccountService _accountService;

    public GetBeneficiaryUnitTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _passwordHasher = Substitute.For<IPasswordHasher<User>>();
        _accountService = new AccountService(_unitOfWork, _passwordHasher);
    }

    [Fact]
    public async Task GivenBeneficiaryExists_ShouldReturnBeneficiaryDetails()
    {
        // Arrange
        var request = new GetBeneficiaryRequest
        {
            EmailAddress = "beneficiary@example.com"
        };

        var user = new User
        {
            Id = 1,
            EmailAddress = request.EmailAddress
        };

        var customer = new Customer
        {
            Id = 1,
            UserId = user.Id,
            FirstName = "John",
            LastName = "Doe"
        };

        var account = new Data.Entities.Account
        {
            Id = 1,
            CustomerId = customer.Id,
            AccountNumber = "1234567890",
            Balance = new Money(1000.00M)
        };

        _unitOfWork.UserRepository
            .GetAll()
            .Returns(new[] { user }.BuildMock());

        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(new[] { customer }.BuildMock());

        _unitOfWork.AccountRepository
            .GetAll()
            .Returns(new[] { account }.BuildMock());

        // Act
        var result = await _accountService.GetBeneficiary(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldSatisfyAllConditions(
            () => result.Data.FullName.ShouldBe("John Doe"),
            () => result.Data.EmailAddress.ShouldBe(request.EmailAddress),
            () => result.Data.AccountNumber.ShouldBe("1234567890")
        );
    }

    [Fact]
    public async Task GivenBeneficiaryNotFound_ShouldReturnFailure()
    {
        // Arrange
        var request = new GetBeneficiaryRequest
        {
            EmailAddress = "nonexistent@example.com"
        };

        _unitOfWork.UserRepository
            .GetAll()
            .Returns(new List<User>().BuildMock());

        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(new List<Customer>().BuildMock());

        _unitOfWork.AccountRepository
            .GetAll()
            .Returns(new List<Data.Entities.Account>().BuildMock());

        // Act
        var result = await _accountService.GetBeneficiary(request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(AccountServiceErrors.BeneficiaryNotFound.Description);
    }
}
