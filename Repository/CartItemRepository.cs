using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ShopEProduction.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ShopEproductionContext _context;
        public CartItemRepository(ShopEproductionContext context)
        {
            _context = context;
        }
        public async Task<CartItem> UpdateCartItemAsync(int cartItemId, CartItem cartItem)
        {
            var cartItemFromDb = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);
            if (cartItemFromDb == null)
            {
                return null;
            }
            cartItemFromDb = cartItem;
            await _context.SaveChangesAsync();
            return cartItemFromDb;
        }

        public async Task<CartItem> GetCartItemByCartIdAndProductDetailIdAsync(int cartId, int productDetailId)
        {
            return await _context.CartItems.FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductDetailId == productDetailId);
        }

        public async Task<CartItem> AddNewCartItemAsync(int cartId, CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }
        public async Task<List<CartItem>> getListCartItem(int cartId)
        {
            return await _context.CartItems.Where(c => c.CartId == cartId).ToListAsync();
        }

    }
}
