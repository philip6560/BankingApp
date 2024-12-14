using BankingApp.Services.Authentication.Dtos;
using BankingApp.Services.Common.Response;

namespace BankingApp.Services.Authentication.Abstraction
{
    public interface IAuthenticationService
    {
        Task<Result<TokenGenerationRequest>> Authenticate(AuthenticationRequest request);

        Result<string> GenerateAuthenticationToken(TokenGenerationRequest request);
    }
}
