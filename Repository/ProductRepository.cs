using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopEProductionContext _context;
        public ProductRepository(ShopEProductionContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProductDetail>> GetProductDetailsAsync(int productId)
        {
            return await _context.ProductDetails.Where(pd => pd.ProductId == productId).ToListAsync();
        }
    }
}
