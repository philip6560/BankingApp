using BankingApp.Services.Account.Abstraction;
using BankingApp.Services.Account.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BankingApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController(
        IOptions<IdentityOptions> identityOptions,
        IAccountService accountService)
        : BaseController(identityOptions)
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateAccountRequest request) 
        {
            var result = await accountService.CreateAccount(request);

            if (result.IsError) 
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> Update(UpdateAccountRequest request) 
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null) 
            {
                return Unauthorized();
            }

            var result = await accountService.UpdateAccount((long)currentUserId, request);

            if (result.IsError) 
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null) 
            {
                return Unauthorized();
            }

            var result = await accountService.GetAccountDetails(new GetAccountDetailsRequest 
            {
                UserId = (long)currentUserId
            });

            if (result.IsError) 
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetBeneficiary([FromQuery]GetBeneficiaryRequest request) 
        {
            var result = await accountService.GetBeneficiary(request);

            if (result.IsError) 
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Data);
        }
    }
}
