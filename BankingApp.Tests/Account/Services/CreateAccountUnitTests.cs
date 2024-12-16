using BankingApp.Data.Entities;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Account;
using BankingApp.Services.Account.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;

namespace BankingApp.Tests.Account.Services;

public class CreateAccountUnitTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AccountService _accountService;

    public CreateAccountUnitTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _passwordHasher = Substitute.For<IPasswordHasher<User>>();
        _accountService = new AccountService(_unitOfWork, _passwordHasher);
    }

    [Fact]
    public async Task GivenValidRequest_ShouldCreateAccountSuccessfully()
    {
        // Arrange
        var request = new CreateAccountRequest
        {
            EmailAddress = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe"
        };

        const string hashedPassword = "hashedPassword123";
        User? capturedUser = null;
        Customer? capturedCustomer = null;
        Data.Entities.Account? capturedAccount = null;

        _passwordHasher
            .HashPassword(Arg.Any<User>(), request.Password)
            .Returns(hashedPassword);

        _unitOfWork.UserRepository
            .InsertAsync(Arg.Do<User>(u => capturedUser = u))
            .Returns(Task.CompletedTask);

        _unitOfWork.CustomerRepository
            .InsertAsync(Arg.Do<Customer>(c => capturedCustomer = c))
            .Returns(Task.CompletedTask);

        _unitOfWork.AccountRepository
            .InsertAsync(Arg.Do<Data.Entities.Account>(a => capturedAccount = a))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _accountService.CreateAccount(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldBeTrue();

        // Verify User creation
        capturedUser.ShouldNotBeNull();
        capturedUser.EmailAddress.ShouldBe(request.EmailAddress);
        capturedUser.Password.ShouldBe(hashedPassword);

        await _unitOfWork.UserRepository
            .Received(1)
            .InsertAsync(Arg.Any<User>());

        // Verify Customer creation
        capturedCustomer.ShouldNotBeNull();
        capturedCustomer.FirstName.ShouldBe(request.FirstName);
        capturedCustomer.LastName.ShouldBe(request.LastName);
        capturedCustomer.User.ShouldBe(capturedUser);

        await _unitOfWork.CustomerRepository
            .Received(1)
            .InsertAsync(Arg.Any<Customer>());

        // Verify Account creation
        capturedAccount.ShouldNotBeNull();
        capturedAccount.Customer.ShouldBe(capturedCustomer);
        capturedAccount.Balance.Amount.ShouldBe(0.00M);
        capturedAccount.AccountNumber.ShouldNotBeNullOrEmpty();
        capturedAccount.AccountNumber.Length.ShouldBe(10);

        await _unitOfWork.AccountRepository
            .Received(1)
            .InsertAsync(Arg.Any<Data.Entities.Account>());

        // Verify SaveChanges was called
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAccount_WhenSaveChangesFails_ShouldThrowException()
    {
        // Arrange
        var request = new CreateAccountRequest
        {
            EmailAddress = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe"
        };

        _unitOfWork
            .SaveChangesAsync()
            .Returns(Task.FromException(new DbUpdateException("Save failed")));

        // Act & Assert
        var exception = await Should.ThrowAsync<DbUpdateException>(() =>
            _accountService.CreateAccount(request)
        );

        exception.Message.ShouldBe("Save failed");
    }
}
