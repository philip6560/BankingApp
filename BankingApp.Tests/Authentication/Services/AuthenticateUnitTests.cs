using BankingApp.Data.Entities;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Authentication;
using BankingApp.Services.Authentication.Dtos;
using BankingApp.Services.Authentication.Utils;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using NSubstitute;
using Shouldly;

namespace BankingApp.Tests.Authentication.Services;

public class AuthenticateUnitTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationService _authenticationService;

    public AuthenticateUnitTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _passwordHasher = Substitute.For<IPasswordHasher<User>>();
        _authenticationService = new AuthenticationService(_unitOfWork, _passwordHasher);
    }

    [Fact]
    public async Task GivenValidCredentials_ShouldReturnTokenGenerationRequest()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            EmailAddress = "test@example.com",
            Password = "Password123!"
        };

        var user = GetUser(request.EmailAddress);

        var customer = GetCustomer(user.Id);

        var queryResult = new[]
        {
            new
            {
                UserId = user.Id,
                customer.FirstName,
                customer.LastName,
                user.EmailAddress,
                HashedPassword = user.Password
            }
        }.BuildMock();

        _unitOfWork.UserRepository
            .GetAll()
            .Returns(new[] { user }.BuildMock());

        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(new[] { customer }.BuildMock());

        _passwordHasher
            .VerifyHashedPassword(
                Arg.Is<User>(u => u.EmailAddress == request.EmailAddress),
                user.Password,
                request.Password)
            .Returns(PasswordVerificationResult.Success);

        // Act
        var result = await _authenticationService.Authenticate(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldSatisfyAllConditions(
            () => result.Data.UserId.ShouldBe(user.Id),
            () => result.Data.EmailAddress.ShouldBe(user.EmailAddress),
            () => result.Data.FirstName.ShouldBe(customer.FirstName),
            () => result.Data.LastName.ShouldBe(customer.LastName)
        );
    }

    [Fact]
    public async Task GivenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            EmailAddress = "nonexistent@example.com",
            Password = "Password123!"
        };

        _unitOfWork.UserRepository
            .GetAll()
            .Returns(new List<User>().BuildMock());

        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(new List<Customer>().BuildMock());

        // Act
        var result = await _authenticationService.Authenticate(request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(AuthenticationServiceErrors.InvalidEmailOrPassword.Description);

        _passwordHasher
            .DidNotReceive()
            .VerifyHashedPassword(
                Arg.Any<User>(),
                Arg.Any<string>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task GivenInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            EmailAddress = "test@example.com",
            Password = "WrongPassword123!"
        };

        var user = GetUser(request.EmailAddress);

        var customer = GetCustomer(user.Id);

        _unitOfWork.UserRepository
            .GetAll()
            .Returns(new[] { user }.BuildMock());

        _unitOfWork.CustomerRepository
            .GetAll()
            .Returns(new[] { customer }.BuildMock());

        _passwordHasher
            .VerifyHashedPassword(
                Arg.Any<User>(),
                user.Password,
                request.Password)
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await _authenticationService.Authenticate(request);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Description.ShouldBe(AuthenticationServiceErrors.InvalidEmailOrPassword.Description);
    }

    private User GetUser(string emailAddress) => new User
    {
        Id = 1,
        EmailAddress = emailAddress,
        Password = "hashedPassword123"
    };

    private Customer GetCustomer(long userId)
        => new()
        {
            UserId = userId,
            FirstName = "John",
            LastName = "Doe"
        };
}

