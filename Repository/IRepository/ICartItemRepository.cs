using ShopEProduction.Models;   

namespace ShopEProduction.Repository.IRepository
{
    public interface ICartItemRepository
    {
        Task<CartItem> UpdateCartItemAsync(int cartItemId, CartItem cartItem);
        Task<CartItem> GetCartItemByCartIdAndProductDetailIdAsync(int cartId, int productDetailId);
        Task<CartItem> AddNewCartItemAsync(int cartId, CartItem cartItem);
        Task<List<CartItem>> getListCartItem(int cartId);
    }
}
