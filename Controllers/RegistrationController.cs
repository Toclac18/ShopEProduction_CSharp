using Microsoft.AspNetCore.Mvc;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IUserRepository _userRepository;

        public RegistrationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> createNewUser(User user)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("Registration", user); // Return the form if validation fails
            //}

            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(user.Username);
            if (existingUserByUsername != null)
            {
                ViewBag.Message = "Username already exists!";
                return View("Registration", user);
            }

            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(user.Email);
            if (existingUserByEmail != null)
            {
                ViewBag.Message = "Email already exists!";
                return View("Registration", user);
            }

            var existingUserByPhone = await _userRepository.GetUserByPhoneNumberAsync(user.Phonenumber);
            if (existingUserByPhone != null)
            {
                ViewBag.Message = "Phone number already exists!";
                return View("Registration", user);
            }

            user.UserCreateAt = DateTime.UtcNow;
            user.UserStatus = true;

            // Save the user to the database
            await _userRepository.RegisterUserAsync(user);

            TempData["SuccessMessage"] = "Registration successful! Please login.";

            return RedirectToAction("Login", "Login");
        }

    }
}
