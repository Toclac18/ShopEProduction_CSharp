using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;

namespace ShopEProduction.Controllers
{
    public class WalletController : Controller
    {
        private readonly ShopEproductionContext _context;
        public WalletController(ShopEproductionContext context)
        {
            _context = context;
        }
        public IActionResult ShowWallet()
        {
            int? userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));
            if (userId == null)
            {
                ViewBag.Message = "Please login to view your wallet";
                return RedirectToAction("Login", "Login");
            }

            var walletHistory = _context.WalletHistories
                .Include(wh => wh.WalletHistoryDetails) // Ensure Details is included
                .FirstOrDefault(wh => wh.UserId == userId);

            if (walletHistory == null)
            {
                walletHistory = new WalletHistory
                {
                    UserId = userId.Value,
                    CurrentBalance = 0,
                    WalletHistoryDetails = new List<WalletHistoryDetail>() // Empty collection
                };
            }

            return View("ShowWallet", walletHistory);
        }
    }
}
