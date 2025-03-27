using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ShopEProduction.DTOs;
using ShopEProduction.Models;

namespace ShopEProduction.Controllers
{
    public class DepositController : Controller
    {
        private readonly ShopEproductionContext _context;

        public DepositController(ShopEproductionContext context)
        {
            _context = context;
        }
        public IActionResult AddDeposit(decimal changeAmount)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wallet = _context.WalletHistories.Where(w => w.UserId == userId).FirstOrDefault();
            if (wallet == null)
            {
                ViewBag.Message = "You don't have a wallet yet. Please create a wallet first.";
                return View();
            }

            return View();
        }

        [HttpPost]
        public IActionResult ProcessDeposit(string paymentMethod, decimal amount)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(paymentMethod) || amount <= 0 || amount > 100000000)
            {
                TempData["ErrorMessage"] = "Invalid deposit amount or payment method!";
                return RedirectToAction("AddDeposit");
            }

            try
            {
                // Retrieve or create wallet
                var wallet = _context.WalletHistories.FirstOrDefault(w => w.UserId == userId);
                if (wallet == null)
                {
                    wallet = new WalletHistory
                    {
                        UserId = userId, // Fixed syntax (no semicolon needed here)
                        CurrentBalance = 0
                    };
                    _context.WalletHistories.Add(wallet);
                    _context.SaveChanges();
                }

                // Store previous balance and calculate new balance
                decimal previousBalance = wallet.CurrentBalance;
                decimal newBalance = previousBalance + amount;

                // Insert into WalletHistoryDetail
                var historyDetail = new WalletHistoryDetail
                {
                    HistoryId = wallet.Id,
                    HistoryType = "IN",
                    PreValue = previousBalance,
                    ChangeAmount = amount,
                    PostValue = newBalance,
                    TimeExecution = DateTime.Now,
                    Description = "User deposit via " + paymentMethod
                };
                _context.WalletHistoryDetails.Add(historyDetail);

                // Update wallet balance
                wallet.CurrentBalance = newBalance;
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Deposit successful!";
                return RedirectToAction("ShowWallet", "Wallet");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message); // Log the exception for debugging
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
                return RedirectToAction("AddDeposit");
            }
        }

        //public IActionResult DepositSuccess()
        //{
        //    return View();
        //}

        //public async Task<bool> VerifyBankDeposit(string transactionId, decimal expectedAmount)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_BANK_API_KEY");
        //        HttpResponseMessage response = await client.GetAsync($"https://api.yourbank.com/check-deposit/{transactionId}");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Deserialize response into BankDepositResponse instead of object
        //            var jsonString = await response.Content.ReadAsStringAsync();
        //            var bankResponse = JsonConvert.DeserializeObject<BankDepositResponse>(jsonString);

        //            if (bankResponse != null && bankResponse.Amount == expectedAmount && bankResponse.Status == "SUCCESS")
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}

    }
}
