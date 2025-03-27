using Microsoft.AspNetCore.Mvc;

namespace ShopEProduction.Controllers
{
    public class AdminContactController : Controller
    {
        public IActionResult ContactForPassword()
        {
            return View();
        }
    }
}
