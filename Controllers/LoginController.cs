using Microsoft.AspNetCore.Mvc;
using ShopEProduction.Models;

namespace ShopEProduction.Controllers
{
    public class LoginController : Controller
    {
        private readonly ShopEproductionContext _context;
        private readonly IConfiguration _config;

        public LoginController(ShopEproductionContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Show login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handle login form submission
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {

            var adminEmail = _config["AdminAccount:AdminEmail"];
            var adminPassword = _config["AdminAccount:AdminPassword"];

            if (user.Username == adminEmail && user.Password == adminPassword)
            {
                HttpContext.Session.SetString("userId", "0");
                HttpContext.Session.SetString("userRole", "1");

                return RedirectToAction("Dashboard", "Admin"); // Redirect Admins
            }

            var loginUser = _context.Users.FirstOrDefault(u =>
                u.Username == user.Username && u.Password == user.Password);

            if (loginUser != null)
            {
                Console.WriteLine(loginUser);
                HttpContext.Session.SetString("userId", loginUser.Id.ToString());
                HttpContext.Session.SetString("userRole", loginUser.UserRoleId.ToString());
                if (loginUser.UserRoleId == 1)
                {
                    return RedirectToAction("Dashboard", "Admin");
                } else
                {
                    return RedirectToAction("Dashboard", "Home"); // Redirect Users
                }
            }

            ViewBag.Message = "Invalid Username or Password!";
            return View(user);
        }

        // Logout and clear session
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}
