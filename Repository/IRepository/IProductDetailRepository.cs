using ShopEProduction.Models;

namespace ShopEProduction.Repository.IRepository
{
    public interface IProductDetailRepository
    {
        Task<List<ProductDetail>> getAllProductDetail();
        Task<List<ProductDetail>> GetAvailableProductDetailsAsync(int productId);
        Task<ProductDetail> GetProductDetailByIdAsync(int productDetailId);
        Task<int> FindProductIdByProductDetailId(int productDetailId);
        Task<int> CountRemainQuantity(int productId);
    }
}
