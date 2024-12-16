using BankingApp.Data.Entities;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Account;
using BankingApp.Services.Account.Dtos;
using BankingApp.Services.Account.Utils;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using NSubstitute;
using Shouldly;

namespace BankingApp.Tests.Account.Services;

public class UpdateAccountUnitTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AccountService _accountService;

    public UpdateAccountUnitTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _passwordHasher = Substitute.For<IPasswordHasher<User>>();
        _accountService = new AccountService(_unitOfWork, _passwordHasher);
    }

    [Fact]
    public async Task GivenAllFieldsChanged_ShouldUpdateSuccessfully()
    {
        // Arrange
        var userId = 1L;
        var request = new UpdateAccountRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Address = "456 New St"
        };

        var existingCustomer = new Customer
        {
            Id = 1,
            UserId = userId,
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Old St"
        };

        var customers = new List<Customer> { existingCustomer }.BuildMock();
        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(customers);

        // Act
        var result = await _accountService.UpdateAccount(userId, request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldBeTrue();

        await _unitOfWork.CustomerRepository
            .Received(1)
            .UpdateAsync(Arg.Is<Customer>(c =>
                c.FirstName == request.FirstName &&
                c.LastName == request.LastName &&
                c.Address == request.Address));

        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GivenNoChangesNeeded_ShouldNotUpdateDatabase()
    {
        // Arrange
        var userId = 1L;
        var existingCustomer = new Customer
        {
            Id = 1,
            UserId = userId,
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Old St"
        };

        var request = new UpdateAccountRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Old St"
        };

        var customers = new List<Customer> { existingCustomer }.BuildMock();
        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(customers);

        // Act
        var result = await _accountService.UpdateAccount(userId, request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldBeTrue();

        await _unitOfWork.CustomerRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Customer>());

        await _unitOfWork
            .DidNotReceive()
            .SaveChangesAsync();
    }

    [Fact]
    public async Task GivenCustomerNotFound_ShouldReturnFailure()
    {
        // Arrange
        var userId = 999L;
        var request = new UpdateAccountRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Address = "456 New St"
        };

        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(new List<Customer>().BuildMock());

        // Act
        var result = await _accountService.UpdateAccount(userId, request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(AccountServiceErrors.AccountNotFound("Update").Description);

        await _unitOfWork.CustomerRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Customer>());

        await _unitOfWork
            .DidNotReceive()
            .SaveChangesAsync();
    }

    [Theory]
    [InlineData("Jane", null, null)]
    [InlineData(null, "Smith", null)]
    [InlineData(null, null, "456 New St")]
    [InlineData("Jane", "Smith", null)]
    [InlineData(null, "Smith", "456 New St")]
    [InlineData("Jane", null, "456 New St")]
    public async Task GivenPartialUpdate_ShouldOnlyUpdateChangedFields(
        string firstName,
        string lastName,
        string address)
    {
        // Arrange
        var userId = 1L;
        var existingCustomer = new Customer
        {
            Id = 1,
            UserId = userId,
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Old St"
        };

        var request = new UpdateAccountRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Address = address
        };

        var customers = new List<Customer> { existingCustomer }.BuildMock();
        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(customers);

        Customer? capturedCustomer = null;
        await _unitOfWork.CustomerRepository
            .UpdateAsync(Arg.Do<Customer>(c => capturedCustomer = c));

        // Act
        var result = await _accountService.UpdateAccount(userId, request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldBeTrue();

        capturedCustomer.ShouldNotBeNull();

        if (firstName != null)
        {
            capturedCustomer.FirstName.ShouldBe(firstName);
        }
        else
        {
            capturedCustomer.FirstName.ShouldBe(existingCustomer.FirstName);
        }

        if (lastName != null)
        {
            capturedCustomer.LastName.ShouldBe(lastName);
        }
        else
        {
            capturedCustomer.LastName.ShouldBe(existingCustomer.LastName);
        }

        if (address != null)
        {
            capturedCustomer.Address.ShouldBe(address);
        }
        else
        {
            capturedCustomer.Address.ShouldBe(existingCustomer.Address);
        }

        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}
