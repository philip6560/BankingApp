﻿using BankingApp.Data.Entities;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Authentication.Abstraction;
using BankingApp.Services.Authentication.Dtos;
using BankingApp.Services.Authentication.Utils;
using BankingApp.Services.Common.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingApp.Services.Authentication
{
    public class AuthenticationService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher) : IAuthenticationService
    {
        public async Task<Result<TokenGenerationRequest>> Authenticate(AuthenticationRequest request)
        {
            var query = from u in unitOfWork.UserRepository.GetAll()
                       join c in unitOfWork.CustomerRepository.GetAll() on u.Id equals c.UserId
                       where u.EmailAddress == request.EmailAddress
                       select new 
                       {
                           UserId = u.Id,
                           c.FirstName,
                           c.LastName,
                           u.EmailAddress,
                           HashedPassword = u.Password,
                       };

            var queryResult = await query.AsNoTracking().FirstOrDefaultAsync();

            if (queryResult == null) 
            {
                return Result<TokenGenerationRequest>.Failure(AuthenticationServiceErrors.InvalidEmailOrPassword);
            }

            var passwordHashResult = passwordHasher.VerifyHashedPassword(new User 
            {
                EmailAddress = queryResult.EmailAddress,
            }, queryResult.HashedPassword, request.Password);

            if (passwordHashResult == PasswordVerificationResult.Failed)
            {
                return Result<TokenGenerationRequest>.Failure(AuthenticationServiceErrors.InvalidEmailOrPassword);
            }

            return Result<TokenGenerationRequest>.Success(new TokenGenerationRequest 
            { 
                EmailAddress = queryResult.EmailAddress,
                UserId = queryResult.UserId,
                FirstName = queryResult.FirstName,
                LastName = queryResult.LastName,
            });
        }

        public Result<string> GenerateAuthenticationToken(TokenGenerationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Issuer)
                || string.IsNullOrWhiteSpace(request.Audience) 
                || string.IsNullOrWhiteSpace(request.SecurityKey)) 
            {
                return Result<string>.Failure(AuthenticationServiceErrors.TokenGeneration);  
            }

            var encodedSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(request.SecurityKey));

            var signingCredentials = new SigningCredentials(encodedSecurityKey, SecurityAlgorithms.HmacSha256);

            var tokenClaims = new List<Claim>();
            tokenClaims.Add(new Claim("sub", $"{request.UserId}"));
            tokenClaims.Add(new Claim("given_name", $"{request.FirstName}"));
            tokenClaims.Add(new Claim("family_name", $"{request.LastName}"));
            tokenClaims.Add(new Claim("email", $"{request.EmailAddress}"));

            var jwtSecurityToken = new JwtSecurityToken(
                request.Issuer,
                request.Audience,
                tokenClaims, 
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Result<string>.Success(token);
        }
    }
}
