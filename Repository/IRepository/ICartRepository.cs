using ShopEProduction.Models;

namespace ShopEProduction.Repository.IRepository
{
    public interface ICartRepository
    {
        Task<Cart> AddCartAsync(Cart cart);
        Task<Cart> GetCartByUserIdAsync(int userId);
    }
}
