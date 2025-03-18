using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ShopEproductionContext _context;
        public CartRepository(ShopEproductionContext context)
        {
            _context = context;
        }
        // Add a new cart
        public async Task<Cart> AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        // Retrieve a cart by user ID
        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems) // Load related cart items
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
