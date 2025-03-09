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

            User u = new User();
            Console.WriteLine(" Input: " + user.Username);
            Console.WriteLine(" Input: " + user.Email);
            Console.WriteLine(" Input: " + user.Password);
            Console.WriteLine(" Input: " + user.Fullname);
            Console.WriteLine(" Input: " + user.Phonenumber);

            u.Username = user.Username;
            u.Password = user.Password;
            u.Fullname = user.Fullname;
            u.UserImage = "local_image";
            u.Email = user.Email;
            u.Phonenumber = user.Phonenumber;
            u.UserCreateAt = DateTime.Now;
            u.UserPoint = 0;
            u.UserRoleId = 2; //Default is User with role 2
            u.UserStatus = true;

            // Save the user to the database
            await _userRepository.RegisterUserAsync(u);

            return RedirectToAction("Login", "Login");
        }

    }
}
