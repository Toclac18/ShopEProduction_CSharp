using Microsoft.AspNetCore.Mvc;
using ShopEProduction.Models;
using ShopEProduction.Security.Password;

namespace ShopEProduction.Controllers
{
    public class LoginController : Controller
    {
        private readonly ShopEProductionContext _context;
        private readonly IConfiguration _config;
        PasswordPBKDF2 _passwordPBKDF2 = new PasswordPBKDF2();

        public LoginController(ShopEProductionContext context, IConfiguration config)
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
                HttpContext.Session.SetString("userRoleId", "1");

                return RedirectToAction("Dashboard", "Admin"); // Redirect Admins
            }

            var loginUser = _context.Users.FirstOrDefault(u =>
                string.Equals(u.Username, user.Username));

            if (loginUser != null)
            {
                if(_passwordPBKDF2.VerifyPasswordPBKDF2(user.Password, loginUser.Password))
                {
                    if (!(bool)loginUser.UserStatus)
                    {
                        ViewBag.Message = "User is currently inactive! Please contact to active your account!";
                        return View(user);
                    }
                    HttpContext.Session.SetString("userId", loginUser.Id.ToString());
                    HttpContext.Session.SetString("userRoleId", loginUser.UserRoleId.ToString());
                    HttpContext.Session.SetString("userName", loginUser.Fullname);
                    if (loginUser.UserRoleId == 1)
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home"); // Redirect Users
                    }
                }
                ViewBag.Message = "Wrong password!!! Please try again.";
                return View(user);
            }
            ViewBag.Message = "Invalid Username! Please try again.";
            return View(user);
        }

        // Logout and clear session
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Dashboard", "Home");
        }
    }
}
