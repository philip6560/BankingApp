using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : Controller
    {

        [HttpPost]
        public IActionResult Authenticate() 
        {
            
        }
    }
}
