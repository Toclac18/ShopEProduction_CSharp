using Microsoft.AspNetCore.Mvc;
using ShopEProduction.Models;
using ShopEProduction.Security.Password;

namespace ShopEProduction.Controllers
{
    public class LoginController : Controller
    {
        private readonly ShopEproductionContext _context;
        private readonly IConfiguration _config;
        PasswordPBKDF2 _passwordPBKDF2 = new PasswordPBKDF2();

        public LoginController(ShopEproductionContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Show login form
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            TempData.Clear();
            return View();
        }

        // Handle login form submission
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            // Track incorrect password attempts in session
            const string attemptKey = "PasswordAttempts";
            int attempts = HttpContext.Session.GetInt32(attemptKey) ?? 0;

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
                    // Reset attempts on successful password verification
                    HttpContext.Session.SetInt32(attemptKey, 0);
                    HttpContext.Session.SetString("userId", loginUser.Id.ToString());
                    HttpContext.Session.SetString("userRoleId", loginUser.UserRoleId.ToString());
                    HttpContext.Session.SetString("userName", loginUser.Username);
                    HttpContext.Session.SetString("sessionId", HttpContext.Session.Id);
                    if (loginUser.UserRoleId == 1)
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home"); // Redirect Users
                    }
                }
                attempts++;
                HttpContext.Session.SetInt32(attemptKey, attempts);
                if (attempts >= 3)
                {
                    // Update user status to 0 (locked/inactive)
                    user.UserStatus = false;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    // Log out by clearing session
                    HttpContext.Session.Clear();

                    // Redirect to AdminContact/ContactForPassword
                    return RedirectToAction("ContactForPassword", "AdminContact");
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
