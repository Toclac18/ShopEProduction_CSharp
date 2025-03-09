using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ShopEProduction.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            // Check if the user is an admin (Session "userRole" should be "Admin")
            if (HttpContext.Session.GetString("userRole") != "1")
            {
                return RedirectToAction("Login", "Login"); // Redirect unauthorized users to Login
            }

            ViewBag.Username = HttpContext.Session.GetString("userId"); // Example: Get user ID
            return View();
        }
    }
}
