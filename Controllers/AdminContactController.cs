using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.DTOs;
using ShopEProduction.Models;
using ShopEProduction.Services.Email.IService;
using ShopEProduction.Services.OTP.IOTP;
using ShopEProduction.Services.SMS.ISMS;

namespace ShopEProduction.Controllers
{
    public class AdminContactController : Controller
    {
        private readonly ShopEproductionContext _context;
        private readonly IOTPService _otpService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public AdminContactController(ShopEproductionContext context, IOTPService otpService, 
            IEmailService emailService, ISmsService smsService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
        }

        [HttpGet]
        public IActionResult AdminContact()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ContactForPassword()
        {
            return View(new ForgotPasswordDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyUsername(ForgotPasswordDto model)
        {
            Console.WriteLine("VerifyUsername: Username=" + model.Username);
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

            if (string.IsNullOrEmpty(model.Username))
            {
                ModelState.AddModelError("", "Please enter a username.");
                return View("ContactForPassword", model);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Account is invalid!!!");
                return View("ContactForPassword", model);
            }

            model.IsOTPStage = true;
            TempData["Username"] = user.Username;
            TempData.Keep("Username");
            string otp = _otpService.GenerateOtp(HttpContext.Session.GetString("sessionId")).Result.Value;
            TempData["OTP"] = otp;
            TempData.Keep("OTP");
            return View("ContactForPassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChooseOTPMethod(ForgotPasswordDto model)
        {
            Console.WriteLine($"ChooseOTPMethod: SelectedMethod={model.SelectedMethod}");

            // If no method is selected, show an error
            if (string.IsNullOrEmpty(model.SelectedMethod))
            {
                ModelState.AddModelError("", "Please select a verification method.");
                model.IsOTPStage = true;
                return View("ContactForPassword", model);
            }

            string username = TempData["Username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "User session lost. Please start over.");
                return View("ContactForPassword", new ForgotPasswordDto());
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found. Please start over.");
                return View("ContactForPassword", new ForgotPasswordDto());
            }

            string otp = TempData["OTP"]?.ToString();
            if (string.IsNullOrEmpty(otp))
            {
                ModelState.AddModelError("", "OTP session expired. Please start over.");
                return View("ContactForPassword", new ForgotPasswordDto());
            }

            try
            {
                // Send OTP based on selected method
                if (model.SelectedMethod == "Phone")
                {
                    Console.WriteLine($"Sending OTP via phone: {user.Phonenumber} - OTP: {otp}");
                    _smsService.SendSmsAsync(user.Phonenumber, $"Your OTP is: {otp}");
                }
                else if (model.SelectedMethod == "Email")
                {
                    Console.WriteLine($"Sending OTP via email: {user.Email} - OTP: {otp}");
                    _emailService.SendEmailAsync(user.Email, "OTP Verification", $"Your OTP is: {otp}");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid verification method selected.");
                    model.IsOTPStage = true;
                    return View("ContactForPassword", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to send OTP. Please try again.");
                model.IsOTPStage = true;
                return View("ContactForPassword", model);
            }

            // Set flags for the next stage
            model.IsOTPStage = true;
            model.Username = username;
            model.IsSelectedMethod = true; // Method has been selected, so set it to true
            TempData["Method"] = model.SelectedMethod;
            TempData.Keep("Username");
            TempData.Keep("OTP");
            TempData.Keep("Method");

            // Return view with updated model
            return View("ContactForPassword", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOTP(ForgotPasswordDto model)
        {
            const string attemptKey = "PasswordAttempts";
            int attempts = HttpContext.Session.GetInt32(attemptKey) ?? 0;
            string otp = TempData["OTP"]?.ToString();
            string method = TempData["Method"]?.ToString();

            Console.WriteLine($"OTP: OTP={otp}, Method={method}");
            Console.WriteLine($"VerifyOTP: OTP={model.OTP}, SelectedMethod={method}");

            // Check if OTP is empty
            if (string.IsNullOrEmpty(model.OTP))
            {
                ModelState.AddModelError("", "Please enter an OTP.");
                model.IsOTPStage = true;
                model.Username = TempData["Username"]?.ToString();
                model.SelectedMethod = TempData["Method"]?.ToString();
                return View("ContactForPassword", model);
            }

            string username = TempData["Username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "User session lost. Please start over.");
                return View("ContactForPassword", new ForgotPasswordDto());
            }

            // Check if OTP session has expired
            if (string.IsNullOrEmpty(otp))
            {
                ModelState.AddModelError("", "OTP session expired. Please start over.");
                return View("ContactForPassword", new ForgotPasswordDto());
            }

            try
            {
                // Verify the OTP
                if (_otpService.VerifyOTPAsync(HttpContext.Session.GetString("sessionId"), model.OTP, otp).Result)
                {
                    // Clear TempData after successful OTP verification
                    TempData.Clear();
                    HttpContext.Session.SetInt32(attemptKey, 0);
                    return RedirectToAction("ChangePassword", "Profile");
                }
                else
                {
                    ModelState.AddModelError("", "Wrong OTP. Please try again.");
                    
                    attempts++;
                    HttpContext.Session.SetInt32(attemptKey, attempts);
                    if (attempts >= 3)
                    {
                        TempData.Clear();
                        // Redirect to AdminContact/ContactForPassword
                        return RedirectToAction("Logout", "Login");
                    }
                    model.IsOTPStage = true;
                    model.IsSelectedMethod = true; // To keep the method selected
                    model.Username = username;
                    TempData.Keep("Username");
                    TempData.Keep("Method");
                    TempData.Keep("OTP");
                    return View("ContactForPassword", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to verify OTP. Please try again.");
            }

            return View("ContactForPassword", model);
        }

    }
}