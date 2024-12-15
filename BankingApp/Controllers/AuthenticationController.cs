using BankingApp.Services.Authentication.Abstraction;
using BankingApp.Services.Authentication.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController(IAuthenticationService authenticationService) : Controller
    {

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var tokenGenerationResult = await authenticationService.Authenticate(request);

            if (tokenGenerationResult.IsError) 
            {
                return BadRequest(tokenGenerationResult.Error);
            }

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
