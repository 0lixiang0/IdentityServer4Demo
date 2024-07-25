using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4MVCHybrid
{
    public class AccountController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
