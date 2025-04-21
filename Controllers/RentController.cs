using Microsoft.AspNetCore.Mvc;

namespace ShopEProduction.Controllers
{
    public class RentController : Controller
    {
        [HttpGet]
        public IActionResult ShowRentForm()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SubmitForm(string name, string email, string phone, string address)
        {
            /*
             * - Validate for user.
             * - Validate for product id.
             * - Validate for product type of rented product.
             * - Validate for rented day (must be > 1 by today). End date must be > start date for at least 1 days.
             * - Validate for months duration must be in choosen list.
             * Incase extend expired date, check for flag IS_EXTENDED = 1 then execute validation:
             *  - rented_date > expired_date at least 1 days.
             */
            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));
            if(userId == null)
            {
                TempData["ErrorMessage"] = "Cannot make a rent!";
                return RedirectToAction("Home");
            }
            ViewBag.Message = "Rent form submitted successfully!";
            return View("ShowRentForm");
        }
    }
}
