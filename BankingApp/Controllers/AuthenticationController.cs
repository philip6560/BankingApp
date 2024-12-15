using BankingApp.Services.Authentication.Abstraction;
using BankingApp.Services.Authentication.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController(IConfiguration configuration, IAuthenticationService authenticationService) : Controller
    {

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request) 
        {
            var tokenGenerationResult = await authenticationService.Authenticate(request);

            if (tokenGenerationResult.IsError || configuration == null) 
            {
                return BadRequest(tokenGenerationResult.Error);
            }

            tokenGenerationResult.Data!.Issuer = configuration["Authentication:Issuer"];
            tokenGenerationResult.Data!.Audience = configuration["Authentication:Audience"];
            tokenGenerationResult.Data!.SecurityKey = configuration["Authentication:SecurityKey"];
            var authenticationToken = authenticationService
                .GenerateAuthenticationToken(tokenGenerationResult.Data!);

            if (authenticationToken.IsError) 
            {
                return BadRequest(authenticationToken.Error);
            }

            return Ok(authenticationToken.Data);
        }
    }
}
