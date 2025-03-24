using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Controllers
{
    public class PurchaseController : Controller
    {
        ShopEproductionContext _context = new ShopEproductionContext();
        ICartRepository _cartRepository;
        IProductDetailRepository _productDetailRepository;
        ICartItemRepository _cartItemRepository;

        public PurchaseController(ICartRepository cartRepository, IProductDetailRepository productDetailRepository, ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _productDetailRepository = productDetailRepository;
            _cartItemRepository = cartItemRepository;
        }


        [HttpPost]
        public async Task<IActionResult> BuyNow(int detailId)
        {
            int? userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));
            if (userId == null)
            {
                return Json(new { success = false, message = "Please log in." });
            }

            var detail = await _context.ProductDetails.FirstOrDefaultAsync(pd => pd.Id == detailId);
            if (detail == null || detail.Status != true || detail.ProductType != 0)
            {
                return Json(new { success = false, message = "Product unavailable or not for sale." });
            }

            var wallet = await _context.WalletHistories.FirstOrDefaultAsync(wh => wh.UserId == userId);
            if (wallet == null || wallet.CurrentBalance < (decimal)detail.Price)
            {
                return Json(new { success = false, message = "Insufficient funds." });
            }

            // Create PurchaseHistory
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            var purchase = new PurchaseHistory
            {
                UserId = userId.Value,
                CartId = cart?.Id ?? 0, // Adjust if cart is optional
                PurchaseDate = DateTime.Now,
                PurchaseHistoryDetails = new List<PurchaseHistoryDetail>
                {
                    new PurchaseHistoryDetail
                    {
                        ProductDetailId = detail.Id,
                        PriceAtPurchase = detail.Price,
                        IsBoughtFlg = true,
                        IsRentedFlg = null
                    }
                }
            };

            // Update Wallet
            var walletDetail = new WalletHistoryDetail
            {
                HistoryId = wallet.Id,
                HistoryType = "OUT",
                TimeExecution = DateTime.Now,
                PreValue = wallet.CurrentBalance,
                ChangeAmount = -(decimal)detail.Price,
                PostValue = wallet.CurrentBalance - (decimal)detail.Price,
                Description = $"Purchase of {detail.DetailDesc}",
                PurchaseDetailId = null // Will be set after saving purchase
            };

            wallet.CurrentBalance -= (decimal)detail.Price;
            detail.IsBoughtFlg = true; // Mark as bought

            _context.PurchaseHistories.Add(purchase);
            await _context.SaveChangesAsync();
            walletDetail.PurchaseDetailId = purchase.PurchaseHistoryDetails.First().Id;
            _context.WalletHistoryDetails.Add(walletDetail);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Purchase completed successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> BuyCart([FromBody] List<CartItemSelectionDto> selectedItems)
        {
            var rawBody = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Console.WriteLine("Raw request body: " + rawBody);

            try
            {
                var userId = HttpContext.Session.GetString("userId");
                if (userId == null)
                {
                    return Json(new { success = false, message = "Please log in." });
                }

                int parsedUserId = int.Parse(userId);
                var wallet = await _context.WalletHistories.FirstOrDefaultAsync(wh => wh.UserId == parsedUserId);
                if (wallet == null)
                {
                    return Json(new { success = false, message = "Wallet not found." });
                }

                if (selectedItems == null || !selectedItems.Any())
                {
                    return Json(new { success = false, message = "No items selected." });
                }

                decimal totalCost = 0;
                var cart = await _cartRepository.GetCartByUserIdAsync(parsedUserId);
                var purchaseDetails = new List<PurchaseHistoryDetail>();

                foreach (var selected in selectedItems)
                {
                    var cartItem = await _cartItemRepository.GetCartItemByCartIdAndProductDetailIdAsync(cart.Id, selected.productDetailId);
                    if (cartItem == null || cartItem.CartId != cart.Id)
                    {
                        return Json(new { success = false, message = $"Invalid cart item ID: {selected.productDetailId}" });
                    }

                    var productDetail = await _productDetailRepository.GetProductDetailByIdAsync(cartItem.ProductDetailId);
                    if (productDetail == null)
                    {
                        return Json(new { success = false, message = $"ProductDetail not found for ID {selected.productDetailId}" });
                    }

                    
                    if (productDetail.Status != true || productDetail.ProductType != 0)
                    {
                        return Json(new { success = false, message = $"Item {cartItem.ProductDetailName} is unavailable." });
                    }

                    int remainingQuantity = await _productDetailRepository.CountRemainQuantity(productDetail.ProductId);
                    if (remainingQuantity < selected.Quantity)
                    {
                        return Json(new { success = false, message = $"Not enough stock for {cartItem.ProductDetailName}" });
                    }

                    totalCost += (decimal)productDetail.Price * selected.Quantity;
                    purchaseDetails.Add(new PurchaseHistoryDetail
                    {
                        ProductDetailId = cartItem.ProductDetailId,
                        PriceAtPurchase = productDetail.Price,
                        IsBoughtFlg = true,
                        IsRentedFlg = null
                    });

                    await _cartItemRepository.RemoveCartItemById(cartItem.Id);
                }

                if (wallet.CurrentBalance < totalCost)
                {
                    return Json(new { success = false, message = "Insufficient wallet balance." });
                }

                var purchase = new PurchaseHistory
                {
                    UserId = parsedUserId,
                    CartId = cart.Id,
                    PurchaseDate = DateTime.Now,
                    PurchaseHistoryDetails = purchaseDetails
                };

                var walletDetail = new WalletHistoryDetail
                {
                    HistoryId = wallet.Id,
                    HistoryType = "OUT",
                    TimeExecution = DateTime.Now,
                    PreValue = wallet.CurrentBalance,
                    ChangeAmount = -totalCost,
                    PostValue = wallet.CurrentBalance - totalCost,
                    Description = $"Purchase of {selectedItems.Count} item(s)",
                    PurchaseDetailId = null
                };

                wallet.CurrentBalance -= totalCost;
                _context.PurchaseHistories.Add(purchase);
                await _context.SaveChangesAsync();

                walletDetail.PurchaseDetailId = purchase.PurchaseHistoryDetails.First().Id;
                _context.WalletHistoryDetails.Add(walletDetail);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Purchase completed successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error processing purchase: " + ex.Message });
            }
        }
    }
}
public class CartItemSelectionDto
{
    public int productDetailId { get; set; } // Match frontend case
    public int Quantity { get; set; }
}
