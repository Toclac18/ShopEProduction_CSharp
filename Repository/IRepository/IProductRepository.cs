using ShopEProduction.Models;

namespace ShopEProduction.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<List<ProductDetail>> GetProductDetailsAsync(int productId);
    }
}
