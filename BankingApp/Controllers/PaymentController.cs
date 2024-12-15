using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        
        public IActionResult GetTransactions()
        {
            return View();
        }
        
        public IActionResult TransferMoney()
        {
            return View();
        }
    }
}
