using ShopEProduction.Models;

namespace ShopEProduction.Repository.IRepository
{
    public interface IProductDetailRepository
    {
        Task<List<ProductDetail>> getAllProductDetail();
        Task<List<ProductDetail>> GetAvailableProductDetailsAsync(int productId);
    }
}
