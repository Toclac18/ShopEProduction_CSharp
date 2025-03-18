using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Repository
{
    public class ProductDetailRepository : IProductDetailRepository
    {

        private readonly ShopEproductionContext _context;

        public ProductDetailRepository(ShopEproductionContext context)
        {
            _context = context;
        }
        public async Task<List<ProductDetail>> getAllProductDetail()
        {
            return await _context.ProductDetails.ToListAsync(); 
        }

        public async Task<List<ProductDetail>> GetAvailableProductDetailsAsync(int productId)
        {
            return await _context.ProductDetails.Where(pd => pd.ProductId == productId && pd.Status == true).ToListAsync();
        }

        public async Task<ProductDetail> GetProductDetailByIdAsync(int productDetailId)
        {
            return await _context.ProductDetails.FirstOrDefaultAsync(pd => pd.Id == productDetailId);
        }
    }
}
