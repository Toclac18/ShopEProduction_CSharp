using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;
using ShopEProduction.Services.Email.IService;

namespace ShopEProduction.Controllers
{
    public class RentController : Controller
    {
        private readonly ShopEproductionContext _context;
        private readonly IRentRepository _rentRepository;
        private readonly IEmailService _emailService;
        private readonly IProductDetailRepository _productDetailRepository;
        public RentController(ShopEproductionContext context, IRentRepository  rentRepository, IEmailService emailService, IProductDetailRepository productDetailRepository)
        {
            _context = context;
            _rentRepository = rentRepository;
            _emailService = emailService;
            _productDetailRepository = productDetailRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ShowRentForm(int? detailId)
        {
            // Log for debugging
            Console.WriteLine($"ShowRentForm called with detailId: {detailId}");

            var userIdStr = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                TempData["ErrorMessage"] = "Please log in to rent a product.";
                return RedirectToAction("Login", "Account");
            }

            var model = new RentInProcess
            {
                RentedDate = DateTime.Today.AddDays(1),
                ExpiredDate = DateTime.Today.AddDays(2), // Default to 1 day rental
                RentedType = false // Default to Days
            };

            if (detailId.HasValue && detailId > 0)
            {
                var product = await _context.ProductDetails.FirstOrDefaultAsync(p => p.Id == detailId.Value);
                if (product == null)
                {
                    TempData["ErrorMessage"] = $"Product not found for ID: {detailId}";
                    return RedirectToAction("Home", "Home");
                }
                model.ProductDetailId = detailId.Value;
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid product ID.";
                return RedirectToAction("Home", "Home");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForm(RentInProcess rentInProcess)
        {
            /*
      * - Validate for user.
      * - Validate for product id and isRented flg.
      * - Validate for product type of rented product.
      * - Validate for rented day (must be > 1 by today). End date must be > start date for at least 1 days.
      * - Validate for months duration must be in choosen list.
      * Incase extend expired date, check for flag IS_EXTENDED = 1 then execute validation:
      *  - rented_date > expired_date at least 1 days.
      */
            var detailId = rentInProcess.ProductDetailId;
            int durationInDays = 0;
            try
            {
                // Validate user
                var userIdStr = HttpContext.Session.GetString("userId");
                if (string.IsNullOrEmpty(userIdStr))
                {
                    TempData["ErrorMessage"] = "Cannot make a rent! Please log in.";
                    return RedirectToAction("Home");
                }

                int userId = Convert.ToInt32(userIdStr);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Invalid user.";
                    return RedirectToAction("Home");
                }

                // Validate product ID
                var product = await _context.ProductDetails.FirstOrDefaultAsync(p => p.Id == detailId);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Invalid product ID.";
                    return View("ShowRentForm", rentInProcess);
                }

                if(product.IsRentedFlg == true)
                {
                    TempData["ErrorMessage"] = "Product is already rented.";
                    return View("ShowRentForm", rentInProcess);
                }

                // Validate product type of rented product
                // RentalType: 0 = Days only, 1 = Months only, 2 = Both
                if (rentInProcess.RentedType == null)   // Wants Months, but Days only
                {
                    TempData["ErrorMessage"] = $"Product does not support {(rentInProcess.RentedType ? "month-based" : "day-based")} rentals.";
                    return View("ShowRentForm", rentInProcess);
                }

                // Validate rented date (must be >= today)
                if (rentInProcess.RentedDate.Date < DateTime.Today)
                {
                    TempData["ErrorMessage"] = "Rented date must be today or later.";
                    return View("ShowRentForm", rentInProcess);
                }

                // Validate expired date (must be > rented date by at least 1 day)
                if (rentInProcess.ExpiredDate.Date <= rentInProcess.RentedDate.Date)
                {
                    TempData["ErrorMessage"] = "Expired date must be at least 1 day after rented date.";
                    return View("ShowRentForm", rentInProcess);
                }


                // Validate months duration (if RentedType = 1)
                if (rentInProcess.RentedType) // Months
                {
                    int months = (int)rentInProcess.Duration; // Approximate months
                    int[] validMonths = { 1, 3, 6, 12 }; // Predefined list
                    if (!validMonths.Contains(months))
                    {
                        TempData["ErrorMessage"] = "Month-based rental duration must be 1, 3, 6, or 12 months.";
                        return View("ShowRentForm", rentInProcess);
                    }
                    durationInDays = (int) rentInProcess.Duration * 30; // Standardize to days (1 month = 30 days)
                } else
                {
                    durationInDays = (int)(rentInProcess.ExpiredDate - rentInProcess.RentedDate).TotalDays; // Calculate duration in days
                }

                // Log incoming data
                Console.WriteLine($"Incoming - UserId: {userId}, ProductDetailId: {rentInProcess.ProductDetailId}, RentedDate: {rentInProcess.RentedDate}, ExpiredDate: {rentInProcess.ExpiredDate}, RentedType: {(rentInProcess.RentedType ? "Months" : "Days")}, Duration: {durationInDays} days");
                
                // Set remaining fields
                rentInProcess.UserId = userId;
                rentInProcess.ProductDetailId = product.Id; // Set from product detail
                rentInProcess.RentedDate = rentInProcess.RentedDate.Date; // Set to date only
                rentInProcess.ExpiredDate = rentInProcess.ExpiredDate.Date; // Set to date only 
                rentInProcess.RentedType = rentInProcess.RentedType; // Set to rented type (0 = Days, 1 = Months)
                rentInProcess.Duration = durationInDays; // convert to days all time. 1 months dedault set = 30 days
                rentInProcess.IsExtended = false; // Default per schema
                rentInProcess.IsExpired = false; // Default per schema

                // Save to database
                if (rentInProcess.ProductDetailId > 0)
                {
                    var existingRental = await _context.RentInProcesses.FindAsync(rentInProcess.Id);
                    if (existingRental != null)
                    {
                        existingRental.RentedDate = rentInProcess.RentedDate;
                        existingRental.ExpiredDate = rentInProcess.ExpiredDate;
                        existingRental.RentedType = rentInProcess.RentedType;
                        existingRental.Duration = rentInProcess.Duration;

                        await _rentRepository.updateRentProcess(existingRental);
                    }
                    else
                    {
                        await _rentRepository.createNewRentProcess(rentInProcess);
                        // update flag for detail product.
                        await _productDetailRepository.UpdateIsRentedFlagAsync(rentInProcess.ProductDetailId);
                    }
                }

                var emailBody = $"You have successfully purchased product {product.DetailDesc} for: ${product.Price}.\n" +
                                $"This is information to login system: {product.DetailPrivateDesc}.\n" +
                                "Thanks for enjoying ShopEProduction!\n" +
                                "Have a good day <3";
                _emailService.SendEmailAsync(user.Email, "[ShopEProduction_Purchase Confirmation]", emailBody);
                

                TempData["SuccessMessage"] = "Rented successfully!";
                return RedirectToAction("Home", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while processing your rental.";
                return View("ShowRentForm", rentInProcess);
            }
        }
    }
}
