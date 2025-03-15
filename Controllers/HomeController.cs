using Microsoft.AspNetCore.Mvc;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductDetailRepository _productDetailRepository;

        public HomeController(IProductRepository productRepository, IProductDetailRepository productDetailRepository)
        {
            _productRepository = productRepository;
            _productDetailRepository = productDetailRepository;
        }
        public IActionResult Dashboard()
        {
            // Ensure user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userId")))
            {
                return RedirectToAction("Login", "Login"); // Redirect guests to Login
            }

            ViewBag.Username = HttpContext.Session.GetString("userId"); // Example: Get user ID
            return View();
        }

        public async Task<IActionResult> Home()
        {
            var products = await _productRepository.GetAllProductsAsync();

            return View("Home", products);
        }


        public async Task<IActionResult> ProductDetail(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userId")))
            {
                TempData["ErrorMessage"] = "Please login to view product details.";
                return RedirectToAction("Home"); // Redirect to Home
            }
            if (id <= 0)
            {
                return BadRequest("Invalid product ID.");
            }

            var product = await _productRepository.GetProductByIdAsync(id);
            var productDetails = await _productDetailRepository.GetAvailableProductDetailsAsync(id);
            

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            ViewBag.Product = product;
            
            return View(productDetails);
        }



    }
}
