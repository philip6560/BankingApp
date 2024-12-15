using BankingApp.Services.Payment.Abstraction;
using BankingApp.Services.Payment.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BankingApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController(
        IOptions<IdentityOptions> identityOptions,
        IPaymentService paymentService) 
        : BaseController(identityOptions)
    {
        [HttpPost]
        public async Task<IActionResult> TransferMoney(TransferMoneyRequest request)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var result = await paymentService.TransferMoney((long)currentUserId, request);

            if (result.IsError)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(GetTransactionsRequest request)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null) 
            {
                return Unauthorized();
            }

            var result = await paymentService.GetTransactions((long)currentUserId, request);

            if (result.IsError) 
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Data);
        }
    }
}
