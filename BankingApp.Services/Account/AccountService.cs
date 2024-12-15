using BankingApp.Data.Entities;
using BankingApp.Data.Entities.Common.ValueObjects;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Account.Abstraction;
using BankingApp.Services.Account.Dtos;
using BankingApp.Services.Account.Utils;
using BankingApp.Services.Common.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Services.Account
{
    public class AccountService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher) : IAccountService
    {
        public async Task<Result<bool>> CreateAccount(CreateAccountRequest request)
        {
            var user = new User { EmailAddress = request.EmailAddress};
            var hashedPassword = passwordHasher.HashPassword(user, request.Password);
            user.Password = hashedPassword;
            await unitOfWork.UserRepository.InsertAsync(user);

            var customer = new Customer { 
                User = user,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            await unitOfWork.CustomerRepository.InsertAsync(customer);

            var generatedAccountNumber = AccountNumberGenerator.Generate();
            var account = new Data.Entities.Account { 
                Customer = customer,
                Balance = new Money(0.00M),
                AccountNumber = generatedAccountNumber
            };
            await unitOfWork.AccountRepository.InsertAsync(account);

            await unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<GetAccountDetailsResponse>> GetAccountDetails(GetAccountDetailsRequest request)
        {
            var query = from u in unitOfWork.UserRepository.GetAll()
                        join c in unitOfWork.CustomerRepository.GetAll() on u.Id equals c.UserId
                        join a in unitOfWork.AccountRepository.GetAll() on c.Id equals a.CustomerId
                        where u.Id == request.UserId
                        select new GetAccountDetailsResponse 
                        {
                            FullName = c.FullName,
                            EmailAddress = u.EmailAddress,
                            AccountNumber = a.AccountNumber,
                            Address = c.Address,
                        };

            var result = await query.AsNoTracking().FirstOrDefaultAsync();

            if (result == null) 
            {
                return Result<GetAccountDetailsResponse>.Failure(AccountServiceErrors.AccountNotFound("Details"));
            }

            return Result<GetAccountDetailsResponse>.Success(result);
        }

        public async Task<Result<GetBeneficiaryResponse>> GetBeneficiary(GetBeneficiaryRequest request)
        {
            var query = from u in unitOfWork.UserRepository.GetAll()
                        join c in unitOfWork.CustomerRepository.GetAll() on u.Id equals c.UserId
                        join a in unitOfWork.AccountRepository.GetAll() on c.Id equals a.CustomerId
                        where u.EmailAddress == request.EmailAddress
                        select new GetBeneficiaryResponse
                        {
                            FullName = c.FullName,
                            EmailAddress = u.EmailAddress,
                            AccountNumber = a.AccountNumber,
                        };
            var result = await query.AsNoTracking().FirstOrDefaultAsync();

            if (result == null)
            {
                return Result<GetBeneficiaryResponse>.Failure(AccountServiceErrors.BeneficiaryNotFound);
            }

            return Result<GetBeneficiaryResponse>.Success(result);
        }

        public async Task<Result<bool>> UpdateAccount(long userId, UpdateAccountRequest request)
        {
            var customer = await unitOfWork.CustomerRepository
                .GetAll()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            if (customer == null) 
            {
                return Result<bool>.Failure(AccountServiceErrors.AccountNotFound("Update"));
            }

            var madeChanges = false;
            if (!string.IsNullOrWhiteSpace(request.FirstName) 
                && customer.FirstName.ToLower() != request.FirstName.ToLower()) 
            {
                madeChanges = true;
                customer.FirstName = request.FirstName;
            }
            
            if (!string.IsNullOrWhiteSpace(request.LastName) 
                && customer.LastName.ToLower() != request.LastName.ToLower()) 
            {
                madeChanges = true;
                customer.LastName = request.LastName;
            }
            
            if (!string.IsNullOrWhiteSpace(request.Address) 
                && customer.Address?.ToLower() != request.Address.ToLower()) 
            {
                madeChanges = true;
                customer.Address = request.Address;
            }

            if (madeChanges) 
            {
                await unitOfWork.CustomerRepository.UpdateAsync(customer);
                await unitOfWork.SaveChangesAsync();
            }
            
            return Result<bool>.Success(true);
        }
    }
}
