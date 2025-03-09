using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ShopEProduction.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Dashboard()
        {
            // Ensure user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userId")))
            {
                return RedirectToAction("Login", "Login"); // Redirect guests to Login
            }

            ViewBag.Username = HttpContext.Session.GetString("userId"); // Example: Get user ID
            return View();
        }
    }
}
