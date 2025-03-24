using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;

namespace ShopEProduction.Controllers
{
    public class ProductController : Controller
    {
        ShopEproductionContext _context = new ShopEproductionContext();
        [HttpGet]
        public IActionResult GetProductDetail(int id)
        {
            int? userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));
            if (userId == null)
            {
                return Json(new { success = false, message = "Please log in." });
            }

            var detail = _context.ProductDetails.FirstOrDefault(pd => pd.Id == id);
            var wallet = _context.WalletHistories.FirstOrDefault(wh => wh.UserId == userId);

            if (detail == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            return Json(new
            {
                success = true,
                detailDesc = detail.DetailDesc,
                price = detail.Price,
                walletBalance = wallet?.CurrentBalance ?? 0
            });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
