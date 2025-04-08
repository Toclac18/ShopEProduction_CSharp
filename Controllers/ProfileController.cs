using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.DTOs;
using ShopEProduction.Models;
using ShopEProduction.Security.Password;
using ShopEProduction.Services.Files;

namespace ShopEProduction.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ShopEproductionContext _context;
        private readonly IWebHostEnvironment _environment; // For file system access
        private readonly FileService _fileService;
        PasswordPBKDF2 _passwordPBKDF2 = new PasswordPBKDF2();

        public ProfileController(IWebHostEnvironment environment, ShopEproductionContext context, FileService fileService)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _fileService = fileService;
        }
        [HttpGet]
        public async Task<IActionResult> ShowProfile()
        {
            var userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            int parsedUserId = int.Parse(userId);
            var user = await _context.Users
                .Where(u => u.Id == parsedUserId)
                .Select(u => new ProfileDataUserDto
                {
                    Username = u.Username,
                    Fullname = u.Fullname,
                    UserImage = u.UserImage,
                    Email = u.Email,
                    Phonenumber = u.Phonenumber,
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            ViewBag.UserId = parsedUserId;
            return View(user); // Pass DTO to ShowProfile.cshtml
        }

        // GET: /Profile/UpdateProfile
        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            int parsedUserId = int.Parse(userId);
            var user = await _context.Users
                .Where(u => u.Id == parsedUserId)
                .Select(u => new ProfileDataUserDto
                {
                    Username = u.Username,
                    Fullname = u.Fullname,
                    UserImage = u.UserImage,
                    Email = u.Email,
                    Phonenumber = u.Phonenumber,
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            ViewBag.UserId = parsedUserId;
            return View(user); // Pass DTO to UpdateProfile.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileDataUserDto model, IFormFile userImageFile)
        {
            var userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            int parsedUserId = int.Parse(userId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == parsedUserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            Console.WriteLine($"Before Update - Username: {user.Username}, Fullname: {user.Fullname}, Email: {user.Email}, Phonenumber: {user.Phonenumber}, UserImage: {user.UserImage}");

            bool changesMade = false;

            // Handle file upload
            if (userImageFile != null && userImageFile.Length > 0)
            {
                try
                {
                    // Check for null environment
                    if (_environment == null || string.IsNullOrEmpty(_environment.WebRootPath))
                    {
                        Console.WriteLine("Error: WebRootPath is null or not initialized.");
                        ModelState.AddModelError("", "Server configuration error: Unable to access upload directory.");
                        return View(model);
                    }

                    // Validate file size (max 5MB)
                    if (userImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("userImageFile", "File size exceeds 5MB limit");
                        return View(model);
                    }

                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(userImageFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("userImageFile", "Invalid file format. Only JPG and PNG are allowed");
                        return View(model);
                    }

                    // Use FileService to upload the file
                    user.UserImage = await _fileService.UploadFileAsync(
                        userImageFile,
                        "Assets/Images/UserImage",
                        parsedUserId.ToString()
                    );

                    Console.WriteLine($"UserImage updated to: {user.UserImage}");
                    changesMade = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File upload error: {ex.Message}");
                    ModelState.AddModelError("userImageFile", $"Error uploading image: {ex.Message}");
                    return View(model);
                }
            }
            else
            {
                Console.WriteLine("No file uploaded or file is empty.");
            }

            // Update other fields if provided
            if (!string.IsNullOrEmpty(model.Username) && model.Username != user.Username)
            {
                user.Username = model.Username;
                changesMade = true;
            }
            if (!string.IsNullOrEmpty(model.Fullname) && model.Fullname != user.Fullname)
            {
                user.Fullname = model.Fullname;
                changesMade = true;
            }
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                user.Email = model.Email;
                changesMade = true;
            }
            if (!string.IsNullOrEmpty(model.Phonenumber) && model.Phonenumber != user.Phonenumber)
            {
                user.Phonenumber = model.Phonenumber;
                changesMade = true;
            }


            if (changesMade)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                Console.WriteLine("Database updated successfully.");
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("ShowProfile");
        }


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            int parsedUserId = int.Parse(userId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == parsedUserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Track incorrect password attempts in session
            const string attemptKey = "PasswordAttempts";
            int attempts = HttpContext.Session.GetInt32(attemptKey) ?? 0;

            // Validate current password
            if (!_passwordPBKDF2.VerifyPasswordPBKDF2(currentPassword, user.Password))
            {
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

                ModelState.AddModelError("currentPassword", "Incorrect current password");
                return View("ShowProfile");
            }

            // Reset attempts on successful password verification
            HttpContext.Session.SetInt32(attemptKey, 0);

            // Validate new password
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                ModelState.AddModelError("newPassword", "Password must be at least 6 characters long");
                return View("ShowProfile");
            }

            // Validate password confirmation
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Passwords do not match");
                return View("ShowProfile");
            }

            // Update password
            byte[] salt = _passwordPBKDF2.GenerateSalt();
            user.Password = _passwordPBKDF2.HashPasswordPBKDF2(newPassword, salt);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Login", "Login");
        }
    }
}