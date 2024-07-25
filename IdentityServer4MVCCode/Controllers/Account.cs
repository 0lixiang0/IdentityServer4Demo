using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4MVCCode.Controllers
{
    public class Account : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
