using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Controllers
{
    public class CartController : Controller
    {
        private  readonly ShopEproductionContext _context = new ShopEproductionContext();
        private readonly ICartRepository _cartRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly ICartItemRepository _cartItemRepository;
        public CartController(ICartRepository cartRepository, IProductDetailRepository productDetailRepository,
            ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _productDetailRepository = productDetailRepository;
            _cartItemRepository = cartItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ShowCart()
        {
            string userId = HttpContext.Session.GetString("userId");
            int parseUserId = int.Parse(userId);
            var wallet = await _context.WalletHistories.FirstOrDefaultAsync(wh => wh.UserId == parseUserId);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["CartMessage"] = "You must log in to see your cart!";
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(int.Parse(userId));

            if (cart == null)
            {
                TempData["CartMessage"] = "Your cart is empty!";
                return RedirectToAction("Home", "Home");
            }

            if(wallet == null)
            {
                TempData["CartMessage"] = "Cannot find your  e_wallet!";
                return RedirectToAction("Home", "Home");
            }
            float currentWallet = (float)wallet.CurrentBalance;
            ViewBag.Wallet = currentWallet;    
            List<CartItem> cartItems = await _cartItemRepository.getListCartItem(cart.Id);

            return View("ShowCart", cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int detailId)
        {
            try
            {
                Console.WriteLine("Received detailId: " + detailId);

                var userId = HttpContext.Session.GetString("userId");
                if (userId == null)
                {
                    return Json(new { success = false, message = "Please log in.", redirect = Url.Action("Login", "Account") });
                }

                int parsedUserId = int.Parse(userId);

                // Check if the user already has a cart
                var cart = await _cartRepository.GetCartByUserIdAsync(parsedUserId);

                if (cart == null)
                {
                    // Create a new cart (only if it doesn't exist)
                    cart = new Cart { UserId = parsedUserId };
                    await _cartRepository.AddCartAsync(cart);
                }

                // Retrieve the product detail
                var productDetail = await _productDetailRepository.GetProductDetailByIdAsync(detailId);
                if (productDetail == null)
                {
                    return Json(new { success = false, message = "Invalid product ID." });
                }

                // Check if the item already exists in the cart
                var cartItem = await _cartItemRepository.GetCartItemByCartIdAndProductDetailIdAsync(cart.Id, detailId);

                if (cartItem != null)
                {
                    // If the item exists, increase the quantity
                    cartItem.Quantity += 1;
                    await _cartItemRepository.UpdateCartItemAsync(cartItem.Id, cartItem);
                }
                else
                {
                    // Otherwise, add a new item to the cart
                    var newCartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductDetailId = detailId,
                        ProductDetailName = productDetail.DetailDesc,
                        ProductDetailPrice = (decimal)productDetail.Price,
                        Quantity = 1,
                        ProductId = productDetail.ProductId
                    };

                    await _cartItemRepository.AddNewCartItemAsync(cart.Id, newCartItem);
                }

                return Json(new { success = true, message = "Item added to cart successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to add item to cart: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = HttpContext.Session.GetString("userId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Please log in." });
            }
            try
            {
                var cartItem = await _cartItemRepository.GetCartItemByIdAsync(cartItemId);
                var cart = await _cartRepository.GetCartByUserIdAsync(int.Parse(userId));
                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Invalid cart item ID." });
                }
                await _cartItemRepository.RemoveCartItemById(cartItemId);
               
                return Json(new { success = true, message = "Item removed from cart successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to remove item from cart." });
            }
        }

    }
}
